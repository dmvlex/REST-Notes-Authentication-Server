using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Notes.AuthServer.Data;
using Notes.AuthServer.Model;

namespace Notes.AuthServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetValue<string>("DBConnection");
            builder.Services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            builder.Services.AddIdentity<AppUser, IdentityRole>(config =>
            {
                config.Password.RequiredLength = 4;
                config.Password.RequireDigit = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
            })
                .AddEntityFrameworkStores<AuthDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddIdentityServer()
                .AddAspNetIdentity<AppUser>()
                .AddInMemoryApiResources(Configuration.ApiResources) //���������� InMemory ��������� ��������
                .AddInMemoryApiScopes(Configuration.ApiScopes) //���������� InMemory ��������� ��������
                .AddInMemoryClients(Configuration.Clients) //���������� InMemory ��������� ��������
                .AddInMemoryIdentityResources(Configuration.IdentityResources)
                .AddDeveloperSigningCredential(); //���������� ���������������� ���������� �������

            builder.Services.ConfigureApplicationCookie(config =>
            {
                config.Cookie.Name = "Notes.Identity.Cookie";
                config.LoginPath = "/Auth/Login";
                config.LogoutPath = "/Auth/Logout";
            });

            var app = builder.Build();


            app.UseRouting();
            app.UseIdentityServer();
            app.MapGet("/", () => "Notes AuthServer");
            using(var scope = app.Services.CreateScope())
            {
                try
                {
                    var context = scope.ServiceProvider.GetService<AuthDbContext>();
                    DBInitializer.Initialize(context);
                }
                catch (Exception ex) { }
            }

            app.Run();
        }
    }
}