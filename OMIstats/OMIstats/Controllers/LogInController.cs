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
            if (Persona.isLoggedIn(Session["usuario"]))
                return RedirectToAction("Index", "Home");

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
                ViewBag.logInError = false;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                //Log in fallido
                Session["usuario"] = new Persona();
                ViewBag.logInError = true;
                return View(p);
            }
        }

        //
        // GET: /LogIn/Salir

        public ActionResult Salir()
        {
            Session.Clear();
            Session["usuario"] = new Persona();
            return RedirectToAction("Index", "Home");
        }

    }
}
