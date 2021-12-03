using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Web.Mvc;

namespace CallingCRM.Models
{
    public class StartCallsViewModels
    {
        public Int64? LeadId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Location { get; set; }
        public string LeadType { get; set; }
        public string LeadStatus { get; set; }
        public bool DND { get; set; }
        public string CallStatus { get; set; }
        public string Comment { get; set; }
        public DateTime? FollowupDate { get; set; }
        public string FollowupStatus { get; set; }
        public bool SendEmail { get; set; }

        public DateTime? PaymentDate { get; set; }
        public Decimal? Amount { get; set; }
        public string PaymentInfo { get; set; }
    }

    public class SendEmailViewModels
    {
        public Int64? LeadId { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string template { get; set; }
        public string sender { get; set; }
    }

    public class DatePickerModel
    {
        public Int64? Id { get; set; }
        public DateTime? start { get; set; }
        public DateTime? end { get; set; }
    }

    public class CallHistoryList
    {
        public DateTime? CallDate { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string LeadStatus { get; set; }
        public string CallStatus { get; set; }
        public DateTime? LastFollowupDate { get; set; }
        public DateTime? NextFollowup { get; set; }
        public string UserId { get; set; }
        public Int64? LeadId { get; set; }
        public Int64? FollowupId { get; set; }
        public string UserName { get; set; }
    }

    public class SalesHistoryList
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public string Email { get; set; }
        public DateTime? LeadDate { get; set; }
        public string UserId { get; set; }
        public Sale sale { get; set; }
    }

    public class ForwardCallViewModel
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public string ManagerId { get; set; }
        public DateTime? ForwardDate { get; set; }
        public string ForwardReason { get; set; }
        public DateTime? WhenToFollowup { get; set; }
        public Lead lead { get; set; }
    }

    public class UserBaseInfo
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public DateTime? Validity { get; set; }
        public string NewPassword { get; set; }
    }

    public class UserDCRViewModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public int? TotalCalls { get; set; }
        public int? FreshCalls { get; set; }
        public int? FollowupCalls { get; set; }
        public int? UnresponsoveCalls { get; set; }
        public int? NotInterestedCalls { get; set; }
        public int? InterestedCalls { get; set; }
        public decimal? TotalSales { get; set; }

    }

    public class ImportLeadItem
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string City { get; set; }
        public string ZIPCode { get; set; }
        public string SourceOfLead { get; set; }
        public string AssignedToUserEmail { get; set; }
        public string NextFollowUpDate { get; set; }
        public string Comment { get; set; }
    }

    public class ExportLeadItem
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string City { get; set; }
        public string ZIPCode { get; set; }
        public string SourceOfLead { get; set; }
        public DateTime? AssignedDate { get; set; }
        public string AssignedToUserEmail { get; set; }
        public string LastCallStatus { get; set; }
        public DateTime? LastFollowUpDate { get; set; }
        public string LastFollowUpStatus { get; set; }
        public DateTime? NextFollowUpDate { get; set; }
        public string Comment { get; set; }
    }

    public class ManagerList
    {
        public string id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Password { get; set; }
        public string comment { get; set; }
    }


    public class TelecallerList
    {
        public string id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Password { get; set; }
        public string comment { get; set; }
        public string ManagerId { get; set; }


    }

    public class StaffList
    {
        public long id { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string comment { get; set; }
        public string MobilePersonal { get; set; }
        public string MobileHome { get; set; }
        public string AddressLocal { get; set; }
        public string AddressPermanent { get; set; }
        public string Reference1 { get; set; }
        public string Reference2 { get; set; }
        public string Designation { get; set; }
        public string JoiningDate { get; set; }
        public Nullable<int> TotalExpYears { get; set; }
        public Nullable<decimal> Salary { get; set; }
        public Nullable<int> SalaryDate { get; set; }
        public string Skills { get; set; }
        public string Department { get; set; }
        public string EmployeeCode { get; set; }
    }

    public class InvestorList
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string comment { get; set; }
        public Nullable<decimal> InvestedAmount { get; set; }
        public Nullable<decimal> ReturnPercentage { get; set; }
        public Nullable<int> Duration { get; set; }
        public Nullable<int> Tenure { get; set; }
        public string FirstPayDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public Nullable<System.DateTime> CloseDate { get; set; }
    }

    public class CustomerList
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string comment { get; set; }
        public string Ship_Country { get; set; }
        public string Ship_State { get; set; }
        public string Ship_City { get; set; }
        public Nullable<int> Ship_Pincode { get; set; }
        public string Bill_Country { get; set; }
        public string Bill_State { get; set; }
        public string Bill_City { get; set; }
        public Nullable<int> Bill_Pincode { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }

    }

    public class MonthYear
    {
        public string SelectedDate { get; set; }
        public string month { get; set; }
        public string year { get; set; }

        public void mySplit(string split)
        {
            var results = SelectedDate.Split('/');
            month = results[0];
            year = results[1];
        }
    }
    public class AdminTRViewModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public int? TotalCalls { get; set; }
        public int? FreshCalls { get; set; }
        public int? FollowupCalls { get; set; }
        public int? UnresponsoveCalls { get; set; }
        public int? NotInterestedCalls { get; set; }
        public int? InterestedCalls { get; set; }
        public decimal? TotalSales { get; set; }
        public string ManagerId { get; set; }

    }
}