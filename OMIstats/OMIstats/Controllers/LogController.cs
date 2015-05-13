using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OMIstats.Models;

namespace OMIstats.Controllers
{
    public class LogController : BaseController
    {
        //
        // GET: /Log/

        public ActionResult Index()
        {
            return Redirect("Log/In/");
        }

        //
        // GET: /Log/In/

        public ActionResult In()
        {
            if (Persona.isLoggedIn(Session["usuario"]))
                return RedirectToAction("Index", "Home");

            Persona p = new Persona();
            ViewBag.logInError = false;
            return View(p);
        }

        //
        // POST: /Log/In/

        [HttpPost]
        public ActionResult In(Persona p)
        {
            if (p == null)
                return RedirectToAction("Index", "Home");
            if (p.logIn())
            {
                //Log in exitoso
                Session["usuario"] = p;
                ViewBag.logInError = false;
                return RedirectToAction("view", "Profile");
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
        // GET: /Log/Out/

        public ActionResult Out()
        {
            Session.Clear();
            Session["usuario"] = new Persona();
            return RedirectToAction("Index", "Home");
        }

    }
}
