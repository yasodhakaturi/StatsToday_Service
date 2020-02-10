namespace StatsToday_Service
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MySqlStatsToday_Service = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller_StatsToday_Service = new System.ServiceProcess.ServiceInstaller();
            // 
            // MySqlStatsToday_Service
            // 
            this.MySqlStatsToday_Service.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.MySqlStatsToday_Service.Password = null;
            this.MySqlStatsToday_Service.Username = null;
            this.MySqlStatsToday_Service.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceProcessInstaller_StatsToday_Service_AfterInstall);
            // 
            // serviceInstaller_StatsToday_Service
            // 
            this.serviceInstaller_StatsToday_Service.DisplayName = "MySqlStatsToday_Service";
            this.serviceInstaller_StatsToday_Service.ServiceName = "MySqlStatsToday_Service";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.MySqlStatsToday_Service,
            this.serviceInstaller_StatsToday_Service});

        }

        #endregion

        private System.ServiceProcess.ServiceInstaller serviceInstaller_StatsToday_Service;
        public System.ServiceProcess.ServiceProcessInstaller MySqlStatsToday_Service;
    }
}