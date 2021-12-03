using CallingCRM.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CallingCRM.Services
{
    public class TelecallerServices
    {
        public hwLiveEntities _context = new hwLiveEntities();

        public AspNetUser getUserByUsername(string username)
        {
            return _context.AspNetUsers.Where(x => x.UserName == username).FirstOrDefault();
        }

       

        public List<TelecallerList> GetTelecallersListByRole(string userid)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            List<TelecallerList> users = context.Users.Include(x => x.Roles)
                .Where(x => x.Roles.Any(y => y.RoleId == "5") && x.AdminID == userid)
                .Select(x => new TelecallerList()
                {
                    id = x.Id,
                    Name = x.FullName,
                    Mobile = x.PhoneNumber,
                    Email = x.Email,
                    Password = "",
                    comment = "",
                    ManagerId=x.ManagerID                     
                })
                .ToList();
            return users;
        }


        public List<TelecallerList> GetTelecallersListByManagerId(string userid)
        {
            var clientId = ConfigurationManager.AppSettings["ClientId"].ToString();
            ApplicationDbContext context = new ApplicationDbContext();
            List<TelecallerList> users = context.Users.Include(x => x.Roles)
                .Where(x => x.Roles.Any(y => y.RoleId == "5") && x.ManagerID == userid && x.AdminID == clientId)
                .Select(x => new TelecallerList()
                {
                    id = x.Id,
                    Name = x.FullName,
                    Mobile = x.PhoneNumber,
                    Email = x.Email,
                    Password = "",
                    comment = "",
                    ManagerId = x.ManagerID
                })
                .ToList();
            return users;
        }


        public TelecallerList GetTelecallerById(string id, string UserId)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            TelecallerList users = context.Users.Include(x => x.Roles)
                .Where(x => x.Roles.Any(y => y.RoleId == "5") && x.Id == id && x.AdminID == UserId)
                .Select(x => new TelecallerList()
                {
                    id = x.Id,
                    Name = x.FullName,
                    Mobile = x.PhoneNumber,
                    Email = x.Email,
                    Password = x.PasswordHash,
                    comment = "",
                    ManagerId=x.ManagerID
                }).FirstOrDefault();
            return users;
        }


        public void UpdateTelecaller(TelecallerList telecaller, string id, string UserId)
        {
            AspNetUser user = _context.AspNetUsers.Where(x => x.Id == id).FirstOrDefault();

            user.FullName = telecaller.Name;
            user.PhoneNumber = telecaller.Mobile;
            user.Comment = telecaller.comment;
            user.AdminId = ConfigurationManager.AppSettings["ClientId"].ToString();
            user.ManagerId = telecaller.ManagerId;

            _context.SaveChanges();
        }

         public List<AspNetUser> GetUsersListByRole(string UserId)
        {
            var userList = new List<AspNetUser>();

            ApplicationDbContext context = new ApplicationDbContext();
            List<ApplicationUser> users = context.Users.Include(x => x.Roles).Where(x => x.Roles.Any(y => y.RoleId == "3") && x.EmailConfirmed && x.AdminID== UserId).ToList();

            foreach (ApplicationUser user in users)
            {
                var u = _context.AspNetUsers.Where(x => x.UserName == user.UserName).First();
                userList.Add(u);
            }

            return userList;
        }
    }
}