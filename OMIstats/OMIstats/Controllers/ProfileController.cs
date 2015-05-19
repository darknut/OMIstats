using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OMIstats.Models;

namespace OMIstats.Controllers
{
    public class ProfileController : BaseController
    {
        private const int AñoMinimo = 1950;
        private const int EdadMaxima = 20;

        #region Metodos privados

        private void ponFechasEnViewBag()
        {
            int maximo = MiembroDelegacion.primeraOMIPara((Persona)Session["usuario"]);
            int minimo = MiembroDelegacion.ultimaOMIComoCompetidorPara((Persona)Session["usuario"]);

            if (maximo == 0)
                maximo = DateTime.Today.Year;

            if (minimo == 0)
                minimo = AñoMinimo;
            else
                minimo -= EdadMaxima;

            ViewBag.maximo = maximo;
            ViewBag.minimo = minimo;
        }

        private void limpiaErroresViewBag()
        {
            ViewBag.errorImagen = "";
            ViewBag.errorUsuario = "";
        }

        #endregion

        //
        // GET: /Profile/

        public ActionResult Index()
        {
            return RedirectToAction("view");
        }

        //
        // GET: /Profile/view/

        public ActionResult view(string usuario)
        {
            if (usuario == null || usuario.Length == 0)
            {
                if (Persona.isLoggedIn(Session["usuario"]))
                    return View((Persona)Session["usuario"]);
                else
                    return RedirectToAction("Index", "Home");
            }
            else
            {
                Persona p = Persona.obtenerPersonaDeUsuario(usuario);
                if (p != null)
                    return View(p);
                else
                    return RedirectToAction("Index", "Home");
            }
        }

        //
        // GET: /Profile/Edit/

        public ActionResult Edit()
        {
            if (!Persona.isLoggedIn(Session["usuario"]))
                return RedirectToAction("Index", "Home");

            ponFechasEnViewBag();

            return View((Persona)Session["usuario"]);
        }

        //
        // POST: /Profile/Check/

        [HttpPost]
        public JsonResult Check(string usuario)
        {
            if (!Persona.isLoggedIn(Session["usuario"]))
                return Json("error");

            string respuesta = Persona.revisarNombreUsuarioDisponible((Persona)Session["usuario"], usuario).ToString().ToLower();

            return Json(respuesta);
        }

        //
        // POST: /Profile/Edit/
        [HttpPost]
        public ActionResult Edit(HttpPostedFileBase file, Persona p)
        {
            limpiaErroresViewBag();

            if (file != null)
            {
                Utilities.Archivos.ResultadoImagen resultado = Utilities.Archivos.esImagenValida(file);
                if (resultado != Utilities.Archivos.ResultadoImagen.VALIDA)
                {
                    ViewBag.errorImagen = resultado.ToString().ToLower();
                    return Edit();
                }
            }

            Persona.DisponibilidadUsuario respUsuario = Persona.revisarNombreUsuarioDisponible((Persona)Session["usuario"], p.usuario);
            if (respUsuario != Persona.DisponibilidadUsuario.OK)
            {
                ViewBag.errorUsuario = respUsuario.ToString().ToLower();
                return Edit();
            }

            // Todas las validaciones fueron pasadas, es hora de guardar los datos

            if (file != null)
            {
                Utilities.Archivos.guardaImagen(file, "", Utilities.Archivos.FolderImagenes.TEMPORAL);
                // -TODO- Agregar imagen a tabla de Requests
            }

            return Edit();
        }
    }
}
