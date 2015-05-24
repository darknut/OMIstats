using OMIstats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OMIstats.Controllers
{
    public class RequestController : BaseController
    {
        //
        // GET: /Request/

        public ActionResult Index()
        {
            return RedirectToAction("view");
        }

        //
        // GET: /Request/view/

        public ActionResult view()
        {
            if (!Persona.isLoggedIn(Session["usuario"]))
                return RedirectToAction("Index", "Home");

            ((Persona)Session["usuario"]).recargarDatos();

            return View(Peticion.obtenerPeticionesDeUsuario((Persona) Session["usuario"]));
        }

    }
}
