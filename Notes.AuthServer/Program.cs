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
                .AddInMemoryApiResources(Configuration.ApiResources) //���������� InMemory ��������� ��������
                .AddInMemoryApiScopes(Configuration.ApiScopes) //���������� InMemory ��������� ��������
                .AddInMemoryClients(Configuration.Clients) //���������� InMemory ��������� ��������
                .AddInMemoryIdentityResources(Configuration.IdentityResources)
                .AddDeveloperSigningCredential(); //���������� ���������������� ���������� �������

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