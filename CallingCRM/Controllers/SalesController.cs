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
    public class SalesController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public UserServices _userService = new UserServices();
        public CallService _callService = new CallService();
        public SalesService _saleService = new SalesService();
        public TemplateServices _templateServices = new TemplateServices();
        public TelecallerServices _telecallerServices = new TelecallerServices();
        public ManagerServices _managerService = new ManagerServices();
        static public CommonHelper _helper = new CommonHelper();

        static DateTime thisDateTime = _helper.GetCurrentDate();

        #region Dependencies
        public SalesController()
        {
        }

        public SalesController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        [Authorize(Roles = "Manager,Agent")]
        public ActionResult Candidates()
        {
            var UserId = User.Identity.GetUserId();
            var Role = User.IsInRole("Manager"); 

            ViewBag.Candidates = _saleService.GetSalesHistoryLists(UserId, Role);
            ViewBag.Role = Role;

            return View();
        }
        [Authorize(Roles = "Manager,Agent")]
        [Route("/sales/sendemail/{Id}")]
        public ActionResult SendEMail(Int64 Id)
        {
            var UserId = User.Identity.GetUserId();
            var UserName = User.Identity.GetUserName();
            
            var lead = _callService.GetLeadById(Id);
            var agent = _userService.getUserByUsername(UserName);
            var role = "Admin";
            if (User.IsInRole("Agent")) { role = "Agent"; } else if(User.IsInRole("Manager")) { role = "Manager"; }
            var id = agent.AdminId;
            var Callers = _telecallerServices.GetTelecallersListByManagerId(UserId);
            List<SelectListItem> Caller = new List<SelectListItem>();
            Caller.Add(new SelectListItem() { Text = "Select Caller", Value = "0" });
            foreach (TelecallerList result in Callers)
            {
                Caller.Add(new SelectListItem()
                {
                    Text = result.Email,
                    Value = result.id
                });

            }
            var Template = _templateServices.GetTemplateListByUser(id, role);
            List<SelectListItem> template = new List<SelectListItem>();
            template.Add(new SelectListItem() { Text = "Select Template", Value = "" });

            var subjectList = "";
            var BodyList = "";
            foreach (EmailTemplate result in Template)
            {
                template.Add(new SelectListItem()
                {
                    Text = result.TemplateName,
                    Value = result.Id.ToString()
                });


                subjectList = subjectList + "var eSubject_" + result.Id.ToString() + " = '" + result.EmailSubject + "';";
                BodyList = BodyList + "var eBody_" + result.Id.ToString() + " = '<div>" + result.EmailBody.Replace("'", "\"") + "</div>';";
            }
            ViewBag.Subject = subjectList;
            ViewBag.body = BodyList;
            ViewBag.Callers = Caller;
            if (lead.AssignedToUser == User.Identity.GetUserId() || User.IsInRole("Manager"))
            {
                ViewBag.ThisLead = lead;
                ViewBag.ThisAgent = agent;
            }
            ViewBag.Template = template;
            //_callService.UpdateCallLog(call);

            return View();
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult SendEMail(SendEmailViewModels email)
        {
            var form = Request.Form;

            var UserName = User.Identity.GetUserName();
            var agent = _userService.getUserByUsername(UserName);

            var leadId = Request.Form["LeadId"];
            var fromName = "Telecalling CRM";
            var fromEmail = Request.Form["sender"];
            var toEmail = Request.Form["Email"];
            var subject = Request.Form["Subject"];
            var body = Request.Form["Body"];

            var message = _helper.SendSMTPMail(fromName, fromEmail, toEmail, subject, body);

            if (message != "Success")
            {
                Session["MessageTStatus"] = message;
                return new RedirectResult("/Sales/SendEMail/" + leadId);
            }

            return new RedirectResult("/Sales/Candidates/");
            //return View();
        }

        [Authorize(Roles = "Manager,Agent")]
        public ActionResult PendingTraining()
        {
            return View();
        }

        public ActionResult PendingPractice()
        {
            return View();
        }

        public ActionResult Live()
        {
            return View();
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