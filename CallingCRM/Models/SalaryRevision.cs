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
    
    public partial class SalaryRevision
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public Nullable<decimal> RevisedSalary { get; set; }
        public Nullable<System.DateTime> RevisionDate { get; set; }
        public string RevisionDesignation { get; set; }
        public string Comment { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string AdminId { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
    }
}
