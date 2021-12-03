using CallingCRM.Helpers;
using CallingCRM.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CallingCRM.Services
{
    public class ReportServices
    {
        static CommonHelper _helper = new CommonHelper();
        public UserServices _userService = new UserServices();

        static DateTime thisDateTime = _helper.GetCurrentDate();

        public hwLiveEntities _context = new hwLiveEntities();


        public List<AdminTRViewModel> GetTeamReport(DateTime? StartDate, DateTime? EndDate, string UserId, string ManagerId)
        {
            List<AdminTRViewModel> result = new List<AdminTRViewModel>();
            if (ManagerId == "0")
            {
                var agents = _userService.GetUsersListByAdminId(UserId);

                foreach (ApplicationUser agent in agents)
                {
                    var report = new AdminTRViewModel();
                    report.FullName = agent.FullName;

                    report.TotalCalls = _context.LeadFollowups.Include(x => x.Lead).Where(x => x.Lead.AssignedToUser == agent.Id && x.Lead.LeadStatus != null && (DbFunctions.TruncateTime(x.Lead.AssignedDate) >= DbFunctions.TruncateTime(StartDate) && DbFunctions.TruncateTime(x.Lead.AssignedDate) <= DbFunctions.TruncateTime(EndDate))).Count();

                    report.FreshCalls = _context.Leads.Where(x => x.AssignedToUser == agent.Id && x.LeadStatus != null && (DbFunctions.TruncateTime(x.AssignedDate) >= DbFunctions.TruncateTime(StartDate) && DbFunctions.TruncateTime(x.AssignedDate) <= DbFunctions.TruncateTime(EndDate))).Count();

                    report.FollowupCalls = report.TotalCalls - report.FreshCalls;

                    report.UnresponsoveCalls = _context.LeadFollowups.Include(x => x.Lead).Where(x => x.Lead.AssignedToUser == agent.Id && (DbFunctions.TruncateTime(x.Lead.AssignedDate) >= DbFunctions.TruncateTime(StartDate) && DbFunctions.TruncateTime(x.Lead.AssignedDate) <= DbFunctions.TruncateTime(EndDate))).Where(x => x.CallStatus == "Busy" || x.CallStatus == "Not Picked" || x.CallStatus == "Skipped" || x.CallStatus == "Invalid Number").Count();
                    report.NotInterestedCalls = _context.LeadFollowups.Include(x => x.Lead).Where(x => x.Lead.AssignedToUser == agent.Id && (DbFunctions.TruncateTime(x.Lead.AssignedDate) >= DbFunctions.TruncateTime(StartDate) && DbFunctions.TruncateTime(x.Lead.AssignedDate) <= DbFunctions.TruncateTime(EndDate))).Where(x => x.CallStatus == "Not Interested" || x.CallStatus == "DND Requested").Count();
                    report.InterestedCalls = _context.LeadFollowups.Include(x => x.Lead).Where(x => x.Lead.AssignedToUser == agent.Id && (DbFunctions.TruncateTime(x.Lead.AssignedDate) >= DbFunctions.TruncateTime(StartDate) && DbFunctions.TruncateTime(x.Lead.AssignedDate) <= DbFunctions.TruncateTime(EndDate))).Where(x => x.CallStatus == "Interested - Call Back Later" || x.CallStatus == "Interested - Details Asked" || x.CallStatus == "Interested - Paid").Count();
                    report.TotalSales = _context.Sales.Include(x => x.Lead).Where(x => x.Lead.AssignedToUser == agent.Id && (DbFunctions.TruncateTime(x.PaymentDate) >= DbFunctions.TruncateTime(StartDate) && DbFunctions.TruncateTime(x.PaymentDate) <= DbFunctions.TruncateTime(EndDate))).Sum(x => x.Amount);

                    result.Add(report);
                }
            }
            else
            {
                if (ManagerId != null)
                {
                    var agents = _userService.GetUsersListByManagerId(ManagerId);

                    foreach (AspNetUser agent in agents)
                    {
                        var report = new AdminTRViewModel();
                        report.FullName = agent.FullName;

                        report.TotalCalls = _context.LeadFollowups.Include(x => x.Lead).Where(x => x.Lead.AssignedToUser == agent.Id && x.Lead.LeadStatus != null && (DbFunctions.TruncateTime(x.CreateDate) >= DbFunctions.TruncateTime(StartDate) && DbFunctions.TruncateTime(x.CreateDate) <= DbFunctions.TruncateTime(EndDate))).Count();

                        report.FreshCalls = _context.Leads.Where(x => x.AssignedToUser == agent.Id && x.LeadStatus != null && (DbFunctions.TruncateTime(x.AssignedDate) >= DbFunctions.TruncateTime(StartDate) && DbFunctions.TruncateTime(x.AssignedDate) <= DbFunctions.TruncateTime(EndDate))).Count();

                        report.FollowupCalls = report.TotalCalls - report.FreshCalls;

                        report.UnresponsoveCalls = _context.LeadFollowups.Include(x => x.Lead).Where(x => x.Lead.AssignedToUser == agent.Id && (DbFunctions.TruncateTime(x.CreateDate) >= DbFunctions.TruncateTime(StartDate) && DbFunctions.TruncateTime(x.CreateDate) <= DbFunctions.TruncateTime(EndDate))).Where(x => x.CallStatus == "Busy" || x.CallStatus == "Not Picked" || x.CallStatus == "Skipped" || x.CallStatus == "Invalid Number").Count();
                        report.NotInterestedCalls = _context.LeadFollowups.Include(x => x.Lead).Where(x => x.Lead.AssignedToUser == agent.Id && (DbFunctions.TruncateTime(x.CreateDate) >= DbFunctions.TruncateTime(StartDate) && DbFunctions.TruncateTime(x.CreateDate) <= DbFunctions.TruncateTime(EndDate))).Where(x => x.CallStatus == "Not Interested" || x.CallStatus == "DND Requested").Count();
                        report.InterestedCalls = _context.LeadFollowups.Include(x => x.Lead).Where(x => x.Lead.AssignedToUser == agent.Id && (DbFunctions.TruncateTime(x.CreateDate) >= DbFunctions.TruncateTime(StartDate) && DbFunctions.TruncateTime(x.CreateDate) <= DbFunctions.TruncateTime(EndDate))).Where(x => x.CallStatus == "Interested - Call Back Later" || x.CallStatus == "Interested - Details Asked" || x.CallStatus == "Interested - Paid").Count();
                        report.TotalSales = _context.Sales.Include(x => x.Lead).Where(x => x.Lead.AssignedToUser == agent.Id && (DbFunctions.TruncateTime(x.PaymentDate) >= DbFunctions.TruncateTime(StartDate) && DbFunctions.TruncateTime(x.PaymentDate) <= DbFunctions.TruncateTime(EndDate))).Sum(x => x.Amount);

                        result.Add(report);
                    }
                }
                else
                {
                    var agents = _userService.GetUsersListByManagerId(UserId);

                    foreach (AspNetUser agent in agents)
                    {
                        var report = new AdminTRViewModel();
                        report.FullName = agent.FullName;

                        report.TotalCalls = _context.LeadFollowups.Include(x => x.Lead).Where(x => x.Lead.AssignedToUser == agent.Id && x.Lead.LeadStatus != null && (DbFunctions.TruncateTime(x.CreateDate) >= DbFunctions.TruncateTime(StartDate) && DbFunctions.TruncateTime(x.CreateDate) <= DbFunctions.TruncateTime(EndDate))).Count();

                        report.FreshCalls = _context.Leads.Where(x => x.AssignedToUser == agent.Id && x.LeadStatus != null && (DbFunctions.TruncateTime(x.AssignedDate) >= DbFunctions.TruncateTime(StartDate) && DbFunctions.TruncateTime(x.AssignedDate) <= DbFunctions.TruncateTime(EndDate))).Count();

                        report.FollowupCalls = report.TotalCalls - report.FreshCalls;

                        report.UnresponsoveCalls = _context.LeadFollowups.Include(x => x.Lead).Where(x => x.Lead.AssignedToUser == agent.Id && (DbFunctions.TruncateTime(x.CreateDate) >= DbFunctions.TruncateTime(StartDate) && DbFunctions.TruncateTime(x.CreateDate) <= DbFunctions.TruncateTime(EndDate))).Where(x => x.CallStatus == "Busy" || x.CallStatus == "Not Picked" || x.CallStatus == "Skipped" || x.CallStatus == "Invalid Number").Count();
                        report.NotInterestedCalls = _context.LeadFollowups.Include(x => x.Lead).Where(x => x.Lead.AssignedToUser == agent.Id && (DbFunctions.TruncateTime(x.CreateDate) >= DbFunctions.TruncateTime(StartDate) && DbFunctions.TruncateTime(x.CreateDate) <= DbFunctions.TruncateTime(EndDate))).Where(x => x.CallStatus == "Not Interested" || x.CallStatus == "DND Requested").Count();
                        report.InterestedCalls = _context.LeadFollowups.Include(x => x.Lead).Where(x => x.Lead.AssignedToUser == agent.Id && (DbFunctions.TruncateTime(x.CreateDate) >= DbFunctions.TruncateTime(StartDate) && DbFunctions.TruncateTime(x.CreateDate) <= DbFunctions.TruncateTime(EndDate))).Where(x => x.CallStatus == "Interested - Call Back Later" || x.CallStatus == "Interested - Details Asked" || x.CallStatus == "Interested - Paid").Count();
                        report.TotalSales = _context.Sales.Include(x => x.Lead).Where(x => x.Lead.AssignedToUser == agent.Id && (DbFunctions.TruncateTime(x.PaymentDate) >= DbFunctions.TruncateTime(StartDate) && DbFunctions.TruncateTime(x.PaymentDate) <= DbFunctions.TruncateTime(EndDate))).Sum(x => x.Amount);

                        result.Add(report);
                    }
                }
            }

            return result;
        }


        public List<SalesHistoryList> GetSalesHistoryLists(DateTime? StartDate, DateTime? EndDate, string UserId,string Manager)
        {
            List<SalesHistoryList> history = new List<SalesHistoryList>();
            if (Manager == "0")
            {
                var sales = _context.Sales
                    .Include(x => x.Lead)
                    .Where(x => x.Lead.AssignedToUser != null && x.Lead.AspNetUser.AdminId == UserId && (DbFunctions.TruncateTime(x.PaymentDate) >= DbFunctions.TruncateTime(StartDate) && DbFunctions.TruncateTime(x.PaymentDate) <= DbFunctions.TruncateTime(EndDate)))
                    .OrderByDescending(x => x.CreatDate)
                    .ToList();




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
            }
            else
            {
                if (Manager != null)
                {
                    var sales = _context.Sales
                       .Include(x => x.Lead)
                       .Where(x => x.Lead.AssignedToUser != null && x.Lead.AspNetUser.AdminId == UserId && (DbFunctions.TruncateTime(x.PaymentDate) >= DbFunctions.TruncateTime(StartDate) && DbFunctions.TruncateTime(x.PaymentDate) <= DbFunctions.TruncateTime(EndDate)) && x.Lead.AspNetUser.ManagerId == Manager)
                       .OrderByDescending(x => x.CreatDate)
                       .ToList();

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
                }
                else
                {
                    var Client = ConfigurationManager.AppSettings["ClientId"].ToString();
                    var sales = _context.Sales
                       .Include(x => x.Lead)
                       .Where(x => x.Lead.AssignedToUser != null && x.Lead.AspNetUser.AdminId == Client && (DbFunctions.TruncateTime(x.PaymentDate) >= DbFunctions.TruncateTime(StartDate) && DbFunctions.TruncateTime(x.PaymentDate) <= DbFunctions.TruncateTime(EndDate)) && x.Lead.AspNetUser.ManagerId == UserId)
                       .OrderByDescending(x => x.CreatDate)
                       .ToList();

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
                }
            }
                return history;
        }
    }
    
}