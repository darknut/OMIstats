using Newtonsoft.Json;
using OMIstats.Models;
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

        protected enum Pagina
        {
            HOME,
            LOGIN,
            VIEW_PROFILE,
            SAVED_PROFILE,
            VIEW_REQUEST,
            ERROR
        }

        public BaseController()
        {
            // Se usa System.Web en vez de Session porque a tiempo de construcción, Session aún no esta populada
            ViewBag.usuario = System.Web.HttpContext.Current.Session["usuario"];
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

        protected bool esAdmin()
        {
            if (!estaLoggeado())
                return false;

            recargarDatos();

            return getUsuario().admin;
        }

        protected ActionResult RedirectTo(Pagina pagina, object opciones = null)
        {
            switch(pagina)
            {
                case Pagina.VIEW_REQUEST:
                    return RedirectToAction("view", "Request");
                case Pagina.SAVED_PROFILE:
                    return RedirectToAction("Saved", "Profile", opciones);
                case Pagina.VIEW_PROFILE:
                    return RedirectToAction("view", "Profile");
                case Pagina.LOGIN:
                    return RedirectToAction("In", "Log");
                case Pagina.ERROR:
                    if (opciones != null)
                        return RedirectToAction("Index", "Error", new { code = opciones.ToString() });
                    return RedirectToAction("Index", "Error");
                case Pagina.HOME:
                default:
                    return RedirectToAction("Index", "Home");
            }
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
        }
    }
}
