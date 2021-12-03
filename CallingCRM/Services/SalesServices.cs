using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using MoreLinq;
using System.Linq;
using System.Web;
using CallingCRM.Models;
using CallingCRM.Helpers;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Threading;
using System.Configuration;
using System.Data.Entity;
//using Flurl;

namespace CallingCRM.Services
{
    public class SalesService
    {
        static CommonHelper _helper = new CommonHelper();
        public UserServices _userService = new UserServices();

        static DateTime thisDateTime = _helper.GetCurrentDate();

        public hwLiveEntities _context = new hwLiveEntities();

        public List<SalesHistoryList> GetSalesHistoryLists(string UserId, bool IsManager)
        {
            var sales = _context.Sales
                .Include(x => x.Lead)
                .Where(x => x.Lead.AssignedToUser == UserId)
                .OrderByDescending(x => x.CreatDate)
                .ToList();

            if (IsManager)
            {
                sales = _context.Sales
                    .Include(x => x.Lead)
                    .Where(x => x.Lead.AssignedToUser != null && x.Lead.AspNetUser.ManagerId == UserId)
                    .OrderByDescending(x => x.CreatDate)
                    .ToList();
            }

            List<SalesHistoryList> history = new List<SalesHistoryList>();

            foreach (Sale sale in sales)
            {
                history.Add(new SalesHistoryList()
                {
                    Name = sale.Lead.FullName,
                    Number = sale.Lead.Mobile,
                    Email = sale.Lead.Email,
                    LeadDate = sale.Lead.AssignedDate,
                    UserId = UserId,
                    sale = sale
                });
            }

            return history;
        }

        public List<UserDCRViewModel> GetDCR(DateTime? StartDate, DateTime? EndDate, string UserId)
        {
            List<UserDCRViewModel> result = new List<UserDCRViewModel>();

            var agents = _userService.GetUsersListByManagerId(UserId);

            foreach (AspNetUser agent in agents)
            {
                var report = new UserDCRViewModel();
                report.FullName = agent.FullName;

                report.TotalCalls = _context.LeadFollowups.Include(x => x.Lead).Where(x => x.Lead.AssignedToUser == agent.Id && x.Lead.LeadStatus!=null && (DbFunctions.TruncateTime(x.CreateDate)>= DbFunctions.TruncateTime(StartDate) && DbFunctions.TruncateTime(x.CreateDate) <= DbFunctions.TruncateTime(EndDate))).Count();
                
                report.FreshCalls = _context.Leads.Where(x => x.AssignedToUser == agent.Id && x.LeadStatus !=null && (DbFunctions.TruncateTime(x.AssignedDate) >= DbFunctions.TruncateTime(StartDate) && DbFunctions.TruncateTime(x.AssignedDate) <= DbFunctions.TruncateTime(EndDate))).Count();
                
                report.FollowupCalls = report.TotalCalls - report.FreshCalls;
                
                report.UnresponsoveCalls = _context.LeadFollowups.Include(x => x.Lead).Where(x => x.Lead.AssignedToUser == agent.Id && (DbFunctions.TruncateTime(x.CreateDate) >= DbFunctions.TruncateTime(StartDate) && DbFunctions.TruncateTime(x.CreateDate) <= DbFunctions.TruncateTime(EndDate))).Where(x => x.CallStatus == "Busy" || x.CallStatus == "Not Picked" || x.CallStatus == "Skipped" || x.CallStatus == "Invalid Number").Count();
                report.NotInterestedCalls = _context.LeadFollowups.Include(x => x.Lead).Where(x => x.Lead.AssignedToUser == agent.Id && (DbFunctions.TruncateTime(x.CreateDate) >= DbFunctions.TruncateTime(StartDate) && DbFunctions.TruncateTime(x.CreateDate) <= DbFunctions.TruncateTime(EndDate))).Where(x => x.CallStatus == "Not Interested" || x.CallStatus == "DND Requested").Count();
                report.InterestedCalls = _context.LeadFollowups.Include(x => x.Lead).Where(x => x.Lead.AssignedToUser == agent.Id && (DbFunctions.TruncateTime(x.CreateDate) >= DbFunctions.TruncateTime(StartDate) && DbFunctions.TruncateTime(x.CreateDate) <= DbFunctions.TruncateTime(EndDate))).Where(x => x.CallStatus == "Interested - Call Back Later" || x.CallStatus == "Interested - Details Asked" || x.CallStatus == "Interested - Paid").Count();
                report.TotalSales = _context.Sales.Include(x => x.Lead).Where(x => x.Lead.AssignedToUser == agent.Id && (DbFunctions.TruncateTime(x.PaymentDate) >= DbFunctions.TruncateTime(StartDate) && DbFunctions.TruncateTime(x.PaymentDate) <= DbFunctions.TruncateTime(EndDate))).Sum(x => x.Amount);

                result.Add(report);
            }

            return result;
        }



    }
}