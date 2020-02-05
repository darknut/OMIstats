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
            return Select();
        }

        //
        // GET: /Registro/Select

        public ActionResult Select(string omi = null)
        {
            if (omi == null)
                omi = Olimpiada.obtenerMasReciente().numero;

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, TipoOlimpiada.OMI);
            if (o == null || !tienePermisos(o.registroActivo))
                return RedirectTo(Pagina.HOME);

            Persona p = getUsuario();

            if (p.esSuperUsuario())
                return RedirectTo(Pagina.REGISTRO);

            ViewBag.estados = p.obtenerEstadosDeDelegado();

            return View();
        }

        //
        // GET: /Registro/Delegacion

        public ActionResult Delegacion(string omi = null, string estado = null)
        {
            if (omi == null)
                omi = Olimpiada.obtenerMasReciente().numero;

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, TipoOlimpiada.OMI);
            if (o == null || !tienePermisos(o.registroActivo))
                return RedirectTo(Pagina.HOME);

            Persona p = getUsuario();

            if (!p.esSuperUsuario())
            {
                List<Estado> estados = p.obtenerEstadosDeDelegado();
                if (!estados.Any(e => e.clave == estado))
                    return RedirectTo(Pagina.HOME);
                ViewBag.estado = Estado.obtenerEstadoConClave(estado);
            }

            List<MiembroDelegacion> registrados = MiembroDelegacion.obtenerMiembrosDelegacion(omi, estado, TipoOlimpiada.NULL);
            ViewBag.omi = o;
            return View(registrados);
        }
    }
}
