using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OMIstats.Models;

namespace OMIstats.Controllers
{
    public class LogInController : BaseController
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
                return RedirectToAction("Index", "Home");
            }
            else
            {
                //Log in fallido
                ViewBag.logInError = true;
                return View(p);
            }
        }

        //
        // GET: /LogIn/Salir

        public ActionResult Salir()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

    }
}
