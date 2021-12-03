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
using System.Configuration;
using System.Threading;

namespace CallingCRM.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public UserServices _userService = new UserServices();
        public SalesService _salesService = new SalesService();
        public CallService _callService = new CallService();
        static public CommonHelper _helper = new CommonHelper();

        static DateTime thisDateTime = _helper.GetCurrentDate();

        public HomeController()
        {
        }

        public HomeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            //_userService = userService;
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

        [AllowAnonymous]
        public ActionResult AlwaysOn()
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();

            return View();
        }

        [Authorize]
        public ActionResult Index()
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();

            if (User.IsInRole("Manager"))
            {
                return new RedirectResult("/Manager");
            }

            HomeStatsModels stats = _userService.GetDashboardStats(UserId);
            ViewBag.TotalCalls = stats.TotalCalls;
            ViewBag.TotalLeads = stats.TotalLeads;
            ViewBag.TotalSales = stats.TotalSales;
            ViewBag.TodaysCall = stats.TodaysCalls;

            double TS = Convert.ToInt64(stats.TotalSales);
            double TSCount = 0;
            if (TS > 0)
            {
                TSCount = (int)TS / 999;
                if (TS % 999 > 0)
                {
                    TSCount = TSCount + 0.5;
                }
            }

            ViewBag.TotalSales = TSCount;

            var thisDateTime = _helper.GetCurrentDate();
            ViewBag.StartDate = Convert.ToDateTime(thisDateTime);
            ViewBag.EndDate = Convert.ToDateTime(thisDateTime);

            var dcr = _salesService.GetDCR(Convert.ToDateTime(ViewBag.StartDate), Convert.ToDateTime(ViewBag.EndDate), UserId);

            ViewBag.DCR = dcr;

            return View();
        }


        [Route("Search/{Id}")]
        public ActionResult Search(string Id)
        {
            Id = Id.Replace("-at-", "@");
            Id = Id.Replace("-dot-", ".");

            var UserId = AuthenticationManager.User.Identity.GetUserId();

            var leads = _callService.SearchLeadsByText(Id, UserId);

            ViewBag.SearchResult = leads;

            return View();
        }

        [AllowAnonymous]
        public ActionResult ThankYou()
        {
            return View();
        }

        // GET: /Register
        [AllowAnonymous]
        [Route("/Register/{id}")]
        public ActionResult Register(string id)
        {
            ViewBag.Step = 1;
            ViewBag.SponsorDisplay = id;

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Step = model.Step;
                ViewBag.SponsorNoEdit = "";

                var userData = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(userData, model.Password);
                if (result.Succeeded)
                {
                    var userProfile = new AspNetUser(); // _userService.CreateUserProfile(userData.Id, model);

                    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("thankyou", "Home");
                }
                AddErrors(result);
            }

            return View(model);
        }


        #region Helpers
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