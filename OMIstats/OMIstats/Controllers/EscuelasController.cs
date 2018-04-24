using OMIstats.Controllers;
using OMIstats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OMIstats.Models
{
    public class EscuelasController : BaseController
    {
        //
        // GET: /Escuelas/

        public ActionResult Index(TipoOlimpiada tipo = TipoOlimpiada.OMI)
        {
            ViewBag.tipoOlimpiada = tipo;
            return View(Institucion.obtenerMejoresEscuelas(tipo));
        }
    }
}
