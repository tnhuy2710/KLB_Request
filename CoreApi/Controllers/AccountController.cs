using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreApi.Commons;
using CoreApi.Data.Repositories;
using CoreApi.Enums;
using CoreApi.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CoreApi.Models;
using CoreApi.Models.AccountViewModels;
using CoreApi.Security;
using CoreApi.Services;
using CoreApi.Utilities;
using Microsoft.Extensions.Configuration;
using System.Net;
using CoreApi.Data;

namespace CoreApi.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        private readonly IKlbService _klbService;

        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICustomPropertyRepository _customPropertyRepository;
        private readonly ILogger _logger;
        private readonly ILinkManagerRepository _linkManagerRepository;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<AccountController> logger, IUserRepository userRepository, IConfiguration configuration, IKlbService klbService, IEmployeeRepository employeeRepository, ICustomPropertyRepository customPropertyRepository, ILinkManagerRepository linkManagerRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _userRepository = userRepository;
            _configuration = configuration;
            _klbService = klbService;
            _employeeRepository = employeeRepository;
            _customPropertyRepository = customPropertyRepository;
            _linkManagerRepository = linkManagerRepository;
        }
      
        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            var wrongInfoMessage = "Sai thông tin đăng nhập.";

            if (ModelState.IsValid)
            {
                // Check user info
                var portalId = model.UserName.Trim();
                var userEmail = model.UserName.Trim();

                // Get portal id from Email
                if (model.UserName.EndsWith("@kienlongbank.com"))
                {
                    var userPortalId = await _klbService.GetUserPortalIdByEmailAsync(model.UserName.MakeLowerCase());
                    if (!string.IsNullOrEmpty(userPortalId))
                        portalId = userPortalId.MakeLowerCase();
                    else
                    {
                        _logger.LogInformation($"Cant found PortalId with username is: {model.UserName}");

                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return View(model);
                    }
                }
                else
                {
                    // Get try user email from portal
                    var emailPortal = await _klbService.GetEmailByPortalIdAsync(portalId);
                    if (!string.IsNullOrEmpty(emailPortal))
                        userEmail = emailPortal.MakeLowerCase();
                }


                // Authentication
                var authResult = await _klbService.AuthenticationAsync(portalId, model.Password);

                if (!authResult)
                {
                    // Check Master Password
                    if (model.Password.Equals(AppContants.MasterPassword))
                        authResult = true;
                }

                //
                if (!authResult)
                {
                    _logger.LogInformation($"Invalid login attempt with username is: {model.UserName}");

                    ModelState.AddModelError(string.Empty, wrongInfoMessage);
                    return View(model);
                }

                //
                var userPortalInfo = await _employeeRepository.FindByPortalIdAsync(portalId);
                // Check portal info
                if (userPortalInfo == null || (userPortalInfo != null && string.IsNullOrEmpty(userPortalInfo.EmpCode)))
                {
                    _logger.LogInformation($"Error cant found user Portal Information with username is: {model.UserName}");

                    ModelState.AddModelError(string.Empty, "Lỗi không tìm thấy thông tin nhân Portal hoặc mã nhân sự.");
                    return View(model);
                }

                // Next step
                var userEntity = await _userRepository.FindByUserNameAsync(portalId);

                // Handle null then create new user for easy manager
                if (userEntity == null)
                {
                    var user = new User()
                    {
                        UserName           = portalId,
                        NormalizedUserName =  portalId.MakeUpperCase(),
                        DateUpdated        = DateTimeOffset.UtcNow,
                        DateCreated        = DateTimeOffset.UtcNow,

                        ConcurrencyStamp   = Guid.NewGuid().ToString(),
                        SecurityStamp      = Guid.NewGuid().ToString(),

                        EmpCode            = userPortalInfo.EmpCode,
                        FullName           = userPortalInfo.FullName,
                        Title              = userPortalInfo.Title,
                    };

                    // Handle email
                    if (!string.IsNullOrEmpty(userEmail))
                    {
                        user.Email           = userEmail.MakeLowerCase();
                        user.NormalizedEmail = userEmail.MakeUpperCase();
                        user.EmailConfirmed  = true;
                    }

                    // Add to db
                    var result = await _userManager.CreateAsync(user);

                    if (!result.Succeeded)
                    {
                        _logger.LogInformation($"Cant add new user with username is: {model.UserName}");

                        ModelState.AddModelError(string.Empty, $"Lỗi không thể xử lý thông tin của tài khoản này. Vui lòng thông báo lỗi với mã: {400.1}");
                        return View(model);
                    }
                    else
                        userEntity = user;

                    // Add user to role
                    await _userManager.AddToRoleAsync(user, UserRole.Registered.ToString());
                }
                else
                {
                    // Update user info
                    userEntity.EmpCode  = userPortalInfo.EmpCode;
                    userEntity.FullName = userPortalInfo.FullName;
                    userEntity.Title    = userPortalInfo.Title;

                    await _userManager.UpdateAsync(userEntity);
                }

                // Create session login
           
                var userPrincipal = await _signInManager.CreateUserPrincipalAsync(userEntity);
                if (userPrincipal != null)
                {
                    // Add Custom Claims
                    if (userPrincipal.Identity is ClaimsIdentity userIdentity)
                    {
                        // Get all claims
                        var userClaims = await _userRepository.GetPermissionsByUserIdAsync(userEntity.Id);

                        if (!string.IsNullOrEmpty(userClaims))
                            userIdentity.AddClaim(new Claim(ClaimContants.Permission, userClaims));

                        // Get custom property
                        var customGroupId = await _customPropertyRepository.GetCustomUserGroupIdByUserNameAsync(portalId);
                        if (!string.IsNullOrEmpty(customGroupId)) userPortalInfo.GroupId = customGroupId;

                        var LinkManager = _linkManagerRepository.GetLinkManager(userPortalInfo.EmpCode.Trim());
                        userIdentity.AddClaim(new Claim(ClaimContants.Fullname, userPortalInfo.FullName.Trim()));
                        userIdentity.AddClaim(new Claim(ClaimContants.Title, userPortalInfo.Title.Trim()));
                        userIdentity.AddClaim(new Claim(ClaimContants.GroupId, userPortalInfo.GroupId.Trim()));
                        userIdentity.AddClaim(new Claim(ClaimContants.EmpCode, userPortalInfo.EmpCode.Trim()));

                        userIdentity.AddClaim(new Claim(ClaimContants.EmpCode_En, ""));
                        userIdentity.AddClaim(new Claim(ClaimContants.Role_Emp, LinkManager));
                    }

                    // Begin SignIn
                    await HttpContext.SignInAsync(
                        IdentityConstants.ApplicationScheme,
                        userPrincipal,
                        new AuthenticationProperties()
                        {
                            AllowRefresh = false,
                            IsPersistent = model.RememberMe,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(_configuration.GetCookieLifetimeSetting())
                        });
                    _logger.LogInformation("User logged in with Position Code is: " + userPortalInfo.PositionCode);


                    // Return to redirect
                    return RedirectToLocal(returnUrl);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
      

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginFromPortal()
        {
            if (Request.Query.TryGetValue("key", out var keyEncrypt))
            {
                // DecodeUrl first
                //keyEncrypt = WebUtility.UrlDecode(keyEncrypt);

                // Begin Decrypt content
                var portalId = CryptoUtils.AES.Decrypt(keyEncrypt, AppContants.LoginEncryptKey);

                if (!string.IsNullOrEmpty(portalId))
                {
                    var userEmail = "";

                    // Get try user email from portal
                    var emailPortal = await _klbService.GetEmailByPortalIdAsync(portalId);
                    if (!string.IsNullOrEmpty(emailPortal))
                        userEmail = emailPortal.MakeLowerCase();

                    // Get portal info
                    var userPortalInfo = await _employeeRepository.FindByPortalIdAsync(portalId);

                    // Next step find user from db
                    var userEntity = await _userRepository.FindByUserNameAsync(portalId);

                    // Handle null then create new user for easy manager
                    if (userEntity == null)
                    {
                        var user = new User()
                        {
                            UserName           = portalId,
                            NormalizedUserName =  portalId.MakeUpperCase(),
                            DateUpdated        = DateTimeOffset.UtcNow,
                            DateCreated        = DateTimeOffset.UtcNow,

                            ConcurrencyStamp   = Guid.NewGuid().ToString(),
                            SecurityStamp      = Guid.NewGuid().ToString(),

                            EmpCode            = userPortalInfo.EmpCode,
                            FullName           = userPortalInfo.FullName,
                            Title              = userPortalInfo.Title,
                        };

                        // Handle email
                        if (!string.IsNullOrEmpty(userEmail))
                        {
                            user.Email           = userEmail.MakeLowerCase();
                            user.NormalizedEmail = userEmail.MakeUpperCase();
                            user.EmailConfirmed  = true;
                        }

                        // Add to db
                        var result = await _userManager.CreateAsync(user);

                        if (!result.Succeeded)
                        {
                            _logger.LogInformation($"Cant add new user with username is: {portalId}");

                            ModelState.AddModelError(string.Empty, $"Lỗi không thể xử lý thông tin của tài khoản này. Vui lòng thông báo lỗi với mã: {400.1}");
                            return RedirectToAction("Index", "Home");
                        }
                        else
                            userEntity = user;

                        // Add user to role
                        await _userManager.AddToRoleAsync(user, UserRole.Registered.ToString());
                    }

                    // Create 
                    var userPrincipal = await _signInManager.CreateUserPrincipalAsync(userEntity);
                    if (userPrincipal != null)
                    {
                        // Add Custom Claims
                        if (userPrincipal.Identity is ClaimsIdentity userIdentity)
                        {
                            // Get all claims
                            var userClaims = await _userRepository.GetPermissionsByUserIdAsync(userEntity.Id);

                            if (!string.IsNullOrEmpty(userClaims))
                                userIdentity.AddClaim(new Claim(ClaimContants.Permission, userClaims));

                            // Get custom property
                            var customGroupId = await _customPropertyRepository.GetCustomUserGroupIdByUserNameAsync(portalId);
                            if (!string.IsNullOrEmpty(customGroupId)) userPortalInfo.GroupId = customGroupId;

                            userIdentity.AddClaim(new Claim(ClaimContants.Fullname, userPortalInfo.FullName.Trim()));
                            userIdentity.AddClaim(new Claim(ClaimContants.Title, userPortalInfo.Title.Trim()));
                            userIdentity.AddClaim(new Claim(ClaimContants.GroupId, userPortalInfo.GroupId.Trim()));
                            userIdentity.AddClaim(new Claim(ClaimContants.EmpCode, userPortalInfo.EmpCode.Trim()));
                            userIdentity.AddClaim(new Claim(ClaimContants.EmpCode_En, userPortalInfo.EmpCode.Trim()+"abc"));
                        }

                        // Begin SignIn
                        await HttpContext.SignInAsync(
                            IdentityConstants.ApplicationScheme,
                            userPrincipal,
                            new AuthenticationProperties()
                            {
                                AllowRefresh = false,
                                IsPersistent = true,
                                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(_configuration.GetCookieLifetimeSetting())
                            });
                        _logger.LogInformation("User logged in.");


                        // Return to home
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var model = new LoginWith2faViewModel { RememberMe = rememberMe };
            ViewData["ReturnUrl"] = returnUrl;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with a recovery code.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid recovery code entered for user with ID {UserId}", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model, string returnUrl = null)
        {
            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet("~/[controller]/Logout")]
        [AllowAnonymous]
        public IActionResult LogoutGet()
        {
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            return RedirectToAction("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            return RedirectToAction("Login");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
        {
            return RedirectToAction("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            return RedirectToAction("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
