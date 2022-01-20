// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AdminHubNC6.Areas.Identity.Data;
using AdminHubNC6.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminHubNC6.Areas.Identity.Pages.Account.Manage
{
    public class EditUserInfoModel : PageModel
    {
        private readonly UserManager<AdminHubUser> _userManager;
        private readonly SignInManager<AdminHubUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AuthLocalizationService _authLocalizer;

        public EditUserInfoModel(
            UserManager<AdminHubUser> userManager,
            SignInManager<AdminHubUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            AuthLocalizationService authLocalizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _authLocalizer = authLocalizer;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Required]
            [PersonalData]
            [Column(TypeName = "nvarchar(100)")]
            public string FirstName { get; set; }

            [Required]
            [PersonalData]
            [Column(TypeName = "nvarchar(100)")]
            public string LastName { get; set; }

            [Required]
            [PersonalData]
            [Column(TypeName = "nvarchar(100)")]
            public string UserPost { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            public string RoleName { get; set; }
            public string SelectedUid { get; set; }
        }

        private async Task LoadAsync(AdminHubUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            Debug.WriteLine("PhoneNumber:");
            Debug.WriteLine(phoneNumber);

            Username = userName;

            Input = new InputModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserPost = user.UserPost,
                PhoneNumber = phoneNumber,
                RoleName = roles.ElementAt(0)
            };
        }

        public async Task<IActionResult> OnGetAsync(string selectedUid = null)
        {
            //var user = await _userManager.GetUserAsync(User);
            //if (user == null)
            //{
            //    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            //}

            //await LoadAsync(user);
            //return Page();

            AdminHubUser user;
                        
            ViewData["roles"] = _roleManager.Roles.ToList();
            ViewData["selectedUid"] = selectedUid;
            ViewData["selectedRoleId"] = "";

            System.Diagnostics.Debug.WriteLine("selectedUid:" + selectedUid);

            if (selectedUid == null)
            {
                user = await _userManager.GetUserAsync(User);
            }
            else
            {
                user = await _userManager.FindByIdAsync(selectedUid);
            }

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            else //my added
            {
                var roles = await _userManager.GetRolesAsync(user);
                var rol = await _roleManager.FindByNameAsync(roles.ElementAtOrDefault(0));
                /*System.Diagnostics.Debug.WriteLine("Role ID:"+ rol.Id);
                System.Diagnostics.Debug.WriteLine("Role NAME:" + rol.Name );*/
                ViewData["selectedRoleId"] = rol.Id;
            }

            await LoadAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ViewData["selectedUid"] = Input.SelectedUid;

            AdminHubUser user;
            if (Input.SelectedUid == "")
            {
                user = await _userManager.GetUserAsync(User);
            }
            else
            {
                user = await _userManager.FindByIdAsync(Input.SelectedUid);
            }
                        
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            if (Input.FirstName != user.FirstName)
            {
                user.FirstName = Input.FirstName;
            }

            if (Input.LastName != user.LastName)
            {
                user.LastName = Input.LastName;
            }

            if (Input.UserPost != user.UserPost)
            {
                user.UserPost = Input.UserPost;
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);           
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
                else { 
                    user.PhoneNumber = Input.PhoneNumber;
                }
            }

            var result = await _userManager.UpdateAsync(user);
            Debug.WriteLine("User");
            Debug.WriteLine(user);

            if (result.Succeeded) //update role info if user detail updated successfully
            {
                
                var currentRole = await _userManager.GetRolesAsync(user);
                var role = _roleManager.FindByIdAsync(Input.RoleName).Result;

                Debug.WriteLine("currentRole:");
                Debug.WriteLine(currentRole.ElementAtOrDefault(0));
                Debug.WriteLine("role:");
                Debug.WriteLine(role);

                await _userManager.RemoveFromRoleAsync(user, currentRole.ElementAtOrDefault(0));
                await _userManager.AddToRoleAsync(user, role.Name);

                StatusMessage = _authLocalizer.GetLocalizedHtmlString("STATUS_PROFILE_UPDATED");
            }
            else
            {
                StatusMessage = _authLocalizer.GetLocalizedHtmlString("STATUS_PROFILE_UPDATED_FAILURE");
            }

            //await _signInManager.RefreshSignInAsync(user);
            //StatusMessage = "Your profile has been updated";

            return RedirectToPage("EditUserInfo", new { selectedUid = Input.SelectedUid });
        }
    }
}
