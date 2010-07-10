using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Wyeth.ApplicationBlocks.Data;
using System.Data.SqlClient;
using System.Collections.Generic;


public class DAL
{
    public static string DSN = ConfigurationManager.AppSettings.Get("DSN");

    public static DataSet ExecuteDataSet(string procedureName)
    {
        SqlHelper helper = new SqlHelper();

        return helper.ExecuteDataset(DSN, CommandType.StoredProcedure, procedureName);
    }

    public static DataSet ExecuteDataSet(string procedureName, params IDataParameter[] commandParameters)
    {
		// TraceSQL(procedureName, commandParameters);
		
        SqlHelper helper = new SqlHelper();

        return helper.ExecuteDataset(DSN, CommandType.StoredProcedure, procedureName, commandParameters);
    }

    public static int ExecuteNonQuery(string procedureName)
    {
        SqlHelper helper = new SqlHelper();

        return helper.ExecuteNonQuery(DSN, CommandType.StoredProcedure, procedureName);
    }

    public static int ExecuteNonQuery(string procedureName, params IDataParameter[] commandParameters)
    {
		// TraceSQL(procedureName, commandParameters);
    
        SqlHelper helper = new SqlHelper();

        return helper.ExecuteNonQuery(DSN, CommandType.StoredProcedure, procedureName, commandParameters);
    }

    public static IDataReader ExecuteReader(string procedureName)
    {
        SqlHelper helper = new SqlHelper();

        return helper.ExecuteReader(DSN, CommandType.StoredProcedure, procedureName);
    }

    public static IDataReader ExecuteReader(string procedureName, params IDataParameter[] commandParameters)
    {
		// TraceSQL(procedureName, commandParameters);
        SqlHelper helper = new SqlHelper();

        return helper.ExecuteReader(DSN, CommandType.StoredProcedure, procedureName, commandParameters);
    }
    
    private static void TraceSQL(string procedureName, IDataParameter[] commandParameters)
    {
        string debug = string.Format("exec {0}", procedureName);

        foreach (SqlParameter param in commandParameters)
        {
            if (param.Value == null)
                debug += string.Format("{0} = null,", param.ParameterName, param.Value);
            else
                debug += string.Format("{0} = '{1}',", param.ParameterName, param.Value);
        }

        HttpContext.Current.Response.Write(debug.TrimEnd(new char[] { ',' }));
    }
}