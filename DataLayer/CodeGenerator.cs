using System;
using System.Collections.Generic;
using System.Text;
using Netology.Helpers;
using System.Configuration;

namespace DataLayer
{
    public class CodeGenerator
    {
        public static string TemplatesPath 
        {
            get 
            {
                return ConfigurationManager.AppSettings["Templates"].ToString();
            }
        }

        public static string PrepareTableClass(Table table)
        {
            Template template = new Template(TemplatesPath+"dalTable.tpl");
            template.selectSection("DAL_TABLE");
            template.setField("PROJECT_NAMESPACE", string.Format("{0}.DataAccessLayer", table.Catalog));
            template.setField("TABLE_NAME", table.Name);
            template.setField("TABLE_PKFIELD_NAME", table.PKField.ColumnName);
            template.setField("TABLE_PKFIELD_TYPE", table.PKField.DataType4Code);

            template.selectSection("TABLE_STRING_FIELDS");
            foreach (KeyValuePair<string, Column> columnItem in table.Columns)
            {
                template.setField("TABLE_FIELD_TYPE", columnItem.Value.DataType4Code);
                template.setField("TABLE_FIELD_NAME", columnItem.Value.ColumnName);
                template.appendSection();
            }
            template.deselectSection();

            template.selectSection("TABLE_FIELDS");
            foreach (KeyValuePair<string, Column> columnItem in table.Columns)
            {
                template.setField("TABLE_FIELD_TYPE", columnItem.Value.DataType4Code);
                template.setField("TABLE_FIELD_NAME", columnItem.Value.ColumnName);
                template.appendSection();
            }
            template.deselectSection();


            template.selectSection("SAVE_TABLE_FIELDS");
            foreach (KeyValuePair<string, Column> columnItem in table.Columns)
            {
                template.setField("TABLE_DBFIELD_NAME", "@" + columnItem.Value.ColumnName);
                template.setField("TABLE_FIELD_NAME", columnItem.Value.ColumnName);
                template.appendSection();
            }
            template.deselectSection();

            template.selectSection("GETLIST_TABLE_FIELDS");
            foreach (KeyValuePair<string, Column> columnItem in table.Columns)
            {
                template.setField("TABLE_FIELD_TYPE", columnItem.Value.DataType4Code);
                template.setField("TABLE_FIELD_NAME", columnItem.Value.ColumnName);
                template.appendSection();
            }
            template.deselectSection();

            template.selectSection("INIT_TABLE_FIELDS");
            foreach (KeyValuePair<string, Column> columnItem in table.Columns)
            {
                template.setField("TABLE_FIELD_TYPE", columnItem.Value.DataType4Code);
                template.setField("TABLE_FIELD_NAME", columnItem.Value.ColumnName);
                template.appendSection();
            }
            template.deselectSection();

            template.appendSection();
            template.deselectSection();

            return template.getContent();
        }

        public static string PrepareUserTableClass(Table table)
        {
            Template template = new Template(TemplatesPath + "dalUserTable.tpl");
            template.selectSection("DAL_TABLE");

            template.setField("PROJECT_NAMESPACE", string.Format("{0}.DataAccessLayer", table.Catalog));
            template.setField("TABLE_NAME", table.Name);
            template.setField("TABLE_PKFIELD_NAME", table.PKField.ColumnName);

            template.appendSection();
            template.deselectSection();

            return template.getContent();
        }

        public static string PrepareInsertStoredProcedure(Table table)
        {
            Template template = new Template(TemplatesPath + "dalSPInsert.tpl");

            template.selectSection("DAL_SP");
            template.setField("DB_NAME", table.Catalog);
            template.setField("SP_NAME", string.Format("[{0}].[dal_{1}_insert]", table.Schema, table.Name));
            template.setField("TABLE_NAME", string.Format("[{0}].[{1}]", table.Schema, table.Name));
            template.setField("INSERT_FIELD_NAMES", table.FieldNames);
            //template.setField("INSERT_FIELD_VALUES", "@" + table.FieldNames.Trim().Replace(", \n", ", \n@"));

            template.setField("INSERT_FIELD_NAMES_WITHOUT_PKFIELD", table.FieldNames_WithoutPK);
            //template.setField("INSERT_FIELD_VALUES_WITHOUT_PKFIELD", "@" + table.FieldNames_WithoutPK.Trim().Replace(", \n", ", \n@"));

            string spParams = string.Empty;
            string insertStatements = string.Empty;
            string insertStatements_withoutpk = string.Empty;
            foreach (KeyValuePair<string, Column> col in table.Columns)
            {
                spParams += string.Format("@{0} {1}, \n", col.Value.ColumnName, col.Value.DataTypeFull);
                insertStatements += string.Format("@{0}, \n", col.Value.ColumnName);

                if (col.Value.Key != ConstraintKey.Primary)
                    insertStatements_withoutpk += string.Format("@{0}, \n", col.Value.ColumnName);
            }
            spParams = spParams.TrimEnd(new char[] { ',', ' ', '\n' });
            insertStatements = insertStatements.TrimEnd(new char[] { ',', ' ', '\n' });
            insertStatements_withoutpk = insertStatements_withoutpk.TrimEnd(new char[] { ',', ' ', '\n' });

            template.setField("TABLE_PKFIELD_PARAMNAME", "@"+table.PKField.ColumnName);
            template.setField("TABLE_PKFIELD_DBTYPE", "int"); //table.PKField.DataTypeFull
            template.setField("SP_PARAMS", spParams);
            template.setField("INSERT_FIELD_VALUES", insertStatements);
            template.setField("INSERT_FIELD_VALUES_WITHOUT_PKFIELD", insertStatements_withoutpk);

            template.appendSection();
            template.deselectSection();

            return template.getContent();
        }

        public static string PrepareUpdateStoredProcedure(Table table)
        {
            Template template = new Template(TemplatesPath + "dalSPUpdate.tpl");

            template.selectSection("DAL_SP");
            template.setField("DB_NAME", table.Catalog);
            template.setField("SP_NAME", string.Format("[{0}].[dal_{1}_updatebyid]", table.Schema, table.Name));
            template.setField("TABLE_NAME", string.Format("[{0}].[{1}]", table.Schema, table.Name));
            template.setField("TABLE_PKFIELD_NAME", table.PKField.ColumnName);
            template.setField("TABLE_PKFIELD_PARAMNAME", "@" + table.PKField.ColumnName);
            
            string spParams = string.Empty;
            string setStatements = string.Empty;
            foreach(KeyValuePair<string, Column> col in table.Columns)
            {
                spParams += string.Format("@{0} {1}, \n", col.Value.ColumnName, col.Value.DataTypeFull);
                setStatements += string.Format("[{0}].[{1}].[{2}] = @{2}, \n", table.Schema, table.Name, col.Value.ColumnName);
            }
            spParams = spParams.TrimEnd(new char[] { ',', ' ', '\n' });
            setStatements = setStatements.TrimEnd(new char[] { ',', ' ', '\n' });

            template.setField("SP_PARAMS", spParams);
            template.setField("SP_SET_STATEMENT", setStatements);
            
            template.appendSection();
            template.deselectSection();

            return template.getContent();
        }

        public static string PrepareDeleteStoredProcedure(Table table)
        {
            Template template = new Template(TemplatesPath + "dalSPDelete.tpl");

            template.selectSection("DAL_SP");
            template.setField("DB_NAME", table.Catalog);
            template.setField("SP_NAME", string.Format("[{0}].[dal_{1}_deletebyid]", table.Schema, table.Name));
            template.setField("TABLE_NAME", string.Format("[{0}].[{1}]", table.Schema, table.Name));
            template.setField("TABLE_PKFIELD_NAME", table.PKField.ColumnName);
            template.setField("TABLE_PKFIELD_PARAMNAME", "@" + table.PKField.ColumnName);
            template.setField("TABLE_PKFIELD_TYPE", table.PKField.DataTypeFull);
            template.appendSection();
            template.deselectSection();

            return template.getContent();
        }

        public static string PrepareSelectStoredProcedure(Table table)
        {
            Template template = new Template(TemplatesPath + "dalSPSelect.tpl");

            template.selectSection("DAL_SP");
            template.setField("DB_NAME", table.Catalog);
            template.setField("SP_NAME", string.Format("[{0}].[dal_{1}_selectbyid]", table.Schema, table.Name));
            template.setField("TABLE_NAME", string.Format("[{0}].[{1}]", table.Schema, table.Name));
            template.setField("SELECT_FIELD_NAMES", table.FieldNames);
            template.setField("TABLE_PKFIELD_NAME", table.PKField.ColumnName);
            template.setField("TABLE_PKFIELD_PARAMNAME", "@"+table.PKField.ColumnName);
            template.setField("TABLE_PKFIELD_TYPE", table.PKField.DataTypeFull);
            template.appendSection();
            template.deselectSection();

            return template.getContent();
        }

        public static string PrepareGetListStoredProcedure(Table table)
        {
            Template template = new Template(TemplatesPath + "dalSPGetList.tpl");

            template.selectSection("DAL_SP");
            template.setField("DB_NAME", table.Catalog);
            template.setField("SP_NAME", string.Format("[{0}].[dal_{1}_getlist]", table.Schema, table.Name));
            template.setField("TABLE_NAME", string.Format("[{0}].[{1}]", table.Schema, table.Name));
            template.setField("SELECT_FIELD_NAMES", table.FieldNames);
            template.appendSection();
            template.deselectSection();

            return template.getContent();
        }
    }
}
