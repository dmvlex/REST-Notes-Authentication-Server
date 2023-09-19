using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;

namespace Notes.AuthServer
{
    public static class Configuration
    {
        
        //Набор доступных областей
        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope> 
            { 
                new ApiScope("NotesWebAPI","Web API")
            };

        //[Заметка] В Identity Server области представлены ресурсами

        //Настраиваем видимые клеймы
        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>()
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile() 

            };

        public static IEnumerable<ApiResource> ApiResources =>
            new List<ApiResource>()
            {
                new ApiResource("NotesWebAPI","Web API",new [] { JwtClaimTypes.Name})
                {
                    Scopes = { "NotesWebAPI" } //Доступные области для этого ресурса
                }
            };

        //список приложений,у которых есть доступ
        public static IEnumerable<Client> Clients =>
            new List<Client>()
            {
                new Client()
                {
                    ClientId = "note-web-api",
                    ClientName = "Web API",
                    AllowedGrantTypes = GrantTypes.Code, //"authorization_code" запрашивает код авторизации для обмена на токен доступа
                    RequireClientSecret = false, //Не юзаем секрет клиента
                    RequirePkce = true, //нужен ключ подтверждения для кода авторизации
                    AllowAccessTokensViaBrowser = true, //Разрешаем передачу токена доступа через браузер
                    AllowedScopes = //Доступные области
                    {
                        "NotesWebAPI",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    },
                    RedirectUris =
                    {
                        "http://localhost:3000/signin-oidc" //Сюда будет перенаправлять после аутентификации клиентского приложения
                    },
                    AllowedCorsOrigins =  //разрешаем корсы для определенных URI
                    {
                        "http://localhost:3000/"
                    },
                    PostLogoutRedirectUris = //Редирект после логаута
                    {
                        "http://localhost:3000/signout-oidc"
                    }
                }
            };
    }
}
