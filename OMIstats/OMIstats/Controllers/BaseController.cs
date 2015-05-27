using OMIstats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OMIstats.Controllers
{
    public class BaseController : Controller
    {
        protected const string ERROR = "error";
        protected const string OK = "OK";

        protected enum Pagina
        {
            HOME,
            LOGIN,
            VIEW_PROFILE,
            SAVED_PROFILE,
            VIEW_REQUEST
        }

        public BaseController()
        {
            // Se usa System.Web en vez de Session porque a tiempo de construcción, Session aún no esta populada
            ViewBag.usuario = System.Web.HttpContext.Current.Session["usuario"];
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
                case Pagina.HOME:
                default:
                    return RedirectToAction("Index", "Home");
            }
        }
    }
}
