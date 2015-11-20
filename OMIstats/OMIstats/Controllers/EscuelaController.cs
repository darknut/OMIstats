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

        public ActionResult Index(string clave)
        {
            Institucion i = Institucion.obtenerInstitucionConNombreURL(clave);
            if (i == null)
                return RedirectTo(Pagina.ERROR, 404);

            ViewBag.sedes = i.obtenerOlimpiadasSede();

            return View(i);
        }

        //
        // GET: /Escuela/Edit/

        public ActionResult Edit(string clave)
        {
            if (!estaLoggeado())
            {
                guardarParams(Pagina.LOGIN, Pagina.EDIT_ESCUELA, clave);
                return RedirectTo(Pagina.LOGIN);
            }

            Institucion i = Institucion.obtenerInstitucionConNombreURL(clave);

            if (i == null)
                return RedirectTo(Pagina.ERROR, 404);

            if (!esAdmin()) // -TODO- Agregar validacion para usuarios
                return RedirectTo(Pagina.ERROR, 401);

            limpiarErroresViewBag();

            return View(i);
        }

        //
        // POST: /Escuela/Edit/

        [HttpPost]
        public ActionResult Edit(HttpPostedFileBase file, Institucion escuela)
        {
            if (!esAdmin() || escuela == null) // -TODO- Agregar validacion para usuarios
                RedirectTo(Pagina.HOME);

            limpiarErroresViewBag();

            return View(escuela);
        }
    }
}
