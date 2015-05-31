using OMIstats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OMIstats.Controllers
{
    public class RequestController : BaseController
    {
        //
        // GET: /Request/

        public ActionResult Index()
        {
            return RedirectTo(Pagina.VIEW_REQUEST);
        }

        //
        // GET: /Request/view/

        public ActionResult view()
        {
            if (!estaLoggeado())
                return RedirectTo(Pagina.HOME);

            recargarDatos();

            return View(Peticion.obtenerPeticionesDeUsuario(getUsuario()));
        }

        //
        // POST: /Request/Delete/

        [HttpPost]
        public JsonResult Delete(int clave)
        {
            if (!estaLoggeado())
                return Json(ERROR);

            Peticion pe = Peticion.obtenerPeticionConClave(clave);
            if (pe == null)
                return Json(ERROR);

            if (!(esAdmin() ||
                  getUsuario().clave == pe.usuario.clave))
                return Json(ERROR);

            if (!pe.eliminarPeticion())
                return Json(ERROR);

            return Json(OK);
        }

        //
        // POST: /Request/Aprove/

        [HttpPost]
        public JsonResult Aprove(int clave)
        {
            if (!esAdmin())
                return Json(ERROR);

            Peticion p = Peticion.obtenerPeticionConClave(clave);
            if (p == null)
                return Json(ERROR);

            p.aceptarPeticion();

            return Json(OK);
        }

        //
        // GET: /Request/Manage/

        public ActionResult Manage()
        {
            if (!esAdmin())
                return RedirectTo(Pagina.HOME);

            ViewBag.totalPeticiones = Peticion.cuentaPeticiones();

            return View(Peticion.obtenerPeticiones());
        }

        //
        // GET: /Request/Access/

        public ActionResult Access(string clave, string guid)
        {
            if (estaLoggeado() || String.IsNullOrEmpty(clave) || String.IsNullOrEmpty(guid))
                return RedirectTo(Pagina.HOME);

            int claveInt;
            if (!Int32.TryParse(clave, out claveInt))
                return RedirectTo(Pagina.HOME);

            Peticion pe = Peticion.obtenerPeticionConClave(claveInt);
            if (pe == null || !pe.datos1.Equals(guid))
            {
                ViewBag.errorPeticion = true;
                return View(new Persona());
            }

            pe.aceptarPeticion();
            ViewBag.errorPeticion = false;
            return View(pe.usuario);
        }
    }
}
