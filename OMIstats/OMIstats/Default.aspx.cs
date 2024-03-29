﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OMIstats
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string guid = Page.Request.Params.Get("GUID");
            Response.Redirect("Olimpiada/Resultados" + (
                guid != null ?
                ("?guid=" + guid)
                : ""
                ));
        }
    }
}