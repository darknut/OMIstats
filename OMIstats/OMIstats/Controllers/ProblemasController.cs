using OMIstats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OMIstats.Controllers
{
    public class ProblemasController : BaseController
    {
        //
        // GET: /Problemas/

        public ActionResult Index(TipoOlimpiada tipo = TipoOlimpiada.OMI)
        {
            ViewBag.tipo = tipo;
            ViewBag.olimpiadas = Olimpiada.obtenerOlimpiadas(tipo);
            return View(Problema.obtenerProblemas(tipo));
        }
    }
}
