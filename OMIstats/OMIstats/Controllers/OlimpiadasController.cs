﻿using OMIstats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OMIstats.Controllers
{
    public class OlimpiadasController : BaseController
    {
        //
        // GET: /Olimpiadas/

        public ActionResult Index()
        {
            limpiarErroresViewBag();
            return View(Olimpiada.obtenerOlimpiadas(TipoOlimpiada.OMI));
        }

    }
}
