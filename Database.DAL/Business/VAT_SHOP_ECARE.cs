using System;
using System.Data;
using System.Collections;
 namespace Database.DAL
{
 public class VAT_SHOP_ECARE
{
	/// <summary>
	/// Default Contructor
	/// <summary>
	public VAT_SHOP_ECARE()
	{
	}
public string ResultDesc{ get; set; }
	public string REQ_XML
	{ 
		get { return _REQ_XML; }
		set { _REQ_XML = value; }
	}
    private string _Partition_ID;
        public string Partition_ID
    {
        get { return _Partition_ID; }
        set { _Partition_ID = value; }
    }
        private string _REQ_XML;
	public const string REQ_XML_Name="REQ_XML";
	public string SEQ_ID
	{ 
		get { return _SEQ_ID; }
		set { _SEQ_ID = value; }
	}
	private string _SEQ_ID;
	public const string SEQ_ID_Name="SEQ_ID";
	public string OFFER_NAME
	{ 
		get { return _OFFER_NAME; }
		set { _OFFER_NAME = value; }
	}
	private string _OFFER_NAME;
	public const string OFFER_NAME_Name="OFFER_NAME";
	public int STATUS
	{ 
		get { return _STATUS; }
		set { _STATUS = value; }
	}
	private int _STATUS;
	public const string STATUS_Name="STATUS";
	public string CHANNEL
	{ 
		get { return _CHANNEL; }
		set { _CHANNEL = value; }
	}
	private string _CHANNEL;
	public const string CHANNEL_Name="CHANNEL";
	public decimal VAT_AMOUNT
	{ 
		get { return _VAT_AMOUNT; }
		set { _VAT_AMOUNT = value; }
	}
	private decimal _VAT_AMOUNT;
	public const string VAT_AMOUNT_Name="VAT_AMOUNT";
	public int RECHARGE_AMOUNT
	{ 
		get { return _RECHARGE_AMOUNT; }
		set { _RECHARGE_AMOUNT = value; }
	}
	private int _RECHARGE_AMOUNT;
	public const string RECHARGE_AMOUNT_Name="RECHARGE_AMOUNT";
	public string MSISDN
	{ 
		get { return _MSISDN; }
		set { _MSISDN = value; }
	}
	private string _MSISDN;
	public const string MSISDN_Name="MSISDN";
	public string TRANSDATE
	{ 
		get { return _TRANSDATE; }
		set { _TRANSDATE = value; }
	}
	private string _TRANSDATE;
	public const string TRANSDATE_Name="TRANSDATE";
	public string TRACK_ID
	{ 
		get { return _TRACK_ID; }
		set { _TRACK_ID = value; }
	}
	private string _TRACK_ID;
	public const string TRACK_ID_Name="TRACK_ID";
	/// <summary>
	/// User defined Contructor
	/// <summary>
}
}
