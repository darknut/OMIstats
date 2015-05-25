using OMIstats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OMIstats.Controllers
{
    public class AdminController : BaseController
    {
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Admin/Change/

        public ActionResult Change(string usuario)
        {
            if(!esAdmin() || String.IsNullOrEmpty(usuario))
                return RedirectToAction("Index", "Home");

            Persona p = Persona.obtenerPersonaDeUsuario(usuario);

            if (p == null)
                return RedirectToAction("Index", "Home");

            @ViewBag.logInError = false;

            return View(p);
        }
    }
}
