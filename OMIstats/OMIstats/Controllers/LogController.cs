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
            ViewBag.faltante = false;
            ViewBag.saved = false;
            ViewBag.errorMail = false;
            ViewBag.errorCaptcha = false;
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
                Peticion.borraPeticionesPassword(p);
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
            if (!revisaCaptcha())
            {
                ViewBag.errorCaptcha = true;
                return View(p);
            }

            p = Persona.obtenerPersonaDeUsuario(p.usuario);

            if (p == null)
            {
                ViewBag.logInError = true;
                return View(new Persona());
            }

            if (String.IsNullOrEmpty(p.correo))
            {
                ViewBag.faltante = true;
                return View(new Persona());
            }

            Peticion pe = new Peticion();
            pe.tipo = Peticion.TipoPeticion.USUARIO;
            pe.subtipo = Peticion.TipoPeticion.PASSWORD;
            pe.usuario = p;
            if (pe.guardarPeticion())
                ViewBag.saved = true;
            else
                ViewBag.errorMail = true;

            return View(new Persona());
        }
    }
}
