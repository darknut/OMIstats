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
        private void limpiarErroresViewBag()
        {
            ViewBag.logInError = false;
            ViewBag.changed = false;
        }

        //
        // GET: /Admin/

        public ActionResult Index()
        {
            return RedirectTo(Pagina.HOME);
        }

        //
        // GET: /Admin/Change/

        public ActionResult Change(string usuario)
        {
            if(!esAdmin() || String.IsNullOrEmpty(usuario))
                return RedirectTo(Pagina.HOME);

            Persona p = Persona.obtenerPersonaDeUsuario(usuario);

            if (p == null)
                return RedirectTo(Pagina.HOME);

            limpiarErroresViewBag();

            return View(p);
        }

        //
        // POST: /Admin/Change/

        [HttpPost]
        public ActionResult Change(Persona p)
        {
            if (!esAdmin() || p == null)
                return RedirectTo(Pagina.HOME);

            limpiarErroresViewBag();

            Persona cambiar = Persona.obtenerPersonaDeUsuario(p.usuario);
            Persona admin = Persona.obtenerPersonaConClave(getUsuario().clave);

            admin.password = p.password;
            if (!admin.logIn())
            {
                ViewBag.logInError = true;
                return View(cambiar);
            }

            cambiar.admin = !cambiar.admin;
            cambiar.guardarDatos();

            ViewBag.changed = true;

            return View(cambiar);
        }

        //
        // GET: /Admin/ResetPassword/

        public ActionResult ResetPassword(string usuario)
        {
            if (!esAdmin() || String.IsNullOrEmpty(usuario))
                return RedirectTo(Pagina.HOME);

            Persona p = Persona.obtenerPersonaDeUsuario(usuario);
            if (p == null)
                return RedirectTo(Pagina.HOME);

            limpiarErroresViewBag();
            return View(p);
        }

        //
        // POST: /Admin/ResetPassword/

        [HttpPost]
        public ActionResult ResetPassword(Persona p)
        {
            if (!esAdmin() || p == null)
                return RedirectTo(Pagina.HOME);

            limpiarErroresViewBag();
            p = Persona.obtenerPersonaDeUsuario(p.usuario);

            if (p == null || !ModelState.IsValidField("correo"))
            {
                ViewBag.logInError = true;
                return View(p);
            }

            Peticion pe = new Peticion();
            pe.tipo = Peticion.TipoPeticion.USUARIO;
            pe.subtipo = Peticion.TipoPeticion.PASSWORD;
            pe.usuario = p;
            if (pe.guardarPeticion())
                ViewBag.changed = true;
            else
                ViewBag.logInError = true;

            return View(p);
        }
    }
}
