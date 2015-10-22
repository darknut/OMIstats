using OMIstats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OMIstats.Controllers
{
    public class OlimpiadaController : BaseController
    {
        //
        // GET: /Olimpiada/

        public ActionResult Index()
        {
            return RedirectTo(Pagina.ERROR, 404);
        }

        //
        // GET: /Olimpiada/Edit/

        public ActionResult Edit(string clave)
        {
            if (!estaLoggeado())
            {
                guardarParams(Pagina.LOGIN, Pagina.EDIT_OLIMPIADA, clave);
                return RedirectTo(Pagina.LOGIN);
            }

            if (!esAdmin())
                return RedirectTo(Pagina.ERROR, 401);

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(clave);

            if (o == null)
                return RedirectTo(Pagina.ERROR, 404);

            limpiarErroresViewBag();

            return View(o);
        }
    }
}
