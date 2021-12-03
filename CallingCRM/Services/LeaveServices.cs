using CallingCRM.Helpers;
using CallingCRM.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CallingCRM.Services
{
    public class LeaveServices
    {
        static CommonHelper _helper = new CommonHelper();

        public DateTime thisDateTime = _helper.GetCurrentDate();

        public hwLiveEntities _context = new hwLiveEntities();

        public List<Leave> GetLeaves(DateTime? startDate, DateTime? endDate, string Id)
        {
            List<Leave> leaves = new List<Leave>();
            if (Id != null)
            {
                leaves = _context.Leaves
                                  .Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate && x.UserId == Id).ToList();
            }
            else
            {
                leaves = _context.Leaves
                               .Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate).ToList();
            }

            return leaves;
        }

        public Leave getLeaveById(long? Id)
        {
            Leave result = _context.Leaves.Where(x => x.Id == Id).FirstOrDefault();

            return result;

        }

        public void UpdateLeave(Leave leave)
        {
            Leave result = _context.Leaves.Where(x => x.Id == leave.Id).FirstOrDefault();
            if (result != null)
            {
                result.LeaveStatus = leave.LeaveStatus;

            }
            else
            {
                Leave newleave = new Leave();
                newleave.LeaveDate = leave.LeaveDate;
                newleave.Purpose = leave.Purpose;
                newleave.Reason = leave.Reason;
                newleave.UserId = leave.UserId;
                newleave.CreatedAt = _helper.GetCurrentDate();
                _context.Leaves.Add(newleave);
            }
            _context.SaveChanges();
        }

        public void RemoveLeavebyId(long? id)
        {
            Leave result = _context.Leaves.Where(x => x.Id == id).FirstOrDefault();
            _context.Leaves.Remove(result);
            _context.SaveChanges();
        }
    }
} 