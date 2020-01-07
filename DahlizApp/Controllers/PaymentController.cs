using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DahlizApp.Data;
using DahlizApp.Models;
using DahlizApp.Models.PaymentModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DahlizApp.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text;
using DahlizApp.Core.Infrastructure;
using Microsoft.AspNetCore.Authorization;

namespace DahlizApp.Controllers
{
    public class PaymentController : Controller
    {
        private readonly DahlizDb db;
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;
        public PaymentController(DahlizDb _db, UserManager<User> _userManager, IConfiguration _configuration)
        {
            db = _db;
            userManager = _userManager;
            configuration = _configuration;
        }

        private string GetMD5HashCode(string input)
        {
            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] data = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(input));
                return System.BitConverter.ToString(data).Replace("-", "").ToLower();
            }
            catch
            {
                return null;
            }
        }

        public async Task<IActionResult> Checkout(string cardType, string Zip, string City, string Address)
        {
            string user_id = HttpContext.Session.GetString("user_id");
            if (user_id != null)
            {
                List<Card> cards = HttpContext.Session.GetObjectFromJson<Card>("Card") as List<Card>;
                if (cardType != null && Zip != null && City != null && Address != null)
                {


                    #region Make CPaymentItem For API
                    decimal amount = Convert.ToDecimal(HttpContext.Session.GetString("Total"));
                    decimal shippingPrice = amount - cards.Sum(c => c.Quantity * c.Price);
                    CPaymentItem cPaymentItem = new CPaymentItem();
                    cPaymentItem.merchantName = configuration["PaymentInformation:MerchantName"];
                    cPaymentItem.amount = (int)(amount * 100);
                    cPaymentItem.cardType = cardType;
                    cPaymentItem.lang = "lv";
                    cPaymentItem.description = "Product";
                    cPaymentItem.hashCode = GetMD5HashCode(configuration["PaymentInformation:AuthKey"] +
                                                            configuration["PaymentInformation:MerchantName"] +
                                                            cPaymentItem.cardType +
                                                            cPaymentItem.amount +
                                                            cPaymentItem.description);
                    #endregion

                    #region Create Payment
                    List<Payment> userOrders = new List<Payment>();
                    for (var i = 0; i < cards.Count; i++)
                    {

                        Payment currentPayment = new Payment();
                        //City
                        currentPayment.City = City;
                        //Postal Code
                        currentPayment.PostalCode = Zip;

                        //Address
                        currentPayment.Address = Address;

                        currentPayment.ProductDiscount = cards[i].DiscountPercent;

                        //User
                        string userId = HttpContext.Session.GetString("user_id");
                        User user = db.Users.Where(u => u.Id == userId).FirstOrDefault();
                        currentPayment.UserName = user.Name;
                        currentPayment.UserSurname = user.Surname;
                        currentPayment.UserEmail = user.Email;
                        currentPayment.UserId = user.Id;
                        //If user address was null on register  then on payment we take address and append to it
                        user.Address = user.Address == null ? Address : user.Address;


                        //Cardtype
                        CardType type = await db.CardTypes.Where(c => c.Key == cardType).FirstOrDefaultAsync();
                        currentPayment.CardType = type;

                        //Product
                        Product orderedProduct = await db.Products.Where(p => p.Id == cards[i].Id).FirstOrDefaultAsync();
                        ProductLanguage productLanguage = await db.ProductLanguages
                                                                    .Include(l => l.Language)
                                                                    .Where(p => p.Language.Key == "az" && p.ProductId == orderedProduct.Id)
                                                                    .FirstOrDefaultAsync();

                        currentPayment.ProductName = productLanguage.Name;
                        currentPayment.ProductPrice = orderedProduct.Price;
                        currentPayment.ProductId = orderedProduct.Id;

                        //Datetime
                        currentPayment.Date = DateTime.Now;

                        //Shipping Price
                        currentPayment.ShippingPrice = shippingPrice;

                        //ProductSize
                        currentPayment.ProductSize = cards[i].SizeName;

                        //Quantity
                        currentPayment.Count = cards[i].Quantity;

                        //Total Price
                        if (currentPayment.ProductDiscount != 0 || currentPayment.ProductDiscount != null)
                            currentPayment.TotalPrice = (decimal)(cards[i].Price * currentPayment.ProductDiscount / 100) * cards[i].Quantity;
                        else
                            currentPayment.TotalPrice =cards[i].Price * cards[i].Quantity;
                       
                        //Status
                        currentPayment.Status = 2;

                        //TransactionId
                        currentPayment.TransactionId = cPaymentItem.hashCode;

                        userOrders.Add(currentPayment);
                    }
                    await db.Payments.AddRangeAsync(userOrders);
                    await db.SaveChangesAsync();
                    #endregion

                    #region Send Web Request
                    var jsonValues = JsonConvert.SerializeObject(cPaymentItem);

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(configuration["PaymentInformation:RequestToServerUrlGetPaymentKey"]);
                    request.ContentType = "application/json; charset=utf-8";
                    request.Method = "POST";
                    request.Accept = "application/json";
                    using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(jsonValues);
                        streamWriter.Flush();
                    }
                    #endregion

                    #region Get Web Response
                    string responseData = string.Empty;
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseData = reader.ReadToEnd();
                    }
                    CRespGetPaymentKey cRespPymnt = (CRespGetPaymentKey)JsonConvert.DeserializeObject(responseData, typeof(CRespGetPaymentKey));

                    #endregion

                    if (cRespPymnt.status.code != 1)
                        throw new Exception("Error while getting paymentKey, code=" + cRespPymnt.status.code + ", message=" + cRespPymnt.status.message);

                    return Redirect(configuration["PaymentInformation:ReuqestToServerUrlPayPage"] + cRespPymnt.paymentKey);
                }
                else
                {
                    return RedirectToAction("Error", "Payment");
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        [AllowAnonymous]
        public async Task<IActionResult> Callback(string payment_key, [FromServices]EmailService service)
        {
            int langId = HttpContext.GetLanguage("langId");
            if (langId == 0)
            {
                langId = db.Languages.FirstOrDefault().Id;
            }
            #region Get Payment Result
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(configuration["PaymentInformation:RequestToServerUrlGetPaymentResult"] + "?payment_key="
                    + payment_key + "&hash_code=" + GetMD5HashCode(configuration["PaymentInformation:AuthKey"] + payment_key));
            request.ContentType = "application/json; charset=utf-8";
            request.Method = "POST";
            request.Accept = "application/json";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string responseData = string.Empty;
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                responseData = reader.ReadToEnd();
            }

            #endregion
            CRespGetPaymentResult paymentResult = (CRespGetPaymentResult)JsonConvert.DeserializeObject(responseData, typeof(CRespGetPaymentResult));

            #region Make Payment Status 1 if succeeded(Payed)
            string user_id = HttpContext.Session.GetString("user_id");
            List<Payment> payments = await db.Payments.Where(p => p.UserId == user_id && p.Status == 2).ToListAsync();
            foreach (var payment in payments)
            {
                payment.PaymentKey = payment_key;
                payment.Status = paymentResult.status.code == 1 ? 1 : 0;
                await db.SaveChangesAsync();
            }
            #endregion


            if (paymentResult.status.code == 1)
            {
                #region Send Email to User
                await Task.Run(async () =>
                 {
                     User user = await db.Users.Where(u => u.Id == user_id).FirstOrDefaultAsync();
                     string userEmail = user?.Email;

                     string orderHtmlPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Email", "order.html");
                     StringBuilder orderHtml = new StringBuilder();

                     StringBuilder orderItem = new StringBuilder();
                     string orderItemPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Email", "orderitem.html");

                     using (StreamReader orderHtmlReader = new StreamReader(orderHtmlPath))
                     {
                         orderHtml.Append(orderHtmlReader.ReadToEnd());
                     }

                     using (StreamReader orderItemreader = new StreamReader(orderItemPath))
                     {
                         orderItem.Append(orderItemreader.ReadToEnd());
                     }

                     StringBuilder builder = new StringBuilder();
                     foreach (Payment payment1 in payments)
                     {
                         StringBuilder finalOrderItem = new StringBuilder();
                         finalOrderItem.Append(orderItem.ToString());
                         finalOrderItem.Replace("{ProductName}", payment1.ProductName);
                         finalOrderItem.Replace("{ProductSize}", payment1.ProductSize);
                         finalOrderItem.Replace("{ProductCount}", payment1.Count.ToString());
                         finalOrderItem.Replace("{ProductPrice}", payment1.TotalPrice.ToString());
                         builder.Append(finalOrderItem.ToString());
                     }
                     orderHtml.Replace("{OrderItem}", builder.ToString());

                     await service.SendMailAsync(user?.Email, "Order", orderHtml.ToString());
                 });
                #endregion
                return RedirectToAction("Success", "Payment");
            }
            else
            {
                return RedirectToAction("Error", "Payment");
            }
        }
        public IActionResult Success()
        {
            return View();
        }
        public IActionResult Error()
        {
            return View();
        }


    }
}