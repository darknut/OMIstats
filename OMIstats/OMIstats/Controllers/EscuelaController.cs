using OMIstats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OMIstats.Controllers
{
    public class EscuelaController : BaseController
    {
        //
        // GET: /Escuela/

        public ActionResult Index(string url)
        {
            Institucion i = Institucion.obtenerInstitucionConNombreURL(url);
            if (i == null)
                return RedirectTo(Pagina.ERROR, 404);

            return View(i);
        }

    }
}
