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
    public class PayrollServices
    {
        static CommonHelper _helper = new CommonHelper();

        public DateTime thisDateTime = _helper.GetCurrentDate();

        public hwLiveEntities _context = new hwLiveEntities();


        public List<AspNetUser> GetUsersListByRole()
        {
            var userList = new List<AspNetUser>();

            ApplicationDbContext context = new ApplicationDbContext();
            List<ApplicationUser> users = context.Users.Include(x => x.Roles).Where(x => x.Roles.Any(y => y.RoleId == "7") /*&& x.EmailConfirmed*/).ToList();

            foreach (ApplicationUser user in users)
            {
                var u = _context.AspNetUsers.Where(x => x.UserName == user.UserName).First();
                userList.Add(u);
            }

            return userList;
        }


        

        public List<SalaryRevision> GetPayrollList()
        {
            var userId = ConfigurationManager.AppSettings["ClientID"].ToString();
            List<SalaryRevision> result = _context.SalaryRevisions.Where(x=>x.AdminId == userId).ToList();

            return result;

        }
         
        public SalaryRevision GetPayrollbyId(string Id)
        {
            long? id = Convert.ToInt64(Id);
            SalaryRevision result = _context.SalaryRevisions.Where(x => x.Id == id).FirstOrDefault();

            return result;
                
        }

        public void UpdatePayroll(Payroll payroll ,string id)
        {
            Payroll result = _context.Payrolls.Where(x => x.Id == payroll.Id).FirstOrDefault();
           if(result == null) { 
                Payroll newpayroll = new Payroll();
                newpayroll.UserId = id;
                newpayroll.Basic = payroll.Basic;
                newpayroll.HRA = payroll.HRA;
                newpayroll.Conveyance = payroll.Conveyance;
                newpayroll.SpecialAllowance= payroll.SpecialAllowance;
                newpayroll.TravellingAllowance= payroll.TravellingAllowance;
                newpayroll.DistanceAllowance = payroll.DistanceAllowance;
                newpayroll.MobileInternetAllowance = payroll.MobileInternetAllowance;
                newpayroll.ArrearBF = payroll.ArrearBF;
                newpayroll.OtherAllowance= payroll.OtherAllowance;
                newpayroll.Overtime= payroll.Overtime;
                newpayroll.MealAllowance= payroll.MealAllowance;
                newpayroll.CabAllowance= payroll.CabAllowance;
                newpayroll.LoanDeduction= payroll.LoanDeduction;
                newpayroll.AdvanceSalary= payroll.AdvanceSalary;
                newpayroll.ExpensesPaid= payroll.ExpensesPaid;
                newpayroll.ArrearCF= payroll.ArrearCF;
                newpayroll.OtherDeductions= payroll.OtherDeductions;
                newpayroll.Month= payroll.Month;
                newpayroll.Year = _helper.GetCurrentDate().Year;
                newpayroll.CreatedDate = _helper.GetCurrentDate();
                _context.Payrolls.Add(newpayroll);
            }
            _context.SaveChanges();

        }

        public List<Payroll> GetPayrollListByUser(string id)
        {
            List<Payroll> result = _context.Payrolls.OrderByDescending(x => x.Month).Where(x => x.UserId == id).ToList();

            return result;

        }

        public Payroll GetCurrentPayrollByUserID(string Id, string Month, string Year)
        {
            int month= 0;
            int year = 0;
            if (Month == "")
            {
               month = Convert.ToInt16(_helper.GetCurrentDate().Month);
            }
            else
            {
                month = Convert.ToInt16(Month);
            }
            if (Year == "")
            {
                year = Convert.ToInt16(_helper.GetCurrentDate().Year);
            }
            else
            {
                year = Convert.ToInt16(Year);
            }
            Payroll pr = _context.Payrolls.OrderByDescending(x => x.Month).Where(x => x.UserId == Id && x.Month <= month && x.Year <= year).Take(1).FirstOrDefault();
            return pr;
        }
        public Staff getStaffbyId(string id)
        {
            var result = _context.Staffs.Where(x => x.UserId == id).FirstOrDefault();
            return result;
        }
    }
}