using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Web;
using System.IO;
using System.Net;
using System.Configuration;
using System.Diagnostics;
using Database.DAL;
using Oracle.ManagedDataAccess.Client;
namespace VATSolution
{
    public partial class Form1 : Form
    {
        public static string ESPCon = "";
        public static string CRMPCon = "";
        public static int ApplyVAT = 0;
        public static string ControlService = "0";
        public static string HybridOffers = "";
        public Form1()
        {
            InitializeComponent();
        }
        private void cmdsms_Click(object sender, EventArgs e)
        {
            Start();
        }
        public void Start()
        {
            try
            {
                VATBusiness.Common.Writelog = ConfigurationSettings.AppSettings["Writelog"].ToString() == "1" ? true : false;
                VATBusiness.Common.ESPCon = ConfigurationSettings.AppSettings["ESP"];
                VATBusiness.Common.CRMPCon = ConfigurationSettings.AppSettings["CRM"].ToString();
                VATBusiness.Common.FeeDedAPI = ConfigurationSettings.AppSettings["FeeDedAPI"].ToString();
                VATBusiness.Common.HybridOffers = ConfigurationManager.AppSettings["HybridOffers"];
                VATBusiness.Common.MySQLCon = ConfigurationManager.AppSettings["MySQLCon"];
                ControlService = ConfigurationManager.AppSettings["ControlService"];
                VATBusiness.Common.ApplyVAT = int.Parse(ConfigurationManager.AppSettings["ApplyVAT"].ToString());
                if (ControlService == "1")
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
        void MyLogEvent(string Msg)
        {
            MessageBox.Show(Msg);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            VATBusiness.Common.ProcessFailed();
            MessageBox.Show("done");
        }
    }
}
