using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Web.UI.MobileControls;
using System.Collections.Generic;

public class DALBase
{
    protected bool isNew = false;

    public bool IsNew
    {
        get { return isNew; }
    }
}
