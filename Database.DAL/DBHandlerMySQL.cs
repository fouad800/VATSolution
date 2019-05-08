using System;
using System.Configuration;
using System.Data;
using MySql.Data;
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
namespace Database.DAL
{
    public class DBHandlerMySQL : IDBHandler
    {
        public string ConnectionString = "";
        #region General Database
        public DBHandlerMySQL(string Con)
        {
           ConnectionString = Con;
        }
        public DBHandlerMySQL()
        {
        }
        public string ToDBString(string val)
        {
            string retval = string.Empty;
            if (val != null)
            {
                if (val.IndexOf("'") != -1)
                { retval = "" + val.Replace("'", "''") + ""; }
                else { retval = "'" + val + "'"; } //So it will add ‘ ‘ with value. before it was giving syntax error exception.
            }
            else { retval = null; }
            return retval;
        }
        public int ExecuteNonQuery(string SqlStatment)
        {
            MySql.Data.MySqlClient.MySqlConnection Cn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
            int result = -1;
            if (Cn.State != ConnectionState.Open)
                Cn.Open();
            try
            {
                MySql.Data.MySqlClient.MySqlCommand Cmd = new MySql.Data.MySqlClient.MySqlCommand(SqlStatment, Cn);
                result = Cmd.ExecuteNonQuery(); 
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                Cn.Close();
            }
            return result;
        }
        public int ExecuteBulk(string SqlStatment)
        {
            MySql.Data.MySqlClient.MySqlConnection Cn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
            int result = -1;
            if (Cn.State != ConnectionState.Open)
                Cn.Open();
            MySql.Data.MySqlClient.MySqlCommand command = Cn.CreateCommand();
            MySql.Data.MySqlClient.MySqlTransaction transaction;
            // Start a local transaction
            transaction = Cn.BeginTransaction(IsolationLevel.ReadCommitted);
            // Assign transaction object for a pending local transaction
            command.Transaction = transaction;
            try
            {
                foreach (string sql in SqlStatment.Split(';'))
                {
                    if (!String.IsNullOrEmpty(sql))
                    {
                        command.CommandText = sql;
                        result = command.ExecuteNonQuery();
                    }
                }
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback(); throw e;
            }
            finally
            {
                Cn.Close();
                Cn.Dispose();
                MySql.Data.MySqlClient.MySqlConnection.ClearPool(Cn);
            }
            return result;
        }
        public int ExecuteScalar(string SqlStatment)
        {
            MySql.Data.MySqlClient.MySqlConnection Cn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
            Cn.Open();
            object result;
            MySql.Data.MySqlClient.MySqlCommand Cmd = new MySql.Data.MySqlClient.MySqlCommand(SqlStatment, Cn);
            try
            {
                result = Cmd.ExecuteScalar();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                Cn.Close();
            }
            if (result != null && result.ToString() != "")
                return int.Parse(result.ToString());
            else
                return -1;
        }
        public DataTable ExecuteDataTable(string SqlStatment)
        {
            MySql.Data.MySqlClient.MySqlConnection Cn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
            try
            {
                if (Cn.State != ConnectionState.Open)
                    Cn.Open();
                MySql.Data.MySqlClient.MySqlDataAdapter adtp = new MySql.Data.MySqlClient.MySqlDataAdapter(SqlStatment, Cn);
                DataSet Ds = new DataSet();
                adtp.Fill(Ds);
                Cn.Close();
                return Ds.Tables[0];
            }
            catch (Exception ex)
            {
                Cn.Close();
                throw ex;
            }
            finally
            {
                Cn.Close();
            }
        }
        public static void SendSMS(string content, string receiptnos)
        {
             foreach (string receiptno in receiptnos.Split(','))
                {
                    if (receiptno != "")
                    {
                        //System.Threading.Thread.Sleep(2000);
                        Mythread MyT = new Mythread();
                        MyT.content = content;
                        MyT.receiptnos = receiptno;
                        System.Threading.Thread m_WorkerThread = new System.Threading.Thread(new System.Threading.ThreadStart(MyT.WorkdingThread));
                        m_WorkerThread.Start();
                    }
                }
        }
        class Mythread
        {
            public string content; 
            public string receiptnos;
            public void WorkdingThread()
            {
                SendSMS(content,receiptnos);
            }
            private void SendSMS(string content, string receiptno)
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
                                                        + "<com:ReqTime>20160509205729</com:ReqTime>"
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
                       throw e;
                    }
                }
        }
        #endregion
    }
}
