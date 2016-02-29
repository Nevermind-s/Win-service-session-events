using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace WindowsService1
{
    public partial class UserMonitor : ServiceBase   // Class Extended from ServiceBase to perform Windows Services actions
    {
        private System.Timers.Timer serviceTimer = null; // 
        string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name; // Method used to get current user's name
        string date = DateTime.Now.ToString(); // Method used to get current time coverted to String format
        SessionChangeDescription lol = new SessionChangeDescription();
        private Thread mthread;

        public UserMonitor()
        {
            InitializeComponent(); // Initializing the component of the service specialy "CanHandleSessionChangeEvent" to true, (false if not modified) which is going listen to Session Event
      
        }

        protected override void OnStart(string[] args)
        {
            if (!System.Diagnostics.EventLog.SourceExists("UserMonitor"))                         // Creating EventLog if not exist 
                System.Diagnostics.EventLog.CreateEventSource("UserMonitor", "Application");

            this.LogEvent(String.Format("UserMonitor startsss on lol {0} {1}", System.DateTime.Now.ToString("dd-MMMyyyy"), // Log the launch of our Service
            DateTime.Now.ToString("hh:mm:ss tt")), EventLogEntryType.Information);
            this.serviceTimer = new System.Timers.Timer(100000);
            this.serviceTimer.AutoReset = true;
            this.serviceTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.timer_Elapsed);
            this.serviceTimer.Start();
            mthread = new Thread(DoWork);
            
        }


         private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
          
        }

        protected override void OnStop()  // Handle the endlife of the service by disposing it and loging the event
        {
            this.serviceTimer.Stop();
            this.serviceTimer.Dispose();
            this.serviceTimer = null;
            this.LogEvent(String.Format("MonitorService Stops on {0} {1}", System.DateTime.Now.ToString("dd-MMMyyyy"),
            DateTime.Now.ToString("hh:mm:ss tt")), EventLogEntryType.Information);
        }

        private void LogEvent(string message, EventLogEntryType entryType)  // Hadle the log entry of our service
        {
            System.Diagnostics.EventLog eventLog = new System.Diagnostics.EventLog();
            eventLog = new System.Diagnostics.EventLog();
            eventLog.Source = "UserMonitor";
            eventLog.Log = "Application";
            eventLog.WriteEntry(message, entryType);
        }
        private void DoWork()
        {
            OnSessionChange(lol);
        }
        protected override void OnSessionChange(SessionChangeDescription changeDescription) // The most important method of our service which gonna handle all the event of the current session
        {
           // String table = "dbo.sys_event"; 
            base.OnSessionChange(changeDescription);

            if (changeDescription.Reason == SessionChangeReason.SessionLock ||
                changeDescription.Reason == SessionChangeReason.SessionLogoff ||
                changeDescription.Reason == SessionChangeReason.ConsoleDisconnect)
            {
                String Session = "Session Off";
                this.LogEvent(String.Format("Session lock or logoff or disconnect", System.DateTime.Now.ToString("dd-MMMyyyy"),
                DateTime.Now.ToString("hh:mm:ss tt")), EventLogEntryType.Information);
            }
            else if (changeDescription.Reason == SessionChangeReason.SessionUnlock ||
               changeDescription.Reason == SessionChangeReason.SessionLogon ||
               changeDescription.Reason == SessionChangeReason.ConsoleConnect)
            {
                String Session = "Session On";
                this.LogEvent(String.Format("Session unlock or logon or connect", System.DateTime.Now.ToString("dd-MMMyyyy"),
                DateTime.Now.ToString("hh:mm:ss tt")), EventLogEntryType.Information);
            }
        }

    }

}
