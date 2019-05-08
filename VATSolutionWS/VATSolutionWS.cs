using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using Database.DAL;
using System.Net;
using System.Xml;
using System.Configuration;
namespace VATSolutionWS
{
    public partial class srvCdrHist : ServiceBase
    {
        public srvCdrHist()
        {
            InitializeComponent();
        }
        public static string ESPCon = "";
        public static string CRMPCon = "";
        public static int ApplyVAT = 0;
        public static string ControlService = "0";
        public static string HybridOffers = "";
        protected override void OnStart(string[] args)
        {
            this.VATSolutionWSTimer.Enabled = true;
            Intialize();
        }
        public void Intialize()
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                Start();
            }
            catch (Exception ex)
            {
                MyLogEvent(ex.Message);
            }
        }
        protected override void OnStop()
        {
            this.VATSolutionWSTimer.Enabled = false;
        }
        protected override void OnContinue()
        {
            Start();
        }
        private void VATSolutionWSTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Start();
        }
        public void Start()
        {
            try
            {
                VATBusiness.Common.Writelog=ConfigurationSettings.AppSettings["Writelog"].ToString() == "1" ? true : false;
                VATBusiness.Common.ESPCon = ConfigurationSettings.AppSettings["ESP"];
                VATBusiness.Common.CRMPCon = ConfigurationSettings.AppSettings["CRM"].ToString();
                VATBusiness.Common.FeeDedAPI = ConfigurationSettings.AppSettings["FeeDedAPI"].ToString();
                VATBusiness.Common.HybridOffers = ConfigurationManager.AppSettings["HybridOffers"];
                VATBusiness.Common.MySQLCon = ConfigurationManager.AppSettings["MySQLCon"];
                ControlService =ConfigurationManager.AppSettings["ControlService"];
                VATBusiness.Common.ApplyVAT = int.Parse(ConfigurationManager.AppSettings["ApplyVAT"].ToString());
                this.VATSolutionWSTimer.Interval = double.Parse(ConfigurationManager.AppSettings["TimeBand"]);
                if(ControlService=="1")
                {
                    VATBusiness.Common MyCo = new VATBusiness.Common();
                    MyCo.Download();
                }
            }
            catch (Exception ex)
            {
                MyLogEvent(ex.Message);
            }
        }
        private static void MyLogEvent(string Message)
        {
            try
            {
                bool flag = ConfigurationManager.AppSettings["Writelog"].ToString() == "1" ? true : false;
                if (flag)
                    if (!EventLog.SourceExists("VATSolutionWS"))
                        EventLog.CreateEventSource("VATSolutionWS", "VATSolutionWS");
                EventLog.WriteEntry("VATSolutionWS", Message);
            }
            catch (Exception)
            {
            }
        }
    }
}
