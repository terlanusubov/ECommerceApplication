using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DahlizApp.Core.Infrastructure;
using DahlizApp.Data;
using DahlizApp.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DahlizApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
           IWebHost webHost =  CreateWebHostBuilder(args).Build();
            using(IServiceScope scope = webHost.Services.CreateScope())
            {
                using(DahlizDb db = scope.ServiceProvider.GetRequiredService<DahlizDb>())
                {
                    if (!db.Languages.Any())
                    {
                        Language az = new Language()
                        {
                            Key = "az",
                            Value = "Azerbaijan",
                            Icon = "az.png"
                        };

                        Language en = new Language()
                        {
                            Key = "en",
                            Value = "English",
                            Icon = "en.png"
                        };

                        db.Languages.AddRange(az, en);
                        db.SaveChanges();
                    }

                    Seed.UserCreator(scope, db).Wait();
                }
            }

            webHost.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
