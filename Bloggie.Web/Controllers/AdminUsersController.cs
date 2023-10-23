using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace Bloggie.Web.Controllers
{
    //Saray functions pe alag alag authorization dene se behtar hai puri class ke constructor pe dedo
    //saray functions lelenge

    [Authorize(Roles = "Admin")]
    public class AdminUsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<IdentityUser> _userManager;
        public AdminUsersController(IUserRepository userRepository, UserManager<IdentityUser> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager; 
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var allUsers = await _userRepository.GetAll();

            var usersViewModel = new UserViewModel();
            usersViewModel.Users = new List<User>();

            foreach(var user in allUsers)
            {
                usersViewModel.Users.Add(new Models.ViewModels.User
                {
                    Id = Guid.Parse(user.Id),
                    Username = user.UserName,
                    EmailAddress = user.Email
                });        
            }

            return View(usersViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> List(UserViewModel userViewModel)
        {
            var IdentityUser = new IdentityUser()
            {
                UserName = userViewModel.UserName,
                Email = userViewModel.EmailAddress,
            };

            //Using UserManagerService Creating the User which we are created in IdentityUser just above
            var identityResult = await _userManager.CreateAsync(IdentityUser, userViewModel.Password);

            if(identityResult.Succeeded)
            {
                //Assign this user a role admin or user only.
                var roles = new List<string> { "User" };

                if(userViewModel.AdminRoleChecked == true)
                {
                    roles.Add("Admin");
                }

                //Add to Roles take IEnumerable<UserRoles>
                var roleIdentityResult = await _userManager.AddToRolesAsync(IdentityUser, roles);
                
                if(roleIdentityResult != null)
                {
                    return RedirectToAction("List", "AdminUsers");
                }

            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(Guid Id)
        {
            var user = await _userRepository.GetById(Id);
            
            if(user != null)
            {
                var identityResult = await _userManager.DeleteAsync(user);
                if(identityResult.Succeeded && identityResult is not null) 
                {
                    return RedirectToAction("List", "AdminUsers");
                }
            }
            return View();
        }
    }
}
