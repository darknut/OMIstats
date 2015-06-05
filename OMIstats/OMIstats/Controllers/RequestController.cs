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
        private void limpiarErroresViewBag()
        {
            ViewBag.errorImagen = "";
            ViewBag.errorUsuario = "";
            ViewBag.errorMail = "";
            ViewBag.errorInfo = "";
            ViewBag.guardado = false;
            ViewBag.errorPeticion = false;
            ViewBag.admin = esAdmin();
        }

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

            if (!esAdmin())
            {
                if(getUsuario().clave != pe.usuario.clave)
                    return Json(ERROR);

                if (pe.subtipo == Peticion.TipoPeticion.PASSWORD ||
                    pe.subtipo == Peticion.TipoPeticion.ACCESO)
                    return Json(ERROR);
            }

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

        //
        // GET: /Request/Claim/

        public ActionResult Claim(string usuario)
        {
            if (!esAdmin() && estaLoggeado())
                return RedirectTo(Pagina.HOME);

            limpiarErroresViewBag();

            Persona p = Persona.obtenerPersonaDeUsuario(usuario);
            if (p == null)
                p = new Persona();

            return View(p);
        }

        //
        // POST: /Request/Claim

        [HttpPost]
        public ActionResult Claim(HttpPostedFileBase file, string informacion, Persona p)
        {
            if (p == null || (!esAdmin() && estaLoggeado()))
                return RedirectTo(Pagina.HOME);

            limpiarErroresViewBag();

            if (!ModelState.IsValid)
                return View(p);

            Persona temp = Persona.obtenerPersonaDeUsuario(p.usuario);
            if (temp == null)
            {
                ViewBag.errorUsuario = Persona.DisponibilidadUsuario.USER_NOT_FOUND.ToString().ToLower();
                return View(p);
            }

            if (String.IsNullOrEmpty(p.correo))
            {
                ViewBag.errorMail = ERROR;
                return View(p);
            }

            if (informacion != null && informacion.Length > Peticion.TamañoDatos3)
            {
                ViewBag.errorInfo = ERROR;
                ViewBag.informacion = informacion;
                return View(p);
            }

            // Validaciones imagen
            if (file != null)
            {
                Utilities.Archivos.ResultadoImagen resultado =
                    Utilities.Archivos.esImagenValida(file, Peticion.TamañoPeticionMaximo, allowContainer:true);
                if (resultado != Utilities.Archivos.ResultadoImagen.VALIDA)
                {
                    ViewBag.errorImagen = resultado.ToString().ToLower();
                    return View(p);
                }
                p.foto = Utilities.Archivos.guardaArchivo(file);
            }

            Peticion pe = new Peticion();
            pe.tipo = Peticion.TipoPeticion.USUARIO;
            if (esAdmin())
            {
                pe.subtipo = Peticion.TipoPeticion.PASSWORD;
                pe.usuario = temp;
            }
            else
            {
                pe.subtipo = Peticion.TipoPeticion.ACCESO;
                pe.usuario = temp;
                pe.datos1 = p.foto;
                pe.datos2 = p.correo;
                pe.datos3 = informacion;
            }
            if (pe.guardarPeticion())
                ViewBag.guardado = true;
            else
                ViewBag.errorPeticion = true;

            return View(p);
        }
    }
}
