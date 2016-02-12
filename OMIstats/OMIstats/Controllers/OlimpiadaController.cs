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

        public ActionResult Index(string clave)
        {
            if (clave == Olimpiada.TEMP_CLAVE)
                return RedirectTo(Pagina.EDIT_OLIMPIADA, clave);

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(clave);

            if (o == null)
                return RedirectTo(Pagina.ERROR, 404);

            limpiarErroresViewBag();
            ViewBag.clave = clave;

            ViewBag.dia1 = Problema.obtenerProblemasDeOMI(clave, 1);
            ViewBag.dia2 = Problema.obtenerProblemasDeOMI(clave, 2);

            return View(o);
        }

        //
        // GET: /Olimpiada/New/

        public ActionResult New()
        {
            if (!estaLoggeado())
            {
                guardarParams(Pagina.LOGIN, Pagina.OLIMPIADAS, "");
                return RedirectTo(Pagina.LOGIN);
            }

            if (!esAdmin())
                return RedirectTo(Pagina.ERROR, 401);

            Olimpiada.nuevaOMI();
            return RedirectTo(Pagina.EDIT_OLIMPIADA, Olimpiada.TEMP_CLAVE);
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
            ViewBag.clave = clave;
            ViewBag.poster = o.poster;

            return View(o);
        }

        //
        // POST: /Olimpiada/Edit/

        [HttpPost]
        public ActionResult Edit(Olimpiada omi, string clave, string poster, HttpPostedFileBase fileLogo, HttpPostedFileBase filePoster)
        {
            if (!esAdmin() || omi == null)
                return RedirectTo(Pagina.HOME);

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(clave);
            if (o == null)
                return RedirectTo(Pagina.ERROR, 404);

            limpiarErroresViewBag();
            ViewBag.clave = clave;
            ViewBag.poster = poster;
            omi.poster = poster;
            omi.logo = o.logo;

            if (!ModelState.IsValid)
                return View(omi);

            if (omi.numero.Trim().Length == 0 || omi.numero == Olimpiada.TEMP_CLAVE)
            {
                ViewBag.errorOMI = true;
                return View(omi);
            }

            Estado e = Estado.obtenerEstadoConClave(omi.claveEstado);
            if (e == null)
            {
                ViewBag.errorEstado = true;
                return View(omi);
            }

            if (fileLogo != null)
            {
                Utilities.Archivos.ResultadoImagen resultadoLogo = Utilities.Archivos.esImagenValida(fileLogo);
                if (resultadoLogo != Utilities.Archivos.ResultadoImagen.VALIDA)
                {
                    ViewBag.errorImagen = resultadoLogo.ToString().ToLower();
                    return View(omi);
                }
            }

            if (filePoster != null)
            {
                Utilities.Archivos.ResultadoImagen resultadoPoster = Utilities.Archivos.esImagenValida(filePoster, allowContainer: true);
                if (resultadoPoster != Utilities.Archivos.ResultadoImagen.VALIDA)
                {
                    ViewBag.errorInfo = resultadoPoster.ToString().ToLower();
                    return View(omi);
                }
                omi.poster = filePoster.FileName;
            }

            if (!omi.guardarDatos(clave: clave))
            {
                ViewBag.errorGuardar = true;
                return View(omi);
            }

            if (fileLogo != null)
                Utilities.Archivos.guardaArchivo(fileLogo, omi.numero + ".png",
                    Utilities.Archivos.FolderImagenes.OLIMPIADAS);

            if (filePoster != null)
                Utilities.Archivos.guardaArchivo(filePoster, filePoster.FileName,
                    Utilities.Archivos.FolderImagenes.POSTERS);

            ViewBag.guardado = true;

            return View(omi);
        }

        //
        // GET: /Olimpiada/Attendees/

        public ActionResult Attendees(string clave)
        {
            if (!estaLoggeado())
            {
                guardarParams(Pagina.LOGIN, Pagina.ATTENDEES_OMI, clave);
                return RedirectTo(Pagina.LOGIN);
            }

            if (!esAdmin())
                return RedirectTo(Pagina.ERROR, 401);

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(clave);

            if (o == null)
                return RedirectTo(Pagina.ERROR, 404);

            ViewBag.asistentes = o.obtenerTablaAsistentes();
            ViewBag.omi = clave;
            limpiarErroresViewBag();

            return View();
        }

        //
        // POST: /Olimpiada/Attendees/

        [HttpPost]
        public ActionResult Attendees(string tabla, string clave)
        {
            if (!esAdmin() || tabla == null || clave == null)
                return RedirectTo(Pagina.HOME);

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(clave);

            if (o == null || clave == Olimpiada.TEMP_CLAVE)
                return RedirectTo(Pagina.ERROR, 404);

            limpiarErroresViewBag();
            ViewBag.omi = clave;

            string errores = o.guardarTablaAsistentes(tabla);

            if (errores.Length > 0)
            {
                ViewBag.errorOMI = true;
                ViewBag.asistentes = errores;
            }
            else
                ViewBag.guardado = true;

            return View();
        }
    }
}
