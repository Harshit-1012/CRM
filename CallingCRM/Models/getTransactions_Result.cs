//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CallingCRM.Models
{
    using System;
    
    public partial class getTransactions_Result
    {
        public string RequestId { get; set; }
        public Nullable<decimal> RequestAmount { get; set; }
        public string Purpose { get; set; }
        public string description { get; set; }
        public string RequestByUserId { get; set; }
        public Nullable<System.DateTime> RequestDate { get; set; }
        public Nullable<System.DateTime> ApprovalDate { get; set; }
        public string RequestStatus { get; set; }
        public string SystemComment { get; set; }
        public string paymentMethod { get; set; }
        public string TxnFromUserId { get; set; }
        public string TxnFromWallet { get; set; }
        public string TxnFromAddress { get; set; }
        public string TxnToUserId { get; set; }
        public string TxnToWallet { get; set; }
        public string TxnToAddress { get; set; }
        public Nullable<System.DateTime> TxnDate { get; set; }
        public string TxnStatus { get; set; }
        public string Username { get; set; }
    }
}
