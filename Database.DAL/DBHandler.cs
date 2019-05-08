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
using Oracle.ManagedDataAccess.Client;
namespace Database.DAL
{
    public class DBHandler : IDBHandler
    {
        public string ConnectionString = "";
        #region General Database
        public DBHandler(string Con)
        {
            ConnectionString = Con;
        }
        public DBHandler()
        {
        }
        public string ToDBString(string val)
        {
            string retval = string.Empty;
            if (val != null)
            {
                if (val.IndexOf("'") != -1)
                { retval = "" + val.Replace("'", "''") + ""; }
                else { retval = val; }
            }
            else { retval = null; }
            return retval;
        }
        public int ExecuteNonQuery(string SqlStatment)
        {
            OracleConnection Cn = new OracleConnection(ConnectionString);
            int result = -1;
            if (Cn.State != ConnectionState.Open)
                Cn.Open();
            try
            {
                OracleCommand Cmd = new OracleCommand(SqlStatment, Cn);
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
            OracleConnection Cn = new OracleConnection(ConnectionString);
            int result = -1;
            if (Cn.State != ConnectionState.Open)
                Cn.Open();
            OracleCommand command = Cn.CreateCommand();
            OracleTransaction transaction;
            // Start a local transaction
            transaction = Cn.BeginTransaction(IsolationLevel.ReadCommitted);
            // Assign transaction object for a pending local transaction
            command.Transaction = transaction;
            try
            {
                foreach (string sql in SqlStatment.Split(';'))
                {
                    command.CommandText = sql;
                    result = command.ExecuteNonQuery();
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
                OracleConnection.ClearPool(Cn);
            }
            return result;
        }
        public int ExecuteScalar(string SqlStatment)
        {
            OracleConnection Cn = new OracleConnection(ConnectionString);
            Cn.Open();
            object result;
            OracleCommand Cmd = new OracleCommand(SqlStatment, Cn);
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
                return - 1;
        }
        public DataTable ExecuteDataTable(string SqlStatment)
        {
            OracleConnection Cn = new OracleConnection(ConnectionString);
            try
            {
                if (Cn.State != ConnectionState.Open)
                    Cn.Open();
                OracleDataAdapter adtp = new OracleDataAdapter(SqlStatment, Cn);
                DataSet Ds = new DataSet();
                adtp.Fill(Ds);
                Cn.Close();
                return Ds.Tables[0];
            }
            catch (Exception e)
            {
                Cn.Close();
                throw e;
            }
            finally
            {
                Cn.Close();
            }
        }
        #endregion
    }
}
