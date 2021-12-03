using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CallingCRM.Models;
using CallingCRM.Services;
using CallingCRM.Helpers;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace CallingCRM.Controllers
{
    [Authorize]
    public class FollowupsController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public UserServices _userService = new UserServices();
        public CallService _callService = new CallService();
        static public CommonHelper _helper = new CommonHelper();

        static DateTime thisDateTime = _helper.GetCurrentDate();

        public FollowupsController()
        {
        }

        public FollowupsController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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


        /// <summary>
        /// Methods starts below
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Manager,Agent")]
        public ActionResult PendingOld()
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();

            var history = _callService.GetFollowups("Pending", UserId, User.IsInRole("Manager"));

            ViewBag.CallHistoryList = history;

            return View();
        }

        [Authorize(Roles = "Manager,Agent")]
        public ActionResult Todays()
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();

            var history = _callService.GetFollowups("Today", UserId, User.IsInRole("Manager"));

            ViewBag.CallHistoryList = history;

            return View();
        }

        [Authorize(Roles = "Manager,Agent")]
        public ActionResult Upcoming()
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();

            var history = _callService.GetFollowups("Upcoming", UserId, User.IsInRole("Manager"));

            ViewBag.CallHistoryList = history;

            return View();
        }

        /// <summary>
        /// Methods ends above
        /// </summary>


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