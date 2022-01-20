using AdminHubNC6.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;

namespace InfoHub.Controllers
{
    public class UserManagementController : Controller
    {
        private UserManager<AdminHubUser> UserManager;
        private readonly ILogger<UserManagementController> _logger;

        public UserManagementController(UserManager<AdminHubUser> userManager, ILogger<UserManagementController> logger)
        {
            this.UserManager = userManager;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index(bool? saveChangesError = false)
        {
            setMenu("USER");

            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }

            var users = UserManager.Users.ToList();
            return View(users);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string uid)
        {
            setMenu("USER");

            try
            {
                var adminUser = await UserManager.GetUserAsync(User);
                var user = await UserManager.FindByIdAsync(uid);
                var delUser = await UserManager.DeleteAsync(user);

                if (delUser.Succeeded)
                {
                    _logger.LogInformation("PC Info Deleted By:" + adminUser.Id + " [DATA]:" + JsonConvert.SerializeObject(user));
                }
            }
            catch (DataException dex)
            {
                _logger.LogError(dex.ToString());
                return RedirectToAction("Index", new { saveChangesError = true });
            }


            var users = UserManager.Users.ToList();
            return View(users);
        }

        private void setMenu(string currentPage = "", string SubMenu = "")
        {
            
        }
    }
}