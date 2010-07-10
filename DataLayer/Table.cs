using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

namespace DataLayer
{
    public class Table
    {
        string catalog;
        string schema;
        string name;
        string classFile;
        string userClassFile;
        string storedProcedures;
        ColumnList columns;

        public string Catalog
        {
            get { return catalog; }
            set { catalog = value; }
        }
        public string Schema
        {
            get { return schema; }
            set { schema = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string ClassFile
        {
            get { return classFile; }
            set { classFile = value; }
        }
        public string UserClassFile
        {
            get { return userClassFile; }
            set { userClassFile = value; }
        }
        public string StoredProcedures
        {
            get { return storedProcedures; }
            set { storedProcedures = value; }
        }    
        public ColumnList Columns
        {
            get { return columns; }
            set { columns = value; }
        }
        public string FieldNames
        {
            get
            {
                string fieldNames = "";
                foreach (KeyValuePair<string, Column> col in Columns)
                {
                    fieldNames += string.Format("[{0}].[{1}].[{2}], \n", this.Schema, this.Name, col.Value.ColumnName);
                }

                return fieldNames.TrimEnd(new char[] { ',', ' ', '\n' });
            }
        }
        public string FieldNames_WithoutPK
        {
            get
            {
                string fieldNames = "";
                foreach (KeyValuePair<string, Column> col in Columns)
                {
                    if (col.Value.Key != ConstraintKey.Primary)
                        fieldNames += string.Format("[{0}].[{1}].[{2}], \n", this.Schema, this.Name, col.Value.ColumnName);
                }

                return fieldNames.TrimEnd(new char[] { ',', ' ', '\n' });
            }
        }
        public Column PKField
        {
            get 
            { 
                foreach (KeyValuePair<string, Column> col in Columns)
                {
                    if (col.Value.Key == ConstraintKey.Primary)
                    {
                        return col.Value;
                    }
                }

                return null;
            }
        }
        

        public static TableList GetTables()
        {
            SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DSN"].ConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;
            cmd.Connection.Open();
            cmd.CommandText =
                @"
                    SELECT TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME 
                            FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_TYPE LIKE 'BASE TABLE'; 
                ";

            SqlDataReader reader = cmd.ExecuteReader();
            TableList tableList = new TableList();

            while (reader.Read())
            {
                Table tbl = new Table();
                tbl.Catalog = reader.GetString(0);
                tbl.Schema = reader.GetString(1);
                tbl.Name = reader.GetString(2);
                tbl.Columns = Column.GetColumns(tbl.Name);

                tableList.Add(tbl);
            }
            cmd.Connection.Close();

            return tableList;
        }
    }

    public class TableList : List<Table>
    {}
}
