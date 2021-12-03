using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using CallingCRM.Models;
using CallingCRM.Services;
using CallingCRM.Helpers;
using System.Configuration;
using System.Threading;
using System.IO;
using CsvHelper;
using System.Data;
using System.Reflection;

namespace CallingCRM.Controllers
{
    [Authorize(Roles ="Admin")]
    public class TransactionController : Controller
    {

        public LedgerServices _ledgerServices = new LedgerServices();
        public ReturnServices _returnServices = new ReturnServices();

        // GET: Transactions
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        [HttpGet]
        [Route("{id?}")]
        public ActionResult AddTransaction(long? id)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            //var histories = _ledgerServices.GetHistoriesList();
            //ViewBag.history = histories;
            var users = _ledgerServices.GetUsersList(UserId);
            LedgerHistory ThisHistory = new LedgerHistory();
            if (id != null)
            {
                ThisHistory = _ledgerServices.GetHistoryById(id, UserId);
            }
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() {Text = "Select User",Value = ""});
            foreach (AspNetUser user in users)
            {
                list.Add(new SelectListItem()
                {
                    Text = user.FullName,
                    Value = user.Id
                });
            }
            ViewBag.UserList = list;
            List<SelectListItem> TransactionType = new List<SelectListItem>();
            TransactionType.Add(new SelectListItem() { Text = "---Select---", Value = "" });
            TransactionType.Add(new SelectListItem() { Text = "Credit", Value = "Credit" });
            TransactionType.Add(new SelectListItem() { Text = "Debit", Value = "Debit" });
            ViewBag.transactiontype = TransactionType;
            List<SelectListItem> Status = new List<SelectListItem>();
            Status.Add(new SelectListItem() { Text = "Pending", Value = "Pending" });
            Status.Add(new SelectListItem() { Text = "Processing", Value = "Processing" });
            Status.Add(new SelectListItem() { Text = "Paid", Value = "Paid" });
            Status.Add(new SelectListItem() { Text = "Cancel", Value = "Cancel" });
            Status.Add(new SelectListItem() { Text = "Overdue", Value = "Overdue" });

            ViewBag.status = Status;
            return View(ThisHistory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTransaction(LedgerHistory history)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            var _existingOrder = _ledgerServices.GetHistoryListById(history.Id, UserId);
            try
            {

                _ledgerServices.UpdateHistory(history);
                Session["sMessage"] = "Details has been saved successfully.";

                return RedirectToAction("/TransactionList");
            }
            catch (Exception ex)
            {
                Session["eMessage"] = ex.Message;

            }
            return View(history);
        }


        [HttpGet]
        [Route("{id?}")]
        public ActionResult TransactionList(string id)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            var Histories = _ledgerServices.GetHistoryListByClient(id, UserId);
            ViewBag.History = Histories;
            return View();
        }

        [HttpGet]
        [Route("{id?}")]
        public ActionResult Pendingpayout(long? id)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            var returns = _returnServices.GetReturnListByClient(UserId);
            ViewBag.Return = returns;
            InvestmentReturn ThisReturn = new InvestmentReturn();
            if(id !=null && id != 0)
            {
               ThisReturn = _returnServices.GetIReturnById(id, UserId);
            }
            List<SelectListItem> ReturnStatus = new List<SelectListItem>();
            ReturnStatus.Add(new SelectListItem() { Text = "Pending", Value = "Pending" });
            ReturnStatus.Add(new SelectListItem() { Text = "Processing", Value = "Processing" });
            ReturnStatus.Add(new SelectListItem() { Text = "Paid", Value = "Paid" });
            ReturnStatus.Add(new SelectListItem() { Text = "Cancel", Value = "Cancel" });
            ViewBag.returnStatus =  ReturnStatus;


            return View(ThisReturn);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Pendingpayout(InvestmentReturn iReturns)
        {
            try
            {

                _returnServices.UpdatePayouts(iReturns);
                Session["sMessage"] = "Details has been saved successfully.";

                return RedirectToAction("/Pendingpayout");
            }
            catch (Exception ex)
            {
                Session["eMessage"] = ex.Message;

            }
            return View();
        }





       
    }
}