using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.Entity;
using CallingCRM.Models;
using Microsoft.AspNet.Identity;

namespace CallingCRM.Services
{
    public class ManagerServices
    {
        public hwLiveEntities _context = new hwLiveEntities();

        public AspNetUser getUserByUsername(string username)
        {
            return _context.AspNetUsers.Where(x => x.UserName == username).FirstOrDefault();
        }

        public List<ManagerList> GetManagerListByRole(string Userid)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var Client = ConfigurationManager.AppSettings["ClientId"].ToString();
            List<ManagerList> users = context.Users.Include(x => x.Roles)
                .Where(x => x.Roles.Any(y => y.RoleId == "3") && x.AdminID == Userid)
                .Select(x => new ManagerList() {
                    id = x.Id,
                    Name = x.FullName,
                    Mobile=x.PhoneNumber,
                    Email=x.Email,
                    Password = "",
                    comment = ""
                })
                .ToList();
            return users;
        }

        public ManagerList GetManagerById(string id, string Userid)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            ManagerList users = context.Users.Include(x => x.Roles)
                .Where(x => x.Roles.Any(y => y.RoleId == "3") && x.Id == id && x.AdminID == Userid)
                .Select(x => new ManagerList()
                {
                    id = x.Id,
                    Name = x.FullName,
                    Mobile = x.PhoneNumber,
                    Email = x.Email,
                    Password = "",
                    comment = ""
                })
                .FirstOrDefault();
            return users;
        }


        public void UpdateManager(ManagerList manager, string id)
        {
            AspNetUser user = _context.AspNetUsers.Where(x => x.Id == id).FirstOrDefault();
            user.FullName = manager.Name;
            user.PhoneNumber = manager.Mobile;
            user.Comment = manager.comment;
            user.AdminId = ConfigurationManager.AppSettings["ClientId"].ToString();
            _context.SaveChanges();

        }




    }

}
