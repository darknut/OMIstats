using OMIstats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OMIstats.Controllers
{
    public class HallOfFameController : BaseController
    {
        //
        // GET: /HallOfFame/

        public ActionResult Index()
        {
            int cabeceras;
            List<HallOfFamer> medallistas = HallOfFamer.obtenerMultimedallistas(out cabeceras);

            ViewBag.cabeceras = cabeceras;
            return View(medallistas);
        }
    }
}
