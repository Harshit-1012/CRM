using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using CallingCRM.Models;
using CallingCRM.Services;
using CallingCRM.Helpers;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.IO;
using CsvHelper;
using System.Data;
using System.Reflection;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;

namespace CallingCRM.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public UserServices _userService = new UserServices();
        public CallService _callService = new CallService();
        public SalesService _salesService = new SalesService();
        static public CommonHelper _helper = new CommonHelper();
        public ManagerServices _managerService = new ManagerServices();
        public TelecallerServices _telecallerServices = new TelecallerServices();
        public StaffServices _staffServices = new StaffServices();
        public InvestorServices _investorServices = new InvestorServices();
        public CustomerServices _customerServices = new CustomerServices();
        public InvestmentServices _investmentServices = new InvestmentServices();
        public LeaveServices _leaveServices = new LeaveServices();
        public ReportServices _reportServices = new ReportServices();
        public PayrollServices _payrollServices = new PayrollServices();
        static DateTime thisDateTime = _helper.GetCurrentDate();


        #region Dependencies
        public AdminController()
        {
        }

        public AdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            //_userService = userService;
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        #endregion

        public ActionResult Index()
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();

            return View();
        }

        public ActionResult ManageLeads()
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();

            var Callers = _userService.GetUsersByRoleId("5", "Admin", UserId);

            return View();
        }

        [HttpPost]
        public ActionResult ImportLeads(HttpPostedFileBase file)
        {
            List<ImportLeadItem> leads = new List<ImportLeadItem>();
            int FailedImport = 0;
            int SuccessImport = 0;
            int DuplicateImport = 0;
            int PreAssignedImport = 0;

            try
            {
                if (file.ContentLength > 0)
                {
                    var leadData = new DataTable();
                    StreamReader csvReader = new StreamReader(file.InputStream);

                    using (var csv = new CsvReader(csvReader, CultureInfo.InvariantCulture))
                    {
                        using (var dr = new CsvDataReader(csv))
                        {
                            leadData.Load(dr);
                        }
                    }

                    if (leadData.Rows.Count >= 1)
                    {
                        for (int i = 0; i < leadData.Rows.Count; i++)
                        {
                            try
                            {
                                Lead lead = new Lead();
                                lead.FullName = leadData.Rows[i][0].ToString();
                                lead.Email = leadData.Rows[i][1].ToString();
                                lead.Mobile = leadData.Rows[i][2].ToString();
                                lead.Location = leadData.Rows[i][3].ToString();
                                lead.PinCode = leadData.Rows[i][4].ToString();
                                lead.LeadSource = leadData.Rows[i][5].ToString();

                                if (leadData.Rows[i][6] != null && leadData.Rows[i][6].ToString() != "")
                                {
                                    lead.AssignedToUser = leadData.Rows[i][6].ToString();
                                }
                                lead.LeadStatus = leadData.Rows[i][8].ToString();

                                if (leadData.Rows[i][7] != null && leadData.Rows[i][7].ToString() != "")
                                {
                                    if (Convert.ToDateTime(leadData.Rows[i][7]) >= thisDateTime)
                                    {
                                        lead.AssignedDate = Convert.ToDateTime(leadData.Rows[i][7]);
                                    }
                                }

                                if (lead.FullName != "FullName")
                                {
                                    var result = _callService.AddLead(lead);

                                    if (result == "Success")
                                    {
                                        SuccessImport++;
                                    }
                                    else if (result == "Updated")
                                    {
                                        DuplicateImport++;
                                    }
                                    else if (result == "Lead by this number or email already exist in database.")
                                    {
                                        PreAssignedImport++;
                                    }
                                    else
                                    {
                                        FailedImport++;
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                }
                Session["sMessage"] = "Leads has been imported Successfully!! Success= "+ SuccessImport.ToString() +" | Already Assigned = "+ PreAssignedImport.ToString() +" | Duplicate = "+ DuplicateImport.ToString() +" | Failed = " + FailedImport.ToString();
            }
            catch (Exception ex)
            {
                Session["eMessage"] = "Lead import failed!! " + ex.Message;
            }

            return new RedirectResult("/Admin/ManageLeads");
            //return null;
        }

        [HttpPost]
        public ActionResult ExportLeads()
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();

            try
            {
                DateTime StartDate = Convert.ToDateTime(Request.Form["start"]);
                DateTime EndDate = Convert.ToDateTime(Request.Form["end"]);
                string type = Convert.ToString(Request.Form["leadtype"]);

                List<ExportLeadItem> leads = new List<ExportLeadItem>();

                leads = _callService.GetExportedLeads(StartDate, EndDate, type, UserId);

                var exportedFileName = type + "_" + StartDate.ToString("yyyy-MM-dd") + "_" + EndDate.ToString("yyyy-MM-dd") + ".csv";
                var emptyFile = Server.MapPath("/") + "assets\\downloads\\Leads_Export.csv";
                var path = Server.MapPath("/") + "assets\\downloads\\" + exportedFileName;

                if (!System.IO.File.Exists(path))
                {
                    System.IO.File.Copy(emptyFile, path);
                }

                Type itemType = typeof(ExportLeadItem);
                var props = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                using (var writer = new StreamWriter(path))
                {
                    writer.WriteLine(string.Join(", ", props.Select(p => p.Name)));

                    foreach (var item in leads)
                    {
                        writer.WriteLine(string.Join(", ", props.Select(p => p.GetValue(item, null))));
                    }
                    writer.Flush();
                   
                }
                
                Session["sMessage"] = "Lead export success!! ";
            }
            catch (Exception ex)
            {
                Session["eMessage"] = "Lead export failed!! " + ex.Message;
            }

            return new RedirectResult("/Admin/ManageLeads");
        }

        [HttpGet]
        [Route("{id?}")]
        public ActionResult Managers(string id)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            var manager = _managerService.GetManagerListByRole(UserId);
            ViewBag.Managers = manager;

            ManagerList ThisManager = new ManagerList();

            if (id != null && id != "")
            {
                ThisManager = _managerService.GetManagerById(id, UserId);
            }
            else
            {
                ThisManager.Password = _helper.RandomString(4, false) + DateTime.Now.ToString("MMM").ToLower() + "@" + DateTime.Now.Year.ToString();
            }

            return View(ThisManager);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Managers(ManagerList manager)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            var _existingUser = _userService.getUserByUsername(manager.Email);
            var LimitExceed = _userService.ManagerLimit(UserId);
            if (LimitExceed)
            {
                Session["eMessage"] = "Your have reached the maximum Manager Limit";
                return View(manager);
            }
            if (_existingUser != null)
            {
                //Session["MessageTStatus"] = "Email address already used to register an account. Please check and try again.";
                try
                {
                    _managerService.UpdateManager(manager, _existingUser.Id);

                    if (manager.Password != null && manager.Password.ToString() != "")
                    {
                        var token = await UserManager.GeneratePasswordResetTokenAsync(_existingUser.Id);
                        var result = await UserManager.ResetPasswordAsync(_existingUser.Id, token, manager.Password);
                    }

                    Session["sMessage"] = "Manager's details has been saved successfully.";
                    return RedirectToAction("/Managers");
                }
                catch (Exception ex)
                {
                    Session["eMessage"] = ex.Message;
                }
            }
            else
            {
                var userData = new ApplicationUser { UserName = manager.Email, Email = manager.Email };
                var result = await UserManager.CreateAsync(userData, manager.Password);
                if (result.Succeeded)
                {
                    var addRole = UserManager.AddToRole(userData.Id, "Manager");
                    _managerService.UpdateManager(manager, userData.Id);

                    Session["sMessage"] = "Manager Created Successfully ! ";
                }
                else
                {
                    AddErrors(result);
                }
            }
            return View(manager);
        }

        [HttpGet]
        [Route("{id?}")]
        public ActionResult Telecallers(string id)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            var telecallers = _telecallerServices.GetTelecallersListByRole(UserId);
            ViewBag.telecallers = telecallers;
            var users = _telecallerServices.GetUsersListByRole(UserId);
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem(){Text = "Select Manager",Value = ""});
            foreach (AspNetUser user in users)
            {
                list.Add(new SelectListItem()
                {
                    Text = user.FullName,
                    Value = user.Id
                });
            }
            ViewBag.ManagerList = list;
            TelecallerList ThisCaller = new TelecallerList();
            if (id != null && id != "")
            {
                ThisCaller = _telecallerServices.GetTelecallerById(id, UserId);
            }
            else
            {
                ThisCaller.Password = _helper.RandomString(4, false) + DateTime.Now.ToString("MMM").ToLower() + "@" + DateTime.Now.Year.ToString();
            }
            return View(ThisCaller);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Telecallers(TelecallerList telecaller)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            var _existingUser = _userService.getUserByUsername(telecaller.Email);
            var LimitExceed = _userService.TelecallerLimit(UserId);
            if (LimitExceed)
            {
                Session["eMessage"] = "Your have reached the maximum Agent Limit";
                return RedirectToAction("/Telecallers");
            }
            if (_existingUser != null)
            {
                try
                {
                    //Session["MessageTStatus"] = "Email address already used to register an account. Please check and try again.";
                    _telecallerServices.UpdateTelecaller(telecaller, _existingUser.Id, _existingUser.AdminId);


                    if (telecaller.Password != null && telecaller.Password.ToString() != "")
                    {
                        var token = await UserManager.GeneratePasswordResetTokenAsync(_existingUser.Id);
                        var result = await UserManager.ResetPasswordAsync(_existingUser.Id, token, telecaller.Password);
                    }
                    Session["sMessage"] = "Telecaller's details has been saved successfully.";
                    return RedirectToAction("/Telecallers");
                }
                catch (Exception ex)
                {
                    Session["eMessage"] = ex.Message;
                }
            }
            else
            {
                var userData = new ApplicationUser { UserName = telecaller.Email, Email = telecaller.Email,AdminID = ConfigurationManager.AppSettings["ClientId"].ToString()};
                var result = await UserManager.CreateAsync(userData, telecaller.Password);
                if (result.Succeeded)
                {
                    var addRole = UserManager.AddToRole(userData.Id, "Agent");
                    _telecallerServices.UpdateTelecaller(telecaller, userData.Id,userData.AdminID);
                    Session["sMessage"] = "Telecaller Created Successfully ! ";
                }
                else
                {
                    AddErrors(result);
                    var users = _telecallerServices.GetUsersListByRole(userData.AdminID);
                    List<SelectListItem> list = new List<SelectListItem>();
                    list.Add(new SelectListItem() { Text = "Select Manager", Value = "" });
                    foreach (AspNetUser user in users)
                    {
                        list.Add(new SelectListItem()
                        {
                            Text = user.FullName,
                            Value = user.Id
                        });
                    }
                    ViewBag.ManagerList = list;

                    return View(telecaller);
                }
            }

            return RedirectToAction("/Telecallers");
        }

        [HttpGet]
        [Route("{id?}")]
        public ActionResult Staff(string id)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            var staff = _staffServices.GetStaffListByRole(UserId);
            ViewBag.Staff = staff;

            StaffList ThisCaller = new StaffList();

            if (id != null && id != "")
            {
                ThisCaller = _staffServices.GetStaffById(id, UserId);
            }
            else
            {
                ThisCaller.Password = _helper.RandomString(4, false) + DateTime.Now.ToString("MMM").ToLower() + "@" + DateTime.Now.Year.ToString();
            }

            return View(ThisCaller);
        }


        [HttpGet]
        [Route("{id?}")]
        public ActionResult StaffList(string id)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            var staff = _staffServices.GetStaffListByRole(UserId);
            ViewBag.Staff = staff;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Staff(StaffList staff)
        {
            var _existingUser = _userService.getUserByUsername(staff.Email);
            if (_existingUser != null)
            {
                try
                {
                    //Session["MessageTStatus"] = "Email address already used to register an account. Please check and try again.";
                    _staffServices.UpdateStaff(staff, _existingUser.Id);
                    if (staff.Password != null && staff.Password.ToString() != "")
                    {
                        var token = await UserManager.GeneratePasswordResetTokenAsync(_existingUser.Id);
                        var result = await UserManager.ResetPasswordAsync(_existingUser.Id, token, staff.Password);
                    }
                    Session["sMessage"] = "Staff Member's details has been saved successfully.";
                    return RedirectToAction("/Staff");
                }
                catch (Exception ex)
                {
                    Session["eMessage"] = ex.Message;
                    return View();
                }
            }
            else
            {
                var userData = new ApplicationUser { UserName = staff.Email, Email = staff.Email, FullName = staff.FullName, PhoneNumber = staff.MobilePersonal, AdminID = ConfigurationManager.AppSettings["ClientId"].ToString() };
                var result = await UserManager.CreateAsync(userData, staff.Password);
                if (result.Succeeded)
                {
                    var addRole = UserManager.AddToRole(userData.Id, "Staff");
                    _staffServices.UpdateStaff(staff, userData.Id);
                    Session["sMessage"] = "Staff Member Created Successfully ! ";

                }
                else
                {
                    AddErrors(result);
                }
            }
                Session["MessageTStatus"] = "Data Saved Successfully !!";

                return View(staff);
            }


        [HttpGet]
        [Route("{id?}")]
        public ActionResult Leaves(string id)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            ViewBag.StartDate = _helper.GetCurrentDate();
            ViewBag.EndDate = _helper.GetCurrentDate();
            var leaves = _leaveServices.GetLeaves(ViewBag.StartDate, ViewBag.EndDate, id);
            ViewBag.user = id;
            ViewBag.leave = leaves;
            return View();
        }

        [HttpPost]
        [Route("{id?}")]
        public ActionResult Leaves(DatePickerModel range, string id)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            ViewBag.StartDate = range.start;
            ViewBag.EndDate = range.end;
            var leaves = _leaveServices.GetLeaves(ViewBag.StartDate, ViewBag.EndDate, id);
            ViewBag.leave = leaves;
            return View();
        }

        [HttpGet]
        [Route("{id?}/{iid?}")]
        public ActionResult AddLeave(string id, long? iid)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            Leave ThisLeave = new Leave();
            ThisLeave.UserId = id;
            List<SelectListItem> LeaveStatus = new List<SelectListItem>();
            LeaveStatus.Add(new SelectListItem() { Text = "---Select---", Value = " " });
            LeaveStatus.Add(new SelectListItem() { Text = "Unpaid", Value = "Unpaid" });
            LeaveStatus.Add(new SelectListItem() { Text = "Paid", Value = "Paid" });
            LeaveStatus.Add(new SelectListItem() { Text = "Partial Paid", Value = "PartialPaid" });

            ViewBag.leadStatus = LeaveStatus;

            if (iid != null )
            {
                ThisLeave = _leaveServices.getLeaveById(iid);
                
            }
            return View(ThisLeave);
        }


        [HttpPost]
        [Route("{id?}")]
        [ValidateAntiForgeryToken]
        public ActionResult AddLeave(Leave leave)
        {

            try
            {
                
                _leaveServices.UpdateLeave(leave );
                Session["sMessage"] = "Details has been Saved Successfully";
                return RedirectToAction("/Leaves", new {@id =leave.UserId});
            }
            catch (Exception ex)
            {
                Session["eMessage"] = ex.Message;
            }


            Session["MessageTStatus"] = "Data Saved Successfully !!";

            return View(leave);
        }

        [HttpGet]
        [Route("{id?}")]
        public ActionResult RemoveLeave(long? id)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            if(id !=null)
            {
                _leaveServices.RemoveLeavebyId(id);
            }
            Session["sMessage"] = "Leave has been Deleted Successfully !!";
            return RedirectToAction("/Leaves");
        }


        [HttpGet]
        [Route("{id?}")]
        public ActionResult Investors(string id)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            var investor = _investorServices.GetInvestorsListByRole(UserId);
            ViewBag.Investors = investor;

            InvestorList ThisInvestor = new InvestorList();

            if (id != null && id != "")
            {
                ThisInvestor = _investorServices.GetInvestorById(id, UserId);
            }
            else
            {
                ThisInvestor.Password = _helper.RandomString(4, false) + DateTime.Now.ToString("MMM").ToLower() + "@" + DateTime.Now.Year.ToString();
            }
            return View(ThisInvestor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Investors(InvestorList investors)
        {
            var _existingUser = _userService.getUserByUsername(investors.Email);
            if (_existingUser != null)
            {
                try
                {
                    //Session["MessageTStatus"] = "Email address already used to register an account. Please check and try again.";
                    _investorServices.UpdateInvestor(investors, _existingUser.Id);
                    if (investors.Password != null && investors.Password.ToString() != "")
                    {
                        var token = await UserManager.GeneratePasswordResetTokenAsync(_existingUser.Id);
                        var result = await UserManager.ResetPasswordAsync(_existingUser.Id, token, investors.Password);
                    }
                    Session["sMessage"] = "Investor's details has been saved successfully.";
                    return RedirectToAction("/Investors");
                }
                catch (Exception ex)
                {
                    Session["eMessage"] = ex.Message;
                }
            }
            else
            {
                var userData = new ApplicationUser { UserName = investors.Email, Email = investors.Email, FullName = investors.FullName, PhoneNumber = investors.PhoneNumber, AdminID = ConfigurationManager.AppSettings["ClientId"].ToString() };
                var result = await UserManager.CreateAsync(userData, investors.Password);
                if (result.Succeeded)
                {
                    var addRole = UserManager.AddToRole(userData.Id, "Investor");
                    _investorServices.UpdateInvestor(investors, userData.Id);
                    Session["sMessage"] = "Investor Created Successfully ! ";
                    

                }
                else
                {
                    AddErrors(result);
                }
            }
            Session["MessageTStatus"] = "Data Saved Successfully !!";

            return View(investors);
        }

        
        [HttpGet]
        [Route("{id?}")]
        public ActionResult Customer(string id)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            var Customer = _customerServices.GetCustomersListByRole(UserId);
            ViewBag.Customers = Customer;

            CustomerList ThisCustomer = new CustomerList();

            if (id != null && id != "")
            {
                ThisCustomer = _customerServices.GetCustomerById(id, UserId);
            }
            else
            {
                ThisCustomer.Password = _helper.RandomString(4, false) + DateTime.Now.ToString("MMM").ToLower() + "@" + DateTime.Now.Year.ToString();
            }

            return View(ThisCustomer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Customer(CustomerList customers)
        {
            var _existingUser = _userService.getUserByUsername(customers.Email);
            if (_existingUser != null)
            {
                try
                {
                    //Session["MessageTStatus"] = "Email address already used to register an account. Please check and try again.";
                    _customerServices.UpdateCustomer(customers, _existingUser.Id);
                    if (customers.Password != null && customers.Password.ToString() != "")
                    {
                        var token = await UserManager.GeneratePasswordResetTokenAsync(_existingUser.Id);
                        var result = await UserManager.ResetPasswordAsync(_existingUser.Id, token, customers.Password);
                    }
                    Session["sMessage"] = "Customer's details has been saved successfully.";
                    return RedirectToAction("/Customer");
                }
                catch (Exception ex)
                {
                    Session["eMessage"] = ex.Message;
                }
            }
            else
            {
                var userData = new ApplicationUser { UserName = customers.Email, Email = customers.Email, FullName = customers.FullName, PhoneNumber = customers.PhoneNumber, AdminID = ConfigurationManager.AppSettings["ClientId"].ToString() };
                var result = await UserManager.CreateAsync(userData, customers.Password);
                if (result.Succeeded)
                {
                    var addRole = UserManager.AddToRole(userData.Id, "Customer");
                    _customerServices.UpdateCustomer(customers, userData.Id);
                    Session["sMessage"] = "Customer Created Successfully ! ";

                }
                else
                {
                    AddErrors(result);
                }
            }
            Session["MessageTStatus"] = "Data Saved Successfully !!";

            return View(customers);
        }


        [HttpGet]
        [Route("{id?}/{iid?}")]
        public ActionResult Investments(string id, string iid)
        {
            if (id == null)
            {
                return Redirect("/Admin/Investors");
            }

            long Iid;
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            var investment = _investmentServices.GetInvestmentListByInvestor(id);
            ViewBag.Investments = investment;

            Investment ThisInvestments = new Investment();
            ThisInvestments.UserId = id;
            if (iid != null && iid != "")
            {
                Iid = Convert.ToInt64(iid); 
                ThisInvestments = _investmentServices.GetinvestmentsById(Iid);
            }
            

            List<SelectListItem> Mode = new List<SelectListItem>();
            Mode.Add(new SelectListItem() { Text = "One Time", Value = "OneTime" });
            Mode.Add(new SelectListItem() { Text = "Monthly", Value = "Monthly" });
            ViewBag.Modes = Mode;
            List<SelectListItem> Duration = new List<SelectListItem>();
            Duration.Add(new SelectListItem() { Text = "---Select Duration---", Value = "" });
            Duration.Add(new SelectListItem() { Text = "Daily ", Value = "1" });
            Duration.Add(new SelectListItem() { Text = "Weekly", Value = "7" });
            Duration.Add(new SelectListItem() { Text = "Fortnightly", Value = "15" });
            Duration.Add(new SelectListItem() { Text = "Monthly", Value = "30" });
            Duration.Add(new SelectListItem() { Text = "Quaterly", Value = "90" });
            Duration.Add(new SelectListItem() { Text = "Half Yearly", Value = "180" });
            Duration.Add(new SelectListItem() { Text = "Yearly", Value = "360" });
            ViewBag.Durations = Duration;
            List<SelectListItem> Status = new List<SelectListItem>();
            Status.Add(new SelectListItem() { Text = "Active", Value = "Active" });
            Status.Add(new SelectListItem() { Text = "InActive", Value = "InActive" });
            Status.Add(new SelectListItem() { Text = "Close", Value = "Close" });
            ViewBag.Status = Status;
            return View(ThisInvestments);
        }

        [HttpPost]
        [Route("{id?}")]
        [ValidateAntiForgeryToken]
        public ActionResult Investments(Investment investment)
        {

            try
                {
                    var result = _investmentServices.UpdateInvestment(investment);
                    if (investment.Id == 0)
                    {
                        if (investment.iMode == "Monthly")
                        {
                            _investmentServices.AddLedger(investment);
                        }

                    _investmentServices.AddPayouts(result);
                    
                   
                    }

                return RedirectToAction("/Investments",new { @id= investment.UserId });
                }
                catch (Exception ex)
                {
                    Session["eMessage"] = ex.Message;
                }
          
            
            Session["MessageTStatus"] = "Data Saved Successfully !!";

            return View(investment);
        }



        [HttpGet]
        [Route("{id?}")]
        public ActionResult Payroll(string id)


        {
            try
            {
                var UserId = AuthenticationManager.User.Identity.GetUserId();
                var user = _payrollServices.getStaffbyId(id);
                var Payrolls = _payrollServices.GetPayrollListByUser(id);

                var Month = "";
                var Year = "";

                if (Session["PayrollMonth"] != null && Session["PayrollMonth"].ToString() != "")
                {
                    Month = Session["PayrollMonth"].ToString();
                    Year = Session["PayrollYear"].ToString();
                }

                var CurrentPayroll = _payrollServices.GetCurrentPayrollByUserID(id, Month, Year);
                if (CurrentPayroll == null) { CurrentPayroll = new Payroll(); CurrentPayroll.Id = 0; }
                CurrentPayroll.UserId = id;
                ViewBag.Thisuser = user;
                ViewBag.payrolls = Payrolls;
                return View(CurrentPayroll);
            }
            catch(Exception ex)
            {
                Session["eMessage"] = ex.Message;
            }
           return RedirectToAction("/Payroll", new { id = @id });
        }

        [HttpPost]
        [Route("{id?}")]
        public ActionResult Payroll(string SelectedDate, Payroll payroll, string id)
        {
            try
            {
                var UserId = AuthenticationManager.User.Identity.GetUserId();
                var user = _payrollServices.getStaffbyId(id);
                var Payrolls = _payrollServices.GetPayrollListByUser(id);
                int Month = 0;
                int Year = 0;
                if (SelectedDate != null && SelectedDate != "")
                {
                    string[] date = SelectedDate.Split('/');
                    Month = Convert.ToInt32(date[0]);
                    Year = Convert.ToInt32(date[1]);

                    Session["PayrollMonth"] = Month;
                    Session["PayrollYear"] = Year;

                    return RedirectToAction("/Payroll", new { id = @id });
                }
                if (payroll.Id == 0)
                {
                    _payrollServices.UpdatePayroll(payroll, id);
                    payroll.UserId = id;
                    return RedirectToAction("/Payroll", new { id = @id });
                }

                //Payroll CurrentPayroll = _payrollServices.GetCurrentPayrollByUserID(id, Month, Year);
                //if (CurrentPayroll == null) { CurrentPayroll = new Payroll(); CurrentPayroll.Id = 0; }
                //CurrentPayroll.UserId = id;
                ViewBag.Thisuser = user;
                ViewBag.payrolls = Payrolls;

                
            }
            catch (Exception ex)
            {
                Session["eMessage"] = ex.Message;
            }

            Session["MessageTStatus"] = "Data Saved Successfully !!";
            return View(payroll);

        }

        //[HttpGet]
        //[Route("{id?}/{iid?}")]
        //public ActionResult Payroll( string id)
        //{
        //    var UserId = AuthenticationManager.User.Identity.GetUserId();
        // var Payrolls = _payrollServices.GetPayrollList();
        //    ViewBag.payrolls = Payrolls;

        //    var users = _payrollServices.GetUsersListByRole();
        //    List<SelectListItem> list = new List<SelectListItem>();
        //    list.Add(new SelectListItem() { Text = "Select Staff", Value = "" });
        //    foreach (AspNetUser user in users)
        //    {
        //        list.Add(new SelectListItem()
        //        {
        //            Text = user.FullName,
        //            Value = user.Id
        //        });
        //    }
        //    ViewBag.stafflist = list;

        //    SalaryRevision ThisSalary = new SalaryRevision();
        //    if(id !=null)
        //    {
        //        ThisSalary = _payrollServices.GetPayrollbyId(id);
        //    }
        //    return View(ThisSalary);

        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Payroll(SalaryRevision payroll)
        //{
        //   try
        //        {
        //            _payrollServices.UpdatePayroll(payroll);

        //            Session["sMessage"] = "Details has been saved successfully.";
        //            return RedirectToAction("/Customer");
        //        }
        //        catch (Exception ex)
        //        {
        //            Session["eMessage"] = ex.Message;
        //        }
        //        return View(payroll);

        //}

        [HttpPost]
        public ActionResult Export(string table)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                StringReader sr = new StringReader(table);
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                //SpdfDoc.Add(new Chunk(""));
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();
                return File(stream.ToArray(), "application/pdf", "Grid.pdf");
            }
        }

        public ActionResult TeamReport()
        {
            return View();
        }

        public ActionResult SalesReport()
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();

            return View();
        }

        public ActionResult EmailTemplates()
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();

            return View();
        }

        public ActionResult Profile()
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            var client = _userService.GetClientById(UserId);

            return View(client);
           
        }

        public ActionResult Support()
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();

            return View();
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

    }
}