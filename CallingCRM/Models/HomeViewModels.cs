using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace CallingCRM.Models
{
    public class HomeViewModels
    {
    }

    public class HomeStatsModels
    {
        public int TotalLeads { get; set; }
        public int TotalCalls { get; set; }
        public decimal TotalSales { get; set; }
        public int TodaysCalls { get; set; }
    }
}