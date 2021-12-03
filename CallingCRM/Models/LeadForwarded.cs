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
    using System.Collections.Generic;
    
    public partial class LeadForwarded
    {
        public long Id { get; set; }
        public Nullable<long> LeadId { get; set; }
        public string ForwardedTo { get; set; }
        public Nullable<System.DateTime> ForwardDate { get; set; }
        public string ForwardReason { get; set; }
        public Nullable<System.DateTime> WhenToCall { get; set; }
        public string ActionByNewUser { get; set; }
        public Nullable<System.DateTime> ActionDate { get; set; }
        public Nullable<System.DateTime> NextReminderDate { get; set; }
    
        public virtual Lead Lead { get; set; }
        public virtual AspNetUser AspNetUser { get; set; }
    }
}
