using DahlizApp.Data;
using DahlizApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Core.Infrastructure
{
    public static class Seed
    {
        public static async Task UserCreator(IServiceScope scope,DahlizDb db)
        {
            UserManager<User> userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            
            if (!db.Users.Any() && !db.Roles.Any())
            {
                User user = new User()
                {
                    UserName = "dahlizAdmin",
                    Email = "admin@dahliz.az"
                };

                IdentityResult createResult = await userManager.CreateAsync(user, "Admin123@");
                if (createResult.Succeeded)
                {
                    IdentityResult roleCreaateResult = await roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
                    if (roleCreaateResult.Succeeded)
                    {
                       IdentityResult roleAttachResult=await userManager.AddToRoleAsync(user, "Admin");
                        if (roleAttachResult.Succeeded)
                        {
                            await db.SaveChangesAsync();
                        }
                    }

                }

            }

        }
    }
}
