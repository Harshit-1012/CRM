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
    public class LedgerServices
    {
        public hwLiveEntities _context = new hwLiveEntities();
        public CommonHelper _helper = new CommonHelper();

        public List<AspNetUser> GetUsersList(string UserId)
        {

            var userList = new List<AspNetUser>();

            ApplicationDbContext context = new ApplicationDbContext();
            List<ApplicationUser> users = context.Users.Where(x => x.EmailConfirmed == true && x.AdminID == UserId).ToList();

            foreach (ApplicationUser user in users)
            {
                var u = _context.AspNetUsers.Where(x => x.UserName == user.UserName && x.AdminId == UserId).First();
                userList.Add(u);
            }

            return userList;
        }

        public LedgerHistory GetHistoryById(long? id, string UserId)
        {
            var result = new LedgerHistory();
            if (id != null)
            {
                id = Convert.ToInt64(id);
                result = _context.LedgerHistories
                    .Include(x => x.AspNetUser2)      // tousr
                    .Include(x => x.AspNetUser1)   // from
                    .Where(x => x.Id == id && x.AdminId == UserId).FirstOrDefault();
            }
            return result;
        }


        public bool GetHistoryListById(long? id, string UserId)
        {
            if (id != null)
            {
                return _context.LedgerHistories.Where(x => x.Id == id && x.AdminId == UserId).Any();
            }
            else
            {
                return false;
            }
        }

        public List<LedgerHistory> GetHistoryListByClient(string Status, string UserId)
        {
            //var ClientID = ConfigurationManager.AppSettings["ClientId"];
            List<LedgerHistory> history = _context.LedgerHistories
                .Include(x=> x.AspNetUser2)      // tousr
                .Include(x => x.AspNetUser)    // admin
                .Include(x => x.AspNetUser1)   // from
                .Where(x => x.AdminId == UserId).ToList();

            if (Status != null && Status != "")
            {
                if (Status == "Overdue")
                {
                    var today = Convert.ToDateTime(_helper.GetCurrentDate().ToString("yyyy-MM-dd"));
                    history = history.Where(x =>x.DueDate<= today || x.DateofTransaction <= today).ToList();                }
                else
                {
                    history = history.Where(x => x.Status == Status).ToList();
                }
            }

            return history;
        }



        public void UpdateHistory(LedgerHistory History)
        {

            LedgerHistory history = _context.LedgerHistories.Where(x => x.Id == History.Id).FirstOrDefault();
            if (history != null)
            {
                history.Id = History.Id;
                history.TypeOfTransaction = History.TypeOfTransaction;
                history.FromUser = History.FromUser;
                history.ToUser = History.ToUser;
                history.Amount = History.Amount;
                history.DateofTransaction = History.DateofTransaction;
                history.PurposeofTransaction = History.PurposeofTransaction;

            }
            else
            {
                LedgerHistory newhistory = new LedgerHistory();
                newhistory.Id = History.Id;
                newhistory.TypeOfTransaction = History.TypeOfTransaction;
                newhistory.FromUser = History.FromUser;
                newhistory.ToUser = History.ToUser;
                newhistory.Amount = History.Amount;
                newhistory.DateofTransaction = History.DateofTransaction;
                newhistory.PurposeofTransaction = History.PurposeofTransaction;
                newhistory.AdminId = ConfigurationManager.AppSettings["ClientID"];
                _context.LedgerHistories.Add(newhistory);
            }
            _context.SaveChanges();

        }
    }
}