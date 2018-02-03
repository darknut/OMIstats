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

        public ActionResult In(string GUID)
        {
            // -TODO- Entrypoint para la OMI
            if (GUID == null)
                return RedirectTo(Pagina.HOME);

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
