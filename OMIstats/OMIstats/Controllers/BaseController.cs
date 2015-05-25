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

        public bool esAdmin()
        {
            return getUsuario().admin;
        }
    }
}
