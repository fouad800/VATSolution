using System;
using System.Data;
using System.Collections;
namespace Database.DAL
{
public class VAT_SHOP_ECAREList
{
	/// <summary>
	/// Default Contructor
	/// <summary>
	public VAT_SHOP_ECAREList(string ConnectionStr)
	{
             DH = new DBHandlerMySQL(ConnectionStr);
    }
	public  string strFieldsToSelect="*";
	public  string strTableName="VAT_SHOP_ECARE";
	public DBHandlerMySQL DH=null;
	public  ArrayList GetList(string Condition)
	{
		ArrayList arrVAT_SHOP_ECAREList =  new ArrayList();
		VAT_SHOP_ECARE VAT_SHOP_ECAREObj;
		try
		{
			DataTable dtVAT_SHOP_ECARE = DH.ExecuteDataTable("select " + strFieldsToSelect + " from " + strTableName+" Where "+Condition);
			foreach(DataRow dr in dtVAT_SHOP_ECARE.Rows)
			{
				VAT_SHOP_ECAREObj = new VAT_SHOP_ECARE();
				if(dr["REQ_XML"] != DBNull.Value)
					{
						VAT_SHOP_ECAREObj.REQ_XML=dr["REQ_XML"].ToString();
					}
				if(dr["SEQ_ID"] != DBNull.Value)
					{
						VAT_SHOP_ECAREObj.SEQ_ID=dr["SEQ_ID"].ToString();
					}
				if(dr["OFFER_NAME"] != DBNull.Value)
					{
						VAT_SHOP_ECAREObj.OFFER_NAME=dr["OFFER_NAME"].ToString();
					}
				if(dr["STATUS"] != DBNull.Value)
					{
						VAT_SHOP_ECAREObj.STATUS=int.Parse(dr["STATUS"].ToString());
					}
				if(dr["CHANNEL"] != DBNull.Value)
					{
						VAT_SHOP_ECAREObj.CHANNEL=dr["CHANNEL"].ToString();
					}
				if(dr["VAT_AMOUNT"] != DBNull.Value)
					{
						VAT_SHOP_ECAREObj.VAT_AMOUNT=decimal.Parse(dr["VAT_AMOUNT"].ToString());
					}
				if(dr["RECHARGE_AMOUNT"] != DBNull.Value)
					{
						VAT_SHOP_ECAREObj.RECHARGE_AMOUNT=int.Parse(dr["RECHARGE_AMOUNT"].ToString());
					}
				if(dr["MSISDN"] != DBNull.Value)
					{
						VAT_SHOP_ECAREObj.MSISDN=dr["MSISDN"].ToString();
					}
				if(dr["TRANSDATE"] != DBNull.Value)
					{
						VAT_SHOP_ECAREObj.TRANSDATE=dr["TRANSDATE"].ToString();
					}
                    if (dr["ResultDesc"] != DBNull.Value)
                    {
                        VAT_SHOP_ECAREObj.ResultDesc = dr["ResultDesc"].ToString();
                    }
                    if (dr["TRACK_ID"] != DBNull.Value)
					{
						VAT_SHOP_ECAREObj.TRACK_ID=dr["TRACK_ID"].ToString();
					}
				arrVAT_SHOP_ECAREList.Add(VAT_SHOP_ECAREObj);
			}
		}
		catch(Exception ex)
		{
			throw ex;
		}
		return arrVAT_SHOP_ECAREList;
	}
	public  VAT_SHOP_ECARE Get(string Condition)
	{
		VAT_SHOP_ECARE VAT_SHOP_ECAREObj = new VAT_SHOP_ECARE();
		try
		{
			DataTable dtVAT_SHOP_ECARE = DH.ExecuteDataTable("select " + strFieldsToSelect + " from " + strTableName+" Where "+Condition);
			if(dtVAT_SHOP_ECARE.Rows.Count > 0)
			{
				DataRow dr = dtVAT_SHOP_ECARE.Rows[0];
				if(dr["REQ_XML"] != DBNull.Value)
					{
						VAT_SHOP_ECAREObj.REQ_XML=dr["REQ_XML"].ToString();
					}
				if(dr["SEQ_ID"] != DBNull.Value)
					{
						VAT_SHOP_ECAREObj.SEQ_ID=dr["SEQ_ID"].ToString();
					}
				if(dr["OFFER_NAME"] != DBNull.Value)
					{
						VAT_SHOP_ECAREObj.OFFER_NAME=dr["OFFER_NAME"].ToString();
					}
                    if (dr["ResultDesc"] != DBNull.Value)
                    {
                        VAT_SHOP_ECAREObj.ResultDesc = dr["ResultDesc"].ToString();
                    }
                    if (dr["STATUS"] != DBNull.Value)
					{
						VAT_SHOP_ECAREObj.STATUS=int.Parse(dr["STATUS"].ToString());
					}
				if(dr["CHANNEL"] != DBNull.Value)
					{
						VAT_SHOP_ECAREObj.CHANNEL=dr["CHANNEL"].ToString();
					}
				if(dr["VAT_AMOUNT"] != DBNull.Value)
					{
						VAT_SHOP_ECAREObj.VAT_AMOUNT=decimal.Parse(dr["VAT_AMOUNT"].ToString());
					}
				if(dr["RECHARGE_AMOUNT"] != DBNull.Value)
					{
						VAT_SHOP_ECAREObj.RECHARGE_AMOUNT=int.Parse(dr["RECHARGE_AMOUNT"].ToString());
					}
				if(dr["MSISDN"] != DBNull.Value)
					{
						VAT_SHOP_ECAREObj.MSISDN=dr["MSISDN"].ToString();
					}
				if(dr["TRANSDATE"] != DBNull.Value)
					{
						VAT_SHOP_ECAREObj.TRANSDATE=dr["TRANSDATE"].ToString();
					}
				if(dr["TRACK_ID"] != DBNull.Value)
					{
						VAT_SHOP_ECAREObj.TRACK_ID=dr["TRACK_ID"].ToString();
					}
			}
		}
		catch(Exception ex)
		{
			throw ex;
		}
		return VAT_SHOP_ECAREObj;
	}
	/// <summary>
	///Return DataTable from Table View to bind it with any Databind control 
	/// <summary>
	public  DataTable GetDataTable(string Condition)
	{
		try
		{
			string Vw_Name="VAT_SHOP_ECARE";
			return DH.ExecuteDataTable("Select * From "+Vw_Name+" Where "+Condition);
		}
		catch(Exception ex)
		{
			throw ex;
		}
	}
	public  int Update(VAT_SHOP_ECARE VAT_SHOP_ECAREObj)
	{
		try
		{
			string strUpdate = "update " + strTableName + " set ";
			string strSets = "";
			string strWhere = " where  SEQ_ID=" + VAT_SHOP_ECAREObj.SEQ_ID;
				if(VAT_SHOP_ECAREObj.REQ_XML!= null)
				{
					strSets += ",REQ_XML="+DH.ToDBString(VAT_SHOP_ECAREObj.REQ_XML);
				}
				else
				{
					strSets += ",REQ_XML=null";
				}
				if(VAT_SHOP_ECAREObj.SEQ_ID != null)
                {
					strSets += ",SEQ_ID="+ DH.ToDBString(VAT_SHOP_ECAREObj.SEQ_ID);
				}
				else
				{
					strSets += ",SEQ_ID=null";
				}
                if (VAT_SHOP_ECAREObj.ResultDesc != null)
                {
                    strSets += ",ResultDesc=" + DH.ToDBString(VAT_SHOP_ECAREObj.ResultDesc);
                }
                else
                {
                    strSets += ",ResultDesc=null";
                }
                if (VAT_SHOP_ECAREObj.OFFER_NAME!= null)
				{
					strSets += ",OFFER_NAME="+DH.ToDBString(VAT_SHOP_ECAREObj.OFFER_NAME);
				}
				else
				{
					strSets += ",OFFER_NAME=null";
				}
				if(VAT_SHOP_ECAREObj.STATUS>0)
				{
					strSets += ",STATUS="+VAT_SHOP_ECAREObj.STATUS;
				}
				else
				{
					strSets += ",STATUS=null";
				}
				if(VAT_SHOP_ECAREObj.CHANNEL!=null)
				{
					strSets += ",CHANNEL="+ DH.ToDBString(VAT_SHOP_ECAREObj.CHANNEL);
				}
				else
				{
					strSets += ",CHANNEL=null";
				}
				if(VAT_SHOP_ECAREObj.VAT_AMOUNT>0)
				{
					strSets += ",VAT_AMOUNT="+VAT_SHOP_ECAREObj.VAT_AMOUNT;
				}
				else
				{
					strSets += ",VAT_AMOUNT=null";
				}
				if(VAT_SHOP_ECAREObj.RECHARGE_AMOUNT>0)
				{
					strSets += ",RECHARGE_AMOUNT="+VAT_SHOP_ECAREObj.RECHARGE_AMOUNT;
				}
				else
				{
					strSets += ",RECHARGE_AMOUNT=null";
				}
				if(VAT_SHOP_ECAREObj.MSISDN!= null)
				{
					strSets += ",MSISDN="+DH.ToDBString(VAT_SHOP_ECAREObj.MSISDN);
				}
				else
				{
					strSets += ",MSISDN=null";
				}
				if(VAT_SHOP_ECAREObj.TRANSDATE!= null)
				{
					strSets += ",TRANSDATE="+DH.ToDBString(VAT_SHOP_ECAREObj.TRANSDATE);
				}
				else
				{
					strSets += ",TRANSDATE=null";
				}
				if(VAT_SHOP_ECAREObj.TRACK_ID != null)
                {
					strSets += ",TRACK_ID="+ DH.ToDBString(VAT_SHOP_ECAREObj.TRACK_ID);
				}
				else
				{
					strSets += ",TRACK_ID=null";
				}
				if(strSets.Length > 0)
				{
					DH.ExecuteNonQuery(strUpdate + strSets + strWhere);
				}
			return 1;
		}
		catch(Exception ex)
		{
			throw ex;
		}
	}
    public string GetMaxseq_Id()
        {
            string Max = "";
            try
            {
                DataTable x = DH.ExecuteDataTable("select max(seq_Id) from " + strTableName);
                if (x.Rows.Count > 0)
                    Max = x.Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Max;
        }
	public  int Add(VAT_SHOP_ECARE VAT_SHOP_ECAREObj)
	{
		try
		{
			string strColumns = "";
			string strValues = "";
				strColumns += "REQ_XML";
				if(VAT_SHOP_ECAREObj.REQ_XML!= null)
				{
					strValues += ""+DH.ToDBString(VAT_SHOP_ECAREObj.REQ_XML);
				}
				else
				{
					strValues += "null";
				}
                strColumns += ",PARTITION_ID";
                if (VAT_SHOP_ECAREObj.Partition_ID != null)
                {
                    strValues += "," + DH.ToDBString(VAT_SHOP_ECAREObj.Partition_ID);
                }
                else
                {
                    strValues += ",null";
                }
                strColumns += ",SEQ_ID";
				if(VAT_SHOP_ECAREObj.SEQ_ID != null) 
				{
					strValues += ","+ DH.ToDBString(VAT_SHOP_ECAREObj.SEQ_ID);
				}
				else
				{
					strValues += ",null";
				}
                strColumns += ",ResultDesc";
                if (VAT_SHOP_ECAREObj.ResultDesc != null)
                {
                    strValues += "," + DH.ToDBString(VAT_SHOP_ECAREObj.ResultDesc);
                }
                else
                {
                    strValues += ",null";
                }
                strColumns += ",OFFER_NAME";
				if(VAT_SHOP_ECAREObj.OFFER_NAME!= null)
				{
					strValues += ","+DH.ToDBString(VAT_SHOP_ECAREObj.OFFER_NAME);
				}
				else
				{
					strValues += ",null";
				}
				strColumns += ",STATUS";
				if(VAT_SHOP_ECAREObj.STATUS>=0)
				{
					strValues += ","+VAT_SHOP_ECAREObj.STATUS;
				}
				else
				{
					strValues += ",null";
				}
				strColumns += ",CHANNEL";
				if(VAT_SHOP_ECAREObj.CHANNEL != null)
				{
					strValues += ","+ DH.ToDBString(VAT_SHOP_ECAREObj.CHANNEL);
				}
				else
				{
					strValues += ",null";
				}
				strColumns += ",VAT_AMOUNT";
				if(VAT_SHOP_ECAREObj.VAT_AMOUNT>0)
				{
					strValues += ","+VAT_SHOP_ECAREObj.VAT_AMOUNT;
				}
				else
				{
					strValues += ",null";
				}
				strColumns += ",RECHARGE_AMOUNT";
				if(VAT_SHOP_ECAREObj.RECHARGE_AMOUNT>0)
				{
					strValues += ","+VAT_SHOP_ECAREObj.RECHARGE_AMOUNT;
				}
				else
				{
					strValues += ",null";
				}
				strColumns += ",MSISDN";
				if(VAT_SHOP_ECAREObj.MSISDN!= null)
				{
					strValues += ","+DH.ToDBString(VAT_SHOP_ECAREObj.MSISDN);
				}
				else
				{
					strValues += ",null";
				}
				strColumns += ",TRANSDATE";
				if(VAT_SHOP_ECAREObj.TRANSDATE!= null)
				{
					strValues += ","+DH.ToDBString(VAT_SHOP_ECAREObj.TRANSDATE);
				}
				else
				{
					strValues += ",null";
				}
				strColumns += ",TRACK_ID";
				if(VAT_SHOP_ECAREObj.TRACK_ID != null)
                {
					strValues += ","+ DH.ToDBString(VAT_SHOP_ECAREObj.TRACK_ID);
				}
				else
				{
					strValues += ",null";
				}
				DH.ExecuteNonQuery("insert into " + strTableName + "(" + strColumns + ") values(" + strValues + ")");
				return 1;
		}
		catch(Exception ex)
		{
                throw ex;
		}
	}
        public void Update(string sql)
        {
            try
            {
                DH.ExecuteNonQuery(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public  void Delete(string Condition)
	{
		try
		{
			DH.ExecuteNonQuery("Delete from " + strTableName +" Where "+Condition);
		}
		catch(Exception ex)
		{
			throw ex;
		}
	}
}
}
