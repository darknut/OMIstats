using OMIstats.Models;
using OMIstats.Utilities;
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

        public ActionResult Index(string clave)
        {
            Estado e = Estado.obtenerEstadoConClave(clave);

            if (e == null)
                return RedirectTo(Pagina.ERROR, 404);

            limpiarErroresViewBag();
            ViewBag.sedes = e.obtenerOlimpiadasSede();
            ViewBag.participantes = Resultados.obtenerAlumnosDeEstado(clave, Olimpiada.TipoOlimpiada.OMI);
            ViewBag.medallas = Medallero.obtenerMedallas(Olimpiada.TipoOlimpiada.OMI, Medallero.TipoMedallero.ESTADO, clave);

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

            // Borramos estos datos para que el delegado no haga cambios accidentales
            e.nombreDelegado = "";
            e.mailDelegado = "";

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
                Archivos.ResultadoImagen resultado = Archivos.esImagenValida(file);
                if (resultado != Archivos.ResultadoImagen.VALIDA)
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
                    estado.delegado.nombre = estado.nombreDelegado;
                    estado.delegado.correo = estado.mailDelegado;
                    estado.delegado.nuevoUsuario(Archivos.FotoInicial.DOMI);

                    Peticion pe = new Peticion();
                    pe.tipo = Peticion.TipoPeticion.USUARIO;
                    pe.subtipo = Peticion.TipoPeticion.BIENVENIDO;
                    pe.usuario = estado.delegado;
                    // pe.guardarPeticion(); -TODO- Descomentar esta linea para enviar correo de bienvenida
                }
            }
            else
                estado.delegado = null;

            if (!estado.guardar())
                return RedirectTo(Pagina.ERROR, 500);

            if (file != null)
                Archivos.guardaArchivo(file, e.clave + ".png", Archivos.FolderImagenes.ESTADOS);

            ViewBag.guardado = true;
            return View(estado);
        }
    }
}
