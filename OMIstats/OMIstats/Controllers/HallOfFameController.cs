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

        public ActionResult Index(bool todos = false)
        {
            int cabeceras;
            List<HallOfFamer> medallistas = HallOfFamer.obtenerMultimedallistas(out cabeceras, excluirNoOros: !todos);

            ViewBag.cabeceras = cabeceras;
            ViewBag.todos = todos;
            return View(medallistas);
        }
    }
}
