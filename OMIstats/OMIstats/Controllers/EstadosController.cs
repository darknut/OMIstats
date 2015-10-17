using OMIstats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OMIstats.Controllers
{
    public class EstadosController : BaseController
    {
        //
        // GET: /Estados/

        public ActionResult Index()
        {
            return View(Estado.obtenerEstados());
        }

    }
}
