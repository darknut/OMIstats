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

            if (pe.tipo.Equals("usuario") &&
                pe.subtipo.Equals("foto"))
                Utilities.Archivos.eliminarArchivo(pe.datos1, Utilities.Archivos.FolderImagenes.TEMPORAL);

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

            // Aceptando la petición
            if (p.tipo.Equals("usuario"))
            {
                if (p.subtipo.Equals("nombre"))
                {
                    p.usuario.nombre = p.datos1;
                    p.usuario.guardarDatos();
                }

                if (p.subtipo.Equals("foto"))
                {
                    p.usuario.foto =
                        Utilities.Archivos.copiarArchivo(p.datos1, Utilities.Archivos.FolderImagenes.TEMPORAL,
                                            p.usuario.clave.ToString(), Utilities.Archivos.FolderImagenes.USUARIOS);
                    p.usuario.guardarDatos();
                    Utilities.Archivos.eliminarArchivo(p.datos1, Utilities.Archivos.FolderImagenes.TEMPORAL);
                }
            }

            p.eliminarPeticion();

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
    }
}
