using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Notes.AuthServer.Model;

namespace Notes.AuthServer.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<AppUser> signInManager; //проводит авторизацию
        private readonly UserManager<AppUser> userManager; //управвляет юзерами в бд
        private readonly IIdentityServerInteractionService interactionService;//Нужен будет для логаута.

        public AuthController(SignInManager<AppUser> _signInManager, UserManager<AppUser> _userManager,
            IIdentityServerInteractionService _interactionService)
        {
            signInManager = _signInManager;
            userManager = _userManager;
            interactionService = _interactionService;
        }


        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            var viewModel = new LoginViewModel()
            {
                ReturnUrl = returnUrl
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (!ModelState.IsValid) //валидируем модель
            {
                return View(viewModel);
            }
            //ищем юзера
            var user = await userManager.FindByNameAsync(viewModel.Username);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found");
                return View(viewModel);
            }

            var signResult = await signInManager.PasswordSignInAsync(viewModel.Username, viewModel.Password,
                false, false); //куки не храним, юзера не лочим
            if (signResult.Succeeded)
            {
                return Redirect(viewModel.ReturnUrl); //логин удачный - редиректим
            }

            ModelState.AddModelError(string.Empty, "Login error");
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Register(string returnUrl)
        {
            var vm = new RegisterViewModel()
            {
                ReturnUrl = returnUrl
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var user = new AppUser
            {
                UserName = viewModel.Username
            };

            var result = await userManager.CreateAsync(user,viewModel.Password);

            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, false); //при неудачных попытках не лочим
                return Redirect(viewModel.ReturnUrl);
            }
            ModelState.AddModelError(string.Empty, "Error occurred");
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            await signInManager.SignOutAsync();
            //Получаем урлу для логаута определенного пользователя
            var logoutRequest = await interactionService.GetLogoutContextAsync(logoutId);

            //Редиректим на post метод логаута по ссылке пользователя
            return Redirect(logoutRequest.PostLogoutRedirectUri);
        }

    }
}
