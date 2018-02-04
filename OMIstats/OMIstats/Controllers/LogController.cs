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
            // -TODO- Entrypoint para la OMI
            if (GUID == null)
            {
                //setUsuario(Persona.obtenerPersonaConClave(1000));
                return RedirectTo(Pagina.HOME);
            }

            if (GUID != null /* LOG IN EXITOSO */)
            {
                //Log in exitoso
                //setUsuario(p);

                Object t = obtenerParams(Pagina.LOGIN);
                limpiarParams(Pagina.LOGIN);
                if (t != null)
                {
                    KeyValuePair<Pagina, string> redireccion = (KeyValuePair<Pagina, string>)t;
                    return RedirectTo(redireccion.Key, redireccion.Value);
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
        }
    }
}
