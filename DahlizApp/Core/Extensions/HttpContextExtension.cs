using DahlizApp.Data;
using DahlizApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Core.Extensions
{
    public static class HttpContextExtension
    {
        public static async Task SetLanguage(this HttpContext context, string culture, DahlizDb db, string sessionName)
        {
            Language language = await db.Languages.Where(l => l.Key == culture).FirstOrDefaultAsync();
            context.Session.SetString(sessionName, language.Id.ToString());
        }

        public static int GetLanguage(this HttpContext context, string key)
        {
            return Convert.ToInt32(context.Session.GetString(key));
        }

        public static void SetCurrentPage(this HttpContext context, string action, string controller)
        {
            context.Session.SetString("action", action);
            context.Session.SetString("controller", controller);

        }
        public static string GetCurrentPageAction(this HttpContext context)
        {
            return context.Session.GetString("action");

        }
        public static string GetCurrentPageController(this HttpContext context)
        {
            return context.Session.GetString("controller");
        }

        public static void SetUserInfoToSession(this HttpContext context, string sessionName, string value)
        {
            context.Session.SetString(sessionName, value);
        }

        public static string GetUserInfoFromSession(this HttpContext context, string sessionName)
        {
            return context.Session.GetString(sessionName);
        }

        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static List<T> GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default(List<T>) : JsonConvert.DeserializeObject<List<T>>(value);
        }

        // new extension method
        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
