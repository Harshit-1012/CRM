using CallingCRM.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CallingCRM.Services
{
    public class StaffServices
    {
        public hwLiveEntities _context = new hwLiveEntities();
        private object id;

        public AspNetUser getUserByUsername(string username)
        {
            return _context.AspNetUsers.Where(x => x.UserName == username).FirstOrDefault();
        }

        public List<StaffList> GetStaffListByRole(string UserId)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            List<StaffList> users = _context.Staffs.Where(x=>x.AdminId ==UserId)
                .Select(x => new StaffList
                {
                    
                    UserId = x.UserId,
                    FullName = x.FullName,
                    Email =x.AspNetUser.Email,
                    MobilePersonal =x.AspNetUser.PhoneNumber,
                    Skills =x.Skills,
                    Designation=x.Designation,
                    Department = x.Department
            }
            ).ToList();
            
            return users;
        }

        public StaffList GetStaffById(string id, string Userid)
        {
            
            ApplicationDbContext context = new ApplicationDbContext();
            StaffList users = context.Users.Include(x => x.Roles)
                .Where(x => x.Roles.Any(y => y.RoleId == "7") && x.Id == id && x.AdminID== Userid)
                .Select(x => new StaffList()
                {
                    UserId = x.Id,
                    FullName = x.FullName,
                    MobilePersonal = x.PhoneNumber,
                    Email = x.Email,
                    Password = "Abcd@1234",
                    comment= x.Comment
                    })
                .FirstOrDefault();
           StaffList staff = _context.Staffs.Where(x => x.UserId == id)
                .Select(x => new StaffList()
                {
                    AddressLocal = x.AddressLocal,
                    AddressPermanent = x.AddressPermanent,
                    Designation = x.Designation,
                    JoiningDate = x.JoiningDate.ToString(),
                    MobileHome = x.MobileHome,
                    Reference1 = x.Reference1,
                    Reference2 = x.Reference2,
                    Salary = x.Salary,
                    SalaryDate = x.SalaryDate,
                    Skills = x.Skills,
                    TotalExpYears = x.TotalExpYears,
                    EmployeeCode = x.EmployeeCode

                }).FirstOrDefault();
            

            users.AddressLocal = staff.AddressLocal;
            users.AddressPermanent = staff.AddressPermanent;
            users.Designation = staff.Designation;
            users.JoiningDate = Convert.ToDateTime(staff.JoiningDate).ToString("yyyy-MM-dd");
            users.MobileHome = staff.MobileHome;
            users.Reference1 = staff.Reference1;
            users.Reference2 = staff.Reference2;
            users.Salary = staff.Salary;
            users.SalaryDate = staff.SalaryDate;
            users.Skills = staff.Skills;
            users.TotalExpYears = staff.TotalExpYears;
            users.EmployeeCode = staff.EmployeeCode;
            //users.comment = user.Comment;
           
            return users;
        }


        public void UpdateStaff(StaffList staff, string id)
        {
            AspNetUser users = _context.AspNetUsers.Where(x => x.Id == id).FirstOrDefault();
            Staff user = _context.Staffs.Where(x => x.UserId == id).FirstOrDefault();

            if (user != null)
            {
                users.FullName = staff.FullName;
                users.PhoneNumber = staff.MobilePersonal;
                users.Comment = staff.comment;
                user.FullName = staff.FullName;
                user.MobilePersonal = staff.MobilePersonal;
                user.MobileHome = staff.MobileHome;
                user.AddressLocal = staff.AddressLocal;
                user.AddressPermanent = staff.AddressPermanent;
                user.Reference1 = staff.Reference1;
                user.Reference2 = staff.Reference2;
                user.Designation = staff.Designation;
                user.JoiningDate = Convert.ToDateTime(staff.JoiningDate);
                user.TotalExpYears = staff.TotalExpYears;
                user.Salary = staff.Salary;
                user.SalaryDate = staff.SalaryDate;
                user.Skills = staff.Skills;
                user.UserId = id;
            }
            else
            {
                Staff NewStaff = new Staff();
                NewStaff.FullName = staff.FullName;
                NewStaff.Email = staff.Email;
                NewStaff.MobilePersonal = staff.MobilePersonal;
                NewStaff.MobileHome = staff.MobileHome;
                NewStaff.AddressLocal = staff.AddressLocal;
                NewStaff.AddressPermanent = staff.AddressPermanent;
                NewStaff.Reference1 = staff.Reference1;
                NewStaff.Reference2 = staff.Reference2;
                NewStaff.Designation = staff.Designation;
                NewStaff.JoiningDate = Convert.ToDateTime(staff.JoiningDate);
                NewStaff.TotalExpYears = staff.TotalExpYears;
                NewStaff.Salary = staff.Salary;
                NewStaff.SalaryDate = staff.SalaryDate;
                NewStaff.Skills = staff.Skills;
                NewStaff.UserId = id;
                NewStaff.EmployeeCode = staff.EmployeeCode;
                _context.Staffs.Add(NewStaff);
            }
            _context.SaveChanges();

        }

    }
}