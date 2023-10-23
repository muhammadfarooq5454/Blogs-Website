using Bloggie.Web.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bloggie.Web.Controllers
{
    public class AccountController : Controller
    {
        //Injecting UserManagerService Identity user ko manage krrhe hain (basically registration krwanay mai help krta hai)
        private readonly UserManager<IdentityUser> _UserManager;

        //Ye similarly sign in krwane mai madad krega
        private readonly SignInManager<IdentityUser> _SignInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _UserManager = userManager;
            _SignInManager = signInManager; 
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            //Server side validations
            if (ModelState.IsValid)
            {
                var IdentityUser = new IdentityUser()
                {
                    UserName = registerViewModel.Username,
                    Email = registerViewModel.Email,
                };

                //Using UserManagerService Creating the User which we are created in IdentityUser just above
                var identityResult = await _UserManager.CreateAsync(IdentityUser, registerViewModel.Password);

                if(identityResult.Succeeded)
                {
                    //Assign "User" role to newly created user
                    //Admin tu pehle hi create krdia tha AuthDbContext class mai

                    var roleIdentityResult = await _UserManager.AddToRoleAsync(IdentityUser, "User");

                    if (roleIdentityResult.Succeeded)
                    {
                        //show success notification
                        return RedirectToAction("Register");
                    }
                }
            }
            //show error notification 
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            
            if (!ModelState.IsValid)
            {
                return View();
            }
            //Backend par hi ye user id ke through match krlega database se aur agr milgya tu login krwadega jitne users hain database mai sabse check krega
            var signInResult = await _SignInManager.PasswordSignInAsync(loginViewModel.Username, loginViewModel.Password, false, false);

            if (signInResult != null && signInResult.Succeeded)
            {
                if (!string.IsNullOrEmpty(loginViewModel.ReturnUrl))
                {
                    //Show success Login
                    return RedirectToPage(loginViewModel.ReturnUrl);
                }
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("Password", "Incorrect Password");
            //Show Failure Login
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _SignInManager.SignOutAsync();

            return RedirectToAction("Login");   
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
