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
            return RedirectToAction("view");
        }

        //
        // GET: /Request/view/

        public ActionResult view()
        {
            if (!Persona.isLoggedIn(Session["usuario"]))
                return RedirectToAction("Index", "Home");

            ((Persona)Session["usuario"]).recargarDatos();

            return View(Peticion.obtenerPeticionesDeUsuario((Persona) Session["usuario"]));
        }

        //
        // POST: /Request/Delete/

        [HttpPost]
        public JsonResult Delete(int clave)
        {
            if (!Persona.isLoggedIn(Session["usuario"]))
                return Json("error");

            ((Persona)Session["usuario"]).recargarDatos();

            Peticion pe = Peticion.obtenerPeticionConClave(clave);
            if (pe == null)
                return Json("error");

            if (!(((Persona)Session["usuario"]).admin ||
                  ((Persona)Session["usuario"]).clave == pe.usuario.clave))
                return Json("error");

            if (!pe.eliminarPeticion())
                return Json("error");

            if (pe.tipo.Equals("usuario") &&
                pe.subtipo.Equals("foto"))
                Utilities.Archivos.eliminarArchivo(pe.datos1, Utilities.Archivos.FolderImagenes.TEMPORAL);

            return Json("ok");
        }

        //
        // POST: /Request/Aprove/

        [HttpPost]
        public JsonResult Aprove(int clave)
        {
            if (!Persona.isLoggedIn(Session["usuario"]))
                return Json("error");

            ((Persona)Session["usuario"]).recargarDatos();

            if (!((Persona)Session["usuario"]).admin)
                return Json("error");

            Peticion p = Peticion.obtenerPeticionConClave(clave);
            if (p == null)
                return Json("error");

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

            return Json("ok");
        }

        //
        // GET: /Request/Manage/

        public ActionResult Manage()
        {
            if (!Persona.isLoggedIn(Session["usuario"]))
                return RedirectToAction("Index", "Home");

            ((Persona)Session["usuario"]).recargarDatos();

            if (!((Persona)Session["usuario"]).admin)
                return RedirectToAction("Index", "Home");

            ViewBag.totalPeticiones = Peticion.cuentaPeticiones();

            return View(Peticion.obtenerPeticiones());
        }
    }
}
