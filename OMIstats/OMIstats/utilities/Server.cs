using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OMIstats.utilities
{
    public class Server
    {
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