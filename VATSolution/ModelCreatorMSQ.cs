using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using MySql.Data;
using System.Data.OleDb;
namespace VATSolution
{
	public class frModelmysq : System.Windows.Forms.Form
	{
		#region Controls
		private string SQL_CONN_STRING = string.Empty;
		private System.Windows.Forms.TextBox textBoxServer;
		private System.Windows.Forms.Button lbtnConnect;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ProgressBar progressBarMain;
		#endregion
		private System.ComponentModel.Container components = null;
		private DataTable TablesColumns;
		private System.Windows.Forms.CheckedListBox LstTables;
        private Button BtnGetTables;
        private DataTable TablesPrimaryKeys;
		public frModelmysq()
		{
			InitializeComponent();
		}
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frModelmysq));
            this.textBoxServer = new System.Windows.Forms.TextBox();
            this.lbtnConnect = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.progressBarMain = new System.Windows.Forms.ProgressBar();
            this.LstTables = new System.Windows.Forms.CheckedListBox();
            this.BtnGetTables = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxServer
            // 
            this.textBoxServer.Location = new System.Drawing.Point(12, 38);
            this.textBoxServer.Name = "textBoxServer";
            this.textBoxServer.Size = new System.Drawing.Size(904, 27);
            this.textBoxServer.TabIndex = 0;
            this.textBoxServer.Text = "Server=10.200.101.27;Port=3306;Database=ej;Uid=appuser;Pwd=Ej2014kS@;";
            this.textBoxServer.TextChanged += new System.EventHandler(this.textBoxServer_TextChanged);
            // 
            // lbtnConnect
            // 
            this.lbtnConnect.Enabled = false;
            this.lbtnConnect.Location = new System.Drawing.Point(861, 500);
            this.lbtnConnect.Name = "lbtnConnect";
            this.lbtnConnect.Size = new System.Drawing.Size(218, 37);
            this.lbtnConnect.TabIndex = 3;
            this.lbtnConnect.Text = "Connect and Create";
            this.lbtnConnect.Click += new System.EventHandler(this.lbtnConnect_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 25);
            this.label1.TabIndex = 4;
            this.label1.Text = "connection string";
            // 
            // progressBarMain
            // 
            this.progressBarMain.Location = new System.Drawing.Point(16, 552);
            this.progressBarMain.Name = "progressBarMain";
            this.progressBarMain.Size = new System.Drawing.Size(1063, 27);
            this.progressBarMain.Step = 100;
            this.progressBarMain.TabIndex = 7;
            // 
            // LstTables
            // 
            this.LstTables.CheckOnClick = true;
            this.LstTables.Location = new System.Drawing.Point(16, 85);
            this.LstTables.MultiColumn = true;
            this.LstTables.Name = "LstTables";
            this.LstTables.Size = new System.Drawing.Size(900, 400);
            this.LstTables.Sorted = true;
            this.LstTables.TabIndex = 10;
            this.LstTables.ThreeDCheckBoxes = true;
            this.LstTables.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.LstTables_ItemCheck);
            this.LstTables.SelectedIndexChanged += new System.EventHandler(this.LstTables_SelectedIndexChanged);
            // 
            // BtnGetTables
            // 
            this.BtnGetTables.Location = new System.Drawing.Point(942, 28);
            this.BtnGetTables.Name = "BtnGetTables";
            this.BtnGetTables.Size = new System.Drawing.Size(137, 43);
            this.BtnGetTables.TabIndex = 11;
            this.BtnGetTables.Text = "get tables";
            this.BtnGetTables.UseVisualStyleBackColor = true;
            this.BtnGetTables.Click += new System.EventHandler(this.BtnGetTables_Click_1);
            // 
            // frModelmysq
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(8, 20);
            this.ClientSize = new System.Drawing.Size(1106, 609);
            this.Controls.Add(this.BtnGetTables);
            this.Controls.Add(this.LstTables);
            this.Controls.Add(this.textBoxServer);
            this.Controls.Add(this.progressBarMain);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbtnConnect);
            this.Name = "frModelmysq";
            this.Text = "Model Creator";
            this.Load += new System.EventHandler(this.ModelCreator_Load);
            this.Resize += new System.EventHandler(this.ModelCreator_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();
		}
		#endregion
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private void lbtnConnect_Click(object sender, System.EventArgs e)
		{
			if (CreateConnectionString())
			{
				FillDataTables();
				GenerateModelClassFiles();
			}
		}
		/// <summary>
		/// This will create the connection string
		/// </summary>
		/// <returns>state of the connection string creation process</returns>
		private bool CreateConnectionString()
		{
			try
			{
				SQL_CONN_STRING = this.textBoxServer.Text;
                return true;
			}
			catch (System.Exception e)
			{
				MessageBox.Show(e.Message);
				return false;
			}
		}
		/// <summary>
		/// Create the Model class list iterating through the tables
		/// </summary>
		/// <param name="dr">Sql Data reader for the database schema</param>
		private void GenerateModelClassFiles()
		{
			if (TablesColumns != null)
			{
				string lstrOldTableName = string.Empty;
				StreamWriter sw = null;
				StreamWriter swList = null;
				System.Text.StringBuilder sbList = null;	
				System.Text.StringBuilder sb = null;	
				System.Text.StringBuilder sbAttr = null;
				progressBarMain.Value=0;
				progressBarMain.Maximum=TablesColumns.Rows.Count;
				foreach (DataRow dr in TablesColumns.Rows)
				{
					string StrTableName = dr["table_name"].ToString().Replace(" ","");
					string StrColumnName = dr["column_name"].ToString();
					string StrColumnDataType = GetSystemType(dr["data_type"].ToString());
					if (lstrOldTableName != StrTableName)
					{
						#region MainClass
						sb = new System.Text.StringBuilder(StrTableName);
						sb.Append(".cs");
						FileInfo lobjFileInfo = new FileInfo(sb.ToString());
						sw = lobjFileInfo.CreateText();
						#endregion
						#region MainClassList
						sbList = new System.Text.StringBuilder(StrTableName+"List");
						sbList.Append(".cs");
						FileInfo FileInfoList = new FileInfo(sbList.ToString());
						swList = FileInfoList.CreateText();
						#endregion	
						this.ClassTop(sw, StrTableName);
						sb = new System.Text.StringBuilder("\r\n\t/// <summary>\r\n\t/// User defined Contructor\r\n\t/// <summary>\r\n\tpublic ");
						sbAttr = new System.Text.StringBuilder();
						sb.Append(StrTableName);
						sb.Append("(");
						GenerateMainClass(StrTableName,swList,StrColumnDataType,StrColumnName);
							this.ClassBody(sw, StrColumnDataType, StrColumnName);
							sb.AppendFormat("{0} {1}, \r\n\t\t", new object[]{StrColumnDataType, StrColumnName}); 
							sbAttr.AppendFormat("\r\n\t\tthis._{0} = {0};", new object[]{StrColumnName});
					}
					else
					{
						this.ClassBody(sw, StrColumnDataType, StrColumnName);
						sb.AppendFormat("{0} {1}, \r\n\t\t", new object[]{StrColumnDataType, StrColumnName}); 
						sbAttr.AppendFormat("\r\n\t\tthis._{0} = {0};", new object[]{StrColumnName});
					}
					lstrOldTableName = StrTableName;
					this.progressBarMain.Increment(1);	
				}
                if (sw != null)
                {
                    this.ClassBottom(sw, sb.ToString().TrimEnd(new char[] { ',', ' ', '\r', '\t', '\n' }), sbAttr.ToString());
                    sw.Close();
                }
                MessageBox.Show("Done !!");
			}
		}
		/// <summary>
		/// Get the SqlDataReader object
		/// </summary>
		/// <returns>SqlDataReader</returns>
		private string GetTablesConlumnsOraQuery()
		{
			return "select t.table_name,t.Column_Name,t.data_type"+
			" from user_tab_columns t"+
			" inner join user_tables s"+
			" on  t.TABLE_NAME=s.table_name";
		}
		private string GetTablesPKSqlQuery()
		{
			return @"select t.table_name, kcu.COLUMN_NAME as PrimaryColumnName
					 from information_schema.tables t inner join 
					 INFORMATION_SCHEMA.TABLE_CONSTRAINTS as tc on (t.table_name = tc.table_name)
					 join INFORMATION_SCHEMA.KEY_COLUMN_USAGE as kcu
					 on kcu.CONSTRAINT_SCHEMA = tc.CONSTRAINT_SCHEMA
					 and kcu.CONSTRAINT_NAME = tc.CONSTRAINT_NAME
					 and kcu.TABLE_SCHEMA = tc.TABLE_SCHEMA
					 and kcu.TABLE_NAME =tc.table_name
					 where t.table_name in
					(
					 select table_name
					 from Information_Schema.Tables
					 where Table_Type='Base Table'
					)
					 and  tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
					Order by t.table_name";
		}
		private string GetTablesConlumnsSqlQuery()
		{
			string SelectedTables=string.Empty;
			foreach(DataRowView itemChecked  in LstTables.CheckedItems)
			{
				SelectedTables+="'"+(string)itemChecked[0]+"',";
			}
			SelectedTables=SelectedTables.Substring(0,SelectedTables.Length-1);
			return @"select table_name, column_name, data_type
					from information_schema.columns 
					where table_name in
					(
						select table_name
						from Information_Schema.Tables
						where Table_Type='Base Table'
					)
					and table_name in("+SelectedTables+") order by table_name";
		}
		private DataTable FillTableLst()
		{
						string sql=@"select distinct table_name
					from information_schema.columns 
					order by table_name";
            return ExecuteDataTable(sql);
        }
        /// <summary>
        /// Create data base conection
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string SqlStatment)
        {
            MySql.Data.MySqlClient.MySqlConnection Cn = new MySql.Data.MySqlClient.MySqlConnection(SQL_CONN_STRING);
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
            catch (Exception )
            {
                Cn.Close();
                return null;
            }
            finally
            {
                Cn.Close();
            }
        }
        public void FillDataTables()
		{
////////Get the Coulmns of the database Tables//////////////////
				TablesColumns = ExecuteDataTable(GetTablesConlumnsSqlQuery());
                    //////////////////////////////////////////////////////
                    ////////////////////Get the Primary Keys of the database Tables/////////////////
				TablesPrimaryKeys = ExecuteDataTable(GetTablesPKSqlQuery());
		}
		/// <summary>
		/// Create the body of each Model Class
		/// </summary>
		/// <param name="sw">Stream Writer of the current file</param>
		/// <param name="StrColumnDataType">Property Item Data Type</param>
		/// <param name="StrColumnName">Property Item Name</param>
		private void ClassBody(StreamWriter sw, string StrColumnDataType, string StrColumnName)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder("\r\n");
			sb.AppendFormat("\r\n\tpublic {0} {1}\r\n\t{2} \r\n\t\tget {2} return _{1}; {3}\r\n\t\tset {2} _{1} = value; {3}\r\n\t{3}\r\n\tprivate {0} _{1};", new object[]{StrColumnDataType, StrColumnName, "{", "}"}); 
			sb.AppendFormat("\r\n\tpublic const {0} {1}_Name=\"{1}\";", new object[]{"string",StrColumnName}); 
			sw.WriteLine(sb.ToString());
		}
		/// <summary>
		/// Create the Top part of the current file
		/// </summary>
		/// <param name="sw">Stream Writer of the current file</param>
		/// <param name="tstrClassName">Name of the Current Class</param>
		private void ClassTop(StreamWriter sw, string tstrClassName)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder("using System;\nusing System.Data;\nusing System.Collections;\n\n namespace Database.DAL\n{\n public class ");
			sb.Append(tstrClassName);
			sb.Append("\r\n{\r\n\t/// <summary>\r\n\t/// Default Contructor\r\n\t/// <summary>\r\n\tpublic ");
			sb.Append(tstrClassName);
			sb.Append("()\r\n\t{\n\t}");
			sw.WriteLine(sb.ToString());
		}
		/// <summary>
		/// Create the Bottom part of the current file
		/// </summary>
		/// <param name="sw">Stream Writer of the current file</param>
		/// <param name="tstrAttrbuteList">List of variables to be used with the user defined contructor</param>
		private void ClassBottom(StreamWriter sw, string tstrAttrbuteList, string tstrVariableList)
		{
			sw.WriteLine(tstrAttrbuteList + ")\r\n\t{" + tstrVariableList + "\r\n\t}");
			sw.WriteLine("}");
			sw.WriteLine("\n}");
		}
		private void ClassListBody(StreamWriter sw, string StrColumnDataType, string StrColumnName)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder("\r\n");
			sb.AppendFormat("\r\n\tpublic {0} {1}\r\n\t{2} \r\n\t\tget {2} return _{1}; {3}\r\n\t\tset {2} _{1} = value; {3}\r\n\t{3}\r\n\tprivate {0} _{1};", new object[]{StrColumnDataType, StrColumnName, "{", "}"}); 
			sw.WriteLine(sb.ToString());
		}
		/// <summary>
		/// Create the Top part of the current file
		/// </summary>
		/// <param name="sw">Stream Writer of the current file</param>
		/// <param name="tstrClassName">Name of the Current Class</param>
		private void ClassListTop(StreamWriter sw, string tstrClassName)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder("using System;\nusing System.Data;\nusing System.Collections;\n\nnamespace Database.DAL\n{\npublic class ");
			sb.Append(tstrClassName);
			sb.Append("\r\n{\r\n\t/// <summary>\r\n\t/// Default Contructor\r\n\t/// <summary>\r\n\tpublic ");
			sb.Append(tstrClassName);
			sb.Append("()\r\n\t{\n\t}");
			sw.WriteLine(sb.ToString());
		}
		/// <summary>
		/// Create the Bottom part of the current file
		/// </summary>
		/// <param name="sw">Stream Writer of the current file</param>
		/// <param name="tstrAttrbuteList">List of variables to be used with the user defined contructor</param>
		private void ClassListBottom(StreamWriter sw, string tstrAttrbuteList, string tstrVariableList)
		{
			sw.WriteLine(tstrAttrbuteList + ")\r\n\t{" + tstrVariableList + "\r\n\t}");
			sw.WriteLine("}");
		}
		/// <summary>
		/// Get the System Type of the SQL ariable type
		/// </summary>
		private string GetSystemType(string tstrSqlType)
		{
			string _Type = string.Empty;
			switch (tstrSqlType) 
			{
				case "biginit":
				{
					_Type = "long";
				}break;
				case "smallint":
				{
					_Type = "short";
				}break;
				case "tinyint":
				{
					_Type = "byte";
				}break;
				case "int":
				{
					_Type = "int";
				}break;
				case "bit":
				{
					_Type = "bool";
				}break;
				case "decimal":
				case "numeric":
				{ 
					_Type = "decimal";
				}break;
				case "money":
				case "smallmoney":
				{
					_Type = "decimal";
				}break;
				case "float":
				case "real":
				{
					_Type = "float";
				}break;
				case "datetime":
				case "smalldatetime":
				{
					_Type = "System.DateTime";
				}break;
				case "char":
				{
					_Type = "char";
				}break;
				case "sql_variant":
				{
					_Type = "object";
				}break;
				case "varchar":
				case "text":
                case "longtext":
                case "nchar" :
				case "nvarchar" :
				case "ntext":
				{
					_Type = "string";
				}break;
				case "binary":
				case "varbinary":
				{
					_Type = "byte[]";
				}break;
				case "image":
				{
					//_Type = "System.Drawing.Image";
					_Type = "string";
				}break;
				case "timestamp":
				case "uniqueidentifier":
				{
					_Type = "string";
				}break;
				default:
				{
					_Type = "unknown";
				}break;
			}
			return _Type;
		}
		private void GenerateMainClass(string TableName,StreamWriter sw, string StrColumnDataType, string StrColumnName)
		{
			ClassListTop(sw,TableName+"List");
			GenerateGetRecordList(TableName,sw,StrColumnName,StrColumnDataType);
			GenerateGetRecord(TableName,sw,StrColumnName,StrColumnDataType);
			GenerateDataTableRecord(TableName,sw);
			GenerateUpdateRecord(TableName,sw,StrColumnName,StrColumnDataType);
			GenerateAddRecord(TableName,sw,StrColumnName,StrColumnDataType);
			GenerateDelete(TableName,sw);
			sw.WriteLine("}");
			sw.WriteLine("\n}");
			sw.Close();
		}
		private void GenerateGetRecordList(string TableName,StreamWriter sw,string StrColumnName,string StrColumnDataType)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder("");
			sb.Append("\r\n\tpublic  string strFieldsToSelect=\"*\";");
            sb.Append("\r\n\tpublic  string strTableName=\""+ TableName + "\";"); 
			sb.Append("\r\n\tpublic DBHandler DH = new DBHandler(\"\");\n"); 
			sb.Append("\r\n\tpublic  ArrayList GetList(string Condition)\r\n\t{"); 
			sb.Append("\r\n\t\tArrayList arr"+TableName+"List =  new ArrayList();\r\n\t\t"+TableName+" "+TableName+"Obj;"); 
			sb.Append("\r\n\t\ttry");
			sb.Append("\r\n\t\t{");
			sb.Append("\r\n\t\t\tDataTable dt"+TableName+ " = DH.ExecuteDataTable(\"select \" + strFieldsToSelect + \" from \" + strTableName+\" Where \"+Condition);");
			sb.Append("\r\n\t\t\tforeach(DataRow dr in dt"+TableName+".Rows)");
			sb.Append("\r\n\t\t\t{");
			sb.Append("\r\n\t\t\t\t"+TableName+"Obj = new "+TableName+"();");
			foreach(DataRow dr in TablesColumns.Rows)
			{
				if(dr[0].ToString()==TableName)
					GetColumnInfo(ref sb,TableName,dr[1].ToString(),dr[2].ToString());
			}
			sb.Append("\r\n\t\t\t\tarr"+TableName+"List.Add("+TableName+"Obj);");
			sb.Append("\r\n\t\t\t}");
			sb.Append("\r\n\t\t}");
			sb.Append("\r\n\t\tcatch(Exception ex)");
			sb.Append("\r\n\t\t{");
			sb.Append("\r\n\t\t\tthrow ex;");
			sb.Append("\r\n\t\t}");
			sb.Append("\r\n\t\treturn arr"+TableName+"List;");
			sb.Append("\r\n\t}"); 
			sw.WriteLine(sb.ToString());
		}
		private void GenerateGetRecord(string TableName,StreamWriter sw,string StrColumnName,string StrColumnDataType)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder("");
			sb.Append("\r\n\tpublic  "+TableName+" Get(string Condition)\r\n\t{"); 
			sb.Append("\r\n\t\t"+TableName+" "+TableName+"Obj = new "+TableName+"();");
			sb.Append("\r\n\t\ttry");
			sb.Append("\r\n\t\t{");
			sb.Append("\r\n\t\t\tDataTable dt"+TableName+ " = DH.ExecuteDataTable(\"select \" + strFieldsToSelect + \" from \" + strTableName+\" Where \"+Condition);");
			sb.Append("\r\n\t\t\tif(dt"+TableName+".Rows.Count > 0)");
			sb.Append("\r\n\t\t\t{\r\n\t\t\t\tDataRow dr = dt"+TableName+".Rows[0];");
			foreach(DataRow dr in TablesColumns.Rows)
			{
				if(dr[0].ToString()==TableName)
					GetColumnInfo(ref sb,TableName,dr[1].ToString(),dr[2].ToString());
			}
			sb.Append("\r\n\t\t\t}");
			sb.Append("\r\n\t\t}");
			sb.Append("\r\n\t\tcatch(Exception ex)");
			sb.Append("\r\n\t\t{");
			sb.Append("\r\n\t\t\tthrow ex;");
			sb.Append("\r\n\t\t}");
			sb.Append("\r\n\t\treturn "+TableName+"Obj;");
			sb.Append("\r\n\t}"); 
			sw.WriteLine(sb.ToString());
		}
		private void GenerateDelete(string TableName,StreamWriter sw)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder("");
			sb.Append("\r\n\tpublic  void Delete(string Condition)\r\n\t{"); 
			sb.Append("\r\n\t\ttry");
			sb.Append("\r\n\t\t{");
			sb.Append("\r\n\t\t\tDH.ExecuteNonQuery(\"Delete from \" + strTableName +\" Where \"+Condition);");
			sb.Append("\r\n\t\t}");
			sb.Append("\r\n\t\tcatch(Exception ex)");
			sb.Append("\r\n\t\t{");
			sb.Append("\r\n\t\t\tthrow ex;");
			sb.Append("\r\n\t\t}");
			sb.Append("\r\n\t}"); 
			sw.WriteLine(sb.ToString());
		}
		private int GetCountOfPKs(string TblName)
		{
			DataRow[] Pks=TablesPrimaryKeys.Select("table_name='"+TblName+"'");
			return Pks.Length;
		}
		private string GetTablePK(string TblName,int Index)
		{
			DataRow[] Pks=TablesPrimaryKeys.Select("table_name='"+TblName+"'");
			return (Pks.Length>Index)?Pks[Index]["PrimaryColumnName"].ToString():"";
		}
		private bool IsPrimaryKey(string TblName,string ClmnName)
		{
			DataRow[] Pks=TablesPrimaryKeys.Select("table_name='"+TblName+"' and PrimaryColumnName='"+ClmnName+"'");
			return (Pks.Length>0)?true:false;
		}
		private void GenerateUpdateRecord(string TableName,StreamWriter sw,string StrColumnName,string StrColumnDataType)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder("");
			sb.Append("\r\n\tpublic  int Update("+TableName+" "+TableName+"Obj)\r\n\t{"); 
			sb.Append("\r\n\t\ttry");
			sb.Append("\r\n\t\t{");
			sb.Append("\r\n\t\t\tstring strUpdate = \"update \" + strTableName + \" set \";");
			sb.Append("\r\n\t\t\tstring strSets = \"\";");
			sb.Append("\r\n\t\t\tstring strWhere = \" where 1=1\" ");
			if(GetCountOfPKs(TableName)==1)
			{
				sb.Append("+\"\"+\" and "+GetTablePK(TableName,0)+"=\" + "+TableName+"Obj."+GetTablePK(TableName,0)+";");
			}
			else if(GetCountOfPKs(TableName)>1)
			{
				for(int i=0;i<GetCountOfPKs(TableName);i++)
				{
					sb.Append("+\"\"+\" and "+GetTablePK(TableName,i)+"=\" + "+TableName+"Obj."+GetTablePK(TableName,i));
				}
				sb.Append(";");
			}
			else
			{
				sb.Append(";");
			}
			int j=0;
			foreach(DataRow dr in TablesColumns.Rows)
			{
				if(dr[0].ToString()==TableName )
				{
					if(!IsPrimaryKey(TableName,dr[1].ToString())||GetCountOfPKs(TableName)>1 )
					UpdateColumn(j,ref sb,TableName,dr[1].ToString(),dr[2].ToString());
					j++;
				}
			}
			sb.Append("\r\n\t\t\t\tif(strSets.Length > 0)");
			sb.Append("\r\n\t\t\t\t{");
			sb.Append("\r\n\t\t\t\t\tDH.ExecuteNonQuery(strUpdate + strSets + strWhere);");			
			sb.Append("\r\n\t\t\t\t}");
			sb.Append("\r\n\t\t\treturn 1;");
			sb.Append("\r\n\t\t}");
			sb.Append("\r\n\t\tcatch(Exception ex)");
			sb.Append("\r\n\t\t{");
			sb.Append("\r\n\t\t\tthrow ex;");
			sb.Append("\r\n\t\t}");
			sb.Append("\r\n\t}"); 
			sw.WriteLine(sb.ToString());
		}
		private void GenerateDataTableRecord(string TableName,StreamWriter sw)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder("");
			sb.Append("\r\n\t/// <summary>");
			sb.Append("\r\n\t///Return DataTable from Table View to bind it with any Databind control ");
			sb.Append("\r\n\t/// <summary>");
			sb.Append("\r\n\tpublic  DataTable GetDataTable(string Condition)\r\n\t{"); 
			sb.Append("\r\n\t\ttry");
			sb.Append("\r\n\t\t{");
			sb.Append("\r\n\t\t\tstring Vw_Name=\""+TableName+"\";");
			sb.Append("\r\n\t\t\treturn DH.ExecuteDataTable(\"Select * From \"+Vw_Name+\" Where \"+Condition);");
			sb.Append("\r\n\t\t}");
			sb.Append("\r\n\t\tcatch(Exception ex)");
			sb.Append("\r\n\t\t{");
			sb.Append("\r\n\t\t\tthrow ex;");
			sb.Append("\r\n\t\t}");
			sb.Append("\r\n\t}"); 
			sw.WriteLine(sb.ToString());
		}
		private void GenerateAddRecord(string TableName,StreamWriter sw,string StrColumnName,string StrColumnDataType)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder("");
			sb.Append("\r\n\tpublic  int Add("+TableName+" "+TableName+"Obj)\r\n\t{"); 
			sb.Append("\r\n\t\ttry");
			sb.Append("\r\n\t\t{");
			sb.Append("\r\n\t\t\tstring strColumns = \"\";");
			sb.Append("\r\n\t\t\tstring strValues = \"\";");
			int i=0;
			foreach(DataRow dr in TablesColumns.Rows)
			{
				if(dr[0].ToString()==TableName)
				{
					if(!IsPrimaryKey(TableName,dr[1].ToString())||GetCountOfPKs(TableName)>1)
					{
						AddColumn(i,ref sb,TableName,dr[1].ToString(),dr[2].ToString());
						i++;
					}
				}
			}
			sb.Append("\r\n\t\t\t\tDH.ExecuteNonQuery(\"insert into \" + strTableName + \"(\" + strColumns + \") values(\" + strValues + \")\");");
			sb.Append("\r\n\t\t\t\treturn 1;");
			sb.Append("\r\n\t\t}");
			sb.Append("\r\n\t\tcatch(Exception ex)");
			sb.Append("\r\n\t\t{");
			sb.Append("\r\n\t\t\tthrow ex;");
			sb.Append("\r\n\t\t}");
			sb.Append("\r\n\t}"); 
			sw.WriteLine(sb.ToString());
		}
		private void GetColumnInfo(ref System.Text.StringBuilder sb,string TableName,string StrColumnName,string StrColumnDataType)
		{
			sb.Append("\r\n\t\t\t\tif(dr[\""+StrColumnName+"\"] != DBNull.Value)");
			sb.Append("\r\n\t\t\t\t\t{");
			if(GetSystemType(StrColumnDataType)!="string")
				sb.Append("\r\n\t\t\t\t\t\t"+TableName+"Obj."+StrColumnName +"="+GetSystemType(StrColumnDataType)+".Parse(dr[\""+StrColumnName+"\"].ToString());");
			else
				sb.Append("\r\n\t\t\t\t\t\t"+TableName+"Obj."+StrColumnName +"=dr[\""+StrColumnName+"\"].ToString();");
			sb.Append("\r\n\t\t\t\t\t}");
		}
		private void UpdateColumn(int i,ref System.Text.StringBuilder sb,string TableName,string StrColumnName,string StrColumnDataType)
		{
			string DelimiterBegin="\"+";
			string DelimiterEnd=";";
			string IsBool="0";
			string DelimiterCondition="!= null";
			string ColumnType=GetSystemType(StrColumnDataType);
			if(ColumnType=="string")
			{
				DelimiterCondition="!= null";
				DelimiterBegin= "\"+DH.ToDBString(";
				DelimiterEnd=");";
			}
			else if(ColumnType=="System.DateTime")
			{
				DelimiterCondition="!= DateTime.MinValue";
				DelimiterBegin="\"+DataBase.DAL.Formatters.DateFormatter.FormatToDateTime(";
				DelimiterEnd=");";
			}
			else if(ColumnType=="bool")
			{
				DelimiterCondition="!= false";
				DelimiterBegin="'\"+";
				DelimiterEnd="+\"'\";";
				IsBool="1";
			}
			else 
			{
				DelimiterCondition=">0";
			}
			sb.Append("\r\n\t\t\t\tif("+TableName+"Obj."+StrColumnName+DelimiterCondition+")");
			sb.Append("\r\n\t\t\t\t{");
			sb.Append("\r\n\t\t\t\t\tstrSets += \""+(i!=0?",":"")+StrColumnName+"="+(IsBool=="1"?"'1'\";":DelimiterBegin+TableName+"Obj."+StrColumnName+DelimiterEnd));
			sb.Append("\r\n\t\t\t\t}");
			sb.Append("\r\n\t\t\t\telse");
			sb.Append("\r\n\t\t\t\t{");
			sb.Append("\r\n\t\t\t\t\tstrSets += \""+(i!=0?",":"")+StrColumnName+"=null\";");
			sb.Append("\r\n\t\t\t\t}");
		}
		private void AddColumn(int i,ref System.Text.StringBuilder sb,string TableName,string StrColumnName,string StrColumnDataType)
		{
			string DelimiterBegin="\"+";
			string DelimiterEnd=";";
			string IsBool="0";
			string DelimiterCondition="!= null";
			string ColumnType=GetSystemType(StrColumnDataType);
			if(ColumnType=="string")
			{
				DelimiterCondition="!= null";
				DelimiterBegin= "\"+DH.ToDBString(";
				DelimiterEnd=");";
			}
			else if(ColumnType=="System.DateTime")
			{
				DelimiterCondition="!= DateTime.MinValue";
				DelimiterBegin="\"+DataBase.DAL.Formatters.DateFormatter.FormatToDateTime(";
				DelimiterEnd=");";
			}
			else if(ColumnType=="bool")
			{
				DelimiterCondition="!= false";
				DelimiterBegin="'\"+";
				DelimiterEnd="+\"'\";";
				IsBool="1";
			}
			else if(ColumnType=="Char")
			{
				DelimiterCondition="!= null";
				DelimiterBegin="'\"+";
				DelimiterEnd="+\"'\";";
			}
			else 
			{
				DelimiterCondition=">0";
			}
			sb.Append("\r\n\t\t\t\tstrColumns += \""+(i!=0?",":"")+StrColumnName+"\";");
			sb.Append("\r\n\t\t\t\tif("+TableName+"Obj."+StrColumnName+DelimiterCondition+")");
			sb.Append("\r\n\t\t\t\t{");
			sb.Append("\r\n\t\t\t\t\tstrValues += \""+(i!=0?",":"")+(IsBool=="1"?"'1'\";":DelimiterBegin+TableName+"Obj."+StrColumnName+DelimiterEnd));
			sb.Append("\r\n\t\t\t\t}");
			sb.Append("\r\n\t\t\t\telse");
			sb.Append("\r\n\t\t\t\t{");
			sb.Append("\r\n\t\t\t\t\tstrValues += \""+(i!=0?",":"")+"null\";");
			sb.Append("\r\n\t\t\t\t}");
		}
		private void ModelCreator_Load(object sender, System.EventArgs e)
		{
			LstTables.Width=this.Width-20;
			progressBarMain.Width =this.Width-20;
		}
		private void BtnGetTables_Click(object sender, System.EventArgs e)
		{
			CreateConnectionString();
			LstTables.DataSource=FillTableLst();
			LstTables.ValueMember="table_name";
			LstTables.DisplayMember="table_name";
			//lbtnConnect.Enabled=true;
		}
		private void ModelCreator_Resize(object sender, System.EventArgs e)
		{
			LstTables.Width=this.Width-20;
			progressBarMain.Width =this.Width-20;
		}
	private void LstTables_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(LstTables.CheckedItems.Count<=0)
				lbtnConnect.Enabled=false;
			else
				lbtnConnect.Enabled=true;
		}
        private void textBoxServer_TextChanged(object sender, EventArgs e)
        {
        }
        private void BtnGetTables_Click_1(object sender, EventArgs e)
        {
            CreateConnectionString();
            LstTables.DataSource = FillTableLst();
            LstTables.ValueMember = "table_name";
            LstTables.DisplayMember = "table_name";
            //lbtnConnect.Enabled=true;
        }
        private void LstTables_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            for (int ix = 0; ix < LstTables.Items.Count; ++ix)
                if (ix != e.Index) LstTables.SetItemChecked(ix, false);
        }
    }
}
