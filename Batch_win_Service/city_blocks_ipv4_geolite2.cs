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
    
    public partial class city_blocks_ipv4_geolite2
    {
        public int PK_City_ID_IPV4 { get; set; }
        public string network { get; set; }
        public string Start_IPv4_address { get; set; }
        public string End_IPv4_address { get; set; }
        public string Start_IP_num { get; set; }
        public string End_IP_num { get; set; }
        public string geoname_id { get; set; }
        public string registered_country_geoname_id { get; set; }
        public string represented_country_geoname_id { get; set; }
        public string is_anonymous_proxy { get; set; }
        public string is_satellite_provider { get; set; }
        public string postal_code { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string accuracy_radius { get; set; }
    }
}
