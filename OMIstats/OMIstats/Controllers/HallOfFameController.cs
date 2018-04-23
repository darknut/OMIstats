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

        public ActionResult Index(bool todos = false, string filtrar = null)
        {
            if (filtrar != null)
            {
                Estado e = Estado.obtenerEstadoConClave(filtrar);
                if (e == null)
                {
                    filtrar = null;
                    todos = false;
                }
                else
                {
                    todos = true;
                }
            }

            int cabeceras;
            List<HallOfFamer> medallistas = HallOfFamer.obtenerMultimedallistas(out cabeceras, excluirNoOros: !todos, estado: filtrar);

            ViewBag.cabeceras = cabeceras;
            ViewBag.todos = todos;
            ViewBag.estados = Estado.obtenerEstados();
            ViewBag.filtro = filtrar;
            return View(medallistas);
        }

        //
        // GET: /HallOfFame/Top3

        public ActionResult Top3()
        {
            ViewBag.olimpiadas = Olimpiada.obtenerOlimpiadas(TipoOlimpiada.OMI);
            return View(HallOfFamer.obtenerTop3(TipoOlimpiada.OMI));
        }
    }
}
