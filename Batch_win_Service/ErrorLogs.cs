using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsToday_Service
{
    class ErrorLogs
    {
        public static void LogErrorData(string stackTraceInfo, string message)
        {
            try
            {
                //string message = "";
                //if (exp.InnerException == null)
                //    message = exp.Message;
                //else
                //    message = exp.InnerException.ToString();

                string utcdatetime = DateTime.UtcNow.Year + "-" + DateTime.UtcNow.Month + "-" + DateTime.UtcNow.Day + " " + DateTime.UtcNow.TimeOfDay;
                DateTime? utcdt = Convert.ToDateTime(utcdatetime);
                shortenurlEntities dc = new shortenurlEntities();
                errorlog objErrorLog = new errorlog();
                objErrorLog.StackTrace = stackTraceInfo;
                objErrorLog.ErrorMessage = message;
                objErrorLog.DateCreated = utcdt;
                dc.errorlogs.Add(objErrorLog);
                dc.SaveChanges();
                string err = "";
                err = "Error Occured on:" + DateTime.Now.ToString() + "<br>";
                err += "Message: " + message;
                err += "Stack Trace:<br>" + stackTraceInfo;
                //EmailUtil.SendMail("yasodha.bitra@gmail.com", ConfigurationManager.AppSettings["Error Mail"].ToString(), "", "", "Error ShortenURL", " <br>Message : " + err);

            }
            catch (Exception ex)
            {

            }
        }
    }
}
