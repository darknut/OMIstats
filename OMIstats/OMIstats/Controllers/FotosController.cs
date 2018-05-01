using OMIstats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OMIstats.Controllers
{
    public class FotosController : BaseController
    {
        //
        // GET: /Fotos/

        public ActionResult Index(string clave, TipoOlimpiada tipo = TipoOlimpiada.OMI)
        {
            if (tipo == TipoOlimpiada.OMIP || tipo == TipoOlimpiada.OMIS)
                tipo = TipoOlimpiada.OMI;

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(clave, tipo);
            if (o == null)
                return RedirectTo(Pagina.ERROR, 404);

            ViewBag.olimpiada = o;
            ViewBag.admin = esAdmin();

            return View();
        }

        //
        // GET: /Fotos/Edit/

        public ActionResult Edit(string omi, TipoOlimpiada tipo = TipoOlimpiada.OMI, int id = 0)
        {
            if (!esAdmin())
                return RedirectTo(Pagina.ERROR, 504);

            if (tipo == TipoOlimpiada.OMIP || tipo == TipoOlimpiada.OMIS)
                tipo = TipoOlimpiada.OMI;

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, tipo);
            if (o == null)
                return RedirectTo(Pagina.ERROR, 404);

            ViewBag.errorImagen = "";

            Fotos f = Fotos.obtenerFotos(id);
            f.olimpiada = omi;
            f.tipoOlimpiada = tipo;

            return View(f);
        }

        //
        // POST: /Fotos/Edit/

        [HttpPost]
        public ActionResult Edit(Fotos fotos, HttpPostedFileBase portada)
        {
            if (!esAdmin() || fotos == null)
                return RedirectTo(Pagina.HOME);

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(fotos.olimpiada, fotos.tipoOlimpiada);
            if (o == null)
                return RedirectTo(Pagina.ERROR, 404);

            ViewBag.errorImagen = "";
            if (!ModelState.IsValid)
                return View(fotos);

            if (portada == null && fotos.clave == 0)
            {
                ViewBag.errorImagen = Utilities.Archivos.ResultadoImagen.IMAGEN_INVALIDA.ToString().ToLower();
                return View(fotos);
            }

            if (portada != null)
            {
                Utilities.Archivos.ResultadoImagen resultadoLogo = Utilities.Archivos.esImagenValida(portada);
                if (resultadoLogo != Utilities.Archivos.ResultadoImagen.VALIDA)
                {
                    ViewBag.errorImagen = resultadoLogo.ToString().ToLower();
                    return View(fotos);
                }
            }

            fotos.guardarDatos();

            if (portada != null)
                Utilities.Archivos.guardaArchivo(portada, fotos.clave + ".jpg",
                    Utilities.Archivos.FolderImagenes.FOTOS);

            Log.add(Log.TipoLog.ADMIN, "Álbum " + fotos.clave + " actualizado por admin " + getUsuario().nombre);

            return RedirectTo(Pagina.FOTOS, fotos.olimpiada);
        }
    }
}
