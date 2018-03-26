using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace OMIstats
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Utilities.Acceso.CADENA_CONEXION = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
            Utilities.Acceso.CADENA_CONEXION_OMI = ConfigurationManager.ConnectionStrings["conexionOMI"].ConnectionString;
            Models.Usuario.PRODUCTION = ConfigurationManager.AppSettings.Get("Production") == "true";
            Controllers.BaseController.CAPTCHA_SECRET = ConfigurationManager.AppSettings.Get("captchaSecret");
            Controllers.BaseController.CAPTCHA_KEY = ConfigurationManager.AppSettings.Get("captchaKey");

            Application["users"] = 0;
        }

        public void Session_Start()
        {
            Session["usuario"] = new Models.Persona(Models.Persona.UsuarioNulo);

            try
            {
                int users = (int) Application["users"];
                users++;
                Application["users"] = users;

                Models.Log.add(Models.Log.TipoLog.USUARIO, "Usuarios en línea: " + users);
            }
            catch (Exception e)
            {
                Models.Log.add(Models.Log.TipoLog.USUARIO, "Error al incrementar el número de usuarios");
                Models.Log.add(Models.Log.TipoLog.USUARIO, e.ToString());
            }
        }

        public void Session_End()
        {
            try
            {
                int users = (int)Application["users"];
                users--;
                Application["users"] = users;
            }
            catch (Exception e)
            {
                Models.Log.add(Models.Log.TipoLog.USUARIO, "Error al decrementar el número de usuarios");
                Models.Log.add(Models.Log.TipoLog.USUARIO, e.ToString());
            }
        }
    }
}