using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsToday_Service
{
    public class StatsTodayClass
    {
    }
    public class StatsModel_visits
    {
        public int Visits_today { get; set; }
        public int uniqueVisits_today { get; set; }
        //public int Todays_ReVisitCount { get; set; }
        
        public int? fk_rid { get; set; }
        // public List<int?> fk_uid { get; set; }
    }
    public class StatsModel_users
    {
        public int Users_Today { get; set; }
        public int UniqueUsers_Today { get; set; }
        //public int UniqueUsers { get; set; }
        public int? fk_rid { get; set; }
    }
    public class StatsModel_uniquevisits
    {
        public int uniquevists { get; set; }
        public int Todays_ReVisitCount { get; set; }
        public int? fk_rid { get; set; }
    }
    public class StatsModel_uniquevisits_Today
    {
        public int uniqueVisits_today { get; set; }
        public int? fk_rid { get; set; }
    }
    public class StatsModel_Novisits_Today
    {
        public List<int> Novists_today { get; set; }
        public int? fk_rid { get; set; }
    }
    public class StatsModel_uniqueusers_today
    {
        public int UniqueUsers_Today { get; set; }
        public int? fk_rid { get; set; }
    }
    public class StatsModel_uniqueusers
    {
        public int UniqueUsers { get; set; }
        public int? fk_rid { get; set; }
    }
    public class StatsCounts_Today
    {
        public int Visits_today { get; set; }
        public int uniqueVisits_today { get; set; }
        public int Todays_ReVisitCount { get; set; }
        public int uniquevists { get; set; }
        public int Users_Today { get; set; }
        public int UniqueUsers_Today { get; set; }
        public int UniqueUsers { get; set; }
        public int? fk_rid { get; set; }

    }
}
