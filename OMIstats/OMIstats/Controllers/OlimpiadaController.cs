﻿using OMIstats.Models;
using OMIstats.Ajax;
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
                return RedirectTo(Pagina.EDIT_OLIMPIADA, clave + ":" + tipo.ToString());

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(clave, tipo);

            if (o == null)
                return RedirectTo(Pagina.ERROR, 404);

            limpiarErroresViewBag();
            ViewBag.clave = clave;

            ViewBag.dia1 = Problema.obtenerProblemasDeOMI(clave, tipo, 1);
            ViewBag.dia2 = Problema.obtenerProblemasDeOMI(clave, tipo, 2);

            ViewBag.metadata = Problema.obetnerMetaDatadeOMI(clave, tipo);

            // Mientras las OMIS y OMIPS sean en el mismo evento que la OMI, no tienen su propia vista
            if (tipo == TipoOlimpiada.OMIS ||
                tipo == TipoOlimpiada.OMISO ||
                tipo == TipoOlimpiada.OMIP ||
                tipo == TipoOlimpiada.OMIPO)
                tipo = TipoOlimpiada.OMI;
            ViewBag.olimpiadas = Olimpiada.obtenerOlimpiadas(tipo);

            ViewBag.fotos = Album.obtenerAlbumsDeOlimpiada(clave, tipo).Count > 0;
            addFavicon(o.numero);

            return View(o);
        }

        //
        // GET: /Olimpiada/Nueva/

        public ActionResult Nueva(TipoOlimpiada tipo = TipoOlimpiada.OMI)
        {
            if (!estaLoggeado())
            {
                guardarParams(Pagina.LOGIN, Pagina.OLIMPIADAS, "");
                return RedirectTo(Pagina.LOGIN);
            }

            if (!esAdmin())
                return RedirectTo(Pagina.ERROR, 401);

            if (tipo == TipoOlimpiada.NULL)
                return RedirectTo(Pagina.ERROR, 404);

            Olimpiada.nuevaOMI(tipo);

            Log.add(Log.TipoLog.ADMIN, "Nueva " + tipo + " creada por admin " + getUsuario().nombreCompleto);

            return RedirectTo(Pagina.EDIT_OLIMPIADA, Olimpiada.TEMP_CLAVE + ":" + tipo.ToString());
        }

        //
        // GET: /Olimpiada/Edit/

        public ActionResult Edit(string clave, TipoOlimpiada tipo = TipoOlimpiada.OMI)
        {
            if (!estaLoggeado())
            {
                guardarParams(Pagina.LOGIN, Pagina.EDIT_OLIMPIADA, clave + ":" + tipo.ToString());
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

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(clave, omi.tipoOlimpiada);
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
                    omi.tipoOlimpiada == TipoOlimpiada.OMIA ? Utilities.Archivos.Folder.OMIA :
                    Utilities.Archivos.Folder.OLIMPIADAS);

            if (filePoster != null)
                Utilities.Archivos.guardaArchivo(filePoster, filePoster.FileName,
                    Utilities.Archivos.Folder.POSTERS);

            ViewBag.guardado = true;
            Log.add(Log.TipoLog.ADMIN, o.tipoOlimpiada + " " + o.año + " actualizada por admin " + getUsuario().nombreCompleto);

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
            {
                ViewBag.guardado = true;
                Log.add(Log.TipoLog.ADMIN, "Participantes de " + o.tipoOlimpiada + " " + o.año + " actualizados por admin " + getUsuario().nombre);
            }

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
        public ActionResult ResultsTable(string tabla, string clave, TipoOlimpiada tipo = TipoOlimpiada.OMI, bool run = false)
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
            string errores = o.guardarTablaResultados(tabla, run);

            if (errores.Length > 0)
            {
                ViewBag.errorOMI = true;
                ViewBag.resultados = errores;
            }
            else
            {
                ViewBag.guardado = true;
                Log.add(Log.TipoLog.ADMIN, "Resultados de " + o.tipoOlimpiada + " " + o.año + " actualizados por admin " + getUsuario().nombreCompleto);
            }

            return View(o);
        }

        //
        // GET: /Olimpiada/Resultados/

        public ActionResult Resultados(string clave = null, TipoOlimpiada tipo = TipoOlimpiada.OMI, string GUID = null)
        {
            if (GUID != null)
                tryLogIn(GUID);
            Olimpiada o = null;

            if (clave == null)
            {
                o = Olimpiada.obtenerMasReciente();
                clave = o.numero;
                tipo = o.tipoOlimpiada;
            }
            else
            {
                o = Olimpiada.obtenerOlimpiadaConClave(clave, tipo);
            }

            if (o == null)
                return RedirectTo(Pagina.ERROR, 404);

            limpiarErroresViewBag();

            ViewBag.liveResults = false;
            ViewBag.secretScoreboard = false;
            ViewBag.resultados = null;
            if (o.liveResults)
            {
                OmegaUp ou = o.calculateCachedResults();
                if (ou != null)
                {
                    Persona p = getUsuario();
                    if (o.esOnline && OmegaUp.RunnerStarted && p != null && p.esSuperUsuario())
                    {
                        ViewBag.secretScoreboard = true;
                        ViewBag.dia = ou.dia;
                        ViewBag.problemasPorDia = 4; // HARDCODED BUT OH WELL....
                        ViewBag.resultados = Models.Resultados.cargarResultadosSecretos(clave, tipo, ou.dia);
                    }
                    else
                    {
                        ViewBag.resultados = o.resultados;
                        if (o.resultados.Count > 0)
                        {
                            o.shouldReload(ou.dia);

                            ViewBag.liveResults = true;
                            ViewBag.RunnerStarted = OmegaUp.RunnerStarted;
                            ViewBag.dia = ou.dia;
                            ViewBag.problemasPorDia = ou.dia == 1 ? o.problemasDia1 : o.problemasDia2;
                            ViewBag.lastUpdate = (DateTime.UtcNow.Ticks - ou.timestamp.Ticks) / TimeSpan.TicksPerSecond;
                            ViewBag.ticks = ou.timestamp.Ticks;
                            ViewBag.scoreboardName = ou.concurso;
                            ViewBag.scoreboardToken = ou.token;
                            ViewBag.remainingSeconds = ou.getRemainingContestTime();
                        }
                    }
                }
            }
            if (ViewBag.resultados == null)
            {
                ViewBag.resultados = Models.Resultados.cargarResultados(clave, tipo, porLugar: !o.puntosDesconocidos && o.noMedallistasConocidos && o.datosPublicos, cargarObjetos: true);
            }

            ViewBag.problemasDia1 = Problema.obtenerProblemasDeOMI(clave, tipo, 1);
            ViewBag.problemasDia2 = Problema.obtenerProblemasDeOMI(clave, tipo, 2);
            ViewBag.claveUsuario = getUsuario().clave;
            ViewBag.olimpiadas = Olimpiada.obtenerOlimpiadas(tipo);

            if (o.alsoOmips)
                ViewBag.omis = Olimpiada.obtenerOlimpiadas(TipoOlimpiada.OMIS);
            if (o.alsoOmip)
                ViewBag.omip = Olimpiada.obtenerOlimpiadas(TipoOlimpiada.OMIP);

            List<Problema> metadata = Problema.obetnerMetaDatadeOMI(clave, tipo);

            if (metadata.Count >= 3)
            {
                ViewBag.numerosDia1 = metadata[1];
                ViewBag.numerosDia2 = metadata[2];
                ViewBag.numerosTotal = metadata[0];
            }

            ViewBag.extranjeros = MiembroDelegacion.obtenerEstadosExtranjerosEnOlimpiada(o.numero);
            addFavicon(o.numero);

            return View(o);
        }

        // POST: /Olimpiada/ResultadosAjax/

        [HttpPost]
        public JsonResult ResultadosAjax(string clave, TipoOlimpiada tipo, long ticks, bool retry)
        {
            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(clave, tipo);

            if (o == null)
                return Json(ERROR);

            OmegaUp ou = o.calculateCachedResults();
            ScoreboardAjax ajax = new ScoreboardAjax();

            if (ou == null || !OmegaUp.RunnerStarted)
            {
                ajax.status = ScoreboardAjax.Status.ERROR.ToString();
                return Json(ajax);
            }

            ajax.timeToFinish = ou.getRemainingContestTime();

            if (ajax.timeToFinish == 0)
            {
                ajax.resultados = o.cachedResults;
                ajax.status = ScoreboardAjax.Status.FINISHED.ToString();

                return Json(ajax);
            }

            // Retry es verdadero cuando le mandamos 0 problemas al cliente
            // en ese caso, es probable que necesitemos recagar los objetos
            // olimpiada de la base de datos
            if (retry)
            {
                ajax.retry = o.shouldReload(ou.dia);
                ajax.status = ScoreboardAjax.Status.NOT_CHANGED.ToString();

                return Json(ajax);
            }

            if (ou.timestamp.Ticks == ticks)
            {
                ajax.status = ScoreboardAjax.Status.NOT_CHANGED.ToString();
            }
            else
            {
                ajax.ticks = ou.timestamp.Ticks.ToString();
                ajax.status = ScoreboardAjax.Status.UPDATED.ToString();
                ajax.resultados = o.cachedResults;
                ajax.secondsSinceUpdate = (int)Math.Round((decimal)(DateTime.UtcNow.Ticks - ou.timestamp.Ticks) / TimeSpan.TicksPerSecond);
            }

            return Json(ajax);
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
            delegaciones.Add(tipo, MiembroDelegacion.obtenerMiembrosDelegacion(clave, estado, tipo, MiembroDelegacion.TipoAsistente.COMPETIDOR, listarPorAño: o.año));
            if (tipo == TipoOlimpiada.OMI)
            {
                if (o.alsoOmips)
                    delegaciones.Add(TipoOlimpiada.OMIS, MiembroDelegacion.obtenerMiembrosDelegacion(clave, estado, TipoOlimpiada.OMIS, MiembroDelegacion.TipoAsistente.COMPETIDOR, listarPorAño: o.año));
                if (o.alsoOmip)
                    delegaciones.Add(TipoOlimpiada.OMIP, MiembroDelegacion.obtenerMiembrosDelegacion(clave, estado, TipoOlimpiada.OMIP, MiembroDelegacion.TipoAsistente.COMPETIDOR, listarPorAño: o.año));
            }

            ViewBag.liveResults = o.liveResults;
            ViewBag.estado = e;
            ViewBag.delegaciones = delegaciones;
            ViewBag.lideres = MiembroDelegacion.obtenerMiembrosDelegacion(clave, estado, tipo, MiembroDelegacion.TipoAsistente.LIDER);
            ViewBag.otros = MiembroDelegacion.obtenerMiembrosDelegacion(clave, estado, tipo, MiembroDelegacion.TipoAsistente.INVITADO);
            ViewBag.delebs = MiembroDelegacion.obtenerMiembrosDelegacion(clave, estado, tipo, MiembroDelegacion.TipoAsistente.DELEB);
            ViewBag.medallas = Medallero.obtenerMedallas(tipo, Medallero.TipoMedallero.ESTADO_POR_OMI, estado + "_" + clave);
            ViewBag.olimpiadas = Olimpiada.obtenerOlimpiadas(tipo);
            ViewBag.olimpiadasParaEstado = Olimpiada.obtenerOlimpiadasParaEstado(estado);
            ViewBag.tipo = tipo;
            ViewBag.estadosEnOlimpiada = MiembroDelegacion.obtenerEstadosEnOlimpiada(o.numero);
            addFavicon(o.numero);

            if (delegaciones[tipo].Count == 0)
                ViewBag.vinoAOlimpiada = ViewBag.estado.estadoVinoAOlimpiada(tipo, clave);

            return View(o);
        }

        //
        // GET: /Olimpiada/Presentacion/

        public ActionResult Presentacion(string clave, string estado)
        {
            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(clave, TipoOlimpiada.OMI);

            if (o == null || o.numero == Olimpiada.TEMP_CLAVE)
                return RedirectTo(Pagina.ERROR, 404);

            Estado e = Estado.obtenerEstadoConClave(estado);

            if (e == null)
                return RedirectTo(Pagina.ERROR, 404);

            Dictionary<TipoOlimpiada, List<MiembroDelegacion>> delegaciones = new Dictionary<TipoOlimpiada, List<MiembroDelegacion>>();
            delegaciones.Add(TipoOlimpiada.OMI, MiembroDelegacion.obtenerMiembrosDelegacion(clave, estado, TipoOlimpiada.OMI, MiembroDelegacion.TipoAsistente.COMPETIDOR, listarPorAño: o.año));
            delegaciones.Add(TipoOlimpiada.OMIS, MiembroDelegacion.obtenerMiembrosDelegacion(clave, estado, TipoOlimpiada.OMIS, MiembroDelegacion.TipoAsistente.COMPETIDOR, listarPorAño: o.año));

            ViewBag.estado = e;
            ViewBag.delegaciones = delegaciones;
            ViewBag.lideres = MiembroDelegacion.obtenerMiembrosDelegacion(clave, estado, TipoOlimpiada.OMI, MiembroDelegacion.TipoAsistente.LIDER);
            ViewBag.otros = MiembroDelegacion.obtenerMiembrosDelegacion(clave, estado, TipoOlimpiada.OMI, MiembroDelegacion.TipoAsistente.INVITADO);
            ViewBag.estadosEnOlimpiada = MiembroDelegacion.obtenerEstadosEnOlimpiada(o.numero);

            return View(o);
        }

        //
        // GET: /Olimpiada/Estados/

        public ActionResult Estados(string clave = null, TipoOlimpiada tipo = TipoOlimpiada.OMI)
        {
            if (tipo != TipoOlimpiada.OMI)
                return RedirectTo(Pagina.ERROR, 404);

            Olimpiada o;

            if (clave == null)
            {
                o = Olimpiada.obtenerMasReciente();
                clave = o.numero;
                tipo = o.tipoOlimpiada;
            }
            else
            {
                o = Olimpiada.obtenerOlimpiadaConClave(clave, tipo);
            }

            if (o == null || o.numero == Olimpiada.TEMP_CLAVE)
                return RedirectTo(Pagina.ERROR, 404);

            Medallero medalleroGeneral;
            Persona p = getUsuario();

            ViewBag.liveResults = o.liveResults;
            if (o.liveResults && o.esOnline && OmegaUp.RunnerStarted && p != null && p.esSuperUsuario())
            {
                ViewBag.secretScoreboard = true;
                ViewBag.estados = Medallero.obtenerTablaEstadosSecreta(o.invitados > 0, clave, o.tipoOlimpiada);
                ViewBag.medalleroGeneral = null;
            }
            else
            {
                ViewBag.secretScoreboard = false;
                ViewBag.estados = Medallero.obtenerTablaEstados(o.tipoOlimpiada, clave, out medalleroGeneral);
                ViewBag.medalleroGeneral = medalleroGeneral;
            }
            ViewBag.olimpiadas = Olimpiada.obtenerOlimpiadas(tipo);
            ViewBag.hayPromedio = Medallero.hayPromedio(ViewBag.estados);
            ViewBag.hayPuntos = Medallero.hayPuntos(ViewBag.estados);
            addFavicon(o.numero);

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

        // POST: /Olimpiada/OverlayAjax/

        [HttpPost]
        public JsonResult OverlayAjax(string omi, TipoOlimpiada tipo, string clave)
        {
            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, tipo);

            if (o == null)
                return Json(ERROR);

            OverlayAjax response = new OverlayAjax();
            response.puntosD1 = DetallePuntos.cargarResultados(omi, tipo, clave, 1, o.problemasDia1);
            response.lugaresD1 = DetalleLugar.cargarResultados(omi, tipo, 1, clave);
            if (o.problemasDia2 > 0)
            {
                response.puntosD2 = DetallePuntos.cargarResultados(omi, tipo, clave, 2, o.problemasDia2);
                response.lugaresD2 = DetalleLugar.cargarResultados(omi, tipo, 2, clave);
            }
            response.problemas = Models.Resultados.cargarMejores(omi, tipo, clave, o.problemasDia1, o.problemasDia2);

            return Json(response);
        }

        //
        // GET: /Olimpiada/Diplomas/

        public ActionResult Diplomas(string clave)
        {
            if (!esAdmin() || clave == null)
                return RedirectTo(Pagina.HOME);
            ViewBag.omi = clave;

            return View();
        }

        //
        // POST: /Olimpiada/Diplomas/

        [HttpPost]
        public ActionResult Diplomas(string clave, string textoX, string textoY, string textoZ, bool? naked)
        {
            if (!esAdmin() || clave == null || textoX == null || textoY == null)
                return RedirectTo(Pagina.HOME);
            ViewBag.omi = clave;

            string[] X = textoX.Split(';');
            string[] Y = textoY.Split(';');
            string[] Z = textoZ.Split(';');
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');

            ViewBag.asistentes = MiembroDelegacion.generarDiplomas(clave, X[0], baseUrl, Y, Z[0], naked == true);
            ViewBag.medallistas = Models.Resultados.generarDiplomas(clave, X[1], baseUrl, Z[1], naked == true);
            ViewBag.especiales = Models.Resultados.generarDiplomasEspeciales(clave, baseUrl);

            return View();
        }
    }
}
