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
    [Authorize(Roles ="Admin, Manager")]
    public class ReportsController : Controller
    {
        public ReportServices _reportServices = new ReportServices();
        public TelecallerServices _telecallerServices = new TelecallerServices();
        static public CommonHelper _helper = new CommonHelper();
        static DateTime thisDateTime = _helper.GetCurrentDate();
        public TemplateServices _templateServices = new TemplateServices();
        public SalesService _saleService = new SalesService();

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult TeamReport()
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            ViewBag.StartDate = Convert.ToDateTime(thisDateTime);
            ViewBag.EndDate = Convert.ToDateTime(thisDateTime);
            var users = _telecallerServices.GetUsersListByRole(UserId);
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "All", Value = "0" });
            foreach (AspNetUser user in users)
            {
                list.Add(new SelectListItem()
                {
                    Text = user.FullName,
                    Value = user.Id
                });
            }
            ViewBag.Manager = list;

            var tr = _reportServices.GetTeamReport(Convert.ToDateTime(ViewBag.StartDate), Convert.ToDateTime(ViewBag.EndDate), UserId, "");

            ViewBag.TeamReport = tr;
            return View();
        }


        [HttpPost]
        public ActionResult TeamReport(DatePickerModel range, string Manager)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            var thisDateTime = _helper.GetCurrentDate();

            ViewBag.StartDate = range.start;
            ViewBag.EndDate = range.end;

            var tr = _reportServices.GetTeamReport(Convert.ToDateTime(ViewBag.StartDate), Convert.ToDateTime(ViewBag.EndDate), UserId, Manager);

            ViewBag.TeamReport = tr;
            var users = _telecallerServices.GetUsersListByRole(UserId);

            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "All", Value = "0" });
            foreach (AspNetUser user in users)
            {
                list.Add(new SelectListItem()
                {
                    Text = user.FullName,
                    Value = user.Id
                });
            }
            ViewBag.Manager = list;

            return View();
        }

        [HttpGet]
        public ActionResult SalesReport()
        {
            var UserId = User.Identity.GetUserId();
            ViewBag.StartDate = Convert.ToDateTime(thisDateTime);
            ViewBag.EndDate = Convert.ToDateTime(thisDateTime);
            var users = _telecallerServices.GetUsersListByRole(UserId);
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "All", Value = "0" });
            foreach (AspNetUser user in users)
            {
                list.Add(new SelectListItem()
                {
                    Text = user.FullName,
                    Value = user.Id
                });
            }
            ViewBag.Manager = list;
            ViewBag.Sales = _reportServices.GetSalesHistoryLists(Convert.ToDateTime(ViewBag.StartDate), Convert.ToDateTime(ViewBag.EndDate), UserId,"0");
            return View();
        }


        [HttpPost]
        public ActionResult SalesReport(DatePickerModel range, string Manager)
        {
            var UserId = User.Identity.GetUserId();
            ViewBag.StartDate = range.start;
            ViewBag.EndDate = range.end;
            ViewBag.Sales = _reportServices.GetSalesHistoryLists(Convert.ToDateTime(ViewBag.StartDate), Convert.ToDateTime(ViewBag.EndDate), UserId, Manager);
            var users = _telecallerServices.GetUsersListByRole(UserId);
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "All", Value = "0" });
            foreach (AspNetUser user in users)
            {
                list.Add(new SelectListItem()
                {
                    Text = user.FullName,
                    Value = user.Id
                });
            }
            ViewBag.Manager = list;
            return View();
        }

       

        [HttpGet]
        [Route("{id?}")]
        public ActionResult EmailTemplates(string id)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            EmailTemplate ThisTemplate = new EmailTemplate();
            var role = "Admin";
            if (User.IsInRole("Agent")) { role = "Agent"; } else if (User.IsInRole("Manager")) { role = "Manager"; }

            if (id != null && id != "")
            {
                ThisTemplate = _templateServices.GetTemplateById(id, UserId);
            }
            var Templates = _templateServices.GetTemplateListByUser(UserId,role);
            ViewBag.Templates = Templates;
            List<SelectListItem> AlwdRole = new List<SelectListItem>(); 
            AlwdRole.Add(new SelectListItem() { Text = "---Select---"});
            AlwdRole.Add(new SelectListItem() { Text = "Manager", Value = "Manager" });
            AlwdRole.Add(new SelectListItem() { Text = "Telecaller", Value = "Telecaller" });
            ViewBag.AllowedRole = AlwdRole;
            
            return View(ThisTemplate);
        }

        [HttpPost]
        [ValidateInput(false)]
        [Route("{id?}")]
        public ActionResult EmailTemplates(EmailTemplate template,string id)
        {
            try
            {
                var UserId = AuthenticationManager.User.Identity.GetUserId();
                if (id != null && id != "")
                {
                    
                    _templateServices.UpdateTemplate(template);
                }
                else
                {
                    _templateServices.UpdateTemplate(template);
                }
                Session["sMessage"] = "Details has been saved successfully.";
                return RedirectToAction("/EmailTemplates");
            }
            catch (Exception ex)
            {
                Session["eMessage"] = ex.Message;
            }
            return View();
        }

        [HttpGet]
        [Route("{id?}")]
        public ActionResult DeleteTemplates(long? id)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            _templateServices.deleteTemplate(id);
            return RedirectToAction("/EmailTemplates");
        }
    }
}