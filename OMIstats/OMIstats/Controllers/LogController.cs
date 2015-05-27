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
        private void limpiarErroresViewBag()
        {
            ViewBag.logInError = false;
            ViewBag.duplicado = false;
        }

        //
        // GET: /Log/

        public ActionResult Index()
        {
            return RedirectTo(Pagina.LOGIN);
        }

        //
        // GET: /Log/In/

        public ActionResult In()
        {
            if (estaLoggeado())
                return RedirectTo(Pagina.HOME);

            Persona p = new Persona();
            limpiarErroresViewBag();
            return View(p);
        }

        //
        // POST: /Log/In/

        [HttpPost]
        public ActionResult In(Persona p)
        {
            if (p == null)
                return RedirectTo(Pagina.HOME);

            if (p.logIn())
            {
                //Log in exitoso
                setUsuario(p);
                ViewBag.logInError = false;
                return RedirectTo(Pagina.VIEW_PROFILE);
            }
            else
            {
                //Log in fallido
                setUsuario(new Persona());
                ViewBag.logInError = true;
                return View(p);
            }
        }

        //
        // GET: /Log/Out/

        public ActionResult Out()
        {
            Session.Clear();
            setUsuario(new Persona());
            return RedirectTo(Pagina.HOME);
        }

        //
        // GET: /Log/Recover/

        public ActionResult Recover()
        {
            if(estaLoggeado())
                return RedirectTo(Pagina.HOME);

            limpiarErroresViewBag();
            return View(new Persona());
        }

        //
        // POST: /Log/Recover/

        [HttpPost]
        public ActionResult Recover(Persona p)
        {
            if (estaLoggeado() || p == null)
                return RedirectTo(Pagina.HOME);

            limpiarErroresViewBag();
            p = Persona.obtenerPersonaDeUsuario(p.usuario);

            if (p == null)
            {
                ViewBag.logInError = true;
                return View(new Persona());
            }

            return View(new Persona());
        }
    }
}
