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
    
    public partial class DomTransactionWithName
    {
        public Nullable<long> Id { get; set; }
        public Nullable<long> LinkId { get; set; }
        public Nullable<decimal> TxnAmount { get; set; }
        public string TxnReason { get; set; }
        public string TxnFromAddress { get; set; }
        public string TxnToAddress { get; set; }
        public string TxnFromWallet { get; set; }
        public string TxnToWallet { get; set; }
        public Nullable<System.DateTime> TxnDate { get; set; }
        public string TxnStatus { get; set; }
        public string TxnFromUserId { get; set; }
        public string TxnToUserId { get; set; }
        public string TxnFund { get; set; }
        public string UsedFundOfWallet { get; set; }
        public string RequestId { get; set; }
        public string UserName { get; set; }
    }
}