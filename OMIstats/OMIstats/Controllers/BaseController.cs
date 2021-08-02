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
        protected const string GUID_STRING = "GUID";
        protected const string GUID_USER = "GUID_user";
        public static string CAPTCHA_SECRET;
        public static string CAPTCHA_KEY;

        protected enum Pagina
        {
            HOME,
            LOGIN,
            VIEW_PROFILE,
            EDIT_PROFILE,
            SAVED_PROFILE,
            VIEW_REQUEST,
            MANAGE_REQUEST,
            ERROR,
            ADMIN_CHANGE,
            EDIT_ESTADO,
            EDIT_OLIMPIADA,
            OLIMPIADAS,
            EDIT_ESCUELA,
            SAVED_ESCUELA,
            ATTENDEES_OMI,
            PROBLEMA,
            OLIMPIADA,
            RESULTS_TABLE,
            ADMIN_UNLINK,
            ADMIN_SCOREBOARD,
            ADMIN_LOGS,
            DELEGACION,
            FOTOS,
            ALBUM,
            REGISTRO
        }

        public BaseController()
        {
            // Se usa System.Web en vez de Session porque a tiempo de construcción, Session aún no esta populada
            Persona usuario = (Persona) System.Web.HttpContext.Current.Session["usuario"];
#if DEBUG
            ViewBag.debug = true;
#else
            ViewBag.debug = false;
#endif
            if (usuario != null)
                usuario.recargarDatos();
            ViewBag.usuario = usuario;
            ViewBag.registroActivo = Olimpiada.obtenerMasReciente(yaEmpezada: false).registroActivo;
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
            if (p.clave != Persona.UsuarioNulo)
            {
                if (p.esAdmin())
                    Log.add(Log.TipoLog.ADMIN, "Admin inició sesión: " + p.nombreCompleto);
                else
                    Log.add(Log.TipoLog.USUARIO, p.nombreCompleto + " inició sesión, " + p.correo);
            }
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

            return getUsuario().esAdmin();
        }

        protected ActionResult RedirectTo(Pagina pagina, object opciones = null)
        {
            switch(pagina)
            {
                case Pagina.VIEW_REQUEST:
                    return RedirectToAction("view", "Request");
                case Pagina.EDIT_ESTADO:
                    if (opciones != null)
                        return RedirectToAction("Edit", "Estado", new { estado = opciones.ToString() });
                    return RedirectTo(Pagina.ERROR, 404);
                case Pagina.MANAGE_REQUEST:
                    return RedirectToAction("Manage", "Request");
                case Pagina.SAVED_PROFILE:
                    if (opciones == null)
                        return RedirectToAction("Saved", "Profile");
                    else
                        return RedirectToAction("Saved", "Profile", new { usuario = opciones.ToString() });
                case Pagina.SAVED_ESCUELA:
                    if (opciones == null)
                        return RedirectTo(Pagina.ERROR, 404);
                    return RedirectToAction("Saved", "Escuela", new { url = opciones.ToString() });
                case Pagina.EDIT_PROFILE:
                    return RedirectToAction("Edit", "Profile");
                case Pagina.EDIT_OLIMPIADA:
                    if (opciones != null)
                        return RedirectToAction("Edit", "Olimpiada", new { clave = opciones.ToString() });
                    return RedirectTo(Pagina.ERROR, 404);
                case Pagina.ATTENDEES_OMI:
                    if (opciones != null)
                        return RedirectToAction("Attendees", "Olimpiada", new { clave = opciones.ToString() });
                    return RedirectTo(Pagina.ERROR, 404);
                case Pagina.RESULTS_TABLE:
                    if (opciones != null)
                        return RedirectToAction("ResultsTable", "Olimpiada", new { clave = opciones.ToString() });
                    return RedirectTo(Pagina.ERROR, 404);
                case Pagina.EDIT_ESCUELA:
                    if (opciones != null)
                        return RedirectToAction("Edit", "Escuela", new { url = opciones.ToString() });
                    return RedirectTo(Pagina.ERROR, 404);
                case Pagina.VIEW_PROFILE:
                    if (opciones != null)
                        return RedirectToAction("view", "Profile", new { usuario = opciones.ToString() });
                    return RedirectToAction("view", "Profile");
                case Pagina.LOGIN:
                    return RedirectToAction("Index", "Log");
                case Pagina.ERROR:
                    if (opciones != null)
                        return RedirectToAction("Index", "Error", new { code = opciones.ToString() });
                    return RedirectToAction("Index", "Error");
                case Pagina.ADMIN_CHANGE:
                    if (opciones != null)
                        return RedirectToAction("Change", "Admin", new { usuario = opciones.ToString() });
                    return RedirectTo(Pagina.ERROR, 404);
                case Pagina.ADMIN_UNLINK:
                    if (opciones != null)
                        return RedirectToAction("Unlink", "Admin", new { usuario = opciones.ToString() });
                    return RedirectTo(Pagina.ERROR, 404);
                case Pagina.ADMIN_SCOREBOARD:
                    return RedirectToAction("Scoreboard", "Admin");
                case Pagina.ADMIN_LOGS:
                    return RedirectToAction("Logs", "Admin");
                case Pagina.DELEGACION:
                    {
                        string[] param = opciones.ToString().Split(':');
                        if (param.Length == 2)
                            return RedirectToAction("Delegacion", "Olimpiada", new { clave = param[0], estado = param[1] });
                    }
                    return RedirectTo(Pagina.ERROR, 404);
                case Pagina.FOTOS:
                    if (opciones != null)
                        return RedirectToAction("Index", "Fotos", new { clave = opciones.ToString() });
                    return RedirectTo(Pagina.ERROR, 404);
                case Pagina.ALBUM:
                    if (opciones != null)
                        return RedirectToAction("Album", "Fotos", new { id = opciones.ToString() });
                    return RedirectTo(Pagina.ERROR, 404);
                case Pagina.PROBLEMA:
                    if (opciones != null)
                    {
                        string[] param = opciones.ToString().Split(':');
                        if (param.Length == 3)
                            return RedirectToAction("Edit", "Problema", new { omi = param[0], dia = param[1], numero = param[2] });
                    }
                    return RedirectTo(Pagina.ERROR, 404);
                case Pagina.OLIMPIADA:
                     if (opciones != null)
                        return RedirectToAction("Index", "Olimpiada", new { clave = opciones.ToString() });
                    return RedirectTo(Pagina.ERROR, 404);
                case Pagina.REGISTRO:
                    return RedirectToAction("Delegacion", "Registro", opciones);
                case Pagina.HOME:
                case Pagina.OLIMPIADAS:
                default:
                    return RedirectToAction("Index", "Olimpiadas");
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
