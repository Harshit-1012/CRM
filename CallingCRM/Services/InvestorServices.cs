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
    public class InvestorServices
    {
        public hwLiveEntities _context = new hwLiveEntities();

        public CommonHelper _helper = new CommonHelper();     

        public AspNetUser getUserByUsername(string username)
        {
            return _context.AspNetUsers.Where(x => x.UserName == username).FirstOrDefault();
        }

        public List<InvestorList> GetInvestorsListByRole(string UserId)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            List<InvestorList> users = context.Users.Include(x => x.Roles)
                .Where(x => x.Roles.Any(y => y.RoleId == "8") && x.AdminID == UserId)
                .Select(x => new InvestorList()
                {
                    UserId = x.Id,
                    FullName = x.FullName,
                    PhoneNumber = x.PhoneNumber,
                    Email = x.Email,
                    Password = "",
                    comment = ""
                }).ToList();
            return users;
        }

        public InvestorList GetInvestorById(string id, string UserId)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            InvestorList users = context.Users.Include(x => x.Roles)
                .Where(x => x.Roles.Any(y => y.RoleId == "8") && x.Id == id && x.AdminID == UserId)
                .Select(x => new InvestorList()
                {
                    UserId = x.Id,
                    FullName = x.FullName,
                    PhoneNumber = x.PhoneNumber,
                    Email = x.Email,
                    Password = "",
                    comment = ""
                    
                })
                .FirstOrDefault();
            InvestorList investor = _context.Investors.Where(x => x.UserId == users.UserId )
             .Select(x => new InvestorList()
             {
                 CloseDate = x.CloseDate
            
            }).FirstOrDefault();
            users.CloseDate = investor.CloseDate;
            
            return users;
        }

        public void UpdateInvestor(InvestorList investor, string id)
        {
            AspNetUser users = _context.AspNetUsers.Where(x => x.Id == id).FirstOrDefault();
            Investor user = _context.Investors.Where(x => x.UserId == id).FirstOrDefault();

            if (user != null)
            {
                users.FullName= investor.FullName;
                users.PhoneNumber = investor.PhoneNumber;
                user.FullName = investor.FullName;
                user.PhoneNumber = investor.PhoneNumber;
                user.CloseDate= investor.CloseDate;
                user.CreatedAt = _helper.GetCurrentDate();
                user.UserId = id;
            }
            else
            {
                Investor newInvestor = new Investor();
                newInvestor.FullName = investor.FullName;
                newInvestor.Email = investor.Email;
                newInvestor.PhoneNumber = investor.PhoneNumber;
                newInvestor.CloseDate= investor.CloseDate;
                newInvestor.UserId = id;
                newInvestor.IsActive = true;
                _context.Investors.Add(newInvestor);

            }

            _context.SaveChanges();

        }

   

    }
}