using System;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Net;
using System.IO;
using System.Xml.Linq;
using Database.DAL;
namespace VATBusiness
{
    public class Common
    {
        public static string FeeDedAPI = ConfigurationSettings.AppSettings["FeeDedAPI"].ToString();//Testing is the default
        public static string ESPCon = "";
        public static string CRMPCon = "";
        public static string MySQLCon = "";
        public static int ApplyVAT = 0;
        public static string HybridOffers = "";
        public static bool Writelog = true;
        public static List<string> ParseSOAPResponse(string soapResult, string NodeNameSpace, string NodeName)
        {
            TextReader tr = new StringReader(soapResult);
            XDocument doc = XDocument.Load(tr);
            IEnumerable<XElement> xNames;
            XNamespace ns = NodeNameSpace;
            xNames = doc.Descendants(ns + NodeName);
            List<string> list = new List<string>();
            list= xNames.Select(item => item.ToString()).ToList();
            return list;
        }
        public static string FeeDeduct(string MSISDN,string FeeAmount)
        {
            string soapResult = "";
            try
            {
                string xmlPayload = "<S:Envelope xmlns:S=\"http://schemas.xmlsoap.org/soap/envelope/\">" +
                            "<S:Body>" +
                            "<ns2:FeeDeductionRequestMsg xmlns:ns2=\"http://www.huawei.com/bme/cbsinterface/bcservices\" xmlns:ns4=\"http://www.huawei.com/bme/cbsinterface/bccommon\" xmlns:ns3=\"http://www.huawei.com/bme/cbsinterface/cbscommon\">" +
                            "<RequestHeader>" +
                            "<ns3:Version>1.1</ns3:Version>" +
                            "<ns3:MessageSeq>" + DateTime.Now.ToString("yyyyMMddhhmmssfffff") + "</ns3:MessageSeq>" +
                            "<ns3:AccessSecurity>" +
                            "<ns3:LoginSystemCode>VAT</ns3:LoginSystemCode>" +
                            "<ns3:Password>Abc1234%</ns3:Password>" +
                            "</ns3:AccessSecurity>" +
                            "</RequestHeader>" +
                            "<FeeDeductionRequest>" +
                            "<ns2:Request_No>" + DateTime.Now.ToString("yyyMddhhmmssfffff") + "</ns2:Request_No>" +
                            "<ns2:MSISDN>"+ MSISDN+"</ns2:MSISDN>" +
                            "<ns2:amount>"+ FeeAmount + "</ns2:amount>" +
                            "</FeeDeductionRequest>" +
                            "</ns2:FeeDeductionRequestMsg>" +
                            "</S:Body>" +
                            "</S:Envelope>";
                string URL_ADDRESS = FeeDedAPI;
                string action = "FeeDed";
                HttpWebRequest request = WebRequest.Create(new Uri(URL_ADDRESS)) as HttpWebRequest;
                // Set type to POST
                request.Method = "POST";
                request.Headers.Add("SOAPAction", action);
                StringBuilder data = new StringBuilder();
                data.Append(xmlPayload);
                byte[] byteData = Encoding.UTF8.GetBytes(data.ToString());      // Create a byte array of the data we want to send
                request.ContentLength = byteData.Length;
                // Set the content length in the request headers
                using (Stream postStream = request.GetRequestStream())
                {
                    postStream.Write(byteData, 0, byteData.Length);
                }
                // Get response and return it
                XmlDocument xmlResult = new XmlDocument();
                IAsyncResult asyncResult = request.BeginGetResponse(null, null);
                asyncResult.AsyncWaitHandle.WaitOne();
                using (WebResponse webResponse = request.EndGetResponse(asyncResult))
                {
                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                    }
                }
            }
            catch (Exception e)
            {
                MyLogEvent(e);
            }
            return soapResult;
        }
        public static  void ProcessFailed()
        {
           try
            {
                VAT_SHOP_ECARE Vobj = new VAT_SHOP_ECARE();
                DataTable dt = new Database.DAL.VAT_SHOP_ECAREList(ConfigurationSettings.AppSettings["MySQLCon"]).GetDataTable("status<>0 or ResultDesc='under processing'");
                foreach (DataRow dr in dt.Rows)
                {
                    string VAT = dr["VAT_amount"].ToString();
                    if (VAT == "" || VAT == null)
                    {
                        VAT = GetVaT(dr["RECHARGE_AMOUNT"].ToString());
                    }
                    else
                        VAT = dr["VAT_amount"].ToString();
                    string result = VATBusiness.Common.FeeDeduct(dr["MSISDN"].ToString(), VAT);
                    Vobj.STATUS = int.Parse(GetValueFromXmlTag(result, "cbs:ResultCode"));
                    Vobj.ResultDesc = GetValueFromXmlTag(result, "cbs:ResultDesc");
                    if (Vobj.STATUS.Equals(0))
                    {
                        //VATBusiness.Common.SendSMS("تم خصم ضريبه القيمه المضافه  \nVAT Tax Applied is " + Math.Round(decimal.Parse(dr["VAT_amount"].ToString()) / 10000, 2), dr["MSISDN"].ToString());
                    }
                    new Database.DAL.VAT_SHOP_ECAREList(ConfigurationSettings.AppSettings["MySQLCon"]).Update("Update VAT_SHOP_ECARE set status=" + Vobj.STATUS + ",VAT_amount="+ VAT + ",ResultDesc='" + Vobj.ResultDesc + "' where SEQ_ID=" + dr["SEQ_ID"].ToString());
                }
            }
            catch (Exception e)
            {
                MyLogEvent(e);
            }
        }
        public static string BinaryToText(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }
        public static string getXMLTagValue(string TAG)
        {
            return (TAG.Substring(TAG.IndexOf(">") + 1, TAG.IndexOf("</") - TAG.IndexOf(">") - 1));
        }
        public static string getXMLTagValue(string TAG, string tagName)
        {
            string endtag = tagName.Replace("<", "</");
            return (TAG.Substring(TAG.IndexOf(tagName) + tagName.Length, TAG.IndexOf(endtag) - TAG.IndexOf(tagName) - endtag.Length + 1));
        }
        private static string GetValueFromXmlTag(string xml, string tag)
        {
            if (xml == null || tag == null || xml.Length == 0 || tag.Length == 0)
                return "";
            string
                startTag = "<" + tag + ">",
                endTag = "</" + tag + ">",
                value = null;
            int
                startTagIndex = xml.IndexOf(tag, StringComparison.OrdinalIgnoreCase),
                endTagIndex = xml.IndexOf(endTag, StringComparison.OrdinalIgnoreCase);
            if (startTagIndex < 0 || endTagIndex < 0)
                return "";
            int valueIndex = startTagIndex += startTag.Length - 1;
            try
            {
                value = xml.Substring(valueIndex, endTagIndex - valueIndex);
            }
            catch (ArgumentOutOfRangeException)
            {
                string err = string.Format("Error reading value for \"{0}\" tag from XXX XML", tag);
                //log.Error(err, responseXmlParserEx);
            }
            return (value ?? "");
        }
        public static void MyLogEvent(Exception ex)
        {
            try
            {
                if (Writelog)
                    if (!EventLog.SourceExists("VATSolution"))
                        EventLog.CreateEventSource("VATSolution", "VATSolution");
                LogException(ex, ex.Source);
                EventLog.WriteEntry("VATSolution", ex.Message);
                //SendSMS("VATSolution  -" + ex.Message, "577773380");
            }
            catch (Exception)
            {
            }
        }
        public static void LogException(Exception exc, string source)
        {
            // Include enterprise logic for logging exceptions 
            // Get the absolute path to the log file 
            string logFile = "ErrorLog" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
            // Open the log file for append and write the log
            StreamWriter sw = new StreamWriter(logFile, true);
            sw.WriteLine("********** {0} **********", DateTime.Now);
            if (exc.InnerException != null)
            {
                sw.Write("Inner Exception Type: ");
                sw.WriteLine(exc.InnerException.GetType().ToString());
                sw.Write("Inner Exception: ");
                sw.WriteLine(exc.InnerException.Message);
                sw.Write("Inner Source: ");
                sw.WriteLine(exc.InnerException.Source);
                if (exc.InnerException.StackTrace != null)
                {
                    sw.WriteLine("Inner Stack Trace: ");
                    sw.WriteLine(exc.InnerException.StackTrace);
                }
            }
            sw.Write("Exception Type: ");
            sw.WriteLine(exc.GetType().ToString());
            sw.WriteLine("Exception: " + exc.Message);
            sw.WriteLine("Source: " + source);
            sw.WriteLine("Stack Trace: ");
            if (exc.StackTrace != null)
            {
                sw.WriteLine(exc.StackTrace);
                sw.WriteLine();
            }
            sw.Close();
        }
        public static void SendSMS(string content, string receiptno)
            {
                try
                {
                    string xmlPayload = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\""
                                                        + " xmlns:util=\"http://www.huawei.com/bss/soaif/interface/UtilityService/\" "
                                                        + " xmlns:com=\"http://www.huawei.com/bss/soaif/interface/common/\">"
                                               + "<soapenv:Header/>"
                                               + "<soapenv:Body>"
                                                  + "<util:SendSMSReqMsg>"
                                                     + "<com:ReqHeader>"
                                                        + "<!--Optional:-->"
                                                        + "<com:Version>1</com:Version>"
                                                        + "<com:Channel>82</com:Channel>"
                                                        + "<!--Optional:-->"
                                                        + "<com:BrandId>101</com:BrandId>"
                                                        + "<com:ReqTime>"+ DateTime.Now.ToString("yyzMMddhhmmssfffff") + "</com:ReqTime>"
                                                        + "<com:AccessUser>LebaraMobile</com:AccessUser>"
                                                        + "<com:AccessPassword>yWJLEODB1+RwJXKe8DFAXToB7wgYyFc6yxflNUz8TYI=</com:AccessPassword>"
                                                        + "<!--Optional:-->"
                                                        + "<com:OperatorId>101</com:OperatorId>"
                                                     + "</com:ReqHeader>"
                                                     + "<!--1 to 100 repetitions:-->"
                                                     + "<util:SMSInfo>"
                                                        + "<util:BatchSeqId>1111</util:BatchSeqId>"
                                                        + "<util:Content>" + content + "</util:Content>"
                                                        + "<util:DestinationNum>" + receiptno + "</util:DestinationNum>"
                                                        + "<!--Optional:-->"
                                                        + "<util:SourceNum>1755</util:SourceNum>"
                                                     + "</util:SMSInfo>"
                                                  + "</util:SendSMSReqMsg>"
                                               + "</soapenv:Body>"
                                            + "</soapenv:Envelope>";
                    string URL_ADDRESS = "http://10.200.102.83:7081/SELFCARE/HWBSS_Utility";
                    string action = "SendSMS";
                    HttpWebRequest request = WebRequest.Create(new Uri(URL_ADDRESS)) as HttpWebRequest;
                    // Set type to POST
                    request.Method = "POST";
                    request.Headers.Add("SOAPAction", action);
                    StringBuilder data = new StringBuilder();
                    data.Append(xmlPayload);
                    byte[] byteData = Encoding.UTF8.GetBytes(data.ToString());      // Create a byte array of the data we want to send
                    request.ContentLength = byteData.Length;
                    // Set the content length in the request headers
                    using (Stream postStream = request.GetRequestStream())
                    {
                        postStream.Write(byteData, 0, byteData.Length);
                    }
                    // Get response and return it
                    XmlDocument xmlResult = new XmlDocument();
                    IAsyncResult asyncResult = request.BeginGetResponse(null, null);
                    asyncResult.AsyncWaitHandle.WaitOne();
                    string soapResult;
                    using (WebResponse webResponse = request.EndGetResponse(asyncResult))
                    {
                        using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                        {
                            soapResult = rd.ReadToEnd();
                        }
                    }
                }
                catch (Exception e)
                {
                    MyLogEvent(e);
                }
            }
        public void Download()
        {
            try
            {
                Database.DAL.DBHandler DHOracle = new DBHandler(ESPCon);
                VAT_SHOP_ECAREList ObjLst = new VAT_SHOP_ECAREList(MySQLCon);
                //string MAX_SEQ_ID = ObjLst.GetMaxseq_Id();
              string DateYYYYMMDD = DateTime.Now.ToString("yyyyMMdd");
                string SQL = "";
                //if (MAX_SEQ_ID == "")
                SQL = @"select  t1.req_message,t.channel,t.REQ_TIMESTAMP,t.ESB_TRACKING_ID,t.SEQ_ID,t.PARTITION_ID from esb.esb_cle_log_ex t , esb.esb_cle_message_ex t1
                        where  t.seq_id like '"+ DateYYYYMMDD + @"%'  and t.DESTINATION = 'CBS'
                        and t.service_name = 'Payment' and result_code=0 and channel is null
                        and t1.seq_id=t.seq_id
                        and dbms_lob.instr(t1.req_message , utl_raw.CAST_TO_RAW('CardPinNumber'), 1, 1) <= 0 ";
                DataTable DTR = DHOracle.ExecuteDataTable(SQL);
                foreach (DataRow dr in DTR.Rows)
                {
                    VAT_SHOP_ECARE Vobj = new VAT_SHOP_ECARE();
                    if (ObjLst.GetDataTable("seq_Id=" + dr["seq_Id"]).Rows.Count == 0)
                    {
                        string RequestXMLcontent = VATBusiness.Common.BinaryToText((byte[])dr["req_message"]);
                        List<string> CashString = VATBusiness.Common.ParseSOAPResponse(RequestXMLcontent, "http://www.huawei.com/bme/cbsinterface/arservices", "CashPayment");
                        if (CashString.Count > 0)
                        {
                            string CashPaymentTAG = CashString[0];
                            string CashPaymentVal = VATBusiness.Common.getXMLTagValue(CashPaymentTAG, "<ars:Amount>");
                            if (VATBusiness.Common.ParseSOAPResponse(RequestXMLcontent, "http://cbs.huawei.com/ar/wsservice/arcommon", "PrimaryIdentity") .Count!= 0)
                            {
                                string MSISDNTAG = VATBusiness.Common.ParseSOAPResponse(RequestXMLcontent, "http://cbs.huawei.com/ar/wsservice/arcommon", "PrimaryIdentity")[0];
                                string MSISDN = VATBusiness.Common.getXMLTagValue(MSISDNTAG);
                                decimal vatRatio = 5;
                                decimal VatPrec = 1.05m;
                                decimal vatAmount = decimal.Parse(GetVaT(CashPaymentVal)); //Math.Round(((decimal.Parse(CashPaymentVal) * vatRatio / 100) / VatPrec), 0);
                                string offerName = GetOfferName(MSISDN);
                                if (vatAmount == 0)
                                { vatAmount = Math.Round(((decimal.Parse(CashPaymentVal) * vatRatio / 100) / VatPrec), 0); }
                                if (!offerName.Equals(""))
                                {
                                    Vobj.STATUS = -1;
                                    Vobj.ResultDesc = "Under processing";
                                    Vobj.MSISDN = MSISDN;
                                    Vobj.OFFER_NAME = offerName;
                                    Vobj.RECHARGE_AMOUNT = int.Parse(CashPaymentVal);
                                    Vobj.REQ_XML = RequestXMLcontent;
                                    Vobj.SEQ_ID = dr["SEQ_ID"].ToString();
                                    Vobj.CHANNEL = dr["CHANNEL"] == null ? "" : dr["CHANNEL"].ToString();
                                    Vobj.Partition_ID = dr["partition_id"].ToString();
                                    Vobj.TRACK_ID = dr["ESB_TRACKING_ID"].ToString();
                                    Vobj.TRANSDATE = dr["REQ_TIMESTAMP"].ToString();
                                    Vobj.VAT_AMOUNT = vatAmount;
                                    ObjLst.Add(Vobj);
                                }
                            }
                        }
                    }
                }
                ProcessFailed(); // process failed records
            }
            catch (Exception e)
            {
                MyLogEvent(e);
            }
        }
        public static string GetVaT(string RechargeAmount)
        {
            string VATAmount = "0";
           switch (RechargeAmount)
                {
                case "50000":
                    VATAmount = "2378"; break;
                case "100000":
                    VATAmount = "4756"; break;
                case "200000":
                    VATAmount = "9512"; break;
                case "300000":
                    VATAmount = "14268"; break;
                case "500000":
                    VATAmount = "23780"; break;
                case "1000000":
                    VATAmount = "47559"; break;
                case "2000000":
                    VATAmount = "95118"; break;
               /*********************************************/
                case "110000":
                    VATAmount = "5238"; break;
                case "150000":
                    VATAmount = "7142"; break;
                case "210000":
                    VATAmount = "10000"; break;
                case "530000":
                    VATAmount = "25238"; break;
                case "1050000":
                    VATAmount = "50000"; break;
                case "2100000":
                    VATAmount = "100000"; break;
            }
            return VATAmount;
        }
        public string GetOfferName(string MSISDN)
        {
            string OfferName = "";
            try
            {
                string SQL = @"select b.offer_name from CCARE.INF_SUBSCRIBER_ALL a ,CCARE.PDM_OFFER b,CCARE.INF_OFFERS c
                        where a.sub_id = c.sub_id and c.offer_ID = b.OFFER_ID AND A.msisdn = '" + MSISDN + @"' AND A.SUB_STATE = 'B01' and b.primary_flag = 1
                        and b.offer_id in (" + HybridOffers + @")";
                DBHandler Dh = new DBHandler(CRMPCon);
                DataTable dt = Dh.ExecuteDataTable(SQL);
                if (dt.Rows.Count > 0)
                    OfferName = dt.Rows[0][0].ToString();
            }
            catch (Exception e)
            {
                MyLogEvent(e);
            }
            return OfferName;
        }
    }
}
