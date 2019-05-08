using System.Data;
    interface IDBHandler
    {
        int ExecuteNonQuery(string SqlStatment);
        int ExecuteScalar(string SqlStatment);
        DataTable ExecuteDataTable(string SqlStatment);
    }

