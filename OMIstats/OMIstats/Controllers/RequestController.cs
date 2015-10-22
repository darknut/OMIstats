using OMIstats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        // GET: /Request/General/

        public ActionResult General()
        {
            Peticion pe = new Peticion();
            if (estaLoggeado())
            {
                Persona p = getUsuario();
                pe.datos1 = p.nombre;
                pe.datos2 = p.correo;
            }
            limpiarErroresViewBag();

            return View(pe);
        }

        //
        // POST: /Request/General/

        [HttpPost]
        public ActionResult General(Peticion pe)
        {
            limpiarErroresViewBag();

            if (pe.datos1 == null || pe.datos1.Trim().Length == 0)
            {
                ViewBag.errorUsuario = ERROR;
                return View(pe);
            }

            if (pe.datos2 == null || !Regex.IsMatch(pe.datos2, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$"))
            {
                ViewBag.errorMail = ERROR;
                return View(pe);
            }

            if (pe.datos3 == null || pe.datos3.Trim().Length == 0)
            {
                ViewBag.errorInfo = ERROR;
                return View(pe);
            }

            if (!revisaCaptcha())
            {
                ViewBag.errorCaptcha = true;
                return View(pe);
            }

            if (pe.subtipo == Peticion.TipoPeticion.NULL) //Quien mande un subtipo inválido, esta tratando de tronar la pagina.
                return RedirectTo(Pagina.ERROR);

            pe.tipo = Peticion.TipoPeticion.GENERAL;

            if (!pe.guardarPeticion())
                return RedirectTo(Pagina.ERROR);

            ViewBag.guardado = true;
            return View(pe);
        }

        //
        // GET: /Request/view/

        public ActionResult view()
        {
            if (!estaLoggeado())
            {
                guardarParams(Pagina.LOGIN, Pagina.VIEW_REQUEST, "");
                return RedirectTo(Pagina.LOGIN);
            }

            if (esAdmin())
                return RedirectTo(Pagina.MANAGE_REQUEST);

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
                    pe.subtipo == Peticion.TipoPeticion.ACCESO ||
                    pe.subtipo == Peticion.TipoPeticion.BIENVENIDO)
                    return Json(ERROR);
            }

            if (!pe.eliminarPeticion())
                return Json(ERROR);

            return Json(OK);
        }

        //
        // POST: /Request/Aprove/

        [HttpPost]
        public JsonResult Aprove(int clave, string mensaje)
        {
            if (!esAdmin())
                return Json(ERROR);

            Peticion p = Peticion.obtenerPeticionConClave(clave);
            if (p == null)
                return Json(ERROR);

            if (p.tipo == Peticion.TipoPeticion.GENERAL)
            {
                p.usuario = getUsuario();
                p.datos3 = mensaje;
            }

            if (p.aceptarPeticion())
                return Json(OK);

            return Json(ERROR);
        }

        //
        // GET: /Request/Manage/

        public ActionResult Manage()
        {
            if (!estaLoggeado())
            {
                guardarParams(Pagina.LOGIN, Pagina.MANAGE_REQUEST, "");
                return RedirectTo(Pagina.LOGIN);
            }

            if (!esAdmin())
                return RedirectTo(Pagina.VIEW_REQUEST);

            ViewBag.totalPeticiones = Peticion.cuentaPeticiones();

            return View(Peticion.obtenerPeticiones());
        }

        //
        // GET: /Request/Access/

        public ActionResult Access(string clave, string guid)
        {
            if (estaLoggeado())
                return RedirectTo(Pagina.HOME);

            if (String.IsNullOrEmpty(clave) || String.IsNullOrEmpty(guid))
            {
                ViewBag.errorPeticion = true;
                return View(new Persona());
            }

            int claveInt;
            if (!Int32.TryParse(clave, out claveInt))
            {
                ViewBag.errorPeticion = true;
                return View(new Persona());
            }

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

            if (!esAdmin() && !revisaCaptcha())
            {
                ViewBag.errorCaptcha = true;
                return View(p);
            }

            if (!ModelState.IsValid)
                return View(p);

            Persona temp = Persona.obtenerPersonaDeUsuario(p.usuario);
            if (temp == null)
            {
                ViewBag.errorUsuario = Persona.DisponibilidadUsuario.USER_NOT_FOUND.ToString().ToLower();
                return View(p);
            }

            if (estaLoggeado())
            {
                Persona actual = getUsuario();
                if (actual != null && actual.clave == temp.clave)
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
                pe.subtipo = Peticion.TipoPeticion.BIENVENIDO;
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
