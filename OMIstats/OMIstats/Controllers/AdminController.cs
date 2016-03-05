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
            return RedirectTo(Pagina.ERROR, 404);
        }

        //
        // GET: /Admin/Change/

        public ActionResult Change(string usuario)
        {
            if (!estaLoggeado())
            {
                guardarParams(Pagina.LOGIN, Pagina.ADMIN_CHANGE, usuario);
                return RedirectTo(Pagina.LOGIN);
            }

            if(!esAdmin())
                return RedirectTo(Pagina.ERROR, 401);

            Persona p = Persona.obtenerPersonaDeUsuario(usuario);
            if (p == null)
                return RedirectTo(Pagina.ERROR, 404);

            Persona u = getUsuario();
            if (u.usuario == p.usuario)
                return RedirectTo(Pagina.ERROR, 401);

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

            ViewBag.guardado = true;

            return View(cambiar);
        }

        //
        // GET: /Admin/ResetPassword/

        public ActionResult ResetPassword(string usuario)
        {
            if (!estaLoggeado())
            {
                guardarParams(Pagina.LOGIN, Pagina.ADMIN_RESET_PASSWORD, usuario);
                return RedirectTo(Pagina.LOGIN);
            }

            if (!esAdmin())
                return RedirectTo(Pagina.ERROR, 401);

            Persona p = Persona.obtenerPersonaDeUsuario(usuario);
            if (p == null)
                return RedirectTo(Pagina.ERROR, 404);

            Persona u = getUsuario();
            if (u.usuario == p.usuario)
                return RedirectTo(Pagina.ERROR, 401);

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
            string mail = p.correo;
            p = Persona.obtenerPersonaDeUsuario(p.usuario);
            p.correo = mail;

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
                ViewBag.guardado = true;
            else
                ViewBag.logInError = true;

            return View(p);
        }

        //
        // GET: /Admin/Zombies/

        public ActionResult Zombies()
        {
            if (!esAdmin())
                return RedirectTo(Pagina.HOME);

            Institucion.borrarZombies();
            Persona.borrarZombies();

            return RedirectTo(Pagina.MANAGE_REQUEST);
        }
    }
}
