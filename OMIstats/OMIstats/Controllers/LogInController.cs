using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OMIstats.Models;

namespace OMIstats.Controllers
{
    public class LogInController : Controller
    {
        //
        // GET: /LogIn/

        public ActionResult Index()
        {
            Persona p = new Persona();
            ViewBag.logInError = false;
            return View(p);
        }

        //
        // POST: /LogIn/

        [HttpPost]
        public ActionResult Index(Persona p)
        {
            if (p.logIn())
            {
                //Log in exitoso
                Session["usuario"] = p;
                return Index();  //TODO: Redirigir hacia otra pagina
            }
            else
            {
                //Log in fallido
                ViewBag.logInError = true;
                return View(p);
            }
        }

    }
}
