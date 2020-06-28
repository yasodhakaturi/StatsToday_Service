using MySql.Data.MySqlClient;
using StatsToday_Service;
//using DemoWinService;2
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StatsToday_Service
{
    public partial class StatsToday_Service : ServiceBase
    {

        shortenurlEntities dc = new shortenurlEntities();
        System.Threading.Timer _timer;
        public StatsToday_Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            MSYNC();
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Enabled = true;
            timer.Interval = new TimeSpan(0, 0, 5, 0).TotalMilliseconds;
            //timer.Interval = 5000;
            //timer.Interval = 60000 * 60;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            // _timer.Change(0, 2000);
        }

        protected void timer_Elapsed(object source, System.Timers.ElapsedEventArgs aa)
        {
            MSYNC();
        }
        protected override void OnStop()
        {
        }
        public void MSYNC()
        {
            try
            {
                
                //ErrorLogs.LogErrorData("test", "test");
                shorturlclickreference shrt_clickref = new shorturlclickreference();
                shorturlclickreference clickref = dc.shorturlclickreferences.Select(x => x).FirstOrDefault();
                int days_diff = 0;
                days_diff = (int)((DateTime.UtcNow.Date - clickref.UpdatedDate.Value.Date).TotalDays);
                if (days_diff == 0)
                {
                int getnextshorturlid = 0;
                int getnextuserid = 0;
                //List<StatsModel_visits> lstvisits = new List<StatsModel_visits>();
                List<StatsModel_uniquevisits_Today> lstuniquevisits_tot_today = new List<StatsModel_uniquevisits_Today>();
                List<StatsModel_uniquevisits> lstuniquevisits_tot = new List<StatsModel_uniquevisits>();
                int gettodayshorturlid = 0;
                int gettodayuserid = 0;
                if (clickref == null)
                {
                    gettodayshorturlid = dc.shorturldatas.AsEnumerable().Where(x => x.CreatedDate != null && x.CreatedDate.Value.Date == DateTime.UtcNow.Date).Min(x => x.PK_Shorturl);
                    gettodayuserid = dc.uiddatas.AsEnumerable().Where(x => x.CreatedDate != null && x.CreatedDate.Value.Date == DateTime.UtcNow.Date).Min(x => x.PK_Uid);
                    //gettodayuserid = dc.riddatas.AsEnumerable().Where(x => x.CreatedDate != null && x.CreatedDate.Value.Date == DateTime.UtcNow.Date).Min(x => x.PK_Rid);

                    shrt_clickref.Ref_ShorturlClickID = gettodayshorturlid;
                    shrt_clickref.Ref_UsersID = gettodayuserid;
                    shrt_clickref.UpdatedDate = DateTime.UtcNow;
                    dc.shorturlclickreferences.Add(shrt_clickref);
                    dc.SaveChanges();
                    clickref = dc.shorturlclickreferences.Select(x => x).FirstOrDefault();
                }
                else
                    clickref = dc.shorturlclickreferences.Select(x => x).FirstOrDefault();


                int cntvisits = dc.shorturldatas.Where(x => x.PK_Shorturl > clickref.Ref_ShorturlClickID).ToList().Count();
                ///campaigns -- start
                int cntusers = dc.uiddatas.Where(x => x.PK_Uid >= clickref.Ref_UsersID).ToList().Count();
                    if (cntvisits > 0)
                    {

                        //int refvisitid = (int)clickref.Ref_ShorturlClickID;
                        //getnextshorturlid = refvisitid + cntvisits;

                        //ErrorLogs.LogErrorData("days count", days_diff.ToString());
                        //if (days_diff == 0)
                        //{
                        getnextshorturlid = dc.shorturldatas.Max(x => x.PK_Shorturl);
                        clickref.Ref_ShorturlClickID = getnextshorturlid;
                        clickref.UpdatedDate = DateTime.UtcNow;
                        dc.SaveChanges();
                        try
                        {
                            using (var dc1 = new shortenurlEntities())
                            {
                                dc1.Database.CommandTimeout = 2 * 60;
                                lstuniquevisits_tot_today = (from s in dc1.shorturldatas
                                                                           .AsEnumerable()
                                                                 //join st in lstvisits on s.FK_RID equals st.fk_rid
                                                             where s.CreatedDate != null && s.CreatedDate.Value.Date == DateTime.UtcNow.Date
                                                             group s by s.FK_RID into res
                                                             select new StatsModel_uniquevisits_Today()
                                                             {
                                                                 fk_rid = res.Select(x => x.FK_RID).FirstOrDefault(),
                                                                 fk_clientid = res.Select(x => x.FK_ClientID).FirstOrDefault(),
                                                                 Visits_today = res.Select(x => x.FK_Uid).ToList().Count(),
                                                                 Todays_ReVisitCount = (res.Select(x => x.FK_Uid).Count()) - (res.Select(x => x.FK_Uid).Distinct().Count()),
                                                                 uniqueVisits_today = res.Select(x => x.FK_Uid).Distinct().ToList().Count()
                                                                 //Todays_ReVisitCount = (res.Where(y => y.CreatedDate.Value.Date == DateTime.UtcNow.Date).Select(x => x.FK_Uid).Count()) - (res.Where(y => y.CreatedDate.Value.Date == DateTime.UtcNow.Date).Select(x => x.FK_Uid).Distinct().Count()),
                                                                 //uniqueVisits_today = res.Where(y => y.CreatedDate.Value.Date == DateTime.UtcNow.Date).Select(x => x.FK_Uid).Distinct().ToList().Count()
                                                             }).ToList();
                                lstuniquevisits_tot = (from s in dc1.shorturldatas
                                                                               .AsEnumerable()
                                                       join st in lstuniquevisits_tot_today on s.FK_RID equals st.fk_rid

                                                       group s by s.FK_RID into res
                                                       select new StatsModel_uniquevisits()
                                                       {
                                                           fk_rid = res.Select(x => x.FK_RID).FirstOrDefault(),
                                                           fk_clientid = res.Select(x => x.FK_ClientID).FirstOrDefault(),
                                                           uniquevists = res.Select(x => x.FK_Uid).Distinct().ToList().Count(),
                                                           totalvisits = res.Select(x => x.FK_Uid).ToList().Count()

                                                       }).ToList();
                            }
                        }
                        catch (Exception ex)
                        {

                            ErrorLogs.LogErrorData("StatsToday_Service,in visit query" + ex.InnerException + ex.StackTrace, ex.Message);
                        }
                        foreach (StatsModel_uniquevisits_Today vst in lstuniquevisits_tot_today)
                        {
                            //ErrorLogs.LogErrorData("StatsToday_Service "+"FK_RID ="+vst.fk_rid +" visits : "  +vst.Visits_today.ToString() , DateTime.UtcNow.Date.ToString());
                            stat_counts st_count = new stat_counts();
                            int uniquevists = 0; int revisitcount = 0; int uniquevistis_today = 0; int todayvisitcount = 0; int total_visits = 0;
                            uniquevists = lstuniquevisits_tot.Where(x => x.fk_rid == vst.fk_rid).Select(x => x.uniquevists).SingleOrDefault();
                            revisitcount = lstuniquevisits_tot_today.Where(x => x.fk_rid == vst.fk_rid).Select(x => x.Todays_ReVisitCount).SingleOrDefault();
                            uniquevistis_today = lstuniquevisits_tot_today.Where(x => x.fk_rid == vst.fk_rid).Select(x => x.uniqueVisits_today).SingleOrDefault();
                            total_visits = lstuniquevisits_tot.Where(x => x.fk_rid == vst.fk_rid).Select(x => x.totalvisits).SingleOrDefault();
                            todayvisitcount = lstuniquevisits_tot_today.Where(x => x.fk_rid == vst.fk_rid).Select(x => x.Visits_today).SingleOrDefault();
                            //int novisits = lstnovisits_today.Where(x => x.fk_rid == vst.fk_rid).Select(x => x.Novists_today).SingleOrDefault();
                            st_count = dc.stat_counts.Where(x => x.FK_Rid == vst.fk_rid).Select(y => y).SingleOrDefault();
                            if (st_count != null)
                            {
                                //ErrorLogs.LogErrorData("StatsToday_Service visitis today st_count" + st_count.VisitsToday, vst.Visits_today.ToString());

                                st_count.VisitsToday = (vst.Visits_today > 0) ? (todayvisitcount) : st_count.VisitsToday;
                                //st_count.UniqueVisitsToday = st_count.UniqueVisitsToday + vst.uniqueVisits_today;
                                st_count.VisitsTotal_Today = (vst.Visits_today > 0) ? (todayvisitcount) : st_count.VisitsTotal_Today;
                                st_count.RevisitsTotal_Today = (revisitcount > 0) ? revisitcount : st_count.RevisitsTotal_Today;

                                st_count.UniqueVisits = (uniquevists > 0) ? (uniquevists) : (st_count.UniqueVisits);
                                st_count.UniqueVisitsToday = (uniquevistis_today > 0) ? (uniquevistis_today) : (st_count.UniqueVisitsToday);
                                //st_count.TotalVisits = (vst.Visits_today > 0) ? (st_count.TotalVisits + vst.Visits_today) : st_count.TotalVisits;
                                st_count.TotalVisits = (total_visits > 0) ? (total_visits) : (st_count.TotalVisits);
                                //st_count.NoVisitsTotal_Today = (st_count.UniqueVisitsToday > 0) ? (st_count.UsersToday - st_count.UniqueVisitsToday) : st_count.NoVisitsTotal_Today;
                                st_count.NoVisitsTotal_Today = (st_count.UniqueVisitsToday > 0 && st_count.UsersToday > 0 && (st_count.NoVisitsTotal_Today != (st_count.UsersToday - st_count.UniqueVisitsToday))) ? Math.Abs((int)(st_count.UsersToday - st_count.UniqueVisitsToday)) : st_count.NoVisitsTotal_Today;
                                st_count.RevisitsPercent_Today = (st_count.RevisitsTotal_Yesterday > 0 && st_count.RevisitsTotal_Today > 0) ? ((st_count.RevisitsTotal_Today - st_count.RevisitsTotal_Yesterday) / (st_count.RevisitsTotal_Yesterday)) : 0;
                                st_count.NoVisitsPercent_Today = (st_count.NoVisitsTotal_Yesterday > 0 && st_count.NoVisitsTotal_Today > 0) ? ((st_count.NoVisitsTotal_Today - st_count.NoVisitsTotal_Yesterday) / (st_count.NoVisitsTotal_Yesterday)) : 0;
                                st_count.UrlPercent_Today = (st_count.UsersYesterday > 0 && st_count.UrlTotal_Today > 0) ? ((st_count.UrlTotal_Today - st_count.UsersYesterday) / (st_count.UsersYesterday)) : 0;
                                //st_count.NoVisitsTotal_Today = (novisits == null) ? st_count.NoVisitsTotal_Today : novisits;
                                dc.SaveChanges();

                            }
                        }
                        ///admin case
                        //List<int> clientid_list = dc.clients.Where(x => x.Role == "Admin").Select(x => x.PK_ClientID).ToList();
                        List<int> clientid_list = (from c in dc.clients
                                                   join r1 in dc.roles on c.FK_RoleId equals r1.PK_RoleId
                                                   where r1.RoleName.ToLower() == "superadmin"
                                                   select c.PK_ClientID).ToList();
                        //if (st_count_admin_list.Count > 0)
                        //    {
                        int? VisitsToday = dc.stat_counts.Where(x => x.FK_Rid != 0).Select(x => x.VisitsToday).Sum();
                        int? revisitstotal = dc.stat_counts.Where(x => x.FK_Rid != 0).Select(x => x.RevisitsTotal_Today).Sum();
                        int? uniquevisits = dc.stat_counts.Where(x => x.FK_Rid != 0).Select(x => x.UniqueVisits).Sum();
                        int? totalvisits = dc.stat_counts.Where(x => x.FK_Rid != 0).Select(x => x.TotalVisits).Sum();
                        int? uniquevisitstoday = dc.stat_counts.Where(x => x.FK_Rid != 0).Select(x => x.UniqueVisitsToday).Sum();
                        int? novisitstotal = dc.stat_counts.Where(x => x.FK_Rid != 0).Select(x => x.NoVisitsTotal_Today).Sum();
                        foreach (int id in clientid_list)
                        {
                            stat_counts st_count_admin = dc.stat_counts.Where(x => x.FK_Rid == 0 && x.FK_ClientID == id).SingleOrDefault();
                            if (st_count_admin != null)
                            {
                                //st_count_admin.VisitsToday = (lstvisits != null) ? (st_count_admin.VisitsToday + lstvisits.Select(x => x.Visits_today).Sum()) : st_count_admin.VisitsToday;
                                st_count_admin.VisitsToday = (lstuniquevisits_tot_today != null) ? VisitsToday : st_count_admin.VisitsToday;
                                st_count_admin.VisitsTotal_Today = st_count_admin.VisitsToday;
                                st_count_admin.RevisitsTotal_Today = (lstuniquevisits_tot_today != null) ? (revisitstotal) : st_count_admin.RevisitsTotal_Today;
                                st_count_admin.UniqueVisits = (lstuniquevisits_tot != null) ? (uniquevisits) : st_count_admin.UniqueVisits;
                                st_count_admin.UniqueVisitsToday = (lstuniquevisits_tot_today != null) ? (uniquevisitstoday) : st_count_admin.UniqueVisitsToday;

                                //st_count_admin.UniqueVisits = (lstuniquevisits_tot != null) ? (st_count_admin.UniqueVisits + lstuniquevisits_tot.Select(x => x.uniquevists).Sum()) : st_count_admin.UniqueVisits;
                                //st_count_admin.UniqueVisitsToday = (lstvisits != null) ? (st_count_admin.UniqueVisitsToday + lstvisits.Select(x => x.uniqueVisits_today).Sum()) : st_count_admin.UniqueVisitsToday;

                                //st_count_admin.TotalVisits = (lstuniquevisits_tot_today != null) ? (st_count_admin.TotalVisits + lstuniquevisits_tot_today.Select(x => x.Visits_today).Sum()) : st_count_admin.TotalVisits;
                                st_count_admin.TotalVisits = (lstuniquevisits_tot != null) ? (totalvisits) : st_count_admin.TotalVisits;
                                st_count_admin.NoVisitsTotal_Today = (novisitstotal);
                                st_count_admin.VisitsPercent_Today = (st_count_admin.VisitsYesterday > 0 && st_count_admin.VisitsTotal_Today > 0) ? ((st_count_admin.VisitsTotal_Today - st_count_admin.VisitsYesterday) / (st_count_admin.VisitsYesterday)) : 0;
                                st_count_admin.RevisitsPercent_Today = (st_count_admin.RevisitsTotal_Yesterday > 0 && st_count_admin.RevisitsTotal_Today > 0) ? ((st_count_admin.RevisitsTotal_Today - st_count_admin.RevisitsTotal_Yesterday) / (st_count_admin.RevisitsTotal_Yesterday)) : 0;
                                st_count_admin.NoVisitsPercent_Today = (st_count_admin.NoVisitsTotal_Yesterday > 0 && st_count_admin.NoVisitsTotal_Today > 0) ? ((st_count_admin.NoVisitsTotal_Today - st_count_admin.NoVisitsTotal_Yesterday) / (st_count_admin.NoVisitsTotal_Yesterday)) : 0;
                                dc.SaveChanges();
                            }
                        }

                    }
                
                if ( cntusers > 0)
                {
                    stat_counts st_count = new stat_counts();
                    stat_counts st_count_admin = new stat_counts();
                    // int getnextshorturlid = 0; int getnextuserid = 0;
                    //List<StatsModel_visits> lstvisits = new List<StatsModel_visits>();
                    StatsModel_users lstusers = new StatsModel_users();
                    StatsModel_uniqueusers lstuniqueusers_tot = new StatsModel_uniqueusers();
                    //List<StatsModel_uniqueusers_today> lstuniqueusers_tot_today = new List<StatsModel_uniqueusers_today>();
                    if (cntusers > 0)
                    {

                        int refurserid = (int)clickref.Ref_UsersID;
                        getnextuserid = refurserid + cntusers;
                        clickref.Ref_UsersID = getnextuserid;
                        clickref.UpdatedDate = DateTime.UtcNow;
                        dc.SaveChanges();
                        List<int?> campaigns_list = dc.uiddatas.Where(x => x.PK_Uid >= refurserid).Select(y => y.FK_RID).Distinct().ToList();
                        foreach (int? rid in campaigns_list)
                        {
                            int uniqueusers = 0; int uniqueusers_today = 0; int userstoday = 0; int totalusers = 0;
                            DateTime? dttime = DateTime.UtcNow.Date;
                            //  ErrorLogs.LogErrorData("StatsToday_Service , campaigns sectionin loop", "rid="+rid);
                            //var lstuiddata = dc.uiddatas.Where(x => x.FK_RID == rid).Select(y => new { y.PK_Uid, y.MobileNumber, y.CreatedDate }).ToList();
                            stat_counts objs = dc.stat_counts.Where(x => x.FK_Rid == rid).Select(y => y).SingleOrDefault();
                            //lstusers = (from u in dc.uiddatas
                            //            .AsEnumerable()
                            //            where u.FK_RID == rid && u.CreatedDate.Value.Date == DateTime.UtcNow.Date
                            //            group u by u.FK_RID into res
                            //            select new StatsModel_users()
                            //            {
                            //                Users_Today = res.Select(x => x.MobileNumber).Count(),
                            //                UniqueUsers_Today = res.Select(x => x.MobileNumber).Distinct().Count()
                            //                //fk_rid = u.FK_RID
                            //            }).SingleOrDefault();
                            //lstuniqueusers_tot = (from u in dc.uiddatas
                            //                      where u.FK_RID == rid
                            //                      group u by u.FK_RID into res
                            //                      select new StatsModel_uniqueusers()
                            //                      {
                            //                          UniqueUsers = res.Select(x => x.MobileNumber).Distinct().Count(),
                            //                          TotalUsers = res.Select(x => x.MobileNumber).Count(),
                            //                          //fk_rid = u.FK_RID
                            //                      }).SingleOrDefault();

                            //ErrorLogs.LogErrorData("StatsToday_Service ,rid=" + rid, "");
                            using (var dc1 = new shortenurlEntities())
                            {
                                dc1.Database.CommandTimeout = 2 * 60;
                                totalusers = dc1.uiddatas.Where(x => x.FK_RID == rid).Select(y => y.MobileNumber).Count();
                                uniqueusers = dc1.uiddatas.Where(x => x.FK_RID == rid).Select(y => y.MobileNumber).Distinct().Count();
                                //userstoday = dc1.uiddatas.AsEnumerable().Where(x => x.FK_RID == rid && x.CreatedDate.Value.Date == DateTime.UtcNow.Date).Select(y => y.MobileNumber).Count();

                                getcampaigntodaycounts(rid, out userstoday, out uniqueusers_today);

                                //userstoday = (from s in dc1.uiddatas
                                //              .AsEnumerable()
                                //                  //join st in lstvisits on s.FK_RID equals st.fk_rid
                                //              where s.FK_RID==rid && s.CreatedDate != null && s.CreatedDate.Value.Date == DateTime.UtcNow.Date
                                //              //group s by s.FK_RID into res
                                //              select s.MobileNumber).ToList().Count();

                                //ErrorLogs.LogErrorData("StatsToday_Service ,userstoday=", userstoday.ToString());
                                ////uniqueusers_today = dc1.uiddatas.AsEnumerable().Where(x => x.FK_RID == rid && x.CreatedDate.Value.Date == DateTime.UtcNow.Date).Select(y => y.MobileNumber).Distinct().Count();
                                //userstoday = (from s in dc1.uiddatas
                                //             .AsEnumerable()
                                //                  //join st in lstvisits on s.FK_RID equals st.fk_rid
                                //              where s.CreatedDate != null && s.CreatedDate.Value.Date == DateTime.UtcNow.Date
                                //              //group s by s.FK_RID into res
                                //              select s.MobileNumber).Distinct().Count();

                                //ErrorLogs.LogErrorData("StatsToday_Service ,uniqueusers_today=", uniqueusers_today.ToString());

                                objs.TotalUsers = totalusers;
                                objs.UniqueUsers = uniqueusers;
                                objs.UsersToday = userstoday;
                                objs.UniqueUsersToday = uniqueusers_today;
                                objs.UrlTotal_Today = objs.UsersToday;
                                objs.UrlPercent_Today = (objs.UsersYesterday == 0) ? 0 : ((objs.UsersToday - objs.UsersYesterday) / (objs.UsersYesterday));
                                //if ((objs.UniqueVisitsToday > 0 && ((dc1.shorturldatas.Max(x => x.PK_Shorturl)) > (dc1.shorturlclickreferences.Select(x => x.Ref_ShorturlClickID).FirstOrDefault())) && (objs.NoVisitsTotal_Today != (objs.UsersToday - objs.UniqueVisitsToday))))
                                    objs.NoVisitsTotal_Today = userstoday - objs.UniqueVisitsToday;
                                //else if (objs.UniqueVisitsToday == 0)
                                //    objs.NoVisitsTotal_Today = objs.UsersToday;
                                //else
                                //    objs.NoVisitsTotal_Today = objs.NoVisitsTotal_Today;
                                dc.SaveChanges();
                            }
                        }

                        //List<int> adminids = dc.clients.Where(x => x.Role == "admin").Select(x => x.PK_ClientID).ToList();
                        List<int> adminids = (from c in dc.clients
                                              join r1 in dc.roles on c.FK_RoleId equals r1.PK_RoleId
                                              where r1.RoleName.ToLower() == "superadmin"
                                              select c.PK_ClientID).ToList();
                        foreach (int adminid in adminids)
                        {
                            stat_counts objadmin = dc.stat_counts.Where(x => x.FK_Rid == 0 && x.FK_ClientID == adminid).Select(y => y).SingleOrDefault();
                            if (objadmin != null)
                            {
                                ErrorLogs.LogErrorData("StatsToday_Service,starting of campaigncounts admin case", "");
                                objadmin.TotalUsers = dc.stat_counts.Where(x => x.FK_Rid != 0).Select(y => y.TotalUsers).Sum();
                                objadmin.UniqueUsers = dc.stat_counts.Where(x => x.FK_Rid != 0).Select(y => y.UniqueUsers).Sum();
                                objadmin.UsersToday = dc.stat_counts.Where(x => x.FK_Rid != 0).Select(y => y.UsersToday).Sum();
                                objadmin.UniqueUsersToday = dc.stat_counts.Where(x => x.FK_Rid != 0).Select(y => y.UniqueUsersToday).Sum();
                                objadmin.UrlTotal_Today = objadmin.UsersToday;
                                objadmin.UrlPercent_Today = (objadmin.UsersYesterday == 0) ? 0 : ((objadmin.UsersToday - objadmin.UsersYesterday) / (objadmin.UsersYesterday));
                                objadmin.NoVisitsTotal_Today = dc.stat_counts.Where(x => x.FK_Rid != 0).Select(y => y.NoVisitsTotal_Today).Sum();
                                //if ((objadmin.UniqueVisitsToday > 0 && ((dc.shorturldatas.Max(x => x.PK_Shorturl)) > (dc.shorturlclickreferences.Select(x => x.Ref_ShorturlClickID).FirstOrDefault())) && (objadmin.NoVisitsTotal_Today != (objadmin.UsersToday - objadmin.UniqueVisitsToday))))
                                //    objadmin.NoVisitsTotal_Today = objadmin.UsersToday - objadmin.UniqueVisitsToday;
                                //else if (objadmin.UniqueVisitsToday == 0)
                                //    objadmin.NoVisitsTotal_Today = objadmin.UsersToday;
                                //else
                                //    objadmin.NoVisitsTotal_Today = objadmin.NoVisitsTotal_Today;
                                ErrorLogs.LogErrorData("StatsToday_Service,before saving of campaigncounts admin case", "");
                                dc.SaveChanges();
                            }
                        }
                    }
                }
                //if (cntvisits > 0 || cntusers > 0)
                //{
                //    stat_counts st_count = new stat_counts();
                //    stat_counts st_count_admin = new stat_counts();
                //    // int getnextshorturlid = 0; int getnextuserid = 0;
                //    //List<StatsModel_visits> lstvisits = new List<StatsModel_visits>();
                //    List<StatsModel_users> lstusers = new List<StatsModel_users>();
                //    //List<StatsModel_uniquevisits_Today> lstuniquevisits_tot_today = new List<StatsModel_uniquevisits_Today>();
                //    //List<StatsModel_uniquevisits> lstuniquevisits_tot = new List<StatsModel_uniquevisits>();
                //    List<StatsModel_uniqueusers> lstuniqueusers_tot = new List<StatsModel_uniqueusers>();
                //    //List<StatsModel_uniqueusers_today> lstuniqueusers_tot_today = new List<StatsModel_uniqueusers_today>();
                //    if (cntusers > 0)
                //    {
                //        //ErrorLogs.LogErrorData("Ref_ShorturlClickID = " + clickref.Ref_ShorturlClickID, "Ref_UsersID = " + clickref.Ref_UsersID);
                //        //ErrorLogs.LogErrorData("cntvisits = " + cntvisits, "cntusers = " + cntusers);
                //        int refurserid = (int)clickref.Ref_UsersID;
                //        getnextuserid = refurserid + cntusers;

                //        clickref.Ref_UsersID = getnextuserid;
                //        dc.SaveChanges();
                //        lstusers = dc.uiddatas
                //            .AsEnumerable()
                //            .Where(x => x.PK_Uid >= refurserid && x.CreatedDate.Value.Date == DateTime.UtcNow.Date)
                //            .GroupBy(x => x.FK_RID)
                //            .Select(res => new StatsModel_users()
                //            {
                //                Users_Today = res.Select(x => x.MobileNumber).Count(),
                //                UniqueUsers_Today = res.Select(x => x.MobileNumber).Distinct().ToList().Count(),
                //                fk_rid = res.Select(x => x.FK_RID).FirstOrDefault()
                //            }).ToList();

                //        lstuniqueusers_tot = (from s in dc.uiddatas
                //                              .AsEnumerable()
                //                              join st in lstusers on s.FK_RID equals st.fk_rid
                //                              //where s.CreatedDate.Value.Date == DateTime.UtcNow.Date
                //                              group s by s.FK_RID into res
                //                              select new StatsModel_uniqueusers()
                //                              {
                //                                  fk_rid = res.Select(x => x.FK_RID).FirstOrDefault(),
                //                                  UniqueUsers = res.Select(x => x.MobileNumber).Distinct().ToList().Count(),
                //                                  TotalUsers = res.Select(x => x.MobileNumber).ToList().Count()
                //                              }).ToList();
                //        foreach (StatsModel_users users in lstusers)
                //        {

                //            int uniqueusers = 0; int uniqueusers_today = 0; int totalusers = 0; int userstoday = 0;
                //            uniqueusers = lstuniqueusers_tot.Where(x => x.fk_rid == users.fk_rid).Select(x => x.UniqueUsers).SingleOrDefault();
                //            totalusers = lstuniqueusers_tot.Where(x => x.fk_rid == users.fk_rid).Select(x => x.TotalUsers).SingleOrDefault();
                //            uniqueusers_today = lstusers.Where(x => x.fk_rid == users.fk_rid).Select(x => x.UniqueUsers_Today).SingleOrDefault();
                //            userstoday = lstusers.Where(x => x.fk_rid == users.fk_rid).Select(x => x.Users_Today).SingleOrDefault();
                //            ErrorLogs.LogErrorData("fk_rid=" + users.fk_rid,"total users"+totalusers );
                //            st_count = dc.stat_counts.Where(x => x.FK_Rid == users.fk_rid).Select(y => y).SingleOrDefault();
                //            if (st_count != null)
                //            {
                //                st_count.UsersToday = (userstoday > 0) ? (st_count.UsersToday + userstoday) : st_count.UsersToday;
                //                st_count.UrlTotal_Today = st_count.UsersToday;
                //                //st_count.UniqueUsersToday = (users.UniqueUsers_Today > 0) ? (st_count.UniqueUsersToday + users.UniqueUsers_Today) : st_count.UniqueUsersToday;
                //                st_count.UniqueUsers = (uniqueusers > 0) ? (uniqueusers) : st_count.UniqueUsers;
                //                st_count.UniqueUsersToday = (uniqueusers_today > 0) ? (st_count.UniqueUsersToday + uniqueusers_today) : st_count.UniqueUsersToday;
                //                st_count.TotalUsers = (totalusers > 0) ? (totalusers) : st_count.TotalUsers;
                //                st_count.UrlPercent_Today = (st_count.UsersYesterday == 0) ? 0 : ((st_count.UsersToday - st_count.UsersYesterday) / (st_count.UsersYesterday));
                //                //if (cntvisits == 0)
                //                //    st_count.NoVisitsTotal_Today = st_count.TotalUsers - st_count.UniqueVisitsToday;
                //                if ((st_count.UniqueVisitsToday > 0 && ((dc.shorturldatas.Max(x => x.PK_Shorturl)) > (dc.shorturlclickreferences.Select(x => x.Ref_ShorturlClickID).FirstOrDefault())) && (st_count.NoVisitsTotal_Today != (st_count.UsersToday - st_count.UniqueVisitsToday))))
                //                    st_count.NoVisitsTotal_Today = st_count.UsersToday - st_count.UniqueVisitsToday;
                //                else if (st_count.UniqueVisitsToday == 0)
                //                    st_count.NoVisitsTotal_Today = st_count.UsersToday;
                //                else
                //                    st_count.NoVisitsTotal_Today = st_count.NoVisitsTotal_Today;
                //                dc.SaveChanges();
                //                //ErrorLogs.LogErrorData("unique users = " + st_count.UniqueUsers + "fk_rid = " + users.fk_rid +"total users = "+lstusers.Count(), "UniqueUsersToday = " + st_count.UniqueUsersToday.ToString() + "fk_rid = " + users.fk_rid);

                //            }
                //        }
                //        //for admin
                //        st_count_admin = dc.stat_counts.Where(x => x.FK_Rid == 0).SingleOrDefault();
                //        if (st_count_admin != null)
                //        {


                //            st_count_admin.UsersToday = (lstusers != null) ? (dc.stat_counts.Where(x => x.FK_Rid != 0).Select(y => y.UsersToday).Sum()) : st_count_admin.UsersToday;
                //            st_count_admin.UrlTotal_Today = st_count_admin.UsersToday;
                //            //st_count_admin.UniqueUsers = (lstuniqueusers_tot != null) ? (lstuniqueusers_tot.Select(x => x.UniqueUsers).Sum()) : st_count_admin.UniqueUsers;
                //            st_count_admin.UniqueUsers = (lstuniqueusers_tot != null) ? (dc.stat_counts.Where(x => x.FK_Rid != 0).Select(y => y.UniqueUsers).Sum()) : st_count_admin.UniqueUsers;
                //            st_count_admin.UniqueUsersToday = (lstusers != null) ? (dc.stat_counts.Where(x => x.FK_Rid != 0).Select(y => y.UniqueUsersToday).Sum()) : st_count_admin.UniqueUsersToday;
                //            st_count_admin.TotalUsers = (lstuniqueusers_tot != null) ? (dc.stat_counts.Where(x => x.FK_Rid != 0).Select(y => y.TotalUsers).Sum()) : st_count_admin.TotalUsers;
                //            st_count_admin.UrlPercent_Today = (st_count_admin.UsersYesterday == 0) ? 0 : ((st_count_admin.UsersToday - st_count_admin.UsersYesterday) / (st_count_admin.UsersYesterday));

                //            //st_count_admin.NoVisitsTotal_Today = dc.stat_counts.Where(x => x.FK_Rid != 0).Select(y => y.NoVisitsTotal_Today).Sum();
                //            if ((st_count_admin.UniqueVisitsToday > 0 && ((dc.shorturldatas.Max(x => x.PK_Shorturl)) > (dc.shorturlclickreferences.Select(x => x.Ref_ShorturlClickID).FirstOrDefault())) && (st_count_admin.NoVisitsTotal_Today != (st_count_admin.UsersToday - st_count_admin.UniqueVisitsToday))))
                //                st_count_admin.NoVisitsTotal_Today = st_count_admin.UsersToday - st_count_admin.UniqueVisitsToday;
                //            else if (st_count_admin.UniqueVisitsToday == 0)
                //                st_count_admin.NoVisitsTotal_Today = st_count_admin.UsersToday;
                //            else
                //                st_count_admin.NoVisitsTotal_Today = st_count_admin.NoVisitsTotal_Today;
                //            //if (cntvisits == 0)
                //            //    st_count_admin.NoVisitsTotal_Today = (st_count_admin.TotalUsers - st_count_admin.UniqueVisitsToday);
                //            dc.SaveChanges();
                //            //st_count_admin.UsersToday = (lstusers != null) ? (st_count_admin.UsersToday + lstusers.Select(x => x.Users_Today).Sum()) : st_count_admin.UsersToday;
                //            //st_count_admin.UniqueUsers = (lstuniqueusers_tot != null) ? (dc.stat_counts.Where(x => x.FK_Rid != 0).Select(y => y.UniqueUsers).Sum()) : st_count_admin.UniqueUsers;
                //            //st_count_admin.UniqueUsersToday = (lstusers != null) ? (st_count_admin.UniqueUsersToday + lstusers.Select(x => x.UniqueUsers_Today).Sum()) : st_count_admin.UniqueUsersToday;
                //            //st_count_admin.TotalUsers = (lstusers != null) ? (st_count_admin.TotalUsers + lstusers.Select(x => x.Users_Today).Sum()) : st_count_admin.TotalUsers;



                //            //ErrorLogs.LogErrorData("StatsToday_Service visitis today st_count_admin " + st_count_admin.VisitsToday, lstvisits.Select(x => x.Visits_today).Sum().ToString());
                //        }
                //        //getnextuserid = dc.uiddatas.Any() ? dc.uiddatas.Max(x => x.PK_Uid) : 0;
                //        //if (cntusers > 0)
                //        //    clickref.Ref_UsersID = getnextuserid +1;
                //        //dc.SaveChanges();
                //    }

                //    //stat_counts st_count = new stat_counts();
                //    //st_count = dc.stat_counts.Where(x => x.FK_Rid == users.fk_rid).Select(y => y).SingleOrDefault();
                //    //stat_counts st_count_admin = dc.stat_counts.Where(x => x.FK_Rid == 0).SingleOrDefault();
                //    //if (cntvisits == 0)
                //    //    st_count_admin.NoVisitsTotal_Today = (st_count_admin.TotalUsers - st_count_admin.UniqueVisitsToday);
                //    //dc.SaveChanges();
                //    //////users --end

                //}
            //}
        }
                
        else
         Stats_update(days_diff);


            }

            catch (Exception ex)
            {

                ErrorLogs.LogErrorData("StatsToday_Service" + ex.InnerException + ex.StackTrace, ex.Message);
                //shorturlclickreference clickref = dc.shorturlclickreferences.Select(x => x).FirstOrDefault();
                //int refvisitid = (int)clickref.Ref_ShorturlClickID;
                //int  getnextshorturlid = refvisitid + cntvisits;

                //clickref.Ref_ShorturlClickID = getnextshorturlid;
                //dc.SaveChanges();

            }
        }

        public void getcampaigntodaycounts(int? rid, out int userstoday, out int uniqueusers_today)
        {

            userstoday = 0; uniqueusers_today = 0;
            var conn1 = System.Configuration.ConfigurationManager.ConnectionStrings["shortenURLConnectionString"].ConnectionString;
            MySqlConnection connection = new MySqlConnection(conn1);
            var csb = new MySqlConnectionStringBuilder(conn1);
            string dbname = csb.Database;
            string strQuery = "select COUNT(MobileNumber)  as UsersToday ,COUNT(distinct MobileNumber)  as uniqueUsersToday from UIDDATA where FK_RID=" + rid + " and cast(CreatedDate as date)=UTC_Date(); ";

            //string strQuery = "SELECT `AUTO_INCREMENT` FROM  INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'shortenurl' AND TABLE_NAME   = 'uiddata';";
            int id = 0;
            //string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["shortenURLConnectionString"].ToString()
            using (MySqlConnection conn = new MySqlConnection(conn1))
            {
                MySqlCommand cmd = new MySqlCommand(strQuery, conn);
                try
                {
                    conn.Open();
                    cmd.CommandTimeout = 2 * 60;
                    //ErrorLogs.LogErrorData("statstodayservice,before execution of query", strQuery);
                    MySqlDataReader myReader = cmd.ExecuteReader();
                    //ErrorLogs.LogErrorData("statstodayservice,after execution of query", strQuery);
                    campcount camptodaycounts = ((IObjectContextAdapter)dc)
                          .ObjectContext
                          .Translate<campcount>(myReader, "uiddatas", MergeOption.AppendOnly).SingleOrDefault();
                    userstoday = camptodaycounts.UsersToday;
                    uniqueusers_today = camptodaycounts.uniqueUsersToday;
                    conn.Close();
                }
                catch (Exception ex)
                {
                    ErrorLogs.LogErrorData("statstodayservice,getcampaigntodaycounts" + "..." + ex.InnerException + ex.StackTrace, ex.Message);

                    Console.WriteLine(ex.Message);
                }
            }


        }
        public class campcount
        {
            public int UsersToday { get; set; }
            public int uniqueUsersToday { get; set; }
        }
        public void Stats_update(int days_diff)
        {
            try
            {

                List<stat_counts> stat_list1 = dc.stat_counts.Select(x => x).ToList();
                //List<Camp_stat_sp> camp_stat_lst = getcamp_stat(stat_list1);
                List<stat_counts> stat_list2 = new List<stat_counts>();
                stat_list2 = stat_list1;
                DateTime todaysDate = DateTime.UtcNow.Date;
                int daysinmonth = DateTime.DaysInMonth(todaysDate.Year, todaysDate.Month);
                stat_list1 = Stats_update_Reocrds(stat_list1, days_diff);
                //dc.SaveChanges();
                int rec = 0;
                foreach (stat_counts st in stat_list2)
                {
                   
                    st.UniqueUsersYesterday = stat_list1[rec].UniqueUsersYesterday;
                    st.UsersYesterday = stat_list1[rec].UsersYesterday;
                    st.VisitsYesterday = stat_list1[rec].VisitsYesterday;
                    st.UniqueVisitsYesterday = stat_list1[rec].UniqueVisitsYesterday;
                    st.RevisitsTotal_Yesterday = stat_list1[rec].RevisitsTotal_Yesterday;
                    st.NoVisitsTotal_Yesterday = stat_list1[rec].NoVisitsTotal_Yesterday;

                    st.UniqueUsersLast7days = stat_list1[rec].UniqueUsersLast7days;
                    st.UsersLast7days = stat_list1[rec].UsersLast7days;
                    st.UniqueVisits = stat_list1[rec].UniqueVisits;
                    st.UniqueVisitsLast7day = stat_list1[rec].UniqueVisitsLast7day;
                    st.VisitsLast7days = stat_list1[rec].VisitsLast7days;
                    st.CampaignsLast7days = stat_list1[rec].CampaignsLast7days;

                    st.CampaignsMonth = stat_list1[rec].CampaignsMonth;
                    st.UniqueUsersToday = stat_list1[rec].UniqueUsersToday;
                    st.UsersToday = stat_list1[rec].UsersToday;
                    st.VisitsToday = stat_list1[rec].VisitsToday;
                    st.UniqueVisitsToday = stat_list1[rec].UniqueVisitsToday;
                    st.UrlTotal_Today = stat_list1[rec].UrlTotal_Today;
                    st.UrlPercent_Today = stat_list1[rec].UrlPercent_Today;
                    st.VisitsTotal_Today = stat_list1[rec].VisitsTotal_Today;
                    st.VisitsPercent_Today = stat_list1[rec].VisitsPercent_Today;
                    st.RevisitsTotal_Today = stat_list1[rec].RevisitsTotal_Today;
                    st.RevisitsPercent_Today = stat_list1[rec].RevisitsPercent_Today;
                    st.NoVisitsTotal_Today = stat_list1[rec].NoVisitsTotal_Today;
                    st.NoVisitsPercent_Today = stat_list1[rec].NoVisitsPercent_Today;

                    st.UrlTotal_Week = stat_list1[rec].UrlTotal_Week;
                    st.UrlPercent_Week = stat_list1[rec].UrlPercent_Week;
                    st.VisitsTotal_Week = stat_list1[rec].VisitsTotal_Week;
                    st.VisitsPercent_Week = stat_list1[rec].VisitsPercent_Week;
                    st.RevisitsTotal_Week = stat_list1[rec].RevisitsTotal_Week;
                    st.RevisitsPercent_Week = stat_list1[rec].RevisitsPercent_Week;
                    st.NoVisitsTotal_Week = stat_list1[rec].NoVisitsTotal_Week;
                    st.NoVisitsPercent_Week = stat_list1[rec].NoVisitsPercent_Week;

                    st.UrlTotal_Month = stat_list1[rec].UrlTotal_Month;
                    st.UrlTotalPercent_Month = stat_list1[rec].UrlTotalPercent_Month;
                    st.VisitsTotal_Month = stat_list1[rec].VisitsTotal_Month;
                    st.VisitsPercent_Month = stat_list1[rec].VisitsPercent_Month;
                    st.RevisitsTotal_Month = stat_list1[rec].RevisitsTotal_Month;
                    st.RevisitsPercent_Month = stat_list1[rec].RevisitsPercent_Month;
                    st.NoVisitsTotal_Month = stat_list1[rec].NoVisitsTotal_Month;
                    st.NoVisitsPercent_Month = stat_list1[rec].NoVisitsPercent_Month;

                    st.DaysCount_Week = stat_list1[rec].DaysCount_Week;
                    st.DaysCount_Month = stat_list1[rec].DaysCount_Month;
                    rec = rec + 1;
                }
                dc.SaveChanges();
                //set shortclick ref poin
                //int gettodayshorturlid = 0;
                shorturlclickreference shrt_clickref = dc.shorturlclickreferences.Select(x => x).FirstOrDefault();
                //gettodayshorturlid = (dc.shorturldatas.AsEnumerable().Where(x => x.CreatedDate != null && x.CreatedDate.Value.Date == DateTime.UtcNow.Date).Count() > 0) ? (dc.shorturldatas.AsEnumerable().Where(x => x.CreatedDate != null && x.CreatedDate.Value.Date == DateTime.UtcNow.Date).Min(y => y.PK_Shorturl)) : (dc.shorturldatas.Max(x => x.PK_Shorturl));
                //shrt_clickref.Ref_ShorturlClickID = gettodayshorturlid;
                shrt_clickref.UpdatedDate = DateTime.UtcNow;
                dc.SaveChanges();
            }


            catch (Exception ex)
            {
                ErrorLogs.LogErrorData(ex.StackTrace, ex.Message);
            }
        }

       public List<stat_counts> Stats_update_Reocrds(List<stat_counts> stat_list1,  int days_diff)
        {
           DateTime todaysDate = DateTime.UtcNow.Date;
           int daysinmonth = DateTime.DaysInMonth(todaysDate.Year, todaysDate.Month);
               
           if(days_diff == 1)
           {
               stat_list1 = (from st1 in stat_list1
                             select new stat_counts()
                             {

                                 CampaignsMonth = (st1.DaysCount_Month < daysinmonth) ? (st1.CampaignsMonth) : 0,
                                 UrlTotal_Month = (st1.DaysCount_Month < daysinmonth) ? (st1.UrlTotal_Month + st1.UrlTotal_Week) : 0,
                                 UrlTotalPercent_Month = (st1.DaysCount_Month < daysinmonth) ? (st1.UrlTotalPercent_Month + st1.UrlPercent_Week) : 0,
                                 VisitsTotal_Month = (st1.DaysCount_Month < daysinmonth) ? (st1.VisitsTotal_Month + st1.VisitsTotal_Week) : 0,
                                 VisitsPercent_Month = (st1.DaysCount_Month < daysinmonth) ? (st1.VisitsPercent_Month + st1.VisitsPercent_Week) : 0,
                                 RevisitsTotal_Month = (st1.DaysCount_Month < daysinmonth) ? (st1.RevisitsTotal_Month + st1.RevisitsTotal_Week) : 0,
                                 RevisitsPercent_Month = (st1.DaysCount_Month < daysinmonth) ? (st1.RevisitsPercent_Month + st1.RevisitsPercent_Week) : 0,
                                 NoVisitsTotal_Month = (st1.DaysCount_Month < daysinmonth) ? (st1.NoVisitsTotal_Month + st1.NoVisitsTotal_Week) : 0,
                                 NoVisitsPercent_Month = (st1.DaysCount_Month < daysinmonth) ? (st1.NoVisitsPercent_Month + st1.NoVisitsPercent_Week) : 0,


                                 UniqueUsersLast7days = ((st1.DaysCount_Week < 2) ? (st1.UniqueUsersYesterday + st1.UniqueUsersToday) : 0)
                                                       + ((st1.DaysCount_Week >= 2 && st1.DaysCount_Week < 7) ? (st1.UniqueUsersLast7days + st1.UniqueUsersYesterday + st1.UniqueUsersToday) : 0),
                                 UsersLast7days = ((st1.DaysCount_Week < 2) ? (st1.UsersYesterday + st1.UsersToday) : 0)
                                                 + ((st1.DaysCount_Week >= 2 && st1.DaysCount_Week < 7) ? (st1.UsersLast7days + st1.UsersYesterday + st1.UsersToday) : 0),
                                 UniqueVisitsLast7day = ((st1.DaysCount_Week < 2) ? (st1.UniqueVisitsYesterday + st1.UniqueVisitsToday) : 0)
                                                       + ((st1.DaysCount_Week >= 2 && st1.DaysCount_Week < 7) ? (st1.UniqueVisitsLast7day + st1.UniqueVisitsYesterday + st1.UniqueVisitsToday) : 0),
                                 VisitsLast7days = ((st1.DaysCount_Week < 2) ? (st1.VisitsYesterday + st1.VisitsToday) : 0)
                                                   + ((st1.DaysCount_Week >= 2 && st1.DaysCount_Week < 7) ? (st1.VisitsLast7days + st1.VisitsYesterday + st1.VisitsToday) : 0),
                                 CampaignsLast7days = (st1.DaysCount_Week < 7) ? (st1.CampaignsLast7days) : 0,


                                 UrlTotal_Week = ((st1.DaysCount_Week < 2) ? (st1.UrlTotal_Today) : 0)
                                                   + ((st1.DaysCount_Week >= 2 && st1.DaysCount_Week < 7) ? (st1.UrlTotal_Week + st1.UrlTotal_Today) : 0),
                                 UrlPercent_Week = ((st1.DaysCount_Week < 2) ? (st1.UrlPercent_Today) : 0)
                                                   + ((st1.DaysCount_Week >= 2 && st1.DaysCount_Week < 7) ? (st1.UrlPercent_Week + st1.UrlPercent_Today) : 0),
                                 VisitsTotal_Week = ((st1.DaysCount_Week < 2) ? (st1.VisitsTotal_Today) : 0)
                                                   + ((st1.DaysCount_Week >= 2 && st1.DaysCount_Week < 7) ? (st1.VisitsTotal_Week + st1.VisitsTotal_Today) : 0),
                                 VisitsPercent_Week = ((st1.DaysCount_Week < 2) ? (st1.VisitsPercent_Today) : 0)
                                                   + ((st1.DaysCount_Week >= 2 && st1.DaysCount_Week < 7) ? (st1.VisitsPercent_Week + st1.VisitsPercent_Today) : 0),
                                 RevisitsTotal_Week = ((st1.DaysCount_Week < 2) ? (st1.RevisitsTotal_Today) : 0)
                                                   + ((st1.DaysCount_Week >= 2 && st1.DaysCount_Week < 7) ? (st1.RevisitsTotal_Week + st1.RevisitsTotal_Today) : 0),
                                 RevisitsPercent_Week = ((st1.DaysCount_Week < 2) ? (st1.RevisitsPercent_Today) : 0)
                                                   + ((st1.DaysCount_Week >= 2 && st1.DaysCount_Week < 7) ? (st1.RevisitsPercent_Week + st1.RevisitsPercent_Today) : 0),
                                 NoVisitsTotal_Week = ((st1.DaysCount_Week < 2) ? (st1.NoVisitsTotal_Today) : 0)
                                                   + ((st1.DaysCount_Week >= 2 && st1.DaysCount_Week < 7) ? (st1.NoVisitsTotal_Week + st1.NoVisitsTotal_Today) : 0),
                                 NoVisitsPercent_Week = ((st1.DaysCount_Week < 2) ? (st1.NoVisitsPercent_Today) : 0)
                                                   + ((st1.DaysCount_Week >= 2 && st1.DaysCount_Week < 7) ? (st1.NoVisitsPercent_Week + st1.NoVisitsPercent_Today) : 0),


                                 UniqueVisits = st1.UniqueVisits,
                                 UniqueUsersYesterday = st1.UniqueUsersToday,
                                 UsersYesterday = st1.UsersToday,
                                 VisitsYesterday = st1.VisitsToday,
                                 UniqueVisitsYesterday = st1.UniqueVisitsToday,
                                 RevisitsTotal_Yesterday = st1.RevisitsTotal_Today,
                                 NoVisitsTotal_Yesterday = st1.NoVisitsTotal_Today,

                                 UrlTotal_Today = 0,
                                 UrlPercent_Today = 0,
                                 VisitsTotal_Today = 0,
                                 VisitsPercent_Today = 0,
                                 RevisitsTotal_Today = 0,
                                 RevisitsPercent_Today = 0,
                                 NoVisitsTotal_Today = 0,
                                 NoVisitsPercent_Today = 0,

                                 UniqueUsersToday = 0,
                                 UsersToday = 0,
                                 VisitsToday = 0,
                                 UniqueVisitsToday = 0,

                                 DaysCount_Week = (st1.DaysCount_Week < 7) ? (st1.DaysCount_Week + 1) : 0,
                                 DaysCount_Month = (st1.DaysCount_Month < daysinmonth) ? (st1.DaysCount_Month + 1) : 0,
                                 FK_Rid = st1.FK_Rid,
                                 FK_ClientID = st1.FK_ClientID
                             }
               ).ToList();
           }
           else if(days_diff > 1 && days_diff <= 7)
           {
                 stat_list1 = (from st1 in stat_list1
                              select new stat_counts()
                              {

                                  CampaignsMonth = (st1.DaysCount_Month < daysinmonth) ? (st1.CampaignsMonth) : 0,
                                  UrlTotal_Month = (st1.DaysCount_Month < daysinmonth) ? (st1.UrlTotal_Month + st1.UrlTotal_Week) : 0,
                                  UrlTotalPercent_Month = (st1.DaysCount_Month < daysinmonth) ? (st1.UrlTotalPercent_Month + st1.UrlPercent_Week) : 0,
                                  VisitsTotal_Month = (st1.DaysCount_Month < daysinmonth) ? (st1.VisitsTotal_Month + st1.VisitsTotal_Week) : 0,
                                  VisitsPercent_Month = (st1.DaysCount_Month < daysinmonth) ? (st1.VisitsPercent_Month + st1.VisitsPercent_Week) : 0,
                                  RevisitsTotal_Month = (st1.DaysCount_Month < daysinmonth) ? (st1.RevisitsTotal_Month + st1.RevisitsTotal_Week) : 0,
                                  RevisitsPercent_Month = (st1.DaysCount_Month < daysinmonth) ? (st1.RevisitsPercent_Month + st1.RevisitsPercent_Week) : 0,
                                  NoVisitsTotal_Month = (st1.DaysCount_Month < daysinmonth) ? (st1.NoVisitsTotal_Month + st1.NoVisitsTotal_Week) : 0,
                                  NoVisitsPercent_Month = (st1.DaysCount_Month < daysinmonth) ? (st1.NoVisitsPercent_Month + st1.NoVisitsPercent_Week) : 0,


                                  UniqueUsersLast7days = ((st1.DaysCount_Week < 2) ? (st1.UniqueUsersYesterday + st1.UniqueUsersToday) : 0)
                                                        + ((st1.DaysCount_Week >= 2 && st1.DaysCount_Week < 7) ? (st1.UniqueUsersLast7days + st1.UniqueUsersYesterday + st1.UniqueUsersToday) : 0),
                                  UsersLast7days = ((st1.DaysCount_Week < 2) ? (st1.UsersYesterday + st1.UsersToday) : 0)
                                                  + ((st1.DaysCount_Week >= 2 && st1.DaysCount_Week < 7) ? (st1.UsersLast7days + st1.UsersYesterday + st1.UsersToday) : 0),
                                  UniqueVisitsLast7day = ((st1.DaysCount_Week < 2) ? (st1.UniqueVisitsYesterday + st1.UniqueVisitsToday) : 0)
                                                        + ((st1.DaysCount_Week >= 2 && st1.DaysCount_Week < 7) ? (st1.UniqueVisitsLast7day + st1.UniqueVisitsYesterday + st1.UniqueVisitsToday) : 0),
                                  VisitsLast7days = ((st1.DaysCount_Week < 2) ? (st1.VisitsYesterday + st1.VisitsToday) : 0)
                                                    + ((st1.DaysCount_Week >= 2 && st1.DaysCount_Week < 7) ? (st1.VisitsLast7days + st1.VisitsYesterday + st1.VisitsToday) : 0),
                                  CampaignsLast7days = (st1.DaysCount_Week < 7) ? (st1.CampaignsLast7days) : 0,

                                 
                                  UrlTotal_Week = ((st1.DaysCount_Week < 2) ? (st1.UrlTotal_Today) : 0)
                                                    + ((st1.DaysCount_Week >= 2 && st1.DaysCount_Week < 7) ? (st1.UrlTotal_Week + st1.UrlTotal_Today) : 0),
                                  UrlPercent_Week = ((st1.DaysCount_Week < 2) ? (st1.UrlPercent_Today) : 0)
                                                    + ((st1.DaysCount_Week >= 2 && st1.DaysCount_Week < 7) ? (st1.UrlPercent_Week + st1.UrlPercent_Today) : 0),
                                  VisitsTotal_Week = ((st1.DaysCount_Week < 2) ? (st1.VisitsTotal_Today) : 0)
                                                    + ((st1.DaysCount_Week >= 2 && st1.DaysCount_Week < 7) ? (st1.VisitsTotal_Week + st1.VisitsTotal_Today) : 0),
                                  VisitsPercent_Week = ((st1.DaysCount_Week < 2) ? (st1.VisitsPercent_Today) : 0)
                                                    + ((st1.DaysCount_Week >= 2 && st1.DaysCount_Week < 7) ? (st1.VisitsPercent_Week + st1.VisitsPercent_Today) : 0),
                                  RevisitsTotal_Week = ((st1.DaysCount_Week < 2) ? (st1.RevisitsTotal_Today) : 0)
                                                    + ((st1.DaysCount_Week >= 2 && st1.DaysCount_Week < 7) ? (st1.RevisitsTotal_Week + st1.RevisitsTotal_Today) : 0),
                                  RevisitsPercent_Week = ((st1.DaysCount_Week < 2) ? (st1.RevisitsPercent_Today) : 0)
                                                    + ((st1.DaysCount_Week >= 2 && st1.DaysCount_Week < 7) ? (st1.RevisitsPercent_Week + st1.RevisitsPercent_Today) : 0),
                                  NoVisitsTotal_Week = ((st1.DaysCount_Week < 2) ? (st1.NoVisitsTotal_Today) : 0)
                                                    + ((st1.DaysCount_Week >= 2 && st1.DaysCount_Week < 7) ? (st1.NoVisitsTotal_Week + st1.NoVisitsTotal_Today) : 0),
                                  NoVisitsPercent_Week = ((st1.DaysCount_Week < 2) ? (st1.NoVisitsPercent_Today) : 0)
                                                    + ((st1.DaysCount_Week >= 2 && st1.DaysCount_Week < 7) ? (st1.NoVisitsTotal_Week + st1.NoVisitsPercent_Today) : 0),


                                  UniqueVisits = st1.UniqueVisits,
                                  UniqueUsersYesterday = 0,
                                  UsersYesterday = 0,
                                  VisitsYesterday = 0,
                                  UniqueVisitsYesterday = 0,
                                  RevisitsTotal_Yesterday = 0,
                                  NoVisitsTotal_Yesterday = 0,
                                  
                                  UrlTotal_Today = 0,
                                  UrlPercent_Today = 0,
                                  VisitsTotal_Today = 0,
                                  VisitsPercent_Today = 0,
                                  RevisitsTotal_Today = 0,
                                  RevisitsPercent_Today = 0,
                                  NoVisitsTotal_Today = 0,
                                  NoVisitsPercent_Today = 0,

                                  UniqueUsersToday = 0,
                                  UsersToday = 0,
                                  VisitsToday = 0,
                                  UniqueVisitsToday = 0,
                                  
                                  DaysCount_Week = (st1.DaysCount_Week < 7) ? (st1.DaysCount_Week + 1) : 0,
                                  DaysCount_Month = (st1.DaysCount_Month < daysinmonth) ? (st1.DaysCount_Month + 1) : 0,
                                  FK_Rid = st1.FK_Rid,
                                  FK_ClientID = st1.FK_ClientID
                              }
                ).ToList();
           }
           else if(days_diff>7)
           {
                stat_list1 = (from st1 in stat_list1
                              select new stat_counts()
                              {

                                  CampaignsMonth = (st1.DaysCount_Month < daysinmonth) ? (st1.CampaignsMonth) : 0,
                                  UrlTotal_Month = (st1.DaysCount_Month < daysinmonth) ? (st1.UrlTotal_Month + st1.UrlTotal_Week) : 0,
                                  UrlTotalPercent_Month = (st1.DaysCount_Month < daysinmonth) ? (st1.UrlTotalPercent_Month + st1.UrlPercent_Week) : 0,
                                  VisitsTotal_Month = (st1.DaysCount_Month < daysinmonth) ? (st1.VisitsTotal_Month + st1.VisitsTotal_Week) : 0,
                                  VisitsPercent_Month = (st1.DaysCount_Month < daysinmonth) ? (st1.VisitsPercent_Month + st1.VisitsPercent_Week) : 0,
                                  RevisitsTotal_Month = (st1.DaysCount_Month < daysinmonth) ? (st1.RevisitsTotal_Month + st1.RevisitsTotal_Week) : 0,
                                  RevisitsPercent_Month = (st1.DaysCount_Month < daysinmonth) ? (st1.RevisitsPercent_Month + st1.RevisitsPercent_Week) : 0,
                                  NoVisitsTotal_Month = (st1.DaysCount_Month < daysinmonth) ? (st1.NoVisitsTotal_Month + st1.NoVisitsTotal_Week) : 0,
                                  NoVisitsPercent_Month = (st1.DaysCount_Month < daysinmonth) ? (st1.NoVisitsPercent_Month + st1.NoVisitsPercent_Week) : 0,


                                  UniqueUsersLast7days =  0,
                                  UsersLast7days = 0,
                                  UniqueVisitsLast7day = 0,
                                  VisitsLast7days = 0,
                                  CampaignsLast7days = 0,

                                 
                                  UrlTotal_Week = 0,
                                  UrlPercent_Week = 0,
                                  VisitsTotal_Week = 0,
                                  VisitsPercent_Week = 0,
                                  RevisitsTotal_Week = 0,
                                  RevisitsPercent_Week = 0,
                                  NoVisitsTotal_Week = 0,
                                  NoVisitsPercent_Week = 0,


                                  UniqueVisits = st1.UniqueVisits,
                                  UniqueUsersYesterday = 0,
                                  UsersYesterday = 0,
                                  VisitsYesterday = 0,
                                  UniqueVisitsYesterday = 0,
                                  RevisitsTotal_Yesterday = 0,
                                  NoVisitsTotal_Yesterday = 0,
                                  
                                  UrlTotal_Today = 0,
                                  UrlPercent_Today = 0,
                                  VisitsTotal_Today = 0,
                                  VisitsPercent_Today = 0,
                                  RevisitsTotal_Today = 0,
                                  RevisitsPercent_Today = 0,
                                  NoVisitsTotal_Today = 0,
                                  NoVisitsPercent_Today = 0,

                                  UniqueUsersToday = 0,
                                  UsersToday = 0,
                                  VisitsToday = 0,
                                  UniqueVisitsToday = 0,
                                  
                                  DaysCount_Week = (st1.DaysCount_Week < 7) ? (st1.DaysCount_Week + 1) : 0,
                                  DaysCount_Month = (st1.DaysCount_Month < daysinmonth) ? (st1.DaysCount_Month + 1) : 0,
                                  FK_Rid = st1.FK_Rid,
                                  FK_ClientID = st1.FK_ClientID
                              }
                ).ToList();
           }
           return stat_list1;
        }
      
    }
}