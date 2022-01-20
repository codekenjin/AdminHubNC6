using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InfoHub.Controllers
{
    public class RoleManagementController : Controller
    {
        private RoleManager<IdentityRole> RoleManager;

        public RoleManagementController(RoleManager<IdentityRole> roleManager)
        {
            this.RoleManager = roleManager;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            setMenu("ROLE");
            var roles = RoleManager.Roles.ToList();
            return View(roles);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            setMenu("CREATE NEW ROLE");
            return View(new IdentityRole());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IdentityRole role)
        {
            setMenu("CREATE NEW ROLE");
            await RoleManager.CreateAsync(role);
            return RedirectToAction("Index");
        }

        private void setMenu(string currentPage = "", string SubMenu = "")
        {
        }
    }
}