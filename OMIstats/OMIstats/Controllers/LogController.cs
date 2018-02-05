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
            //-TODO- descomentar esta linea y quitar el mocking de abajo
            //return Redirect(Utilities.Server.direccionOMI());
            string guid = Models.Usuario.MockUserLoggedIn(1);
            return In(guid);
        }

        //
        // GET: /Log/In/

        public ActionResult In(string GUID)
        {
            if (GUID == null || GUID.Equals(string.Empty))
                return RedirectTo(Pagina.HOME);

            ViewBag.GUIDerror = false;

            if (GUID != null)
            {
                Usuario usuario = Usuario.obtenerUsuarioConGUID(GUID);

                if (usuario == null)
                {
                    ViewBag.GUIDerror = true;
                    return View();
                }

                // Primero revisamos si el usuario ya está enlazado

                Persona persona = Persona.obtenerPersonaDeUsuario(usuario.Id.ToString());

                if (persona != null)
                {
                    // Log in exitoso
                    setUsuario(persona);
                    usuario.borrarGUID();

                    Object t = obtenerParams(Pagina.LOGIN);
                    limpiarParams(Pagina.LOGIN);
                    if (t != null)
                    {
                        KeyValuePair<Pagina, string> redireccion = (KeyValuePair<Pagina, string>)t;
                        return RedirectTo(redireccion.Key, redireccion.Value);
                    }
                }
            }

            return RedirectTo(Pagina.HOME);
        }

        //
        // GET: /Log/Out/

        public ActionResult Out()
        {
            Session.Clear();
            setUsuario(new Persona(Persona.UsuarioNulo));
            return RedirectTo(Pagina.HOME);

            //-TODO- redirigir a log out de la OMI
        }
    }
}
