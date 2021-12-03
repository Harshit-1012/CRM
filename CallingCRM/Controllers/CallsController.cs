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
    public class CallsController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public UserServices _userService = new UserServices();
        public CallService _callService = new CallService();
        public TemplateServices _templateServices = new TemplateServices();
        static public CommonHelper _helper = new CommonHelper();

        static DateTime thisDateTime = _helper.GetCurrentDate();

        public CallsController()
        {
        }

        public CallsController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        [Authorize(Roles ="Agent")]
        public ActionResult Start()
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            // check pending call

            var lead = _callService.GetLead(UserId);

            List<SelectListItem> LeadType = new List<SelectListItem>();
            LeadType.Add(new SelectListItem() { Text = "Raw Lead", Value = "Raw Lead" });
            LeadType.Add(new SelectListItem() { Text = "Hot Lead", Value = "Hot Lead" });
            LeadType.Add(new SelectListItem() { Text = "Cold Lead", Value = "Cold Lead" });
            LeadType.Add(new SelectListItem() { Text = "Warm Lead", Value = "Warm Lead" });
            LeadType.Add(new SelectListItem() { Text = "Qualified Lead", Value = "Qualified Lead" });
            LeadType.Add(new SelectListItem() { Text = "Sales Ready Lead", Value = "Sales Ready Lead" });
            LeadType.Add(new SelectListItem() { Text = "Sales Qualified Lead", Value = "Sales Qualified Lead" });

            List<SelectListItem> LeadStatus = new List<SelectListItem>();
            LeadStatus.Add(new SelectListItem() { Text = "Raw", Value = "Raw" });
            LeadStatus.Add(new SelectListItem() { Text = "Open", Value = "Open" });
            LeadStatus.Add(new SelectListItem() { Text = "Contacted", Value = "Contacted" });
            LeadStatus.Add(new SelectListItem() { Text = "Working", Value = "Working" });
            LeadStatus.Add(new SelectListItem() { Text = "Unqualified", Value = "Unqualified" });
            LeadStatus.Add(new SelectListItem() { Text = "Closed", Value = "Closed" });

            List<SelectListItem> CallStatus = new List<SelectListItem>();
            CallStatus.Add(new SelectListItem() { Text = "---", Value = "" });
            CallStatus.Add(new SelectListItem() { Text = "Invalid Number (Number does not exist)", Value = "Invalid Number" });
            CallStatus.Add(new SelectListItem() { Text = "International Number", Value = "International Number" });
            CallStatus.Add(new SelectListItem() { Text = "Number Switched Off/Not Reachable", Value = "Number Switched Off" });
            CallStatus.Add(new SelectListItem() { Text = "Busy (Call is engaged)", Value = "Busy" });
            CallStatus.Add(new SelectListItem() { Text = "Not Picked (Ringing)", Value = "Not Picked" });
            CallStatus.Add(new SelectListItem() { Text = "Language Barrier", Value = "Language Barrier" });
            CallStatus.Add(new SelectListItem() { Text = "Not Interested (No need job)", Value = "Not Interested" });
            CallStatus.Add(new SelectListItem() { Text = "Not Interested (Can not pay)", Value = "Not Interested (Payment Issue)" });
            CallStatus.Add(new SelectListItem() { Text = "Not Interested (Computer not available)", Value = "Not Interested (Computer not available)" });
            CallStatus.Add(new SelectListItem() { Text = "DND Requested", Value = "DND Requested" });
            CallStatus.Add(new SelectListItem() { Text = "Interested - Call Back Later", Value = "Interested - Call Back Later" });
            CallStatus.Add(new SelectListItem() { Text = "Interested - Details Asked", Value = "Interested - Details Asked" });
            CallStatus.Add(new SelectListItem() { Text = "Paid", Value = "Interested - Paid" });

            ViewBag.ThisLead = lead;
            ViewBag.LeadType = LeadType;
            ViewBag.LeadStatus = LeadStatus;
            ViewBag.CallStatus = CallStatus;

            return View();
        }

        [Authorize(Roles = "Agent")]
        [HttpPost]
        public ActionResult Start(StartCallsViewModels call)
        {
            if (call.LeadId != null)
            {
                _callService.UpdateCallLog(call);

                if (call.SendEmail)
                {
                    return new RedirectResult("/Calls/SendEmail/" + call.LeadId);
                }
            }

            return new RedirectResult("/Calls/Start/");

            //return View();
        }

        [Route("/calls/follow/{Id?}")]
        public ActionResult Follow(Int64? Id)
        {
            if (Id == null || Id.ToString() == "") { Id = 0; }

            var UserId = AuthenticationManager.User.Identity.GetUserId();
            // check pending call

            var lead = _callService.GetLeadById(Convert.ToInt64(Id));
            var callLog = _callService.GetLeadFolloupsByLeadId(Convert.ToInt64(Id));
            var lastCall = _callService.GetCallHistoryByLeadId(Convert.ToInt64(Id)).FirstOrDefault();

            List<SelectListItem> LeadType = new List<SelectListItem>();
            LeadType.Add(new SelectListItem() { Text = "Raw Lead", Value = "Raw Lead" });
            LeadType.Add(new SelectListItem() { Text = "Hot Lead", Value = "Hot Lead" });
            LeadType.Add(new SelectListItem() { Text = "Cold Lead", Value = "Cold Lead" });
            LeadType.Add(new SelectListItem() { Text = "Warm Lead", Value = "Warm Lead" });
            LeadType.Add(new SelectListItem() { Text = "Qualified Lead", Value = "Qualified Lead" });
            LeadType.Add(new SelectListItem() { Text = "Sales Ready Lead", Value = "Sales Ready Lead" });
            LeadType.Add(new SelectListItem() { Text = "Sales Qualified Lead", Value = "Sales Qualified Lead" });

            List<SelectListItem> LeadStatus = new List<SelectListItem>();
            LeadStatus.Add(new SelectListItem() { Text = "Raw", Value = "Raw" });
            LeadStatus.Add(new SelectListItem() { Text = "Open", Value = "Open" });
            LeadStatus.Add(new SelectListItem() { Text = "Contacted", Value = "Contacted" });
            LeadStatus.Add(new SelectListItem() { Text = "Working", Value = "Working" });
            LeadStatus.Add(new SelectListItem() { Text = "Unqualified", Value = "Unqualified" });
            LeadStatus.Add(new SelectListItem() { Text = "Closed", Value = "Closed" });

            List<SelectListItem> CallStatus = new List<SelectListItem>();
            CallStatus.Add(new SelectListItem() { Text = "---", Value = "" });
            CallStatus.Add(new SelectListItem() { Text = "Invalid Number (Number does not exist)", Value = "Invalid Number" });
            CallStatus.Add(new SelectListItem() { Text = "International Number", Value = "International Number" });
            CallStatus.Add(new SelectListItem() { Text = "Number Switched Off/Not Reachable", Value = "Number Switched Off" });
            CallStatus.Add(new SelectListItem() { Text = "Busy (Call is engaged)", Value = "Busy" });
            CallStatus.Add(new SelectListItem() { Text = "Not Picked (Ringing)", Value = "Not Picked" });
            CallStatus.Add(new SelectListItem() { Text = "Language Barrier", Value = "Language Barrier" });
            CallStatus.Add(new SelectListItem() { Text = "Not Interested (No need job)", Value = "Not Interested" });
            CallStatus.Add(new SelectListItem() { Text = "Not Interested (Can not pay)", Value = "Not Interested (Payment Issue)" });
            CallStatus.Add(new SelectListItem() { Text = "Not Interested (Computer not available)", Value = "Not Interested (Computer not available)" });
            CallStatus.Add(new SelectListItem() { Text = "DND Requested", Value = "DND Requested" });
            CallStatus.Add(new SelectListItem() { Text = "Interested - Call Back Later", Value = "Interested - Call Back Later" });
            CallStatus.Add(new SelectListItem() { Text = "Interested - Details Asked", Value = "Interested - Details Asked" });
            CallStatus.Add(new SelectListItem() { Text = "Paid", Value = "Interested - Paid" });

            ViewBag.ThisLead = lead;
            ViewBag.LeadType = LeadType;
            ViewBag.LeadStatus = LeadStatus;
            ViewBag.CallStatus = CallStatus;
            ViewBag.CallLog = callLog;
            ViewBag.LastCall = lastCall;


            if (lead == null) 
            {
                //return new RedirectResult("/calls/start");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Follow(StartCallsViewModels call)
        {
            if (call.LeadId != null)
            {
                _callService.UpdateFollowupLog(call);

                if (call.SendEmail)
                {
                    return new RedirectResult("/Calls/SendEmail/" + call.LeadId);
                }
            }

            //return new RedirectResult("/Calls/follow/" + call.LeadId);
            return new RedirectResult("/Followups/Todays");

            //return View();
        }


        [Route("/calls/log/{Id}")]
        public ActionResult Log(Int64 Id)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            // check pending call

            var lead = _callService.GetLeadById(Id);
            var callLog = _callService.GetLeadFolloupsByLeadId(Id);
            var lastCall = _callService.GetCallHistoryByLeadId(Id).FirstOrDefault();
            var seniorLog = _callService.GetForwardedCallsByLeadId(Id);

            ViewBag.ThisLead = lead;
            ViewBag.CallLog = callLog;
            ViewBag.LastCall = lastCall;
            ViewBag.seniorLog = seniorLog;

            return View();
        }

        [Authorize(Roles = "Agent")]
        [Route("/calls/sendemail/{Id}")]
        public ActionResult SendEMail(Int64 Id)
        {
            var UserId = User.Identity.GetUserId();
            var UserName = User.Identity.GetUserName();
            var role = "Admin";
            if (User.IsInRole("Agent")) { role = "Agent"; } else if (User.IsInRole("Manager")) { role = "Manager"; }

            var lead = _callService.GetLeadById(Id);
            var agent = _userService.getUserByUsername(UserName);
            var id = agent.AdminId;
            var Template = _templateServices.GetTemplateListByUser(id, role);
            List<SelectListItem> template = new List<SelectListItem>();
            template.Add(new SelectListItem() { Text = "Select Template", Value = "0" });

            var subjectList = "";
            var BodyList = "";
            foreach (EmailTemplate result in Template)
            {
                template.Add(new SelectListItem()   
                {
                    Text = result.TemplateName,
                    Value = result.Id.ToString()
                });


                subjectList = subjectList + "var eSubject_"+ result.Id.ToString() +" = '"+ result.EmailSubject +"';";
                BodyList = BodyList + "var eBody_" + result.Id.ToString() + " = '<div>" + result.EmailBody.Replace("'", "\"") +"</div>';";
            }
            ViewBag.Subject = subjectList;
            ViewBag.body = BodyList;
            if (lead.AssignedToUser == User.Identity.GetUserId()) 
            {
                ViewBag.ThisLead = lead;
                ViewBag.ThisAgent = agent;
            }
            ViewBag.Template = template;
            //_callService.UpdateCallLog(call);

            return View();
        }

        [Authorize(Roles = "Agent")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult SendEMail(SendEmailViewModels email)
        {
            var form = Request.Form;

            //if (!ModelState.IsValid)
            //{
            //    return new RedirectResult("/Calls/SendEMail/" + email.LeadId);
            //}

            var UserName = User.Identity.GetUserName();
            var agent = _userService.getUserByUsername(UserName);

            var leadId = Request.Form["LeadId"];
            var fromName = agent.FullName;
            var fromEmail = agent.UserName;
            var toEmail = Request.Form["Email"];
            var subject = Request.Form["Subject"];
            var body = Request.Form["Body"];

            var message = _helper.SendSMTPMail(fromName, fromEmail, toEmail, subject, body);

            if (message != "Success")
            {
                Session["MessageTStatus"] = message;
                return new RedirectResult("/Calls/SendEMail/" + leadId);
            }

            return new RedirectResult("/Calls/Start/");
            //return View();
        }

        [Authorize(Roles = "Manager,Agent")]
        public ActionResult History()
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();

            ViewBag.StartDate = _helper.GetCurrentDate();
            ViewBag.EndDate = _helper.GetCurrentDate();

            var role = "Agent";
            if (User.IsInRole("Manager")) { role = "Manager"; }

            var history = _callService.GetCallHistory(ViewBag.StartDate, ViewBag.EndDate, "", UserId, role);

            ViewBag.CallHistoryList = history;

            return View();
        }


        [Authorize(Roles = "Manager,Agent")]
        [HttpPost]
        public ActionResult History(DatePickerModel range)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
              
            ViewBag.StartDate = range.start;
            ViewBag.EndDate = range.end;

            var role = "Agent";
            if (User.IsInRole("Manager")) { role = "Manager"; }

            var history = _callService.GetCallHistory(ViewBag.StartDate, ViewBag.EndDate, "", UserId, role);

            ViewBag.CallHistoryList = history;

            return View();
        }

        [Authorize(Roles = "Manager,Agent")]
        public ActionResult Unresponsive()
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();

            ViewBag.StartDate = _helper.GetCurrentDate();
            ViewBag.EndDate = _helper.GetCurrentDate();

            var role = "Agent";
            if (User.IsInRole("Manager")) { role = "Manager"; }

            var history = _callService.GetCallHistory(ViewBag.StartDate, ViewBag.EndDate, "Unresponsive", UserId, role);

            ViewBag.CallHistoryList = history;

            return View();
        }

        [Authorize(Roles = "Manager,Agent")]
        [HttpPost]
        public ActionResult Unresponsive(DatePickerModel range)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();

            ViewBag.StartDate = range.start;
            ViewBag.EndDate = range.end;

            var role = "Agent";
            if (User.IsInRole("Manager")) { role = "Manager"; }

            var history = _callService.GetCallHistory(ViewBag.StartDate, ViewBag.EndDate, "Unresponsive", UserId, role);

            ViewBag.CallHistoryList = history;

            return View();
        }

        [Authorize(Roles = "Manager,Agent")]
        public ActionResult NotInterested()
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();

            ViewBag.StartDate = _helper.GetCurrentDate();
            ViewBag.EndDate = _helper.GetCurrentDate();

            var role = "Agent";
            if (User.IsInRole("Manager")) { role = "Manager"; }

            var history = _callService.GetCallHistory(ViewBag.StartDate, ViewBag.EndDate, "NotInterested", UserId, role);

            ViewBag.CallHistoryList = history;

            return View();
        }

        [Authorize(Roles = "Manager,Agent")]
        [HttpPost]
        public ActionResult NotInterested(DatePickerModel range)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();

            ViewBag.StartDate = range.start;
            ViewBag.EndDate = range.end;

            var role = "Agent";
            if (User.IsInRole("Manager")) { role = "Manager"; }

            var history = _callService.GetCallHistory(ViewBag.StartDate, ViewBag.EndDate, "NotInterested", UserId, role);

            ViewBag.CallHistoryList = history;

            return View();
        }

        [Authorize(Roles = "Manager,Agent")]
        public ActionResult Interested()
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();

            ViewBag.StartDate = _helper.GetCurrentDate();
            ViewBag.EndDate = _helper.GetCurrentDate();

            var role = "Agent";
            if (User.IsInRole("Manager")) { role = "Manager"; }

            var history = _callService.GetCallHistory(ViewBag.StartDate, ViewBag.EndDate, "Interested", UserId, role);

            ViewBag.CallHistoryList = history;

            return View();
        }

        [Authorize(Roles = "Manager,Agent")]
        [HttpPost]
        public ActionResult Interested(DatePickerModel range)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();

            ViewBag.StartDate = range.start;
            ViewBag.EndDate = range.end;

            var role = "Agent";
            if (User.IsInRole("Manager")) { role = "Manager"; }

            var history = _callService.GetCallHistory(ViewBag.StartDate, ViewBag.EndDate, "Interested", UserId, role);

            ViewBag.CallHistoryList = history;

            return View();
        }
        
        [Route("/calls/forward/{Id}")]
        public ActionResult Forward(Int64 Id)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            
            var ThisLead = _callService.GetLeadById(Id);

            ForwardCallViewModel lead = new ForwardCallViewModel();

            lead.Name = ThisLead.FullName;
            lead.Number = ThisLead.Mobile;
            lead.lead = ThisLead;
            

            return View(lead);
        }

        [HttpPost]
        public ActionResult Forward(ForwardCallViewModel call)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            var Manager = _callService.ManagerbyId(UserId);
            LeadForwarded lead = new LeadForwarded();

            lead.LeadId = call.lead.Id;
            lead.ForwardedTo = Manager.ManagerId;
            lead.ForwardDate = _helper.GetCurrentDate();
            lead.ForwardReason = call.ForwardReason;
            lead.NextReminderDate = call.WhenToFollowup;

            Session["MessageTStatus"] = "";

            var result = _callService.AddForwardedCall(lead);

            if (result != "Success")
            {
                Session["MessageTStatus"] = result;
                return new RedirectResult("/calls/Follow/" + call.lead.Id);
            }

            //return RedirectToAction("/calls/Follow/" + call.lead.Id);
            return new RedirectResult("/Followups/Todays");
        }

        [Authorize(Roles = "Agent")]
        [Route("/calls/forwarded/{Id}")]
        public ActionResult Forwarded(string Id)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();

            List<LeadForwarded> leads = new List<LeadForwarded>();

            ViewBag.StartDate = _helper.GetCurrentDate();
            ViewBag.EndDate = _helper.GetCurrentDate();
            ViewBag.Status = Id;

            leads = _callService.GetForwardedLeads(ViewBag.StartDate, ViewBag.EndDate, UserId, Id);

            ViewBag.LeadForwarded = leads;

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