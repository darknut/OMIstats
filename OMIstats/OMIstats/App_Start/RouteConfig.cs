using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OMIstats
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Olimpiadas", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Profile",
                url: "Profile/{tipo}/{omi}/{clave}",
                defaults: new { controller = "Profile", action = "view", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Escuelas",
                url: "Escuela/{tipo}/{omi}/{clave}",
                defaults: new { controller = "Escuela", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}