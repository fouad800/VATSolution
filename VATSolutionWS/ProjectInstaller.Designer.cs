namespace VATSolutionWS
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
            this.VATSolutionWSSrv = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller1 = new System.ServiceProcess.ServiceInstaller();
            // 
            // VATSolutionWSSrv
            // 
            this.VATSolutionWSSrv.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.VATSolutionWSSrv.Password = null;
            this.VATSolutionWSSrv.Username = null;
            // 
            // serviceInstaller1
            // 
            this.serviceInstaller1.Description = "This Process for VAT production- contact Fouad Abdel azeem ";
            this.serviceInstaller1.ServiceName = "VATSolutionWS";
            this.serviceInstaller1.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceInstaller1_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.VATSolutionWSSrv,
            this.serviceInstaller1});
        }
        #endregion
        private System.ServiceProcess.ServiceProcessInstaller VATSolutionWSSrv;
        private System.ServiceProcess.ServiceInstaller serviceInstaller1;
    }
}