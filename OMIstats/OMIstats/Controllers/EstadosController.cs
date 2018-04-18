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

        //
        // GET: /Estados/Lugares/

        public ActionResult Lugares()
        {
            bool[] cabeceras;
            List<Olimpiada> olimpiadas = Olimpiada.obtenerOlimpiadas(TipoOlimpiada.OMI);

            ViewBag.olimpiadas = olimpiadas;
            ViewBag.estados = Medallero.obtenerTablaEstadosGeneral(TipoOlimpiada.OMI, out cabeceras);
            ViewBag.cabeceras = cabeceras;

            int ultimoValido = 0;
            for (int i = cabeceras.Length - 1; i >= 0; i--)
                if (cabeceras[i] && olimpiadas[i].estados > 0)
                    ultimoValido = i;

            ViewBag.ultimoValido = ultimoValido;

            return View(Estado.obtenerEstados());
        }

        //
        // GET: /Estados/Medallas/

        public ActionResult Medallas()
        {
            bool[] cabeceras;
            List<Olimpiada> olimpiadas = Olimpiada.obtenerOlimpiadas(TipoOlimpiada.OMI);

            ViewBag.olimpiadas = olimpiadas;
            ViewBag.estados = Medallero.obtenerTablaEstadosGeneral(TipoOlimpiada.OMI, out cabeceras);
            ViewBag.cabeceras = cabeceras;

            return View(Estado.obtenerEstados());
        }
    }
}
