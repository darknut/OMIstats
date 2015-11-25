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

                Object t = obtenerParams(Pagina.LOGIN);
                limpiarParams(Pagina.LOGIN);
                if (t != null)
                {
                    KeyValuePair<Pagina, string> redireccion = (KeyValuePair<Pagina, string>)t;
                    return RedirectTo(redireccion.Key, redireccion.Value);
                }

                return RedirectTo(Pagina.HOME);
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
                ViewBag.guardado = true;
            else
                ViewBag.errorMail = ERROR;

            return View(new Persona());
        }
    }
}
