using CallingCRM.Helpers;
using CallingCRM.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CallingCRM.Services
{
    public class CustomerServices
    {
        public hwLiveEntities _context = new hwLiveEntities();

        public CommonHelper _helper = new CommonHelper();

        public AspNetUser getUserByUsername(string username)
        {
            return _context.AspNetUsers.Where(x => x.UserName == username).FirstOrDefault();
        }

        public List<CustomerList> GetCustomersListByRole(string UserId)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            List<CustomerList> users = context.Users.Include(x => x.Roles)
                .Where(x => x.Roles.Any(y => y.RoleId == "9") && x.AdminID == UserId)
                .Select(x => new CustomerList()
                {
                    UserId = x.Id,
                    FullName = x.FullName,
                    PhoneNumber = x.PhoneNumber,
                    Email = x.Email,
                    Password = "",
                    comment = "",
                })
                .ToList();
            return users;
        }

        public CustomerList GetCustomerById(string id, string UserId)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            CustomerList users = context.Users.Include(x => x.Roles)
                .Where(x => x.Roles.Any(y => y.RoleId == "9") && x.Id == id && x.AdminID== UserId)
                .Select(x => new CustomerList()
                {
                    UserId = x.Id,
                    FullName = x.FullName,
                    PhoneNumber = x.PhoneNumber,
                    Email = x.Email,
                    Password = "",
                    comment = "",

                })
                .FirstOrDefault();

            CustomerList customer = _context.Customers.Where(x => x.UserId == users.UserId)
                .Select(x => new CustomerList()
                {
                    
                    Ship_Country = x.Ship_Country,
                    Ship_City = x.Ship_City,
                    Ship_State = x.Ship_State,
                    Ship_Pincode = x.Ship_Pincode,
                    Bill_Country = x.Bill_Country,
                    Bill_State = x.Bill_State,
                    Bill_City=x.Bill_City,
                    Bill_Pincode=x.Bill_Pincode,
                    IsActive=x.IsActive

                })
                .FirstOrDefault();

            
            users.Ship_Country = customer.Ship_Country;
            users.Ship_City = customer.Ship_City;
            users.Ship_State = customer.Ship_State;
            users.Ship_Pincode = customer.Ship_Pincode;
            users.Bill_Country = customer.Bill_Country;
            users.Bill_State = customer.Bill_State;
            users.Bill_City = customer.Bill_City;
            users.Bill_Pincode = customer.Bill_Pincode;
            users.IsActive = customer.IsActive;

            return users;
        }

        public void UpdateCustomer(CustomerList customer, string id)
        {
            AspNetUser users = _context.AspNetUsers.Where(x => x.Id == id).FirstOrDefault();
            Customer user = _context.Customers.Where(x => x.UserId == id).FirstOrDefault();

            if (user != null)
            {
                users.FullName = customer.FullName;
                users.PhoneNumber = customer.PhoneNumber;
                user.FullName = customer.FullName;
                user.PhoneNumber = customer.PhoneNumber;
                user.Ship_Country = customer.Ship_Country;
                user.Ship_State = customer.Ship_State;
                user.Ship_City = customer.Ship_City;
                user.Ship_Pincode = customer.Ship_Pincode;
                user.Bill_Country = customer.Ship_Country;
                user.Bill_State = customer.Ship_State;
                user.Bill_City = customer.Ship_City;
                user.Bill_Pincode = customer.Bill_Pincode;
                user.IsActive = customer.IsActive;
                
                user.UserId = id;
            }
            else
            {
                Customer newCustomer = new Customer();
                newCustomer.FullName = customer.FullName;
                newCustomer.Email = customer.Email;
                newCustomer.PhoneNumber = customer.PhoneNumber;
                newCustomer.PhoneNumber = customer.PhoneNumber;
                newCustomer.Ship_Country = customer.Ship_Country;
                newCustomer.Ship_State = customer.Ship_State;
                newCustomer.Ship_City = customer.Ship_City;
                newCustomer.Ship_Pincode = customer.Ship_Pincode;
                newCustomer.Bill_Country = customer.Ship_Country;
                newCustomer.Bill_State = customer.Ship_State;
                newCustomer.Bill_City = customer.Ship_City;
                newCustomer.Bill_Pincode = customer.Bill_Pincode;
                newCustomer.IsActive = customer.IsActive;
                newCustomer.UserId = id;
                newCustomer.CreatedAt = _helper.GetCurrentDate(); ;
                _context.Customers.Add(newCustomer);
                users.CreateDate = _helper.GetCurrentDate();
            }

            _context.SaveChanges();

        }

    }
}