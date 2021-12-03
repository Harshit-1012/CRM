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
    public class InvestmentServices
    {
        public hwLiveEntities _context = new hwLiveEntities();

        public CommonHelper _helper = new CommonHelper();

        public AspNetUser getUserByUsername(string username)
        {
            return _context.AspNetUsers.Where(x => x.UserName == username).FirstOrDefault();
        }


        public Investment GetinvestmentsById(long iid)
        {
            Investment investment = _context.Investments.Where(x => x.Id == iid).FirstOrDefault();

            return investment;
        }

        public List<Investment> GetInvestmentListByInvestor(string id)
        {

            List<Investment> investment = _context.Investments.Where(x => x.UserId == id).ToList();
            return investment;
        }

        public Investment UpdateInvestment(Investment Investment)
        {
            AspNetUser users = _context.AspNetUsers.Where(x => x.Id == Investment.UserId).FirstOrDefault();
            Investment investment = _context.Investments.Where(x => x.Id == Investment.Id).FirstOrDefault();
            if (investment != null)
            {
                investment.iStatus = Investment.iStatus;
                investment.CloseDate = Investment.CloseDate;
            }
            else
            {
                Investment newInvestment = new Investment();
                newInvestment.UserId = Investment.UserId;
                newInvestment.InvestedAmount = Investment.InvestedAmount;
                newInvestment.ReturnPercentage = Investment.ReturnPercentage;
                newInvestment.Duration = Investment.Duration;
                newInvestment.Tenure = Investment.Tenure;
                newInvestment.FirstPayDate = Convert.ToDateTime(Investment.FirstPayDate);
                newInvestment.iStatus = "Active";
                newInvestment.CloseDate = null;
                newInvestment.iMode = Investment.iMode;
                newInvestment.CreatedDate = _helper.GetCurrentDate();
                _context.Investments.Add(newInvestment);

                investment = newInvestment;
            }

            _context.SaveChanges();
            
            return investment;
        }

        public void AddPayouts(Investment Investment)
        {
            Investment investment = _context.Investments.Where(x => x.Id == Investment.Id).FirstOrDefault();
            int i;
            Nullable<decimal> amount;
            var cycle = Investment.Tenure;
            var duration = Investment.Duration;
            var FirstPayDate = Convert.ToDateTime(Convert.ToDateTime(Investment.FirstPayDate).ToString("yyyy-MM-dd"));
            var StartDate = Convert.ToDateTime(Convert.ToDateTime(Investment.CreatedDate).ToString("yyyy-MM-dd"));
            double days = Convert.ToDouble(duration);

            //int monthdays = DateTime.DaysInMonth(_helper.GetCurrentDate().Year, _helper.GetCurrentDate().Month);
            for (i = 0; i < cycle; i++)
            {
                InvestmentReturn iReturn = new InvestmentReturn();
                iReturn.UserId = Investment.UserId;
                iReturn.InvestmentId = Investment.Id;
                if (i == 0)
                {
                    if (Investment.FirstPayDate != null)
                    {
                        days = (FirstPayDate - StartDate).TotalDays;
                        amount = ((Investment.InvestedAmount * Investment.ReturnPercentage) / 100) / duration;
                        iReturn.ReturnAmount = Convert.ToDecimal(amount) * Convert.ToDecimal(days);
                        iReturn.ReturnDate = Investment.FirstPayDate;
                    }
                    else
                    {
                        Investment.FirstPayDate = StartDate.AddDays(Convert.ToDouble(duration));
                        amount = (Investment.InvestedAmount * Investment.ReturnPercentage) / 100;
                        iReturn.ReturnAmount = amount;
                        iReturn.ReturnDate = Investment.FirstPayDate;
                    }
                }
                else
                {
                    Investment.FirstPayDate = Convert.ToDateTime(Investment.FirstPayDate).AddDays(Convert.ToDouble(duration));
                    amount = (Investment.InvestedAmount * Investment.ReturnPercentage) / 100;
                    iReturn.ReturnAmount = amount;
                    iReturn.ReturnDate = Investment.FirstPayDate;
                }

                iReturn.ReturnStatus = "Pending";
                iReturn.AdminId = ConfigurationManager.AppSettings["ClientId"].ToString();
                _context.InvestmentReturns.Add(iReturn);
            }
            _context.SaveChanges();
        }

        public void AddLedger(Investment Investment)
        {
            var cycle = Investment.Tenure;
            var duration = Investment.Duration;
            int i;
            DateTime StartDate ;
            if (Investment.CreatedDate != null) { StartDate = Convert.ToDateTime(Investment.CreatedDate); }
            else
            {
                 StartDate = _helper.GetCurrentDate();
            }
            for (i = 0; i < cycle; i++)
            {
                LedgerHistory InvestmentLedger = new LedgerHistory();
                InvestmentLedger.FromUser = Investment.UserId;
                InvestmentLedger.ToUser = ConfigurationManager.AppSettings["ClientId"].ToString();
                InvestmentLedger.Amount = Investment.InvestedAmount;
                InvestmentLedger.PurposeofTransaction = "Investment";
                InvestmentLedger.AdminId = ConfigurationManager.AppSettings["ClientId"].ToString();
                InvestmentLedger.Status = "Pending";
                if (i == 0)
                {
                    if (Investment.FirstPayDate != null)
                    {
                        InvestmentLedger.DueDate = Investment.FirstPayDate;
                    }
                    else
                    {
                        Investment.FirstPayDate = StartDate.AddDays(Convert.ToDouble(duration));
                        InvestmentLedger.DueDate = Investment.FirstPayDate;
                    }
                }
                else
                {
                    Investment.FirstPayDate = Convert.ToDateTime(Investment.FirstPayDate).AddDays(Convert.ToDouble(duration));
                    InvestmentLedger.DueDate = Investment.FirstPayDate;
                }
                InvestmentLedger.ReferenceId = Investment.Id;
                _context.LedgerHistories.Add(InvestmentLedger);
            }
            _context.SaveChanges();
        }
    }
}