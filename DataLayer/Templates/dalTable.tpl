using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.ApplicationBlocks.ExceptionManagement;

<!-- @@DAL_TABLE@@ -->

namespace @@PROJECT_NAMESPACE@@
{
    public class fields_@@TABLE_NAME@@
    {
        <!-- @@TABLE_STRING_FIELDS@@ -->
        public const string @@TABLE_FIELD_NAME@@ = "@@TABLE_FIELD_NAME@@";
        <!-- @@TABLE_STRING_FIELDS@@ -->
    }

    public partial class dal_@@TABLE_NAME@@ : DALBase
    {
        #region Tables Fields

        <!-- @@TABLE_FIELDS@@ -->
        private @@TABLE_FIELD_TYPE@@ _@@TABLE_FIELD_NAME@@;
        public @@TABLE_FIELD_TYPE@@ @@TABLE_FIELD_NAME@@
        {
            get { return _@@TABLE_FIELD_NAME@@; }
            set { _@@TABLE_FIELD_NAME@@ = value; }
        }
        <!-- @@TABLE_FIELDS@@ -->

        #endregion

        #region Stored Procedure Names

        public const string sp_@@TABLE_NAME@@Insert = "dal_@@TABLE_NAME@@_insert";
        public const string sp_@@TABLE_NAME@@SelectByID = "dal_@@TABLE_NAME@@_selectbyid";
        public const string sp_@@TABLE_NAME@@UpdateByID = "dal_@@TABLE_NAME@@_updatebyid";
        public const string sp_@@TABLE_NAME@@DeleteByID = "dal_@@TABLE_NAME@@_deletebyid";
        public const string sp_@@TABLE_NAME@@GetList = "dal_@@TABLE_NAME@@_getlist";

        #endregion


        public dal_@@TABLE_NAME@@()
        {
            Initialize();
        }

        public dal_@@TABLE_NAME@@(@@TABLE_PKFIELD_TYPE@@ id)
        {
            Initialize();
            string id_field = string.Format("@{0}", fields_@@TABLE_NAME@@.@@TABLE_PKFIELD_NAME@@);
            SqlDataReader dr = (SqlDataReader)DAL.ExecuteReader(sp_@@TABLE_NAME@@SelectByID, new SqlParameter(id_field, id));
			
			dal_@@TABLE_NAME@@List getList = FillFields(dr);
			
            if (getList.Count > 0)
            {
                <!-- @@INIT_TABLE_FIELDS@@ -->
                this.@@TABLE_FIELD_NAME@@ = (@@TABLE_FIELD_TYPE@@) getList[0].@@TABLE_FIELD_NAME@@;
                <!-- @@INIT_TABLE_FIELDS@@ -->
            }
        }

        private void Initialize()
        {
            this.isNew = true;
        }

        public bool Save()
        {
            try
            {
                if (IsNew)
                {
                    bool saved = Save(sp_@@TABLE_NAME@@Insert);                   
                    if (saved) this.isNew = false;
                }
                else 
                {
                    Save(sp_@@TABLE_NAME@@UpdateByID);
                }

                return true;
            }
            catch (Exception ex)
            {
				ExceptionManager.Publish(ex);
                return false;
            }
        }

        public bool Delete()
        {
            try
            {
	            string id_field = string.Format("@{0}", fields_@@TABLE_NAME@@.@@TABLE_PKFIELD_NAME@@);
		        int result = DAL.ExecuteNonQuery(sp_@@TABLE_NAME@@DeleteByID, new SqlParameter(id_field, this.@@TABLE_PKFIELD_NAME@@));

                if (result > 0)
                {
                    this.isNew = true;
                    return true;
                }
            }
			catch (Exception ex)
            {
				ExceptionManager.Publish(ex);
			}

            return false;
        }

        private bool Save(string spName)
        {
            List<IDataParameter> dataParameters = new List<IDataParameter>();
            <!-- @@SAVE_TABLE_FIELDS@@ -->
            dataParameters.Add(new SqlParameter("@@TABLE_DBFIELD_NAME@@", this.@@TABLE_FIELD_NAME@@));
            <!-- @@SAVE_TABLE_FIELDS@@ -->

            SqlDataReader dr = (SqlDataReader)DAL.ExecuteReader(spName, dataParameters.ToArray());

            if (dr.Read())
            {
				if (dr["PKFIELD"] != DBNull.Value)
				{
					this.@@TABLE_PKFIELD_NAME@@ = (@@TABLE_PKFIELD_TYPE@@)dr["PKFIELD"];
				}
                
                return true;
            }

            return false;
        }

        public static  dal_@@TABLE_NAME@@List GetList()
        {
            SqlDataReader dr = (SqlDataReader)DAL.ExecuteReader(sp_@@TABLE_NAME@@GetList);

			return FillFields(dr);
        }
        
        private static dal_@@TABLE_NAME@@List FillFields(SqlDataReader dr)
        {
			dal_@@TABLE_NAME@@List getList = new dal_@@TABLE_NAME@@List();

            while (dr.Read())
            {
				dal_@@TABLE_NAME@@ tbl = new dal_@@TABLE_NAME@@();
				
                <!-- @@GETLIST_TABLE_FIELDS@@ -->
                if (dr["@@TABLE_FIELD_NAME@@"] != DBNull.Value)
					tbl.@@TABLE_FIELD_NAME@@ = (@@TABLE_FIELD_TYPE@@) dr["@@TABLE_FIELD_NAME@@"];
                <!-- @@GETLIST_TABLE_FIELDS@@ -->
				tbl.isNew = false;
                
                getList.Add(tbl);
            }	
            
            return getList;
        }
    }

    public partial class dal_@@TABLE_NAME@@List : List<dal_@@TABLE_NAME@@>
    {

    }

}

<!-- @@DAL_TABLE@@ -->