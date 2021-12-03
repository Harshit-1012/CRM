using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
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
    public class UserServices //: IUserServices
    {
        static CommonHelper _helper = new CommonHelper();

        public DateTime thisDateTime = _helper.GetCurrentDate();

        public hwLiveEntities _context = new hwLiveEntities();

        public AspNetUser getUserByUsername(string username)
        {
            return _context.AspNetUsers.Where(x => x.UserName == username).FirstOrDefault(); 
        }

        public AspNetUser getUserById(string UserId)
        {
            return _context.AspNetUsers.Where(x => x.Id == UserId).FirstOrDefault();
        }

        public Boolean isUserExist(string email)
        {
            var userCount = _context.AspNetUsers.Where(x => x.Email == email || x.UserName == email).Count();

            if (userCount == 0) return false;

            return true;
        }

        public void UpdateUser(AspNetUser user)
        {
            var _user = getUserByUsername(user.UserName);
            _user.FullName = user.FullName;
            _user.ManagerId = user.ManagerId;
            _user.TeamLeaderId = user.TeamLeaderId;

            if (user.ManagerId != "0") _user.ManagerId = null;
            if (user.TeamLeaderId != "0") _user.TeamLeaderId = null;

            _context.SaveChanges();
        }

        public HomeStatsModels GetDashboardStats(string UserId)
        {
            HomeStatsModels stats = new HomeStatsModels();
            var client = ConfigurationManager.AppSettings["ClientId"].ToString();
            thisDateTime = _helper.GetCurrentDate();

            var TotalLead = _context.Leads.Where(x => x.AssignedToUser == null && x.AdminId == client).Count();

            //var TotalCalls = _context.Leads.Where(x => x.AssignedToUser == UserId && x.LeadStatus != null).Count();
            
            var TotalCalls = _context.LeadFollowups.Include(x => x.Lead).Where(x => x.Lead.AssignedToUser == UserId && x.Lead.LeadStatus != null && x.Lead.AdminId==client).Count();

            var TotalSales = _context.Sales.Include(x=> x.Lead).Where(x => x.Lead.AssignedToUser == UserId && x.Lead.AdminId == client).Sum(x=> x.Amount);

            //var TodayCalls = _context.Leads.Where(x => x.AssignedToUser == UserId && x.LeadStatus != null && DbFunctions.TruncateTime(x.AssignedDate) == DbFunctions.TruncateTime(thisDateTime)).Count();

            var TodayCalls = _context.LeadFollowups
                .Include(x => x.Lead)
                .Where(x => x.Lead.AssignedToUser == UserId && DbFunctions.TruncateTime(x.CreateDate) == DbFunctions.TruncateTime(thisDateTime) && x.Lead.AdminId == client).Count();
            int roles = _context.AspNetUsers.Where(x => x.Id == UserId && x.AdminId == client && x.AspNetRoles.Where(y => y.Name == "Manager").Any()).Count();
            if (roles>0)
            {
                TotalCalls = _context.LeadFollowups.Include(x => x.Lead).Where(x => x.Lead.AssignedToUser != null && x.Lead.LeadStatus != null && x.Lead.AspNetUser.ManagerId== UserId && x.Lead.AdminId == client).Count();
                TotalSales = _context.Sales.Include(x => x.Lead).Where(x => x.Lead.AssignedToUser != null && x.Lead.AspNetUser.ManagerId == UserId && x.Lead.AdminId == client).Sum(x => x.Amount);
                TodayCalls = _context.LeadFollowups
                    .Include(x => x.Lead)
                    .Where(x => x.Lead.AssignedToUser != null && DbFunctions.TruncateTime(x.CreateDate) == DbFunctions.TruncateTime(thisDateTime) && x.Lead.AspNetUser.ManagerId == UserId && x.Lead.AdminId == client).Count();
            }

            stats.TotalLeads = TotalLead;
            stats.TotalCalls = TotalCalls;
            stats.TotalSales = Convert.ToDecimal(TotalSales);
            stats.TodaysCalls = TodayCalls;

            return stats;
        }

        public List<AspNetUser> GetUsersListByRole()
        {
            var userList = new List<AspNetUser>();

            ApplicationDbContext context = new ApplicationDbContext();
            
            List<ApplicationUser> users = context.Users.Include(x=>x.Roles).Where(x=> x.Roles.Any(y => y.RoleId == "5") && x.EmailConfirmed ).ToList();
            foreach (ApplicationUser user in users)
            {
                var u = _context.AspNetUsers.Where(x=> x.UserName == user.UserName).First();
                userList.Add(u);
            }
            return userList;
        }
        public List<AspNetUser> GetUsersListByRole(string UserId)
        {
            var userList = new List<AspNetUser>();

            ApplicationDbContext context = new ApplicationDbContext();

            List<ApplicationUser> users = context.Users.Include(x => x.Roles).Where(x => x.Roles.Any(y => y.RoleId == "5") && x.ManagerID == UserId).ToList();
            foreach (ApplicationUser user in users)
            {
                var u = _context.AspNetUsers.Where(x => x.UserName == user.UserName).First();
                userList.Add(u);
            }
            return userList;
        }

        public List<Client> GetClientsList()
        {
            var clients = _context.Clients.OrderByDescending(x => x.CreateDate).ToList();
            return clients;
        }

        public List<AspNetUser> GetUsersListByManagerId(string ManagerId)
        {
            var user = _context.AspNetUsers.Where(x => x.ManagerId == ManagerId).ToList();

            return user;
        }
        
         public List<ApplicationUser> GetUsersListByAdminId(string Id)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var user = context.Users.Include(x => x.Roles)
                .Where(x => x.Roles.Any(y => y.RoleId == "5") && x.AdminID == Id).ToList();

            return user;
        }



        public Client CreateClient(Client client)
        {
            var _client = _context.Clients.Add(client);
            _context.SaveChanges();

            return _client;
        }

        public Client GetClientBySuperAdmin(long id)
        {
            Client client = _context.Clients.Where(x => x.Id == id).FirstOrDefault();
            return client;
        }

        public Client GetClientById(string id)
        {
            Client client = _context.Clients.Where(x => x.ClientID == id).FirstOrDefault();
            return client;
        }

        public Client UpdateClient(Client client)
        {
            var _client = client;

            if (client != null && client.Id != 0)
            {
                var oldClient = _context.Clients.Where(x => x.Id == client.Id).FirstOrDefault();

                if (oldClient == null)
                {
                    throw new ApplicationException("This client does not exist.");
                }

                client.ClientID = oldClient.ClientID;
                client.ClientEmailMain = oldClient.ClientEmailMain;
                client.ClientPassword = oldClient.ClientPassword;

                _context.Entry(oldClient).CurrentValues.SetValues(client);
                _context.SaveChanges();

                _client = client;
            }
            else 
            {
                throw new ApplicationException("Invalid data of client.");
            }

            return _client;
        }

        public List<AspNetUser> GetUsersByRoleId(string FindingRoleId, string CurrentRole, string CurrentUserId)
        {
            var userList = new List<AspNetUser>();

            var ClientId = ConfigurationManager.AppSettings["ClientId"].ToString();

            ApplicationDbContext context = new ApplicationDbContext();
            List<ApplicationUser> users = context.Users.Include(x => x.Roles).Where(x => x.AdminID == ClientId && x.Roles.Any(y => y.RoleId == FindingRoleId) && x.EmailConfirmed).ToList();

            foreach (ApplicationUser user in users)
            {
                var u = _context.AspNetUsers.Where(x => x.UserName == user.UserName).First();
                userList.Add(u);
            }

            return userList;
        }

        public string ValidateUserLimit(string name)
        {

            ApplicationDbContext context = new ApplicationDbContext();
            var user = _context.AspNetUsers.Where(x => x.UserName == name).FirstOrDefault();
            var UserType = (user.AspNetRoles.FirstOrDefault()).Name;
            Client ThisUser;
            var currentDate = _helper.GetCurrentDate();
            var date = "";
            if (UserType == "Admin")
            {
                ThisUser = _context.Clients.Where(x => x.ClientID == user.Id).FirstOrDefault();
                
            }
            else if (UserType == "SuperAdmin")
            {

                return date;
            }
            else
            {
                ThisUser = _context.Clients.Where(x => x.ClientID == user.AdminId).FirstOrDefault();
                
            }
            if (ThisUser.ExpiryDate <= currentDate)
            {
                date = Convert.ToDateTime(ThisUser.ExpiryDate).ToString("yyyy-MM-dd");
                return date;
            }
            else if (!ThisUser.IsActive)
            {
                date = "Suspended";
                return date;
            }
            return date;
        }

        public bool ManagerLimit(string id)
        {
            var limit = _context.Clients.Where(x => x.ClientID == id).Select(x => x.ManagerAccountLimit).FirstOrDefault();
            //var Client = ConfigurationManager.AppSettings["ClientId"].ToString();
            var TotalManager = _context.AspNetUsers.Where(x => x.AdminId == id && x.AspNetRoles.Where(y => y.Name == "Manager").Any()).Count();
            if (TotalManager < limit)
            {
                return false;
            }
            return true;
        }

        public bool TelecallerLimit(string id)
        {
            var limit = _context.Clients.Where(x => x.ClientID == id).Select(x => x.UserAccountLimit).FirstOrDefault();
            //var Client = ConfigurationManager.AppSettings["ClientId"].ToString();
            var TotalAgent = _context.AspNetUsers.Where(x => x.AdminId == id && x.AspNetRoles.Where(y => y.Name == "Agent").Any()).Count();
            if (TotalAgent < limit)
            {
                return false;
            }

            return true;
        }

        
    }

}