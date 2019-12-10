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
            this.StatsToday_Service = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller_StatsToday_Service = new System.ServiceProcess.ServiceInstaller();
            // 
            // StatsToday_Service
            // 
            this.StatsToday_Service.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.StatsToday_Service.Password = null;
            this.StatsToday_Service.Username = null;
            this.StatsToday_Service.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceProcessInstaller_StatsToday_Service_AfterInstall);
            // 
            // serviceInstaller_StatsToday_Service
            // 
            this.serviceInstaller_StatsToday_Service.DisplayName = "MySqlStatsToday_Service";
            this.serviceInstaller_StatsToday_Service.ServiceName = "MySqlStatsToday_Service";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.StatsToday_Service,
            this.serviceInstaller_StatsToday_Service});

        }

        #endregion

        private System.ServiceProcess.ServiceInstaller serviceInstaller_StatsToday_Service;
        public System.ServiceProcess.ServiceProcessInstaller StatsToday_Service;
    }
}