using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

namespace DataLayer
{
    public enum ConstraintKey
    { 
        None,
        Primary,
        Foreign,
        Unique
    }

    public class Column
    {
        string tableName;
        string columnName;
        int rank;
        int? dataLimit;
        bool isNullable;
        ConstraintKey key;
        string dataType;

        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }
        public string ColumnName
        {
            get { return columnName; }
            set { columnName = value; }
        }
        public int Rank
        {
            get { return rank; }
            set { rank = value; }
        }
        public int? DataLimit
        {
            get { return dataLimit; }
            set { dataLimit = value; }
        }
        public bool IsNullable
        {
            get { return isNullable; }
            set { isNullable = value; }
        }
        public ConstraintKey Key
        {
            get { return key; }
            set { key = value; }
        }
        public string DataType
        {
            get { return dataType; }
            set { dataType = value; }
        }
        public string DataTypeFull
        {
            get
            {
                string result = DataType;
                switch (DataType)
                {
                    case "uniqueidentifier":
                        break;
                    case "image":
                        break;
                    case "smallint":
                    case "short":
                        break;
                    case "int":
                        break;
                    case "bit":
                        break;
                    case "text":
                    case "ntext":
                        break;
                    case "char":
                    case "varchar":
                    case "nvarchar":
                        result = string.Format("{0}({1})", DataType, DataLimit.Value.ToString());
                        break;
                    case "smalldatetime":
                    case "datetime":
                    case "time":
                    case "date":
                        break;
                    case "decimal":
                        break;
                    case "money":
                        break;
                }

                return result;
            }
        }

        public string DataType4Code
        {
            get
            {
                string result = typeof(Object).ToString();
                switch (DataType)
                {
                    case "uniqueidentifier":
                        result = typeof(Guid).ToString();
                        break;
                    case "image":
                        result = typeof(Byte[]).ToString();
                        break;
                    case "smallint":
                    case "short":
                        result = typeof(Int16).ToString();
                        break;
                    case "int":
                        result = typeof(Int32).ToString();
                        break;
                    case "bit":
                        result = typeof(Boolean).ToString();
                        break;
                    case "char":
                    case "varchar":
                    case "text":
                    case "nvarchar":
                    case "ntext":
                        result = typeof(String).ToString();
                        break;
                    case "smalldatetime":
                    case "datetime":
                    case "time":
                    case "date":
                        result = typeof(DateTime).ToString();
                        break;
                    case "decimal":
                        result = typeof(Decimal).ToString();
                        break;
                    case "money":
                        result = typeof(Double).ToString();
                        break;
                    default:
                        throw new Exception("Nerede bunun türü");
                }

                if (IsNullable)
                {
                    if (DataType == "image")
                        result = typeof(Byte).ToString() + "?[]";
                    else if(result != typeof(String).ToString())
                        result += "?";
                }

                return result;
            }
        }

        public static ColumnList GetColumns(string tableName)
        { 
            SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DSN"].ConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;
            cmd.Connection.Open();
            cmd.CommandText =
                @"
                    SELECT TABLE_NAME, COLUMN_NAME, ORDINAL_POSITION, IS_NULLABLE, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH
                        FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME LIKE @TableName
                    ORDER BY ORDINAL_POSITION
                ";

            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter("@TableName", tableName));
            SqlDataReader reader = cmd.ExecuteReader();
            ColumnList columnList = new ColumnList();

            while (reader.Read())
            {
                Column col = new Column();
                col.TableName = reader.GetString(0);
                col.ColumnName = reader.GetString(1);
                col.Rank = reader.GetInt16(2);
                col.IsNullable = reader.GetString(3).ToLower().Equals("yes");
                col.DataType = reader.GetString(4);
                if (!reader.IsDBNull(5))
                    col.DataLimit = reader.GetInt32(5);

                col.Key = ConstraintKey.None;

                columnList .Add(col.ColumnName, col);
            }
            cmd.Connection.Close();

            // Find Primary Key
            cmd.Connection.Open();
            cmd.CommandText =
                @"
                        SELECT K.COLUMN_NAME
                        FROM  
                            INFORMATION_SCHEMA.TABLE_CONSTRAINTS T 
                            INNER JOIN 
                            INFORMATION_SCHEMA.KEY_COLUMN_USAGE K 
                            ON T.CONSTRAINT_NAME = K.CONSTRAINT_NAME  
                        WHERE T.CONSTRAINT_TYPE LIKE 'PRIMARY KEY'  
                            AND T.TABLE_NAME LIKE @TableName 
                ";

            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter("@TableName", tableName));
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string column_name = reader.GetString(0);
                columnList[column_name].Key = ConstraintKey.Primary;
            }
            cmd.Connection.Close();

            return columnList;
        }            
    }

    public class ColumnList : Dictionary<string, Column>
    { 
    
    }

}
