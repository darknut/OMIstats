using OMIstats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OMIstats.Controllers
{
    public class EscuelasController : BaseController
    {
        //
        // GET: /Escuelas/

        public ActionResult Index(string filtrar = null, TipoOlimpiada tipo = TipoOlimpiada.OMI)
        {
            if (filtrar != null)
            {
                Estado e = Estado.obtenerEstadoConClave(filtrar);
                if (e == null)
                    filtrar = null;
            }

            ViewBag.tipoOlimpiada = tipo;
            ViewBag.filtro = filtrar;
            ViewBag.estados = Estado.obtenerEstados();
            return View(Institucion.obtenerMejoresEscuelas(filtrar, tipo));
        }
    }
}
