using AsyncEnumerable_TEST_MVC.Interface;
using AsyncEnumerable_TEST_MVC.Services;
using DbService.Interface;
using DbService.Services;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace AsyncEnumerable_TEST_MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IHomeService, HomeService>();
            builder.Services.AddTransient<IDbConnection>(sp => new OracleConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddTransient<ISqlRepository, SqlRepository>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
