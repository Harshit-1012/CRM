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
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public UserServices _userService = new UserServices();
        public CallService _callService = new CallService();
        public SalesService _salesService = new SalesService();
        


        static public CommonHelper _helper = new CommonHelper();

        static DateTime thisDateTime = _helper.GetCurrentDate();

        #region Dependencies
        public ManagerController()
        {
        }

        public ManagerController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        #endregion

        // GET: Manager
        public ActionResult Index()
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();

            HomeStatsModels stats = _userService.GetDashboardStats(UserId);
            ViewBag.TotalCalls = stats.TotalCalls;
            ViewBag.TotalLeads = stats.TotalLeads;
            ViewBag.TotalSales = stats.TotalSales;
            ViewBag.TodaysCall = stats.TodaysCalls;

            return View();
        }

        public ActionResult Team()
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            var users = _userService.GetUsersListByManagerId(UserId);

            ViewBag.MyTeam = users;

            return View();
        }

        public ActionResult DCR()
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();

            var role = "Agent";
            if (User.IsInRole("Manager")) { role = "Manager"; }

            var thisDateTime = _helper.GetCurrentDate();

            ViewBag.StartDate = Convert.ToDateTime(thisDateTime);
            ViewBag.EndDate = Convert.ToDateTime(thisDateTime);

            var dcr = _salesService.GetDCR(Convert.ToDateTime(ViewBag.StartDate), Convert.ToDateTime(ViewBag.EndDate), UserId);

            ViewBag.DCR = dcr;

            return View();
        }

        [HttpPost]
        public ActionResult DCR(DatePickerModel range)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            var role = "Agent";
            if (User.IsInRole("Manager")) { role = "Manager"; }

            ViewBag.StartDate = range.start;
            ViewBag.EndDate = range.end;

            var dcr = _salesService.GetDCR(Convert.ToDateTime(ViewBag.StartDate), Convert.ToDateTime(ViewBag.EndDate), UserId);

            ViewBag.DCR = dcr;

            return View();
        }


        [Route("/calls/mycalls/{Id}")]
        public ActionResult MyCalls(string Id)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();

            if (Id == "") { Id = "pending"; }

            List<LeadForwarded> leads = new List<LeadForwarded>();

            ViewBag.StartDate = _helper.GetCurrentDate();
            ViewBag.EndDate = _helper.GetCurrentDate();
            ViewBag.Status = Id;

            leads = _callService.GetForwardedLeads(ViewBag.StartDate, ViewBag.EndDate, UserId, Id);

            ViewBag.LeadForwarded = leads;

            return View();
        }


        [Route("/manager/callaction/{Id}")]
        public ActionResult CallAction(long Id)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();

            var lead = _callService.GetForwardedLeadById(Id);

            return View(lead);
        }


        [HttpPost]
        public ActionResult CallAction(LeadForwarded lead)
        {
            try {
                var result = _callService.UpdateForwardedCallLog(lead);
            }
            catch (Exception ex) 
            {
                Session["eMessage"] = ex.Message;
            }

            return new RedirectResult("/manager/mycalls/pending");
        }


        public ActionResult AddLead()
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            var users = _userService.GetUsersListByManagerId(UserId);

            List<SelectListItem> list = new List<SelectListItem>();

            list.Add(new SelectListItem()
            {
                Text = "Select BDA",
                Value = ""
            });

            foreach (AspNetUser user in users)
            {
                list.Add(new SelectListItem() { 
                    Text = user.FullName,
                    Value  = user.Id
                });
            }

            ViewBag.BDAList = list;

            return View();
        }

        [HttpPost]
        public ActionResult AddLead(Lead lead)
        {
            try
            {
                var result = _callService.AddLead(lead);

                if (result != "")
                {
                    Session["eMessage"] = result;

                    var users = _userService.GetUsersListByRole();
                    List<SelectListItem> list = new List<SelectListItem>();

                    list.Add(new SelectListItem()
                    {
                        Text = "Select BDA",
                        Value = ""
                    });

                    foreach (AspNetUser user in users)
                    {
                        list.Add(new SelectListItem()
                        {
                            Text = user.FullName,
                            Value = user.Id
                        });
                    }

                    ViewBag.BDAList = list;

                    return View(lead);
                }

                Session["sMessage"] = "Lead has been added and assigned successfully.";
            }
            catch (Exception ex)
            {
                Session["eMessage"] = ex.Message;
            }

            return new RedirectResult("/manager/addlead");
        }

        

        //public ActionResult Team()
        //{
        //    return View();
        //}

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