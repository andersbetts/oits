﻿using BITS.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BITS.Models
{
    public static class SeedData
    {
        public static async void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                await InitializeRoles(context, serviceProvider);
                await InitializeUsers(context, serviceProvider);
                await InitializeIssueDescriptionItems(context, serviceProvider);
            }

        }

        static async Task InitializeIssueDescriptionItems(ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            if(context.IssueDescriptionItem.Any())
            {
                return;
            }

            context.IssueDescriptionItem.Add(new IssueDescriptionItem()
            {
                Enabled = true,
                Name = "Root"
            });

            await context.SaveChangesAsync();
        }

        static async Task InitializeRoles(ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            var changes = false;
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            List<IdentityRole> roles = new List<IdentityRole>() {
            new IdentityRole() { Name = "CMG", NormalizedName = "CMG"},
            new IdentityRole() { Name = "Admin", NormalizedName = "ADMINISTRATOR"},
            new IdentityRole() { Name = "User", NormalizedName = "USER"},
            new IdentityRole() { Name = "Customer", NormalizedName = "CUSTOMER"},
        };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role.NormalizedName))
                {
                    context.Roles.Add(role);
                    changes |= true;
                }
            }

            if (changes)
                context.SaveChanges();

            roleManager.Dispose();
        }

        static async Task InitializeUsers(ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            if (context.Users.Any())
            {
                return;
            }


            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var cmg = new ApplicationUser()
            {
                UserName = "cmg@bits",
                Email = "cmg@bits",
            };

            var admin = new ApplicationUser()
            {
                UserName = "admin@bits",
                Email = "admin@bits",
            };

            var user = new ApplicationUser()
            {
                UserName = "user@bits",
                Email = "user@bits",
            };

            var customer = new ApplicationUser()
            {
                UserName = "customer@bits",
                Email = "customer@bits",
            };

            await userManager.CreateAsync(cmg, "Bits123!");
            await userManager.CreateAsync(admin, "Bits123!");
            await userManager.CreateAsync(user, "Bits123!");
            await userManager.CreateAsync(customer, "Bits123!");

            var result = await userManager.AddToRoleAsync(cmg, "CMG");
            result = await userManager.AddToRoleAsync(admin, "ADMINISTRATOR");
            result = await userManager.AddToRoleAsync(user, "USER");
            result = await userManager.AddToRoleAsync(customer, "CUSTOMER");

            await context.SaveChangesAsync();

            userManager.Dispose();
        }

    }
}
