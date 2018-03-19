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

        public ActionResult Index(string clave, TipoOlimpiada tipo = TipoOlimpiada.OMI)
        {
            if (clave == Olimpiada.TEMP_CLAVE)
                return RedirectTo(Pagina.EDIT_OLIMPIADA, clave);

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(clave, tipo);

            if (o == null)
                return RedirectTo(Pagina.ERROR, 404);

            limpiarErroresViewBag();
            ViewBag.clave = clave;

            ViewBag.dia1 = Problema.obtenerProblemasDeOMI(clave, tipo, 1);
            ViewBag.dia2 = Problema.obtenerProblemasDeOMI(clave, tipo, 2);

            // Mientras las OMIS y OMIPS sean en el mismo evento que la OMI, no tienen su propia vista
            ViewBag.olimpiadas = Olimpiada.obtenerOlimpiadas(TipoOlimpiada.OMI);

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

            Olimpiada.nuevaOMI(TipoOlimpiada.OMI);
            return RedirectTo(Pagina.EDIT_OLIMPIADA, Olimpiada.TEMP_CLAVE);
        }

        //
        // GET: /Olimpiada/Edit/

        public ActionResult Edit(string clave, TipoOlimpiada tipo = TipoOlimpiada.OMI)
        {
            if (!estaLoggeado())
            {
                guardarParams(Pagina.LOGIN, Pagina.EDIT_OLIMPIADA, clave);
                return RedirectTo(Pagina.LOGIN);
            }

            if (!esAdmin())
                return RedirectTo(Pagina.ERROR, 401);

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(clave, tipo);

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

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(clave, TipoOlimpiada.OMI);
            if (o == null)
                return RedirectTo(Pagina.ERROR, 404);

            limpiarErroresViewBag();
            ViewBag.clave = clave;
            ViewBag.poster = poster;
            omi.poster = poster;

            omi.copiarDatosPrecalculados(o);

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

        public ActionResult Attendees(string clave, TipoOlimpiada tipo = TipoOlimpiada.OMI)
        {
            if (!estaLoggeado())
            {
                guardarParams(Pagina.LOGIN, Pagina.ATTENDEES_OMI, clave);
                return RedirectTo(Pagina.LOGIN);
            }

            if (!esAdmin())
                return RedirectTo(Pagina.ERROR, 401);

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(clave, tipo);

            if (o == null)
                return RedirectTo(Pagina.ERROR, 404);

            ViewBag.asistentes = o.obtenerTablaAsistentes();
            limpiarErroresViewBag();

            return View(o);
        }

        //
        // POST: /Olimpiada/Attendees/

        [HttpPost]
        public ActionResult Attendees(string tabla, string clave, TipoOlimpiada tipo = TipoOlimpiada.OMI)
        {
            if (!esAdmin() || tabla == null || clave == null)
                return RedirectTo(Pagina.HOME);

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(clave, tipo);

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

            return View(o);
        }

        //
        // GET: /Olimpiada/ResultsTable/

        public ActionResult ResultsTable(string clave, TipoOlimpiada tipo = TipoOlimpiada.OMI)
        {
            if (!estaLoggeado())
            {
                guardarParams(Pagina.LOGIN, Pagina.RESULTS_TABLE, clave);
                return RedirectTo(Pagina.LOGIN);
            }

            if (!esAdmin())
                return RedirectTo(Pagina.ERROR, 401);

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(clave, tipo);

            if (o == null)
                return RedirectTo(Pagina.ERROR, 404);

            ViewBag.dia1 = o.problemasDia1;
            ViewBag.dia2 = o.problemasDia2;
            ViewBag.resultados = o.obtenerResultadosAdmin();
            limpiarErroresViewBag();

            return View(o);
        }

        //
        // POST: /Olimpiada/ResultsTable/

        [HttpPost]
        public ActionResult ResultsTable(string tabla, string clave, TipoOlimpiada tipo = TipoOlimpiada.OMI)
        {
            if (!esAdmin() || tabla == null || clave == null)
                return RedirectTo(Pagina.HOME);

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(clave, tipo);

            if (o == null || clave == Olimpiada.TEMP_CLAVE)
                return RedirectTo(Pagina.ERROR, 404);

            limpiarErroresViewBag();
            ViewBag.omi = clave;
            ViewBag.dia1 = o.problemasDia1;
            ViewBag.dia2 = o.problemasDia2;
            string errores = o.guardarTablaResultados(tabla);

            if (errores.Length > 0)
            {
                ViewBag.errorOMI = true;
                ViewBag.resultados = errores;
            }
            else
                ViewBag.guardado = true;

            return View(o);
        }

        //
        // GET: /Olimpiada/Resultados/

        public ActionResult Resultados(string clave, TipoOlimpiada tipo = TipoOlimpiada.OMI)
        {
            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(clave, tipo);

            if (o == null)
                return RedirectTo(Pagina.ERROR, 404);

            limpiarErroresViewBag();
            ViewBag.resultados = Models.Resultados.cargarResultados(clave, tipo, cargarObjetos: true);
            ViewBag.problemasDia1 = Problema.obtenerProblemasDeOMI(clave, tipo, 1);
            ViewBag.problemasDia2 = Problema.obtenerProblemasDeOMI(clave, tipo, 2);
            ViewBag.claveUsuario = getUsuario().clave;
            ViewBag.olimpiadas = Olimpiada.obtenerOlimpiadas(TipoOlimpiada.OMI);

            if (o.alsoOmips)
            {
                ViewBag.omis = Olimpiada.obtenerOlimpiadas(TipoOlimpiada.OMIS);
                ViewBag.omip = Olimpiada.obtenerOlimpiadas(TipoOlimpiada.OMIP);
            }

            List<Problema> metadata = Problema.obetnerMetaDatadeOMI(clave, tipo);

            if (metadata.Count > 0)
            {
                ViewBag.numerosDia1 = metadata[1];
                ViewBag.numerosDia2 = metadata[2];
                ViewBag.numerosTotal = metadata[0];
            }

            return View(o);
        }

        //
        // GET: /Olimpiada/Delegacion/

        public ActionResult Delegacion(string clave, string estado, TipoOlimpiada tipo = TipoOlimpiada.OMI)
        {
            if (tipo == TipoOlimpiada.OMIS || tipo == TipoOlimpiada.OMIP)
                tipo = TipoOlimpiada.OMI;

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(clave, tipo);

            if (o == null || o.numero == Olimpiada.TEMP_CLAVE)
                return RedirectTo(Pagina.ERROR, 404);

            Estado e = Estado.obtenerEstadoConClave(estado);

            if (e == null)
                return RedirectTo(Pagina.ERROR, 404);

            Dictionary<TipoOlimpiada, List<MiembroDelegacion>> delegaciones = new Dictionary<TipoOlimpiada, List<MiembroDelegacion>>();
            delegaciones.Add(tipo, MiembroDelegacion.obtenerMiembrosDelegacion(clave, estado, tipo, MiembroDelegacion.TipoAsistente.COMPETIDOR));
            if (tipo == TipoOlimpiada.OMI && o.alsoOmips)
            {
                delegaciones.Add(TipoOlimpiada.OMIP, MiembroDelegacion.obtenerMiembrosDelegacion(clave, estado, TipoOlimpiada.OMIP, MiembroDelegacion.TipoAsistente.COMPETIDOR));
                delegaciones.Add(TipoOlimpiada.OMIS, MiembroDelegacion.obtenerMiembrosDelegacion(clave, estado, TipoOlimpiada.OMIS, MiembroDelegacion.TipoAsistente.COMPETIDOR));
            }

            ViewBag.estado = e;
            ViewBag.delegaciones = delegaciones;
            ViewBag.lideres = MiembroDelegacion.obtenerMiembrosDelegacion(clave, estado, tipo, MiembroDelegacion.TipoAsistente.LIDER);
            ViewBag.otros = MiembroDelegacion.obtenerMiembrosDelegacion(clave, estado, tipo);
            ViewBag.medallas = Medallero.contarMedallas(delegaciones[tipo]);
            ViewBag.olimpiadas = Olimpiada.obtenerOlimpiadas(tipo);
            ViewBag.tipo = tipo;

            if (delegaciones[tipo].Count == 0)
                ViewBag.vinoAOlimpiada = ViewBag.estado.estadoVinoAOlimpiada(tipo, clave);

            return View(o);
        }

        //
        // GET: /Olimpiada/Estados/

        public ActionResult Estados(string clave, TipoOlimpiada tipo = TipoOlimpiada.OMI)
        {
            if (tipo != TipoOlimpiada.OMI)
                return RedirectTo(Pagina.ERROR, 404);

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(clave, tipo);

            if (o == null || o.numero == Olimpiada.TEMP_CLAVE)
                return RedirectTo(Pagina.ERROR, 404);

            ViewBag.estados = Medallero.obtenerTablaEstados(o.tipoOlimpiada, clave);
            ViewBag.olimpiadas = Olimpiada.obtenerOlimpiadas(tipo);
            ViewBag.hayPromedio = Medallero.hayPromedio(ViewBag.estados);
            ViewBag.hayPuntos = Medallero.hayPuntos(ViewBag.estados);

            return View(o);
        }

        //
        // GET: /Olimpiada/Numeros/

        public ActionResult Numeros(string clave, TipoOlimpiada tipo = TipoOlimpiada.OMI)
        {
            if (!esAdmin() || clave == null)
                return RedirectTo(Pagina.HOME);

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(clave, tipo);

            if (o == null || clave == Olimpiada.TEMP_CLAVE)
                return RedirectTo(Pagina.ERROR, 404);

            o.calcularNumeros();

            return RedirectTo(Pagina.OLIMPIADA, clave);
        }
    }
}
