using Microsoft.EntityFrameworkCore;
using Mage.Data;
using MajorGamer.Models;
using Microsoft.AspNetCore.Identity;

namespace Mage
{
    public class Program
    {
        public static void Main(string[] args) //Entire application starts from this file
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>() //Enables Admin & User roles which are not on by default
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();
            //package required: microsoft.aspnetcore.authentication.google
            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    //Access Google Auth section of appsettings.Development.json (our environment variables)
                    IConfigurationSection googleAuth = builder.Configuration.GetSection("Authentication:Google");
                    //Read Google API key values from config
                    options.ClientId = googleAuth["ClientId"];
                    options.ClientSecret = googleAuth["ClientSecret"];  
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) //app.= registering a middleware/factory pattern (taking a request)-> resolve or to next middleware
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection(); //loads 
            app.UseStaticFiles(); //registers request

            app.UseRouting(); //where all routing happens

            app.UseAuthentication(); 
            app.UseAuthorization();

            RouteConfig.ConfigureRoutes(app); //routes are read in order, 

            app.MapRazorPages();

            app.Run();
        }
    }
}