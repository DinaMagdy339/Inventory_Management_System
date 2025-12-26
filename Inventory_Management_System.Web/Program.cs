using Inventory.Models;
using Inventory.Repository;
using Inventory.Utility.HelperClass;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Inventory.Repository.CustomerServices;
using Inventory.Utility;
using Inventory.Repository.BillServices;
using Inventory.Repository.BillTypeServices;
using Inventory.Repository.BranchServices;
using Inventory.Repository.CurrencyServices;
using Inventory.Repository.CustomerTypeServices;
using Inventory.Repository.BankServices;
using Inventory.Repository.BrandServices;
using Inventory.Repository.MeasureUnitServices;
using Inventory.Repository.ProductServices;
using Inventory.Repository.ProductTypeService;
using Inventory.Repository.AttachementServices;
using Inventory.Repository.WarehouseServices;
using Inventory.Repository.StockServices;
using Microsoft.Extensions.Options;
using Inventory.Repository.InvoiceServices;
using System;


namespace Inventory_Management_System.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            #region Add services to the container
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(connectionString));
            //builder.Services.AddIdentity<AppUser,IdentityRole>(options =>
            //{
            //    options.SignIn.RequireConfirmedAccount = true;
            //})
            //.AddEntityFrameworkStores<ApplicationDbContext>();


            builder.Services.AddControllersWithViews();
            //builder.Services.Configure<SuperAdmin>(builder.Configuration.GetSection("SuperAdmin"));
            builder.Services.AddScoped<IBankRepo, BankRepo>();
            builder.Services.AddScoped<IBrandRepo, BrandRepo>();
            builder.Services.AddScoped<IBillRepo, BillRepo>();
            builder.Services.AddScoped<IBillTypeRepo, BillTypeRepo>();
            builder.Services.AddScoped<IBranchRepo, BranchRepo>();
            builder.Services.AddScoped<ICurrencyRepo, CurrencyRepo>();
            builder.Services.AddScoped<ICustomerRepo, CustomerRepo>();
            builder.Services.AddScoped<ICustomerTypeRepo, CustomerTypeRepo>();
            builder.Services.AddScoped<IMeasureUnitRepo , MeasureUnitRepo>();
            builder.Services.AddScoped<IProductRepo , ProductRepo>();
            builder.Services.AddScoped<IProductTypeRepo , ProductTypeRepo>();
            builder.Services.AddTransient<IAttachementServices, AttachementServices>();
            builder.Services.AddScoped<IWarehouseRepo, WarehouseRepo>();
            builder.Services.AddScoped<IStockRepo, StockRepo>();
            builder.Services.AddScoped<IInvoiceRepo, InvoiceRepo>();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            #endregion
            var app = builder.Build();
          

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();
            app.UseSession();

            app.MapControllerRoute(
               name: "default",
               pattern: "{controller=Account}/{action=Register}");

         
            app.Run();
        }
    }
}
