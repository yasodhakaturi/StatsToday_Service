//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StatsToday_Service
{
    using System;
    using System.Collections.Generic;
    
    public partial class loginhistory
    {
        public int PKLoginHistoryId { get; set; }
        public Nullable<int> FKPersonId { get; set; }
        public string Role { get; set; }
        public string IpAddress { get; set; }
        public Nullable<System.DateTime> LoginDateTime { get; set; }
    }
}
