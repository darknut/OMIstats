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
        // GET: /Request/LogIn/

        public ActionResult LogIn(string nombre, string correo, Peticion.TipoPeticion tipo)
        {
            Peticion pe = new Peticion();

            pe.subtipo = tipo;
            pe.datos1 = nombre;
            pe.datos2 = correo;

            limpiarErroresViewBag();

            return View(pe);
        }

        //
        // POST: /Request/LogIn/

        [HttpPost]
        public ActionResult LogIn(Peticion pe)
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

            if (pe.subtipo != Peticion.TipoPeticion.ERROR &&
                pe.subtipo != Peticion.TipoPeticion.NO_ERROR &&
                pe.subtipo != Peticion.TipoPeticion.NO_ESTOY)
                return RedirectTo(Pagina.ERROR);

            pe.tipo = Peticion.TipoPeticion.LOGIN;

            if (!pe.guardarPeticion())
                return RedirectTo(Pagina.ERROR);

            ViewBag.guardado = true;
            return View(pe);
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
    }
}
