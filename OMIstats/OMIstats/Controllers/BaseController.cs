using Newtonsoft.Json;
using OMIstats.Models;
using OMIstats.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace OMIstats.Controllers
{
    public class BaseController : Controller
    {
        protected const string ERROR = "error";
        protected const string OK = "ok";
        protected const string ADMIN = "admin";
        public static string CAPTCHA_SECRET;
        public static string CAPTCHA_KEY;

        public BaseController()
        {
            // Se usa System.Web en vez de Session porque a tiempo de construcción, Session aún no esta populada
            Persona usuario = (Persona) System.Web.HttpContext.Current.Session["usuario"];
            if (usuario != null)
                usuario.recargarDatos();
            ViewBag.usuario = usuario;
            ViewBag.captchaKey = CAPTCHA_KEY;
        }

        protected bool estaLoggeado()
        {
            return Persona.isLoggedIn(Session["usuario"]);
        }

        protected void recargarDatos()
        {
            ((Persona)Session["usuario"]).recargarDatos();
        }

        protected Persona getUsuario()
        {
            return (Persona)Session["usuario"];
        }

        protected void setUsuario(Persona p)
        {
            Session["usuario"] = p;
        }

        protected void limpiarParams(Pagina p)
        {
            Session[p.ToString() + "params"] = null;
        }

        protected object obtenerParams(Pagina p)
        {
            return Session[p.ToString() + "params"];
        }

        protected void guardarParams(Pagina p, object pa)
        {
            Session[p.ToString() + "params"] = pa;
        }

        protected void guardarParams(Pagina p, Pagina p2, string s)
        {
            Session[p.ToString() + "params"] = new KeyValuePair<Pagina, string> (p2, s);
        }

        protected bool esAdmin()
        {
            if (!estaLoggeado())
                return false;

            return getUsuario().admin;
        }

        protected ActionResult RedirectTo(Pagina pagina, object opciones = null)
        {
            RedirectData data = Enlaces.RedirectTo(pagina, opciones);
            return RedirectToAction(data.actionName, data.controllerName, data.routeValues);
        }

        protected bool revisaCaptcha()
        {
            string captcha = Request.Form["g-recaptcha-response"];
            if (captcha == null || captcha == "")
                return false;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://www.google.com");
                var param = String.Format("/recaptcha/api/siteverify?secret={0}&response={1}",
                                          CAPTCHA_SECRET,
                                          captcha);

                var result = client.PostAsync(param, null).Result;

                string response = result.Content.ReadAsStringAsync().Result;

                dynamic google = JsonConvert.DeserializeObject(response);

                return google.success.Value;
            }
        }

        protected void limpiarErroresViewBag()
        {
            ViewBag.logInError = false;
            ViewBag.faltante = false;
            ViewBag.guardado = false;
            ViewBag.errorMail = "";
            if (ViewBag.errorCaptcha == null)
                ViewBag.errorCaptcha = false;
            if (ViewBag.errorImagen == null)
                ViewBag.errorImagen = "";
            if (ViewBag.errorUsuario == null)
                ViewBag.errorUsuario = "";
            if (ViewBag.errorPassword == null)
                ViewBag.errorPassword = "";
            ViewBag.errorInfo = "";
            ViewBag.errorPeticion = false;
            ViewBag.admin = esAdmin();
            ViewBag.tienePeticiones = false;
            ViewBag.errorOMI = false;
            ViewBag.errorEstado = false;
            ViewBag.errorGuardar = false;
        }
    }
}
