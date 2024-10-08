﻿using OMIstats.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OMIstats.Controllers
{
    public class EscuelaController : BaseController
    {
        //
        // GET: /Escuela/

        public ActionResult Index(string url = null, string clave = null, string omi = null, TipoOlimpiada tipo = TipoOlimpiada.OMI)
        {
            Institucion i = null;
            if (url == null && clave != null)
            {
                List<MiembroDelegacion> md = MiembroDelegacion.obtenerMiembrosConClave(omi, tipo, clave);
                if (md.Count == 0)
                    return RedirectTo(Pagina.ERROR, 404);
                i = Institucion.obtenerInstitucionConClave(md[0].claveEscuela);
                url = i.nombreURL;
            }

            if (i == null)
                i = Institucion.obtenerInstitucionConNombreURL(url);
            if (i == null)
                return RedirectTo(Pagina.ERROR, 404);

            Medalleros m = Medallero.obtenerMedalleros(Medallero.TipoMedallero.INSTITUCION, i.clave.ToString());
            tipo = m.obtenerDefault(tipo);
            Medallero medallero = m.medalleroDeTipo(tipo);
            if (medallero == null)
                medallero = new Medallero(tipo);

            ViewBag.sedes = i.obtenerOlimpiadasSede();
            ViewBag.participantes = Resultados.obtenerAlumnosDeInstitucion(i.clave, tipo);
            ViewBag.medallas = medallero;
            ViewBag.medalleros = m;
            ViewBag.tipo = tipo;
            limpiarErroresViewBag();

            return View(i);
        }

        //
        // GET: /Escuela/Edit/

        public ActionResult Edit(string url)
        {
            if (!estaLoggeado())
            {
                guardarParams(Pagina.LOGIN, Pagina.EDIT_ESCUELA, url);
                return RedirectTo(Pagina.LOGIN);
            }

            Institucion i = Institucion.obtenerInstitucionConNombreURL(url);

            if (i == null)
                return RedirectTo(Pagina.ERROR, 404);

            if (!esAdmin()) // -TODO- Agregar validacion para usuarios
                return RedirectTo(Pagina.ERROR, 401);

            limpiarErroresViewBag();

            return View(i);
        }

        //
        // POST: /Escuela/Edit/

        [HttpPost]
        public ActionResult Edit(HttpPostedFileBase file, Institucion escuela)
        {
            if (!esAdmin() || escuela == null) // -TODO- Agregar validacion para usuarios
                RedirectTo(Pagina.HOME);

            limpiarErroresViewBag();

            if (!esAdmin() && !revisaCaptcha())
            {
                ViewBag.errorCaptcha = true;
                return View(escuela);
            }

            if (!ModelState.IsValid)
                return View(escuela);

            // Validaciones logo
            if (file != null)
            {
                Utilities.Archivos.ResultadoImagen resultado = Utilities.Archivos.esImagenValida(file);
                if (resultado != Utilities.Archivos.ResultadoImagen.VALIDA)
                {
                    ViewBag.errorImagen = resultado.ToString().ToLower();
                    return View(escuela);
                }
                escuela.logo = Utilities.Archivos.guardaArchivo(file, Path.GetFileNameWithoutExtension(file.FileName) + ".png");
            }

            // Se guardan los datos
            if (escuela.guardar(generarPeticiones: !esAdmin()))
            {
                if (esAdmin())
                {
                    if (file != null)
                    {
                        Utilities.Archivos.copiarArchivo(escuela.logo, Utilities.Archivos.Folder.TEMPORAL,
                                        escuela.clave.ToString(), Utilities.Archivos.Folder.ESCUELAS);
                        Utilities.Archivos.eliminarArchivo(escuela.logo, Utilities.Archivos.Folder.TEMPORAL);
                    }

                    guardarParams(Pagina.SAVED_ESCUELA, OK);
                    return RedirectTo(Pagina.SAVED_ESCUELA, escuela.nombreURL);
                }

                guardarParams(Pagina.SAVED_ESCUELA, ADMIN);
                return RedirectTo(Pagina.SAVED_ESCUELA, escuela.nombreURL);
            }
            else
            {
                guardarParams(Pagina.SAVED_ESCUELA, ERROR);
                return RedirectTo(Pagina.SAVED_ESCUELA, escuela.nombreURL);
            }
        }

        //
        // GET: /Escuela/Saved/

        public ActionResult Saved(string url)
        {
            string value = (string)obtenerParams(Pagina.SAVED_ESCUELA);
            limpiarParams(Pagina.SAVED_ESCUELA);

            if (value == null)
                return RedirectTo(Pagina.HOME);

            ViewBag.value = value;
            ViewBag.redirectURL = url;
            return View();
        }
    }
}
