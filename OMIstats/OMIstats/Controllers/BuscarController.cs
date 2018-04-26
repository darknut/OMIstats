using OMIstats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OMIstats.Controllers
{
    public class BuscarController : BaseController
    {
        //
        // GET: /Buscar/

        public ActionResult Index(string query = null)
        {
            List<SearchResult> resultados = null;
            if (query != null)
                resultados = Persona.buscar(query);
            ViewBag.query = query == null ? "" : query;
            return View(resultados);
        }
    }
}
