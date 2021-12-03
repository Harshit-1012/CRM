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
    public class ManageService
    {
        static public CommonHelper _helper = new CommonHelper();

        static DateTime thisDateTime = _helper.GetCurrentDate();

    }

}