#nullable disable

using AdminHubNC6.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Diagnostics;

namespace AdminHubNC6.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IStringLocalizer<HomeController> _localizer;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(ILogger<HomeController> logger, IStringLocalizer<HomeController> localizer, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _localizer = localizer;
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index(string lang = null, string returnUrl = null)
        {
            string url = "~/";

            if (lang != null)
            {
                string culture;
                if (lang == "zh")
                {
                    culture = "zh-Hant";
                }
                else if (lang == "sc")
                {
                    culture = "zh-Hans";
                }
                else
                {
                    culture = "en";
                }

                Debug.WriteLine("Culture:" + culture);

                Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                    new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
                );

                if (returnUrl != null)
                {
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    return LocalRedirect(url);
                }
            }
            else
            {
                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl, string selectedUid)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            //Debug.WriteLine("culture:" + culture);
            //Debug.WriteLine("returnUrl:" + returnUrl);
            //Debug.WriteLine("selectedUid:" + selectedUid);

            if (selectedUid != "")
            {
                returnUrl = returnUrl + "?selectedUid=" + selectedUid;
            }

            return LocalRedirect(returnUrl);
        }
    }
}