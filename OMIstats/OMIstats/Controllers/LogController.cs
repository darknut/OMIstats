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
                setUsuario(new Persona(Persona.UsuarioNulo));
                ViewBag.logInError = true;
                return View(p);
            }
        }

        //
        // GET: /Log/Out/

        public ActionResult Out()
        {
            Session.Clear();
            setUsuario(new Persona(Persona.UsuarioNulo));
            return RedirectTo(Pagina.HOME);
        }
    }
}
