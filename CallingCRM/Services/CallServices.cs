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

namespace CallingCRM.Services
{
    public class CallService
    {
        static  CommonHelper _helper = new CommonHelper();

        public DateTime thisDateTime = _helper.GetCurrentDate();

        public hwLiveEntities _context = new hwLiveEntities();

        public Lead GetLead(string UserId)
        {
            var Client = ConfigurationManager.AppSettings["ClientId"].ToString();
            Lead lead = _context.Leads.Where(x => x.AssignedToUser == UserId && x.LeadStatus == null && x.AdminId == Client).FirstOrDefault();

            if (lead == null)
            {
                lead = _context.Leads.Where(x => x.AssignedToUser == null && (x.DND == false || x.DND == null)).FirstOrDefault();
                if (lead != null)
                {
                    lead.AssignedToUser = UserId;
                    lead.AssignedDate = _helper.GetCurrentDate();
                    _context.SaveChanges();
                }
            }
            return lead;
        }

        public Lead GetLeadById(Int64 Id)
        {
            Lead lead = _context.Leads
                .Include(x => x.AspNetUser)
                .Where(x => x.Id == Id).FirstOrDefault();

            return lead;
        }

        public void UpdateCallLog(StartCallsViewModels call)
        {
            Lead lead = _context.Leads.Where(x => x.Id == call.LeadId).FirstOrDefault();
            LeadFollowup follow = new LeadFollowup();

            lead.FullName = call.FullName;
            lead.Email = call.Email;
            lead.Location = call.Location;
            lead.LeadType = call.LeadType;
            lead.LeadStatus = call.LeadStatus;
            if (call.CallStatus == "DND Requested") { lead.DND = true; }

            _context.SaveChanges();

            // Add call log
            follow.LeadId = call.LeadId;
            follow.CallStatus = call.CallStatus;
            follow.Comment = call.Comment;
            follow.FollowupStatus = call.FollowupStatus;
            follow.FollowupDate = call.FollowupDate;
            follow.CreateDate = _helper.GetCurrentDate();

            _context.LeadFollowups.Add(follow);
            _context.SaveChanges();

            // Add sale
            if (call.CallStatus == "Interested - Paid")
            {
                bool _existing = _context.Sales.Where(x => x.LeadId == lead.Id).Any();
                if (!_existing)
                {
                    // do not make duplicate entry
                    Sale newSale = new Sale();
                    newSale.LeadId = lead.Id;
                    newSale.PaymentDate = call.PaymentDate;
                    newSale.Amount = call.Amount;
                    newSale.Comment = call.PaymentInfo;
                    newSale.CreatDate = _helper.GetCurrentDate();

                    _context.Sales.Add(newSale);
                    _context.SaveChanges();
                }
            }

        }

        public void UpdateFollowupLog(StartCallsViewModels call)
        {
            if (call.CallStatus == "Interested - Paid")
            {
                if (call.PaymentInfo == "" || call.Amount.ToString() == "" || call.PaymentDate.ToString() == "")
                {
                    throw new Exception("If call status is paid then payment information is madatory.");
                }
            }

            Lead lead = _context.Leads.Where(x => x.Id == call.LeadId).FirstOrDefault();
            LeadFollowup follow = new LeadFollowup();

            lead.FullName = call.FullName;
            lead.Email = call.Email;
            lead.Location = call.Location;
            lead.LeadType = call.LeadType;
            lead.LeadStatus = call.LeadStatus;
            if (call.CallStatus == "DND Requested") { lead.DND = true; }
            _context.SaveChanges();

            // get previous followup
            LeadFollowup followLast = _context.LeadFollowups.Where(x => x.LeadId == call.LeadId && x.FollowupStatus == null).OrderByDescending(x => x.CreateDate).FirstOrDefault();
            if (followLast != null)
            {
                followLast.FollowupStatus = call.FollowupStatus;
                _context.SaveChanges();
            }

            // Add call log for next followup
            if (call.FollowupDate != null)
            {
                follow.LeadId = call.LeadId;
                follow.CallStatus = call.CallStatus;
                follow.Comment = call.Comment;
                follow.FollowupDate = call.FollowupDate;
                follow.CreateDate = _helper.GetCurrentDate();

                _context.LeadFollowups.Add(follow);
                _context.SaveChanges();
            }

            // Add sale
            if (call.CallStatus == "Interested - Paid")
            {
                Sale newSale = new Sale();
                newSale.LeadId = lead.Id;
                newSale.PaymentDate = call.PaymentDate;
                newSale.Amount = call.Amount;
                newSale.Comment = call.PaymentInfo;
                newSale.CreatDate = _helper.GetCurrentDate();

                _context.Sales.Add(newSale);
                _context.SaveChanges();
            }

        }

        public List<CallHistoryList> GetCallHistory(DateTime? startDate, DateTime? endDate, string filter, string userId, string role="Agent")
        {
            List<CallHistoryList> history = new List<CallHistoryList>();

            if (role == "Manager")
            {
                history = _context.LeadFollowups
                    .Include(x => x.Lead)
                    .Include(x => x.Lead.AspNetUser)
                    .Where(x => x.Lead.AssignedToUser != null && (DbFunctions.TruncateTime(x.CreateDate) >= DbFunctions.TruncateTime(startDate) && DbFunctions.TruncateTime(x.CreateDate) <= DbFunctions.TruncateTime(endDate)) && x.Lead.AspNetUser.ManagerId == userId)
                    .Select(x => new CallHistoryList()
                    {
                        CallDate = x.Lead.AssignedDate,
                        Name = x.Lead.FullName,
                        Number = x.Lead.Mobile,
                        LeadStatus = x.Lead.LeadStatus,
                        CallStatus = x.CallStatus,
                        LastFollowupDate = x.CreateDate,
                        NextFollowup = x.FollowupDate,
                        UserId = x.Lead.AssignedToUser,
                        LeadId = x.LeadId,
                        FollowupId = x.Id,
                        UserName = x.Lead.AspNetUser.FullName
                    })
                    .OrderByDescending(x => x.LastFollowupDate)
                    .ToList();
            }
            else 
            {
                history = _context.LeadFollowups
                    .Include(x => x.Lead)
                    .Include(x => x.Lead.AspNetUser)
                    .Where(x => x.Lead.AssignedToUser == userId && (DbFunctions.TruncateTime(x.CreateDate) >= DbFunctions.TruncateTime(startDate) && DbFunctions.TruncateTime(x.CreateDate) <= DbFunctions.TruncateTime(endDate)))
                    .Select(x => new CallHistoryList()
                    {
                        CallDate = x.Lead.AssignedDate,
                        Name = x.Lead.FullName,
                        Number = x.Lead.Mobile,
                        LeadStatus = x.Lead.LeadStatus,
                        CallStatus = x.CallStatus,
                        LastFollowupDate = x.CreateDate,
                        NextFollowup = x.FollowupDate,
                        UserId = x.Lead.AssignedToUser,
                        LeadId = x.LeadId,
                        FollowupId = x.Id,
                        UserName = x.Lead.AspNetUser.FullName
                    })
                    .OrderByDescending(x => x.LastFollowupDate)
                    .ToList();
            }

            if (filter == "Unresponsive")
            {
                history = history.Where(x => x.CallStatus == "Busy" || x.CallStatus == "Not Picked" || x.CallStatus == "Skipped" || x.CallStatus == "Invalid Number").ToList();
            }
            else if (filter == "NotInterested")
            {
                history = history.Where(x => x.CallStatus == "Not Interested" || x.CallStatus == "DND Requested").ToList();
            }
            else if (filter == "Interested")
            {
                history = history.Where(x => x.CallStatus == "Interested - Call Back Later" || x.CallStatus == "Interested - Details Asked" || x.CallStatus == "Interested - Paid").ToList();
            }

            return history;
        }

        public List<CallHistoryList> GetCallHistoryByLeadId(Int64 LeadId)
        {
            var history = _context.LeadFollowups
                .Include(x => x.Lead)
                .Where(x => x.LeadId == LeadId)
                .Select(x => new CallHistoryList()
                {
                    CallDate = x.Lead.AssignedDate,
                    Name = x.Lead.FullName,
                    Number = x.Lead.Mobile,
                    LeadStatus = x.Lead.LeadStatus,
                    CallStatus = x.CallStatus,
                    LastFollowupDate = x.CreateDate,
                    NextFollowup = x.FollowupDate,
                    UserId = x.Lead.AssignedToUser,
                    LeadId = x.LeadId,
                    FollowupId = x.Id
                })
                .OrderByDescending(x => x.LastFollowupDate)
                .ToList();

            return history;
        }

        public List<CallHistoryList> SearchLeadsByText(string SearchText, string UserId)
        {
            List<CallHistoryList> history = new List<CallHistoryList>();

            // get Leads
            var leads = _context.Leads.Where(x => x.AssignedToUser== UserId && (x.Email.Contains(SearchText) == true || x.Mobile.Contains(SearchText) == true || x.FullName.Contains(SearchText) == true)).ToList();
            int roles = _context.AspNetUsers.Where(x => x.Id == UserId && x.AspNetRoles.Where(y => y.Name == "Manager").Any()).Count();

            if (roles>0)
            {
                leads = _context.Leads.Where(x => x.AssignedToUser != null && (x.Email.Contains(SearchText) == true || x.Mobile.Contains(SearchText) == true || x.FullName.Contains(SearchText) == true)).ToList();
            }

            foreach (Lead lead in leads)
            {
                var latestCall = _context.LeadFollowups.Where(x => x.LeadId == lead.Id).OrderByDescending(x => x.CreateDate).FirstOrDefault();

                if (latestCall != null)
                {
                    history.Add(new CallHistoryList()
                    {
                        CallDate = lead.AssignedDate,
                        Name = lead.FullName,
                        Number = lead.Mobile,
                        LeadStatus = lead.LeadStatus,
                        CallStatus = latestCall.CallStatus,
                        LastFollowupDate = latestCall.CreateDate,
                        NextFollowup = latestCall.FollowupDate,
                        UserId = latestCall.Lead.AssignedToUser,
                        LeadId = latestCall.LeadId,
                        FollowupId = latestCall.Id
                    });
                }
                else 
                {
                    history.Add(new CallHistoryList()
                    {
                        CallDate = lead.AssignedDate,
                        Name = lead.FullName,
                        Number = lead.Mobile,
                        LeadStatus = lead.LeadStatus,
                        CallStatus = "",
                        LastFollowupDate = null,
                        NextFollowup = null,
                        UserId = lead.AssignedToUser,
                        LeadId = lead.Id,
                        FollowupId = null
                    });
                }
            }

            return history;
        }
        
        public List<LeadFollowup> GetLeadFolloupsByLeadId(Int64 LeadId)
        {
            var history = _context.LeadFollowups
                .Include(x => x.Lead)
                .Where(x => x.LeadId == LeadId)
                .OrderByDescending(x => x.CreateDate)
                .ToList();

            return history;
        }

        public List<LeadForwarded> GetForwardedCallsByLeadId(Int64 LeadId)
        {
            var history = _context.LeadForwardeds
                .Include(x => x.Lead)
                .Where(x => x.LeadId == LeadId)
                .OrderByDescending(x => x.ForwardDate)
                .ToList();

            return history;
        }

        public List<CallHistoryList> GetFollowups(string filter, string userId, bool IsManager)
        {
            thisDateTime = _helper.GetCurrentDate();

            var history = new List<CallHistoryList>();
            int roles = _context.AspNetUsers.Where(x => x.Id == userId && x.AspNetRoles.Where(y => y.Name == "Manager").Any()).Count();
            if (roles > 0)
            {
               if (filter == "Pending")
                {
                    history = _context.LeadFollowups
                    .Include(x => x.Lead)
                    .Include(x=> x.Lead.AspNetUser)
                    .Where(x => x.Lead.AssignedToUser != null && x.Lead.AspNetUser.ManagerId == userId && x.FollowupStatus == null && DbFunctions.TruncateTime(x.FollowupDate) < DbFunctions.TruncateTime(thisDateTime) && x.Lead.AspNetUser.ManagerId == userId)
                    .Select(x => new CallHistoryList()
                    {
                        CallDate = x.Lead.AssignedDate,
                        Name = x.Lead.FullName,
                        Number = x.Lead.Mobile,
                        LeadStatus = x.Lead.LeadStatus,
                        CallStatus = x.CallStatus,
                        LastFollowupDate = x.CreateDate,
                        NextFollowup = x.FollowupDate,
                        UserId = x.Lead.AssignedToUser,
                        LeadId = x.LeadId,
                        FollowupId = x.Id,
                        UserName = x.Lead.AspNetUser.FullName
                    })
                    .OrderByDescending(x => x.NextFollowup)
                    .ToList();
                }
                else if (filter == "Today")
                {
                    history = _context.LeadFollowups
                    .Include(x => x.Lead)
                    .Where(x => x.Lead.AssignedToUser != null && x.Lead.AspNetUser.ManagerId == userId && x.FollowupStatus == null && DbFunctions.TruncateTime(x.FollowupDate) == DbFunctions.TruncateTime(thisDateTime))
                    .Select(x => new CallHistoryList()
                    {
                        CallDate = x.Lead.AssignedDate,
                        Name = x.Lead.FullName,
                        Number = x.Lead.Mobile,
                        LeadStatus = x.Lead.LeadStatus,
                        CallStatus = x.CallStatus,
                        LastFollowupDate = x.CreateDate,
                        NextFollowup = x.FollowupDate,
                        UserId = x.Lead.AssignedToUser,
                        LeadId = x.LeadId,
                        FollowupId = x.Id,
                        UserName = x.Lead.AspNetUser.FullName
                    })
                    .OrderByDescending(x => x.NextFollowup)
                    .ToList();
                }
                else if (filter == "Upcoming")


                {
                    history = _context.LeadFollowups
                    .Include(x => x.Lead)
                    .Where(x => x.Lead.AssignedToUser != null && x.Lead.AspNetUser.ManagerId == userId && x.FollowupStatus == null && DbFunctions.TruncateTime(x.FollowupDate) > DbFunctions.TruncateTime(thisDateTime))
                    .Select(x => new CallHistoryList()
                    {
                        CallDate = x.Lead.AssignedDate,
                        Name = x.Lead.FullName,
                        Number = x.Lead.Mobile,
                        LeadStatus = x.Lead.LeadStatus,
                        CallStatus = x.CallStatus,
                        LastFollowupDate = x.CreateDate,
                        NextFollowup = x.FollowupDate,
                        UserId = x.Lead.AssignedToUser,
                        LeadId = x.LeadId,
                        FollowupId = x.Id,
                        UserName = x.Lead.AspNetUser.FullName
                    })
                    .OrderByDescending(x => x.NextFollowup)
                    .ToList();
                }
            }
            else 
            {
                if (filter == "Pending")
                {
                    history = _context.LeadFollowups
                    .Include(x => x.Lead)
                    .Where(x => x.Lead.AssignedToUser == userId && x.FollowupStatus == null && DbFunctions.TruncateTime(x.FollowupDate) < DbFunctions.TruncateTime(thisDateTime))
                    .Select(x => new CallHistoryList()
                    {
                        CallDate = x.Lead.AssignedDate,
                        Name = x.Lead.FullName,
                        Number = x.Lead.Mobile,
                        LeadStatus = x.Lead.LeadStatus,
                        CallStatus = x.CallStatus,
                        LastFollowupDate = x.CreateDate,
                        NextFollowup = x.FollowupDate,
                        UserId = x.Lead.AssignedToUser,
                        LeadId = x.LeadId,
                        FollowupId = x.Id,
                        UserName = x.Lead.AspNetUser.FullName
                    })
                    .OrderByDescending(x => x.NextFollowup)
                    .ToList();
                }
                else if (filter == "Today")
                {
                    history = _context.LeadFollowups
                    .Include(x => x.Lead)
                    .Where(x => x.Lead.AssignedToUser == userId && x.FollowupStatus == null && DbFunctions.TruncateTime(x.FollowupDate) == DbFunctions.TruncateTime(thisDateTime))
                    .Select(x => new CallHistoryList()
                    {
                        CallDate = x.Lead.AssignedDate,
                        Name = x.Lead.FullName,
                        Number = x.Lead.Mobile,
                        LeadStatus = x.Lead.LeadStatus,
                        CallStatus = x.CallStatus,
                        LastFollowupDate = x.CreateDate,
                        NextFollowup = x.FollowupDate,
                        UserId = x.Lead.AssignedToUser,
                        LeadId = x.LeadId,
                        FollowupId = x.Id,
                        UserName = x.Lead.AspNetUser.FullName
                    })
                    .OrderByDescending(x => x.NextFollowup)
                    .ToList();
                }
                else if (filter == "Upcoming")
                {
                    history = _context.LeadFollowups
                    .Include(x => x.Lead)
                    .Where(x => x.Lead.AssignedToUser == userId && x.FollowupStatus == null && DbFunctions.TruncateTime(x.FollowupDate) > DbFunctions.TruncateTime(thisDateTime))
                    .Select(x => new CallHistoryList()
                    {
                        CallDate = x.Lead.AssignedDate,
                        Name = x.Lead.FullName,
                        Number = x.Lead.Mobile,
                        LeadStatus = x.Lead.LeadStatus,
                        CallStatus = x.CallStatus,
                        LastFollowupDate = x.CreateDate,
                        NextFollowup = x.FollowupDate,
                        UserId = x.Lead.AssignedToUser,
                        LeadId = x.LeadId,
                        FollowupId = x.Id,
                        UserName = x.Lead.AspNetUser.FullName
                    })
                    .OrderByDescending(x => x.NextFollowup)
                    .ToList();
                }
            }

            return history;
        }

        public string AddForwardedCall(LeadForwarded call)
        {
            var result = "";

            thisDateTime = _helper.GetCurrentDate();

            try {
                // check if call already forwarded today
                var fLead = _context.LeadForwardeds.Where(x => x.LeadId == call.LeadId && DbFunctions.TruncateTime(x.ForwardDate) == DbFunctions.TruncateTime(thisDateTime)).FirstOrDefault();

                if (fLead != null)
                {
                    return "Lead already forwarded to manager today.";
                }
                _context.LeadForwardeds.Add(call);
                _context.SaveChanges();
            }
            catch (Exception ex) {
                result = ex.Message;
            }

            return result;
        }

        public List<LeadForwarded> GetForwardedLeads(DateTime? StartDate, DateTime? EndDate, string UserId, string Status)
        {
            if (Status == null || Status == "") { Status = "Pending"; }

            var aajKiDate = _helper.GetCurrentDate();

            List<LeadForwarded> history = new List<LeadForwarded>();
            int roles = _context.AspNetUsers.Where(x => x.Id == UserId && x.AspNetRoles.Where(y => y.Name == "Manager").Any()).Count();
            if (roles>0)
            {
                if (Status.ToLower() == "pending")
                {
                    history = _context.LeadForwardeds
                    .Include(x => x.Lead)
                    .Include(x => x.Lead.AspNetUser)
                    .Where(x => x.Lead.AssignedToUser != null && x.Lead.AspNetUser.ManagerId==UserId && x.ActionDate == null && (DbFunctions.TruncateTime(x.NextReminderDate) < DbFunctions.TruncateTime(aajKiDate))).OrderBy(x => x.ForwardDate).ToList();
                }
                else if (Status.ToLower() == "todays")
                {
                    history = _context.LeadForwardeds
                    .Include(x => x.Lead)
                    .Include(x => x.Lead.AspNetUser)
                    .Where(x => x.Lead.AssignedToUser != null && x.Lead.AspNetUser.ManagerId == UserId && x.ActionDate == null && (DbFunctions.TruncateTime(x.NextReminderDate) == DbFunctions.TruncateTime(aajKiDate))).OrderBy(x => x.ForwardDate).ToList();
                }
                else if (Status.ToLower() == "upcoming")
                {
                    history = _context.LeadForwardeds
                    .Include(x => x.Lead)
                    .Include(x => x.Lead.AspNetUser)
                    .Where(x => x.Lead.AssignedToUser != null && x.Lead.AspNetUser.ManagerId == UserId && x.ActionDate == null && (DbFunctions.TruncateTime(x.NextReminderDate) > DbFunctions.TruncateTime(aajKiDate))).OrderBy(x => x.ForwardDate).ToList();
                }
                else if (Status.ToLower() == "all")
                {
                    history = _context.LeadForwardeds
                    .Include(x => x.Lead)
                    .Include(x => x.Lead.AspNetUser)
                    .Where(x => x.Lead.AssignedToUser != null && x.Lead.AspNetUser.ManagerId == UserId && x.ActionDate != null).OrderBy(x => x.ForwardDate).ToList();
                }
            }
            else 
            {
                if (Status.ToLower() == "pending")
                {
                    history = _context.LeadForwardeds
                    .Include(x => x.Lead)
                    .Include(x => x.Lead.AspNetUser)
                    .Where(x => x.Lead.AssignedToUser == UserId && x.ActionDate == null && (DbFunctions.TruncateTime(x.NextReminderDate) < DbFunctions.TruncateTime(aajKiDate))).OrderBy(x => x.ForwardDate).ToList();
                }
                else if (Status.ToLower() == "todays")
                {
                    history = _context.LeadForwardeds
                    .Include(x => x.Lead)
                    .Include(x => x.Lead.AspNetUser)
                    .Where(x => x.Lead.AssignedToUser == UserId && x.ActionDate == null && (DbFunctions.TruncateTime(x.NextReminderDate) == DbFunctions.TruncateTime(aajKiDate))).OrderBy(x => x.ForwardDate).ToList();
                }
                else if (Status.ToLower() == "upcoming")
                {
                    history = _context.LeadForwardeds
                    .Include(x => x.Lead)
                    .Include(x => x.Lead.AspNetUser)
                    .Where(x => x.Lead.AssignedToUser == UserId && x.ActionDate == null && (DbFunctions.TruncateTime(x.NextReminderDate) > DbFunctions.TruncateTime(aajKiDate))).OrderBy(x => x.ForwardDate).ToList();
                }
                else if (Status.ToLower() == "all")
                {
                    history = _context.LeadForwardeds
                    .Include(x => x.Lead)
                    .Include(x => x.Lead.AspNetUser)
                    .Where(x => x.Lead.AssignedToUser == UserId && x.ActionDate != null).OrderBy(x => x.ForwardDate).ToList();
                }
            }

            return history;
        }

        public LeadForwarded GetForwardedLeadById(long Id)
        {
            var call = _context.LeadForwardeds
                .Include(x => x.Lead)
                .Include(x => x.Lead.AspNetUser)
                .Where(x => x.Id == Id).FirstOrDefault();

            return call;
        }

        public string UpdateForwardedCallLog(LeadForwarded lead)
        {
            var result = "";

            var _lead = _context.LeadForwardeds.Where(x => x.Id == lead.Id).FirstOrDefault();

            if (_lead != null && lead != null)
            {
                CommonHelper _helper = new CommonHelper();
                DateTime thisDateTime = _helper.GetCurrentDate();

                _lead.ActionDate = thisDateTime;
                _lead.ActionByNewUser = lead.ActionByNewUser;

                _context.SaveChanges();

                if (lead.NextReminderDate != null)
                {
                    LeadForwarded newLead = new LeadForwarded();
                    newLead.LeadId = _lead.LeadId;
                    newLead.ForwardedTo = _lead.ForwardedTo;
                    newLead.ForwardDate = thisDateTime;
                    newLead.ForwardReason = lead.ForwardReason;
                    newLead.NextReminderDate = lead.NextReminderDate;

                    _context.LeadForwardeds.Add(newLead);
                    _context.SaveChanges();
                }
            }

            return result;
        }

        public string AddLead(Lead lead)
        {
            var result = "";

            try {
                Lead IsAlreadyExit = _context.Leads.Where(x => x.Mobile == lead.Mobile).FirstOrDefault();
                if (lead.Email != null && lead.Email != "")
                {
                    IsAlreadyExit = _context.Leads.Where(x => x.Mobile == lead.Mobile || x.Email == lead.Email).FirstOrDefault();
                }

                Lead newLead = new Lead();

                if (IsAlreadyExit != null)
                {
                    if (IsAlreadyExit.AssignedToUser != null)
                    {
                        result = "Lead by this number or email already exist in database.";
                        return result;
                    }
                    else
                    {
                        newLead = IsAlreadyExit;
                    }
                }

                if (lead.AssignedToUser != null && lead.AssignedToUser.ToString() != "")
                {
                    var _user = _context.AspNetUsers.Where(x => x.Id == lead.AssignedToUser || x.Email == lead.AssignedToUser).FirstOrDefault();

                    if (_user == null)
                    {
                        lead.AssignedToUser = null;
                    }
                    else
                    {
                        lead.AssignedToUser = _user.Id;
                    }
                }

                newLead.FullName = lead.FullName;
                newLead.Email = lead.Email;
                newLead.Mobile = lead.Mobile;
                newLead.Location = lead.Location;
                newLead.LeadSource = lead.LeadSource;
                newLead.LeadCreationDate = thisDateTime;
                newLead.AssignedToUser = lead.AssignedToUser;
                newLead.AssignedDate = thisDateTime;
                newLead.DND = false;
                newLead.LeadType = "Hot Lead";
                newLead.LeadStatus = "Open";
                newLead.AdminId = ConfigurationManager.AppSettings["ClientId"].ToString();

                if (IsAlreadyExit == null)
                {
                    _context.Leads.Add(newLead);
                    result = "Success";
                }
                else
                {
                    _context.Entry(IsAlreadyExit).CurrentValues.SetValues(newLead);
                    result = "Updated";
                }
                _context.SaveChanges();

                if (lead.AssignedToUser != null)
                {
                    LeadFollowup follow = new LeadFollowup();

                    follow.LeadId = newLead.Id;
                    follow.CallStatus = "Interested - Details Asked";
                    follow.Comment = lead.LeadStatus;
                    follow.FollowupDate = lead.AssignedDate;
                    follow.FollowupStatus = null;
                    follow.CreateDate = thisDateTime;

                    _context.LeadFollowups.Add(follow);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }

        public List<ExportLeadItem> GetExportedLeads(DateTime StartDate, DateTime EndDate, string ExportType, string LoggedInUserId)
        {
            List<ExportLeadItem> leads = new List<ExportLeadItem>();
            string ClientId = ConfigurationManager.AppSettings["ClientId"];

            var IsValidAdmin = _helper.ValidateAdmin();

            if (IsValidAdmin != "")
            {
                throw new ApplicationException(IsValidAdmin);
            }
            List<Lead> AllLeads;
            if (LoggedInUserId == ClientId)
            {
                // check if loggedinUser is Admin or not
                AllLeads = _context.Leads
                        .Include(x => x.AspNetUser)
                        .Include(x => x.LeadFollowups)
                        .Where(x => DbFunctions.TruncateTime(x.AssignedDate) >= StartDate && DbFunctions.TruncateTime(x.AssignedDate) <= EndDate)
                        .ToList();
            }
            else
            {
                AllLeads = _context.Leads
                        .Include(x => x.AspNetUser)
                        .Include(x => x.LeadFollowups)
                        .Where(x => DbFunctions.TruncateTime(x.AssignedDate) >= StartDate && DbFunctions.TruncateTime(x.AssignedDate) <= EndDate && x.AspNetUser.AdminId == ConfigurationManager.AppSettings["ClientId"].ToString())
                        .ToList();
            }
            leads = AllLeads.Select(x => new ExportLeadItem()
            {
                FullName = x.FullName,
                Email = x.Email,
                Mobile = x.Mobile,
                City = x.Location,
                ZIPCode = x.PinCode,
                SourceOfLead = x.LeadSource,
                AssignedDate = x.AssignedDate,
                AssignedToUserEmail = GetEmailFromUserObject(x.AspNetUser),
                LastCallStatus = GetLatestFollowupOfLead(x.LeadFollowups).CallStatus,
                LastFollowUpDate = GetLatestFollowupOfLead(x.LeadFollowups).CreateDate,
                LastFollowUpStatus = GetLatestFollowupOfLead(x.LeadFollowups).FollowupStatus,
                NextFollowUpDate = GetLatestFollowupOfLead(x.LeadFollowups).FollowupDate,
                Comment = GetLatestFollowupOfLead(x.LeadFollowups).Comment
            }).ToList();

            if (ExportType == "All")
            {

            }
            else if (ExportType == "Unprocessed")
            {
                leads = leads
                    .Where(x => x.LastCallStatus == null).ToList();
            }
            else if (ExportType == "Processed")
            {
                leads = leads
                    .Where(x => x.LastCallStatus != null).ToList();
            }
            else if (ExportType == "Unresponsive")
            {
                leads = leads
                    .Where(x => x.LastCallStatus == "Invalid Number" || x.LastCallStatus == "International Number" || x.LastCallStatus == "Number Switched Off" || x.LastCallStatus == "Busy" || x.LastCallStatus == "Not Picked" || x.LastCallStatus == "DND Requested").ToList();
            }
            else if (ExportType == "NotInterested")
            {
                leads = leads
                    .Where(x => x.LastCallStatus == "Not Interested" || x.LastCallStatus == "Not Interested (Payment Issue)" || x.LastCallStatus == "Not Interested (Resources not available)").ToList();
            }
            else if (ExportType == "Interested")
            {
                leads = leads
                    .Where(x => x.LastCallStatus == "Interested - Call Back Later" || x.LastCallStatus == "Interested - Details Asked").ToList();
            }
            else if (ExportType == "Converted")
            {
                leads = leads
                    .Where(x => x.LastCallStatus == "Interested - Paid").ToList();
            }

            //leads = leads.Where(x => DbFunctions.TruncateTime(x.AssignedDate) >= StartDate && DbFunctions.TruncateTime(x.AssignedDate) <= EndDate).ToList();

            return leads;
        }

        public string GetEmailFromUserObject(AspNetUser user)
        {
            var result = "";

            if (user != null && user.Email != null)
            {
                result = user.Email;
            }

            return result;
        }

        public LeadFollowup GetLatestFollowupOfLead(ICollection<LeadFollowup> followup)
        {
            LeadFollowup LatestFollowup = new LeadFollowup();

            if (followup != null && followup.Count() > 0)
            {
                LatestFollowup  = followup.OrderByDescending(z => z.CreateDate).FirstOrDefault();
            }

            return LatestFollowup;
        }

        public AspNetUser ManagerbyId(string id)
        {
            var ManagerId = _context.AspNetUsers.Where(x => x.Id == id).FirstOrDefault();
            return ManagerId;
        }
    }

}