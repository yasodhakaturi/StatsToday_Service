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
    
    public partial class freegeoipdata
    {
        public int PK_freeGeoip_id { get; set; }
        public string Ipv4 { get; set; }
        public Nullable<long> ip_num { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string MetroCode { get; set; }
    }
}