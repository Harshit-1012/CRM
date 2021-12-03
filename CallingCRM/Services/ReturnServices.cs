using CallingCRM.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace CallingCRM.Services
{
    public class ReturnServices
    {
        public hwLiveEntities _context = new hwLiveEntities();

        public List<InvestmentReturn> GetReturnListByClient(string UserId)
        {
            List<InvestmentReturn> result = _context.InvestmentReturns
                .Where(x => x.AdminId == UserId).ToList();
            return result;
        }

        public InvestmentReturn GetIReturnById(long? id, string UserId)
        {
            InvestmentReturn result = _context.InvestmentReturns.Where(x => x.Id == id && x.AdminId == UserId).FirstOrDefault();
            
            return result;
        }


        public void UpdatePayouts(InvestmentReturn iReturns)
        {
            InvestmentReturn IReturns = _context.InvestmentReturns.Where(x => x.Id == iReturns.Id).FirstOrDefault();

            IReturns.ReturnStatus = iReturns.ReturnStatus;
            IReturns.ClaimDate = iReturns.ClaimDate;
            IReturns.TransactionType = iReturns.TransactionType;
            IReturns.Comment = iReturns.Comment;
            _context.SaveChanges();
        }
    }
}