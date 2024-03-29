﻿using OMIstats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OMIstats.Controllers
{
    public class EstadoController : BaseController
    {
        //
        // GET: /Estado/

        public ActionResult Index(string clave, TipoOlimpiada tipo = TipoOlimpiada.OMI)
        {
            Estado e = Estado.obtenerEstadoConClave(clave);

            if (e == null)
                return RedirectTo(Pagina.ERROR, 404);

            Medalleros m = Medallero.obtenerMedalleros(Medallero.TipoMedallero.ESTADO, e.clave);
            tipo = m.obtenerDefault(tipo);

            limpiarErroresViewBag();
            ViewBag.sedes = e.obtenerOlimpiadasSede();
            ViewBag.participantes = Resultados.obtenerAlumnosDeEstado(clave, tipo);
            ViewBag.medallas = m.medalleroDeTipo(tipo);
            ViewBag.medalleros = m;
            ViewBag.tipo = tipo;
            ViewBag.estados = Estado.obtenerEstados();

            return View(e);
        }

        //
        // GET: /Estado/Desempeño/

        public ActionResult Desempeño(string clave)
        {
            Estado e = Estado.obtenerEstadoConClave(clave);

            if (e == null)
                return RedirectTo(Pagina.ERROR, 404);

            ViewBag.medalleros = Medallero.obtenerMedalleros(Medallero.TipoMedallero.ESTADO, e.clave);
            ViewBag.resultados = Medallero.obtenerDesempeñoEstado(clave);
            ViewBag.olimpiadas = Olimpiada.obtenerOlimpiadas(TipoOlimpiada.OMI);
            ViewBag.estados = Estado.obtenerEstados();

            return View(e);
        }

        //
        // GET: /Estado/Edit/

        public ActionResult Edit(string estado)
        {
            if (!estaLoggeado())
            {
                guardarParams(Pagina.LOGIN, Pagina.EDIT_ESTADO, estado);
                return RedirectTo(Pagina.LOGIN);
            }

            Estado e = Estado.obtenerEstadoConClave(estado);

            if (e == null)
                return RedirectTo(Pagina.ERROR, 404);

            Persona p = getUsuario();
            if (!(esAdmin() || p.clave == e.claveDelegado))
                return RedirectTo(Pagina.ERROR, 401);

            limpiarErroresViewBag();

            return View(e);
        }

        //
        // POST: /Estado/Edit/

        [HttpPost]
        public ActionResult Edit(HttpPostedFileBase file, Estado estado)
        {
            if (!estaLoggeado() || estado == null)
                RedirectTo(Pagina.HOME);

            Estado e = Estado.obtenerEstadoConClave(estado.clave);
            if (e == null)
                return RedirectTo(Pagina.ERROR, 404);

            Persona p = getUsuario();
            if (!(esAdmin() || p.clave == e.claveDelegado))
                RedirectTo(Pagina.HOME);

            limpiarErroresViewBag();

            if (!String.IsNullOrEmpty(estado.nombreDelegado) || !String.IsNullOrEmpty(estado.mailDelegado))
            {
                ViewBag.delegadoModificado = true;
                if (String.IsNullOrEmpty(estado.nombreDelegado))
                {
                    ViewBag.errorUsuario = ERROR;
                    return View(estado);
                }
                if (String.IsNullOrEmpty(estado.mailDelegado))
                {
                    ViewBag.errorMail = ERROR;
                    return View(estado);
                }
            }
            else
            {
                ViewBag.delegadoModificado = false;
            }

            if (!ModelState.IsValid)
                return View(estado);

            // Validaciones logo
            if (file != null)
            {
                Utilities.Archivos.ResultadoImagen resultado = Utilities.Archivos.esImagenValida(file);
                if (resultado != Utilities.Archivos.ResultadoImagen.VALIDA)
                {
                    ViewBag.errorImagen = resultado.ToString().ToLower();
                    return View(estado);
                }
            }

            if (ViewBag.delegadoModificado)
            {
                estado.delegado = Persona.obtenerPersonaConNombre(estado.nombreDelegado);
                if (estado.delegado == null)
                {
                    estado.delegado = new Persona();
                    estado.delegado.breakNombre(estado.nombreDelegado);
                    estado.delegado.correo = estado.mailDelegado;
                    estado.delegado.nuevoUsuario(Utilities.Archivos.FotoInicial.DOMI);

                    Peticion pe = new Peticion();
                    pe.tipo = Peticion.TipoPeticion.USUARIO;
                    pe.usuario = estado.delegado;
                }
            }
            else
                estado.delegado = null;

            // Regresando los valores que solo super usuarios deberían de poder modificar
            // * JUST IN CASE *
            if (!getUsuario().esSuperUsuario())
            {
                estado.nombre = e.nombre;
                estado.ISO = e.ISO;
            }

            if (!estado.guardar())
                return RedirectTo(Pagina.ERROR, 500);

            if (file != null)
                Utilities.Archivos.guardaArchivo(file, e.clave + ".png", Utilities.Archivos.Folder.ESTADOS);

            ViewBag.guardado = true;
            return View(estado);
        }
    }
}
