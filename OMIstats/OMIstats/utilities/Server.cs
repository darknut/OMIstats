﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OMIstats.Utilities
{
    public class Server
    {
        private static string DIRECCION_OMI = "https://www.olimpiadadeinformatica.org.mx/OMI/Ingreso/Login.aspx?estadistica=1";

        public static string direccionOMI()
        {
            return DIRECCION_OMI;
        }

        public static string direccionServer()
        {
            HttpRequest request = HttpContext.Current.Request;
            string urlBase = string.Format("{0}://{1}{2}",
                request.Url.Scheme,
                request.Url.Authority,
                (new System.Web.Mvc.UrlHelper(request.RequestContext)).Content("~"));
            return urlBase;
        }
    }
}