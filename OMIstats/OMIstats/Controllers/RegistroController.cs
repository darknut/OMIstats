using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OMIstats.Models;

namespace OMIstats.Controllers
{
    public class RegistroController : BaseController
    {
        private bool tienePermisos(bool registroActivo)
        {
            if (!estaLoggeado())
                return false;
            Persona p = getUsuario();

            if (p.permisos == Persona.TipoPermisos.NORMAL)
                return false;

            if (p.esSuperUsuario())
                return true;

            if (!registroActivo)
                return false;

            return true;
        }

        //
        // GET: /Registro/

        public ActionResult Index()
        {
            return RedirectTo(Pagina.ERROR, 404);
        }

        //
        // GET: /Registro/Select

        public ActionResult Select(string omi = null)
        {
            if (omi == null)
                omi = Olimpiada.obtenerMasReciente().numero;

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, TipoOlimpiada.OMI);
            if (!tienePermisos(o.registroActivo))
                return RedirectTo(Pagina.ERROR, 403);

            return View();
        }
    }
}
