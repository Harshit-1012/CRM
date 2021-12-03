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

namespace CallingCRM.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public UserServices _userService = new UserServices();
        public CallService _callService = new CallService();
        public SalesService _salesService = new SalesService();
        static public CommonHelper _helper = new CommonHelper();

        static DateTime thisDateTime = _helper.GetCurrentDate();

        #region Dependencies
        public SuperAdminController()
        {
        }

        public SuperAdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Manager
        public ActionResult Index()
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();

            return View();
        }

        public ActionResult Clients()
        {
            var clients = _userService.GetClientsList();

            ViewBag.MyClients = clients;

            return View();
        }

        public ActionResult AddClient()
        {
            Client client = new Client();
            client.ClientPassword = _helper.RandomString(4, false) + DateTime.Now.ToString("MMM").ToLower() + "@" + DateTime.Now.Year.ToString();
            client.IsActive = true;
            client.ExpiryDate = thisDateTime.AddDays(365);

            return View(client);
        }

        [HttpPost]
        public async Task<ActionResult> AddClient(Client client)
        {
            
            if (client != null)
            {
                try {
                    var _existingUser = _userService.isUserExist(client.ClientEmailMain);

                    if (_existingUser) 
                    {
                        Session["eMessage"] = "Client with provided email address already exist.";
                        return View(client);
                    }

                    // create admin account
                    var userData = new ApplicationUser { UserName = client.ClientEmailMain, Email = client.ClientEmailMain};
                    userData.AdminID = userData.Id;
                    var result = await UserManager.CreateAsync(userData, client.ClientPassword);
                    if (result.Succeeded)
                    {
                        var user = _userService.getUserByUsername(userData.UserName);
                        var addRole = UserManager.AddToRole(user.Id, "Admin");

                        client.ClientID = user.Id;
                        
                        

                        try {
                            var newClient = _userService.CreateClient(client);
                        }
                        catch (Exception ex) {
                            Session["eMessage"] = ex.Message;
                            return View(client);
                        }
                    }
                    else 
                    {
                        AddErrors(result);
                        Session["eMessage"] = "Client's admin user creation failed.";
                        return View(client);
                    }
                }
                catch (Exception ex) {
                    Session["eMessage"] = ex.Message;
                    return View(client);
                }
            }
            else 
            {
                Session["eMessage"] = "Invalid attempt, please refresh page and try again. Or, contact to admin";
                return View(client);
            }

            Session["sMessage"] = "New client has been added successfully.";
            return new RedirectResult("/SuperAdmin/Clients");
        }



        [Route("/superadmin/client/{Id}")]
        public ActionResult Client(long Id)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();

            var client = _userService.GetClientBySuperAdmin(Id);

            return View(client);
        }

        [HttpPost]
        public async Task<ActionResult> Client(Client client)
        {
            if (client != null)
            {
                try
                {
                    var _existingUser = _userService.isUserExist(client.ClientEmailMain);

                    // create admin account
                    var userData = new ApplicationUser { UserName = client.ClientEmailMain, Email = client.ClientEmailMain };
                    var result = await UserManager.CreateAsync(userData, client.ClientPassword);

                    var user = _userService.getUserByUsername(userData.UserName);

                    try
                    {
                        var updateClient = _userService.UpdateClient(client);
                    }
                    catch (Exception ex)
                    {
                        Session["eMessage"] = ex.Message;
                        return View(client);
                    }
                }
                catch (Exception ex)
                {
                    Session["eMessage"] = ex.Message;
                    return View(client);
                }
            }
            else
            {
                Session["eMessage"] = "Invalid attempt, please refresh page and try again. Or, contact to admin";
                return View(client);
            }

            Session["sMessage"] = "Client has been udpated successfully.";
            return new RedirectResult("/SuperAdmin/Client/" + client.Id);
        }


        [HttpPost]
        public ActionResult CallAction(LeadForwarded lead)
        {
            try {
                var result = _callService.UpdateForwardedCallLog(lead);
            }
            catch (Exception ex) 
            {
                Session["eMessage"] = ex.Message;
            }

            return new RedirectResult("/manager/mycalls/pending");
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