// Areas/Identity/Pages/Account/Register.cshtml.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using YetenekPusulasi.Data; // ApplicationUser için
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace YetenekPusulasi.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore; // Bu enjekte ediliyor
        private readonly IUserEmailStore<ApplicationUser> _emailStore; // Bu _userStore'dan türetiliyor
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore, // Sadece IUserStore'u enjekte et
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _userStore = userStore; // _userStore atanıyor
            _emailStore = GetEmailStore(); // _emailStore burada GetEmailStore() ile atanıyor
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Lütfen bir kullanıcı rolü seçin.")]
            [Display(Name = "Kullanıcı Rolü")]
            public string UserRole { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            var roles = _roleManager.Roles
                                    .Where(r => r.Name == "Teacher" || r.Name == "Student")
                                    .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
                                    .ToList();
            ViewData["Roles"] = new SelectList(roles, "Value", "Text");
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                // _emailStore'un null olup olmadığını kontrol et (GetEmailStore doğru çalıştıysa null olmamalı)
                if (_emailStore == null)
                {
                    // Bu durum GetEmailStore'da bir sorun olduğunu gösterir, logla veya hata ver.
                    _logger.LogError("IUserEmailStore could not be obtained from IUserStore.");
                    ModelState.AddModelError(string.Empty, "An internal error occurred with email setup.");
                    await ReloadRolesForView(Input.UserRole);
                    return Page();
                }
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None); // Hatanın olduğu satır
                
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    // ... (Rol atama ve e-posta gönderme kodları) ...
                    if (!string.IsNullOrEmpty(Input.UserRole))
                    {
                        if (await _roleManager.RoleExistsAsync(Input.UserRole))
                        {
                            await _userManager.AddToRoleAsync(user, Input.UserRole);
                        } // else log warning
                    } // else assign default or log

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            await ReloadRolesForView(Input.UserRole);
            return Page();
        }

        private async Task ReloadRolesForView(string selectedRole = null)
        {
             var roles = _roleManager.Roles
                                .Where(r => r.Name == "Teacher" || r.Name == "Student")
                                .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
                                .ToList();
            ViewData["Roles"] = new SelectList(roles, "Value", "Text", selectedRole);
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        private ApplicationUser CreateUser()
        {
            try { return Activator.CreateInstance<ApplicationUser>(); }
            catch { throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. "); }
        }

        // --- BU METOT ÇOK ÖNEMLİ ---
        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            // _userStore, IUserStore<ApplicationUser> olarak enjekte edildi.
            // Bu user store'un IUserEmailStore<ApplicationUser> arayüzünü de implemente etmesi beklenir.
            // EntityFrameworkUserStore bunu yapar.
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
        // --- METODUN SONU ---
    }
}