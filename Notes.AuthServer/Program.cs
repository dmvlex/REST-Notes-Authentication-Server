using IdentityServer4.Models;
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

            builder.Services.AddIdentityServer()
                .AddInMemoryApiResources(Configuration.ApiResources) //Используем InMemory хранилище ресурсов
                .AddInMemoryApiScopes(Configuration.ApiScopes) //Используем InMemory хранилище областей
                .AddInMemoryClients(Configuration.Clients) //Используем InMemory хранилище клиентов
                .AddInMemoryIdentityResources(Configuration.IdentityResources)
                .AddDeveloperSigningCredential(); //Используем демонстрационный сертификат подписи

            var app = builder.Build();


            app.UseRouting();
            app.UseIdentityServer();
            app.MapGet("/", () => "Notes AuthServer");

            using(var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<AuthDbContext>();
                DBInitializer.Initialize(context);
            }

            app.Run();
        }
    }
}