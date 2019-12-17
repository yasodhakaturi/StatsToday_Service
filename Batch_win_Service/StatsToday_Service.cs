﻿using MySql.Data.MySqlClient;
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
            timer.Interval = new TimeSpan(0, 0, 1, 0).TotalMilliseconds;
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
                int getnextshorturlid = 0; 
                int getnextuserid = 0;
                List<StatsModel_visits> lstvisits = new List<StatsModel_visits>();
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
                    dc.shorturlclickreferences.Add(shrt_clickref);
                    dc.SaveChanges();
                    clickref = dc.shorturlclickreferences.Select(x => x).FirstOrDefault();
                }
                else
                    clickref = dc.shorturlclickreferences.Select(x => x).FirstOrDefault();

               //var novisitcampaigns = ((from r in dc.riddatas
               //                               //.AsEnumerable()
               //                               where r.PK_Rid > clickref.Ref_UsersID
               //                               select (int)r.PK_Rid).ToList()).Except((from s in dc.shorturldatas
               //                                                                 //.AsEnumerable()
               //                                                                 where s.PK_Shorturl > clickref.Ref_ShorturlClickID
               //                                                                 select (int)s.FK_RID).Distinct().ToList());
               
               // if(novisitcampaigns.Count()>0)
               // {
               //     int cntcamp = dc.riddatas.Where(x => x.PK_Rid > clickref.Ref_UsersID).Count();
               //     clickref.Ref_UsersID = clickref.Ref_UsersID + cntcamp;
               //     dc.SaveChanges();
                    
               //     List<int> no_vist_camp_list = novisitcampaigns.ToList();
               //     foreach (int i in no_vist_camp_list)
               //     {
               //         stat_counts st_today_novisitcount = new stat_counts();
               //          st_today_novisitcount = dc.stat_counts.Where(x => x.FK_Rid == i).Select(y => y).SingleOrDefault();
               //          ErrorLogs.LogErrorData("StatsToday_Service NoVisitsTotal_Today" + st_today_novisitcount.UsersToday, i.ToString());

               //         st_today_novisitcount.NoVisitsTotal_Today = st_today_novisitcount.UsersToday;
               //         dc.SaveChanges();
               //     }
               //     stat_counts st_admin_today_novisitcount = dc.stat_counts.Where(x => x.FK_Rid == 0).Select(y => y).SingleOrDefault();
               //     st_admin_today_novisitcount.NoVisitsTotal_Today = dc.stat_counts.Where(x => x.FK_Rid != 0).Select(y => y.NoVisitsTotal_Today).Sum();
               //     dc.SaveChanges();
               // }
                
                //int cntusers =  dc.uiddatas.Where(x => x.PK_Uid > clickref.Ref_UsersID).ToList().Count();
                
                //if (cntusers > 0)
                //{

                //    int refurserid = (int)clickref.Ref_UsersID;
                //    getnextuserid = refurserid + cntusers;

                //    clickref.Ref_UsersID = getnextuserid;
                //    dc.SaveChanges();
                //    //DateTime? dt_max = dc.uiddatas.Where(x => x.PK_Uid > clickref.Ref_UsersID).Select(y => y.CreatedDate).Max();
                //    //ErrorLogs.LogErrorData("dt_max = " +dt_max.Value.AddMinutes(1),"DateTime.UtcNow ="+DateTime.UtcNow)
                //    //if(dt_max.Value.AddMinutes(1) < DateTime.UtcNow)
                //    //{
                //    List<StatsModel_users> lstusers = dc.uiddatas.AsEnumerable()
                //         .Where(x => x.PK_Uid > refurserid && x.CreatedDate.Value.Date == DateTime.UtcNow.Date)
                //         .GroupBy(x => x.FK_RID)
                //         .Select(res => new StatsModel_users()
                //         {
                //             Users_Today = res.Select(x => x.MobileNumber).Count(),
                //             //UniqueUsers_Today = res.Select(x => x.MobileNumber).Distinct().ToList().Count(),
                //             fk_rid = res.Select(x => x.FK_RID).FirstOrDefault()
                //         }).ToList();
                //    //List<int?> list_fkrid = dc.uiddatas.Where(x => x.PK_Uid > refurserid).Select(y => y.FK_RID).Distinct().ToList();

                //   foreach (StatsModel_users i in lstusers)
                //        //foreach (int? i in list_fkrid)
                //        {
                //            stat_counts novisit_st_count = dc.stat_counts.Where(x => x.FK_Rid == i.fk_rid).Select(y => y).SingleOrDefault();
                //            //novisit_st_count.NoVisitsTotal_Today =(novisit_st_count.UniqueVisitsToday ==0) ? Math.Abs(i.Users_Today) - Math.Abs((int)novisit_st_count.UniqueVisitsToday);
                //            novisit_st_count.NoVisitsTotal_Today = (novisit_st_count.UniqueVisitsToday == 0) ? (i.Users_Today) : novisit_st_count.NoVisitsTotal_Today;
                //       dc.SaveChanges();
                //            ErrorLogs.LogErrorData("StatsToday_Service Novisit_st_count = " + novisit_st_count.NoVisitsTotal_Today, " UsersToday = " + novisit_st_count.UsersToday + " UniqueVisitsToday = " + novisit_st_count.UniqueVisitsToday);

                //        }
                //        stat_counts novisit_st_count_admin = dc.stat_counts.Where(x => x.FK_Rid == 0).Select(y => y).SingleOrDefault();
                //        //novisit_st_count_admin.NoVisitsTotal_Today = (novisit_st_count_admin.UsersToday) - novisit_st_count_admin.UniqueVisitsToday;
                //        novisit_st_count_admin.NoVisitsTotal_Today = (lstusers.Select(x=>x.Users_Today).Sum()) - novisit_st_count_admin.UniqueVisitsToday;
    
                //    dc.SaveChanges();

                        
                //    //}
                //}
                
                int cntvisits = dc.shorturldatas.Where(x => x.PK_Shorturl > clickref.Ref_ShorturlClickID).ToList().Count();
                if (cntvisits > 0)
                    {
                        
                        int refvisitid = (int)clickref.Ref_ShorturlClickID;
                        //getnextshorturlid = refvisitid + cntvisits;
                        getnextshorturlid = dc.shorturldatas.Max(x => x.PK_Shorturl);
                        clickref.Ref_ShorturlClickID = getnextshorturlid;
                        dc.SaveChanges();
                        lstvisits = dc.shorturldatas.AsEnumerable()
                           .Where(x => x.PK_Shorturl > refvisitid && x.CreatedDate.Value.Date == DateTime.UtcNow.Date)
                           .GroupBy(x => x.FK_RID)
                           .Select(res => new StatsModel_visits()
                           {
                               Visits_today = res.Select(x => x.FK_Uid).ToList().Count(),
                               //uniqueVisits_today = res.Select(x => x.FK_Uid).Distinct().ToList().Count(),
                               //Todays_ReVisitCount = (res.Select(x => x.FK_Uid).Count()) - (res.Select(x => x.FK_Uid).Distinct().Count()),
                               fk_rid = res.Select(x => x.FK_RID).FirstOrDefault(),
                               fk_clientid=res.Select(x=>x.FK_ClientID).FirstOrDefault()
                               //fk_uid=res.Select(x=>x.FK_Uid).Distinct().ToList()
                           }).ToList();
                        //lstuniquevisits_tot_today = (from s in dc.shorturldatas
                        //                                           .AsEnumerable()
                        //                 join st in lstvisits on s.FK_RID equals st.fk_rid
                        //                 where s.CreatedDate.Value.Date == DateTime.UtcNow.Date
                        //                 group s by s.FK_RID into res
                        //                 select new StatsModel_uniquevisits_Today()
                        //                 {
                        //                     fk_rid = res.Select(x => x.FK_RID).FirstOrDefault(),
                        //                     uniqueVisits_today = res.Select(x => x.FK_Uid).Distinct().Count()
                        //                 }).ToList();
                        lstuniquevisits_tot_today = (from s in dc.shorturldatas
                                                                   .AsEnumerable()
                                         join st in lstvisits on s.FK_RID equals st.fk_rid
                                         where s.CreatedDate.Value.Date == DateTime.UtcNow.Date
                                         group s by s.FK_RID into res
                                               select new StatsModel_uniquevisits_Today()
                                         {
                                             fk_rid = res.Select(x => x.FK_RID).FirstOrDefault(),
                                             fk_clientid=res.Select(x=>x.FK_ClientID).FirstOrDefault(),
                                             //uniquevists = res.Select(x => x.FK_Uid).Distinct().ToList().Count(),
                                             Todays_ReVisitCount = (res.Select(x => x.FK_Uid).Count()) - (res.Select(x => x.FK_Uid).Distinct().Count()),
                                             uniqueVisits_today = res.Select(x => x.FK_Uid).Distinct().ToList().Count()
                                             //Todays_ReVisitCount = (res.Where(y => y.CreatedDate.Value.Date == DateTime.UtcNow.Date).Select(x => x.FK_Uid).Count()) - (res.Where(y => y.CreatedDate.Value.Date == DateTime.UtcNow.Date).Select(x => x.FK_Uid).Distinct().Count()),
                                             //uniqueVisits_today = res.Where(y => y.CreatedDate.Value.Date == DateTime.UtcNow.Date).Select(x => x.FK_Uid).Distinct().ToList().Count()
                                         }).ToList();
                        lstuniquevisits_tot = (from s in dc.shorturldatas
                                                                       .AsEnumerable()
                                               join st in lstvisits on s.FK_RID equals st.fk_rid
                                              
                                               group s by s.FK_RID into res
                                               select new StatsModel_uniquevisits()
                                               {
                                                   fk_rid = res.Select(x => x.FK_RID).FirstOrDefault(),
                                                   fk_clientid=res.Select(x=>x.FK_ClientID).FirstOrDefault(),
                                                   uniquevists = res.Select(x => x.FK_Uid).Distinct().ToList().Count()
                                                   
                                               }).ToList();

                        foreach (StatsModel_visits vst in lstvisits)
                        {
                            stat_counts st_count = new stat_counts();
                            int uniquevists = 0; int revisitcount = 0; int uniquevistis_today = 0;
                            uniquevists = lstuniquevisits_tot.Where(x => x.fk_rid == vst.fk_rid).Select(x => x.uniquevists).SingleOrDefault();
                            revisitcount = lstuniquevisits_tot_today.Where(x => x.fk_rid == vst.fk_rid).Select(x => x.Todays_ReVisitCount).SingleOrDefault();
                            uniquevistis_today = lstuniquevisits_tot_today.Where(x => x.fk_rid == vst.fk_rid).Select(x => x.uniqueVisits_today).SingleOrDefault();
                            //int novisits = lstnovisits_today.Where(x => x.fk_rid == vst.fk_rid).Select(x => x.Novists_today).SingleOrDefault();
                            st_count = dc.stat_counts.Where(x => x.FK_Rid == vst.fk_rid).Select(y=>y).SingleOrDefault();
                            if (st_count != null)
                            {
                                //ErrorLogs.LogErrorData("StatsToday_Service visitis today st_count" + st_count.VisitsToday, vst.Visits_today.ToString());

                                st_count.VisitsToday = (vst.Visits_today > 0) ? (st_count.VisitsToday + vst.Visits_today) : st_count.VisitsToday;
                                //st_count.UniqueVisitsToday = st_count.UniqueVisitsToday + vst.uniqueVisits_today;
                                st_count.VisitsTotal_Today = (vst.Visits_today > 0) ? (st_count.VisitsTotal_Today + vst.Visits_today) : st_count.VisitsTotal_Today;
                                st_count.RevisitsTotal_Today = (revisitcount>0) ? revisitcount :st_count.RevisitsTotal_Today;

                                st_count.UniqueVisits = (uniquevists > 0) ? (uniquevists) : (st_count.UniqueVisits);
                                st_count.UniqueVisitsToday = (uniquevistis_today > 0) ? (uniquevistis_today) : (st_count.UniqueVisitsToday);
                                st_count.TotalVisits = (vst.Visits_today > 0) ? (st_count.TotalVisits + vst.Visits_today) : st_count.TotalVisits;
                                //st_count.NoVisitsTotal_Today = (st_count.UniqueVisitsToday > 0) ? (st_count.UsersToday - st_count.UniqueVisitsToday) : st_count.NoVisitsTotal_Today;
                                st_count.NoVisitsTotal_Today = (st_count.UniqueVisitsToday > 0 && (st_count.NoVisitsTotal_Today != (st_count.UsersToday - st_count.UniqueVisitsToday))) ? (st_count.UsersToday - st_count.UniqueVisitsToday) : st_count.NoVisitsTotal_Today;
                                st_count.RevisitsPercent_Today = (st_count.RevisitsTotal_Yesterday > 0) ? ((st_count.RevisitsTotal_Today - st_count.RevisitsTotal_Yesterday) / (st_count.RevisitsTotal_Yesterday)) : 0;
                                st_count.NoVisitsPercent_Today = (st_count.NoVisitsTotal_Yesterday > 0) ? ((st_count.NoVisitsTotal_Today - st_count.NoVisitsTotal_Yesterday) / (st_count.NoVisitsTotal_Yesterday)) : 0;
                                st_count.UrlPercent_Today = (st_count.UsersYesterday > 0) ? ((st_count.UrlTotal_Today - st_count.UsersYesterday) / (st_count.UsersYesterday)) : 0;
                                //st_count.NoVisitsTotal_Today = (novisits == null) ? st_count.NoVisitsTotal_Today : novisits;
                                dc.SaveChanges();
                                
                            }
                        }

                    //for admin update
                        //StatsCounts_Today st_today = (from st in dc.stat_counts
                        //                                  .AsEnumerable()
                        //                              where(st.FK_Rid!=0)
                        //                              group st by st.FK_Rid into r
                        //                              select new StatsCounts_Today()
                        //                              {
                        //                                  //Visits_today = r.Sum(x => x.VisitsToday),
                        //                                  //VisitsTotal_Today = r.Sum(x => x.VisitsTotal_Today),
                        //                                  RevisitsTotal_Today = r.Sum(x => x.RevisitsTotal_Today),
                        //                                  UniqueVisits = r.Sum(x => x.UniqueVisits),
                        //                                  UniqueVisitsToday = r.Sum(x => x.UniqueVisitsToday),
                        //                                  //TotalVisits = r.Sum(x => x.TotalVisits),
                        //                                  NoVisitsTotal_Today = r.Sum(x => x.NoVisitsTotal_Today)
                        //                                  //RevisitsPercent_Today = r.Sum(x => x.RevisitsPercent_Today),
                        //                                  //NoVisitsPercent_Today = r.Sum(x => x.NoVisitsPercent_Today),
                        //                                  //UrlPercent_Today = r.Sum(x => x.UrlPercent_Today),

                        //                              }).SingleOrDefault();
                        //stat_counts st_count_admin = dc.stat_counts.Where(x => x.FK_Rid == 0).SingleOrDefault();
                        //if (st_count_admin != null)
                        //{

                        //    st_count_admin.VisitsToday = (lstvisits != null) ? (st_count_admin.VisitsToday + lstvisits.Select(x => x.Visits_today).Sum()) : st_count_admin.VisitsToday; ;
                        //    st_count_admin.VisitsTotal_Today = st_count_admin.VisitsToday;
                        //    st_count_admin.RevisitsTotal_Today = st_today.RevisitsTotal_Today;
                        //    st_count_admin.UniqueVisits = st_today.UniqueVisits;
                        //    st_count_admin.UniqueVisitsToday = st_today.UniqueVisitsToday;
                        //    //st_count_admin.UniqueVisits = (lstuniquevisits_tot != null) ? (st_count_admin.UniqueVisits + lstuniquevisits_tot.Select(x => x.uniquevists).Sum()) : st_count_admin.UniqueVisits;
                        //    //st_count_admin.UniqueVisitsToday = (lstvisits != null) ? (st_count_admin.UniqueVisitsToday + lstvisits.Select(x => x.uniqueVisits_today).Sum()) : st_count_admin.UniqueVisitsToday;

                        //    st_count_admin.TotalVisits = (lstvisits != null) ? (st_count_admin.TotalVisits + lstvisits.Select(x => x.Visits_today).Sum()) : st_count_admin.TotalVisits;
                        //    st_count_admin.NoVisitsTotal_Today = st_today.NoVisitsTotal_Today;
                        //    st_count_admin.VisitsPercent_Today = (st_count_admin.VisitsYesterday > 0) ? ((st_count_admin.VisitsTotal_Today - st_count_admin.VisitsYesterday) / (st_count_admin.VisitsYesterday)) : 0;
                        //    st_count_admin.RevisitsPercent_Today = (st_count_admin.RevisitsTotal_Yesterday > 0) ? ((st_count_admin.RevisitsTotal_Today - st_count_admin.RevisitsTotal_Yesterday) / (st_count_admin.RevisitsTotal_Yesterday)) : 0;
                        //    st_count_admin.NoVisitsPercent_Today = (st_count_admin.NoVisitsTotal_Yesterday > 0) ? ((st_count_admin.NoVisitsTotal_Today - st_count_admin.NoVisitsTotal_Yesterday) / (st_count_admin.NoVisitsTotal_Yesterday)) : 0;
                        //    dc.SaveChanges();

                        //}
                       List<stat_counts> st_count_admin_list = dc.stat_counts.Where(x => x.FK_Rid == 0).ToList();
                    
                    if (st_count_admin_list.Count > 0)
                        {
                            foreach (stat_counts st_count_admin in st_count_admin_list)
                            {

                                st_count_admin.VisitsToday = (lstvisits != null) ? (st_count_admin.VisitsToday + lstvisits.Where(x=>x.fk_clientid==st_count_admin.FK_ClientID).Select(x => x.Visits_today).Sum()) : st_count_admin.VisitsToday;
                                st_count_admin.VisitsTotal_Today = st_count_admin.VisitsToday;
                                st_count_admin.RevisitsTotal_Today = (lstvisits != null) ? (dc.stat_counts.Where(x => x.FK_Rid != 0 && x.FK_ClientID==st_count_admin.FK_ClientID ).Select(x => x.RevisitsTotal_Today).Sum()) : st_count_admin.RevisitsTotal_Today;
                                st_count_admin.UniqueVisits = (lstuniquevisits_tot != null) ? (dc.stat_counts.Where(x => x.FK_Rid != 0 && x.FK_ClientID == st_count_admin.FK_ClientID).Select(x => x.UniqueVisits).Sum()) : st_count_admin.UniqueVisits;
                                st_count_admin.UniqueVisitsToday = (lstvisits != null) ? (dc.stat_counts.Where(x => x.FK_Rid != 0 && x.FK_ClientID == st_count_admin.FK_ClientID).Select(x => x.UniqueVisitsToday).Sum()) : st_count_admin.UniqueVisitsToday;

                                //st_count_admin.UniqueVisits = (lstuniquevisits_tot != null) ? (st_count_admin.UniqueVisits + lstuniquevisits_tot.Select(x => x.uniquevists).Sum()) : st_count_admin.UniqueVisits;
                                //st_count_admin.UniqueVisitsToday = (lstvisits != null) ? (st_count_admin.UniqueVisitsToday + lstvisits.Select(x => x.uniqueVisits_today).Sum()) : st_count_admin.UniqueVisitsToday;

                                st_count_admin.TotalVisits = (lstvisits != null) ? (st_count_admin.TotalVisits + lstvisits.Where(x=>x.fk_clientid ==st_count_admin.FK_ClientID).Select(x => x.Visits_today).Sum()) : st_count_admin.TotalVisits;

                                st_count_admin.NoVisitsTotal_Today = (dc.stat_counts.Where(x => x.FK_Rid != 0 && x.FK_ClientID == st_count_admin.FK_ClientID).Select(x => x.NoVisitsTotal_Today).Sum());
                                st_count_admin.VisitsPercent_Today = (st_count_admin.VisitsYesterday > 0) ? ((st_count_admin.VisitsTotal_Today - st_count_admin.VisitsYesterday) / (st_count_admin.VisitsYesterday)) : 0;
                                st_count_admin.RevisitsPercent_Today = (st_count_admin.RevisitsTotal_Yesterday > 0) ? ((st_count_admin.RevisitsTotal_Today - st_count_admin.RevisitsTotal_Yesterday) / (st_count_admin.RevisitsTotal_Yesterday)) : 0;
                                st_count_admin.NoVisitsPercent_Today = (st_count_admin.NoVisitsTotal_Yesterday > 0) ? ((st_count_admin.NoVisitsTotal_Today - st_count_admin.NoVisitsTotal_Yesterday) / (st_count_admin.NoVisitsTotal_Yesterday)) : 0;
                                dc.SaveChanges();

                            }
                        }
                        //getnextshorturlid = dc.shorturldatas.Any() ? dc.shorturldatas.Max(x => x.PK_Shorturl) : 0;
                        //if (cntvisits > 0)
                        //    clickref.Ref_ShorturlClickID = getnextshorturlid;
                        
                        //dc.SaveChanges();
                    }
                //int cntusers =  dc.uiddatas.Where(x => x.PK_Uid > clickref.Ref_UsersID).ToList().Count();

                //if (cntvisits > 0 || cntusers > 0)
                //{

                //int getnextshorturlid = 0; int getnextuserid = 0;
                //List<StatsModel_visits> lstvisits = new List<StatsModel_visits>();
                //List<StatsModel_users> lstusers = new List<StatsModel_users>();
                //List<StatsModel_uniquevisits_Today> lstuniquevisits_tot_today = new List<StatsModel_uniquevisits_Today>();
                //List<StatsModel_uniquevisits> lstuniquevisits_tot = new List<StatsModel_uniquevisits>();
                //List<StatsModel_uniqueusers> lstuniqueusers_tot = new List<StatsModel_uniqueusers>();
                //List<StatsModel_uniqueusers_today> lstuniqueusers_tot_today = new List<StatsModel_uniqueusers_today>();
                //if (cntusers > 0)
                //{
                //    ErrorLogs.LogErrorData("Ref_ShorturlClickID = " + clickref.Ref_ShorturlClickID, "Ref_UsersID = " + clickref.Ref_UsersID);
                //    ErrorLogs.LogErrorData("cntvisits = " + cntvisits, "cntusers = " + cntusers);
                //    int refurserid = (int)clickref.Ref_UsersID;
                //    getnextuserid = refurserid + cntusers;

                //    clickref.Ref_UsersID = getnextuserid;
                //    dc.SaveChanges();
                //    lstusers = dc.uiddatas.AsEnumerable()
                //        .Where(x => x.PK_Uid > refurserid && x.CreatedDate.Value.Date == DateTime.UtcNow.Date)
                //        .GroupBy(x => x.FK_RID)
                //        .Select(res => new StatsModel_users()
                //        {
                //            Users_Today = res.Select(x => x.MobileNumber).Count(),
                //            UniqueUsers_Today = res.Select(x => x.MobileNumber).Distinct().ToList().Count(),
                //            fk_rid = res.Select(x => x.FK_RID).FirstOrDefault()
                //        }).ToList();

                //    lstuniqueusers_tot = (from s in dc.uiddatas
                //                                                .AsEnumerable()
                //                                join st in lstusers on s.FK_RID equals st.fk_rid
                //                                //where s.CreatedDate.Value.Date == DateTime.UtcNow.Date
                //                                group s by s.FK_RID into res
                //                                select new StatsModel_uniqueusers()
                //                                {
                //                                    fk_rid = res.Select(x => x.FK_RID).FirstOrDefault(),
                //                                    UniqueUsers = res.Select(x => x.MobileNumber).Distinct().ToList().Count()
                //                                }).ToList();
                //    foreach (StatsModel_users users in lstusers)
                //    {
                //        stat_counts st_count = new stat_counts(); int uniqueusers = 0; 
                //        uniqueusers = lstuniqueusers_tot.Where(x => x.fk_rid == users.fk_rid).Select(x => x.UniqueUsers).SingleOrDefault();
                //        //uniqueusers_today = lstuniqueusers_tot_today.Where(x => x.fk_rid == users.fk_rid).Select(x => x.UniqueUsers_Today).SingleOrDefault();
                //        st_count = dc.stat_counts.Where(x => x.FK_Rid == users.fk_rid).Select(y=>y).SingleOrDefault();
                //        if (st_count != null)
                //        {
                //            st_count.UsersToday = (users.Users_Today > 0) ? (st_count.UsersToday + users.Users_Today) : st_count.UsersToday;
                //            st_count.UrlTotal_Today = st_count.UsersToday;
                //            //st_count.UniqueUsersToday = (users.UniqueUsers_Today > 0) ? (st_count.UniqueUsersToday + users.UniqueUsers_Today) : st_count.UniqueUsersToday;
                //            st_count.UniqueUsers = (uniqueusers > 0) ? (uniqueusers) : st_count.UniqueUsers;
                //            st_count.UniqueUsersToday = (users.UniqueUsers_Today > 0) ? (st_count.UniqueUsersToday + users.UniqueUsers_Today)  : st_count.UniqueUsersToday;
                //            st_count.TotalUsers = (users.Users_Today > 0) ? (st_count.TotalUsers + users.Users_Today) : st_count.TotalUsers;
                //            st_count.UrlPercent_Today = (st_count.UsersYesterday == 0) ? 0 : ((st_count.UsersToday - st_count.UsersYesterday) / (st_count.UsersYesterday));
                //            if (cntvisits == 0)
                //                st_count.NoVisitsTotal_Today = st_count.TotalUsers - st_count.UniqueVisitsToday;
                //            dc.SaveChanges();
                //            //ErrorLogs.LogErrorData("unique users = " + st_count.UniqueUsers + "fk_rid = " + users.fk_rid +"total users = "+lstusers.Count(), "UniqueUsersToday = " + st_count.UniqueUsersToday.ToString() + "fk_rid = " + users.fk_rid);

                //        }
                //    }
                //    //for admin
                //    stat_counts st_count_admin = dc.stat_counts.Where(x => x.FK_Rid == 0).SingleOrDefault();
                //    if (st_count_admin != null)
                //    {
                //        //st_count_admin.UsersToday = (lstusers != null) ? (st_count_admin.UsersToday + lstusers.Select(x => x.Users_Today).Sum()) : st_count_admin.UsersToday;
                //        ////st_count_admin.UniqueUsers = (lstuniqueusers_tot != null) ? (lstuniqueusers_tot.Select(x => x.UniqueUsers).Sum()) : st_count_admin.UniqueUsers;
                //        //st_count_admin.UniqueUsers = (lstuniqueusers_tot != null) ? (dc.stat_counts.Where(x => x.FK_Rid != 0).Select(y => y.UniqueUsers).Sum()) : st_count_admin.UniqueUsers;
                //        //st_count_admin.UniqueUsersToday = (lstusers != null) ? (st_count_admin.UniqueUsersToday + lstusers.Select(y => y.UniqueUsers_Today).Sum()) : st_count_admin.UniqueUsersToday;
                //        //st_count_admin.TotalUsers = (lstusers != null) ? (st_count_admin.TotalUsers + lstusers.Select(x => x.Users_Today).Sum()) : st_count_admin.TotalUsers;
                //        //st_count_admin.UrlTotal_Today = st_count_admin.TotalUsers;

                //        st_count_admin.UsersToday = (lstusers != null) ? (st_count_admin.UsersToday + lstusers.Select(x => x.Users_Today).Sum()) : st_count_admin.UsersToday;
                //        //st_count_admin.UniqueUsers = (lstuniqueusers_tot != null) ? (lstuniqueusers_tot.Select(x => x.UniqueUsers).Sum()) : st_count_admin.UniqueUsers;
                //        st_count_admin.UniqueUsers = (lstuniqueusers_tot != null) ? (dc.stat_counts.Where(x => x.FK_Rid != 0).Select(y => y.UniqueUsers).Sum()) : st_count_admin.UniqueUsers;
                //        st_count_admin.UniqueUsersToday = (lstusers != null) ? (dc.stat_counts.Where(x => x.FK_Rid != 0).Select(y => y.UniqueUsersToday).Sum()) : st_count_admin.UniqueUsersToday;
                //        st_count_admin.TotalUsers = (lstusers != null) ? (st_count_admin.TotalUsers + lstusers.Select(x => x.Users_Today).Sum()) : st_count_admin.TotalUsers;
                //        st_count_admin.UrlTotal_Today = st_count_admin.UsersToday;
                //        st_count_admin.UrlPercent_Today = (st_count_admin.UsersYesterday == 0) ? 0 : ((st_count_admin.UsersToday - st_count_admin.UsersYesterday) / (st_count_admin.UsersYesterday));

                //        if (cntvisits == 0)
                //            st_count_admin.NoVisitsTotal_Today =  (st_count_admin.TotalUsers - st_count_admin.UniqueVisitsToday) ;
                //        dc.SaveChanges();
                //        //st_count_admin.UsersToday = (lstusers != null) ? (st_count_admin.UsersToday + lstusers.Select(x => x.Users_Today).Sum()) : st_count_admin.UsersToday;
                //        //st_count_admin.UniqueUsers = (lstuniqueusers_tot != null) ? (dc.stat_counts.Where(x => x.FK_Rid != 0).Select(y => y.UniqueUsers).Sum()) : st_count_admin.UniqueUsers;
                //        //st_count_admin.UniqueUsersToday = (lstusers != null) ? (st_count_admin.UniqueUsersToday + lstusers.Select(x => x.UniqueUsers_Today).Sum()) : st_count_admin.UniqueUsersToday;
                //        //st_count_admin.TotalUsers = (lstusers != null) ? (st_count_admin.TotalUsers + lstusers.Select(x => x.Users_Today).Sum()) : st_count_admin.TotalUsers;



                //        //ErrorLogs.LogErrorData("StatsToday_Service visitis today st_count_admin " + st_count_admin.VisitsToday, lstvisits.Select(x => x.Visits_today).Sum().ToString());
                //       }
                //    //getnextuserid = dc.uiddatas.Any() ? dc.uiddatas.Max(x => x.PK_Uid) : 0;
                //    //if (cntusers > 0)
                //    //    clickref.Ref_UsersID = getnextuserid;
                //    //dc.SaveChanges();
                //}

                //    stat_counts st_count = new stat_counts();     
                //st_count = dc.stat_counts.Where(x => x.FK_Rid == users.fk_rid).Select(y => y).SingleOrDefault();
                //stat_counts st_count_admin = dc.stat_counts.Where(x => x.FK_Rid == 0).SingleOrDefault();
                //if (cntvisits == 0)
                //        st_count_admin.NoVisitsTotal_Today = (st_count_admin.TotalUsers - st_count_admin.UniqueVisitsToday);
                //    dc.SaveChanges();   

                    
                }

            
            catch (Exception ex)
            {

                ErrorLogs.LogErrorData("StatsToday_Service" + ex.InnerException+ex.StackTrace, ex.Message);
                //shorturlclickreference clickref = dc.shorturlclickreferences.Select(x => x).FirstOrDefault();
                //int refvisitid = (int)clickref.Ref_ShorturlClickID;
                //int  getnextshorturlid = refvisitid + cntvisits;

                //clickref.Ref_ShorturlClickID = getnextshorturlid;
                //dc.SaveChanges();

            }
        }


      
    }
}