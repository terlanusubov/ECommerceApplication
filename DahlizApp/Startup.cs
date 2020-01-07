using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DahlizApp.Core.Infrastructure;
using DahlizApp.Data;
using DahlizApp.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DahlizApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            //services.AddMvc().AddJsonOptions(options => { options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore; });
            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                }).AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                        .AddDataAnnotationsLocalization();


            services.Configure<EmailServiceOption>((option) =>
            {
                option.DisplayName = "DAHLIZ.AZ";
                option.Email = "info@dahliz.az";
                option.Password = "Mahmah66@";
                option.Host = "mail.dahliz.az";
                option.Port = 8889;
            });

            services.AddSingleton<EmailService>();




            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                 new CultureInfo("en"),
                 new CultureInfo("az")
                };

                options.DefaultRequestCulture = new RequestCulture(culture: supportedCultures[0], uiCulture: supportedCultures[0]);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

            });

            services.AddDbContext<DahlizDb>(option =>
            {
                option.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"]);
            });


          

            services.AddIdentity<User, IdentityRole>()
                 .AddDefaultTokenProviders()
                 .AddEntityFrameworkStores<DahlizDb>();


            services.Configure<IdentityOptions>(option =>
            {
                option.Password.RequiredUniqueChars = 0;
                option.Password.RequiredLength = 6;
                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequireUppercase = false;
                option.User.RequireUniqueEmail = true;
                option.User.AllowedUserNameCharacters = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNMƏəÖöĞğıIçÇşŞüÜ";
            });
            services.ConfigureApplicationCookie(option =>
            {
                option.ExpireTimeSpan = TimeSpan.FromDays(1);

                option.LoginPath = "/Admin/Account/Login";
                option.AccessDeniedPath = "/Home/Error";
                option.SlidingExpiration = true;
            });

            services.AddDistributedMemoryCache();
            services.AddSession(option =>
            {
                option.IdleTimeout = TimeSpan.FromDays(1);
            });

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie("TempCookie", option =>
            {
                option.AccessDeniedPath = "/Home/Error";
                option.LoginPath = "/Admin/Account/Login";
            })
            .AddFacebook(facebookOptions =>
            {
                facebookOptions.SignInScheme = "TempCookie";
                facebookOptions.AppId = "636167950196306";
                facebookOptions.AppSecret = "dcc23f7d83a13e01f8e5d60778e5c469";
            }).AddGoogle(option =>
            {
                option.SignInScheme = "TempCookie";
                option.ClientSecret = "Ky5GGqZGHBVvyELjG7cREzoC";
                option.ClientId = "465092389464-eigvpqib122litp406acfp8l37mpat64.apps.googleusercontent.com";
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseRequestLocalization();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                name: "Admin",
                template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
