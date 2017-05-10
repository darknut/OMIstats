using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OMIstats.Models;

namespace OMIstats.Controllers
{
    public class ProblemaController : BaseController
    {
        //
        // GET: /Problema/

        public ActionResult Index()
        {
            return RedirectTo(Pagina.HOME);
        }

        //
        // GET: /Problema/Edit/

        public ActionResult Edit(string omi, int dia, int numero)
        {
            if (!estaLoggeado())
            {
                guardarParams(Pagina.LOGIN, Pagina.PROBLEMA, omi + ":" + dia + ":" + numero);
                return RedirectTo(Pagina.LOGIN);
            }

            if (!esAdmin())
                return RedirectTo(Pagina.ERROR, 401);

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, Olimpiada.TipoOlimpiada.OMI);
            if (o == null)
                return RedirectTo(Pagina.ERROR, 401);

            if (dia < 0 || dia > 2)
                return RedirectTo(Pagina.ERROR, 401);

            if (numero < 0 || numero > 4)
                return RedirectTo(Pagina.ERROR, 401);

            Problema p = Problema.obtenerProblema(omi, Olimpiada.TipoOlimpiada.OMI, dia, numero);

            return View(p);
        }

        //
        // POST: /Problema/Edit/

        [HttpPost]
        public ActionResult Edit(Problema p)
        {
            if (!esAdmin())
                return RedirectTo(Pagina.HOME);

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(p.olimpiada, Olimpiada.TipoOlimpiada.OMI);
            if (o == null || p.olimpiada == Olimpiada.TEMP_CLAVE)
                return RedirectTo(Pagina.ERROR, 401);

            if (p.dia < 0 || p.dia > 2)
                return RedirectTo(Pagina.ERROR, 401);

            if (p.numero < 0 || p.numero > 4)
                return RedirectTo(Pagina.ERROR, 401);

            if (!ModelState.IsValid)
                return View(p);

            p.tipoOlimpiada = Olimpiada.TipoOlimpiada.OMI;
            p.guardar();

            return RedirectTo(Pagina.OLIMPIADA, p.olimpiada);
        }
    }
}
