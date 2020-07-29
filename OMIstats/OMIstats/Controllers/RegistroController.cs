using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OMIstats.Models;

namespace OMIstats.Controllers
{
    public class RegistroController : BaseController
    {
        private bool tienePermisos(bool registroActivo, string estado = null)
        {
            if (!estaLoggeado())
                return false;
            Persona p = getUsuario();

            if (p.permisos == Persona.TipoPermisos.NORMAL)
                return false;

            if (p.esSuperUsuario())
                return true;

            if (!registroActivo)
                return false;

            if (estado != null)
            {
                List<Estado> estados = p.obtenerEstadosDeDelegado();
                if (!estados.Any(e => e.clave == estado))
                    return false;
            }

            return true;
        }

        //
        // GET: /Registro/

        public ActionResult Index()
        {
            return Select();
        }

        //
        // GET: /Registro/Select

        public ActionResult Select(string omi = null)
        {
            if (omi == null)
                omi = Olimpiada.obtenerMasReciente().numero;

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, TipoOlimpiada.OMI);
            if (o == null || !tienePermisos(o.registroActivo))
                return RedirectTo(Pagina.HOME);

            Persona p = getUsuario();

            if (p.esSuperUsuario())
                return RedirectTo(Pagina.REGISTRO);

            List<Estado> estados = p.obtenerEstadosDeDelegado();
            if (estados.Count == 1)
                return RedirectTo(Pagina.REGISTRO, new { omi = omi, estado = estados[0].clave });
            ViewBag.estados = estados;

            return View();
        }

        //
        // GET: /Registro/Delegacion

        public ActionResult Delegacion(string omi = null, string estado = null)
        {
            if (omi == null)
                omi = Olimpiada.obtenerMasReciente().numero;

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, TipoOlimpiada.OMI);
            if (o == null || !tienePermisos(o.registroActivo, estado))
                return RedirectTo(Pagina.HOME);

            Persona p = getUsuario();

            if (!p.esSuperUsuario())
            {
                if (estado == null)
                    return RedirectTo(Pagina.HOME);
                ViewBag.estado = Estado.obtenerEstadoConClave(estado);
            }

            List<MiembroDelegacion> registrados = MiembroDelegacion.obtenerMiembrosDelegacion(omi, p.esSuperUsuario() ? null : estado, TipoOlimpiada.NULL);
            ViewBag.omi = o;
            return View(registrados);
        }

        //
        // POST: /Registro/Buscar/

        [HttpPost]
        public JsonResult Buscar(string omi, TipoOlimpiada tipo, string query, string estado)
        {
            return Json(MiembroDelegacion.buscarParaRegistro(omi, tipo, estado, query));
        }

        //
        // GET: /Registro/Eliminar

        public ActionResult Eliminar(string omi, TipoOlimpiada tipo, string estado, string clave)
        {
            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, TipoOlimpiada.OMI);
            if (o == null || !tienePermisos(o.registroActivo, estado))
                return RedirectTo(Pagina.HOME);

            MiembroDelegacion md = MiembroDelegacion.obtenerMiembrosConClave(omi, tipo, clave)[0];
            if (md.estado != estado)
                return RedirectTo(Pagina.HOME);
            md.borrarMiembroDelegacion();

            return RedirectTo(Pagina.REGISTRO, new { omi = omi, estado = estado });
        }

        //
        // GET: /Registro/Asistente

        public ActionResult Asistente(string omi, TipoOlimpiada tipo = TipoOlimpiada.NULL, string estado = null, string clave = null)
        {
            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, TipoOlimpiada.OMI);
            if (o == null || !tienePermisos(o.registroActivo, estado))
                return RedirectTo(Pagina.HOME);

            Persona p = getUsuario();

            if (!p.esSuperUsuario())
            {
                if (estado == null)
                    return RedirectTo(Pagina.HOME);
                ViewBag.estado = Estado.obtenerEstadoConClave(estado);
            }

            MiembroDelegacion md = null;
            TipoOlimpiada tipoOriginal = TipoOlimpiada.NULL;
            if (clave == null)
            {
                if (!p.esSuperUsuario())
                    ViewBag.claveDisponible = MiembroDelegacion.obtenerPrimerClaveDisponible(omi, tipo, estado);
                ViewBag.tipoAsistente = MiembroDelegacion.TipoAsistente.NULL;
            }
            else
            {
                var temp = MiembroDelegacion.obtenerMiembrosConClave(omi, tipo, clave);
                if (temp.Count != 1)
                    return RedirectTo(Pagina.HOME);
                md = temp[0];
                ViewBag.claveOriginal = md.clave;
                ViewBag.claveDisponible = md.clave;
                ViewBag.tipoAsistente = md.tipo;
                ViewBag.estado = Estado.obtenerEstadoConClave(md.estado);
                tipoOriginal = md.tipoOlimpiada;
            }

            ViewBag.md = md;
            ViewBag.nuevo = (clave == null);
            ViewBag.omi = o;
            ViewBag.tipo = tipo;
            ViewBag.estados = Estado.obtenerEstados();
            ViewBag.tipoOriginal = tipoOriginal;
            limpiarErroresViewBag();
            ViewBag.resubmit = false;
            ViewBag.guardado = false;

            p = md == null ? new Persona() : Persona.obtenerPersonaConClave(md.claveUsuario);
            p.breakNombre();

            return View(p);
        }

        //
        // POST: /Registro/Asistente
        [HttpPost]
        public ActionResult Asistente(HttpPostedFileBase file, Persona p, string omi, string tipo, string tipoAsistente, string tipoOriginal, string estado, string claveSelect, string persona, string claveOriginal)
        {
            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, TipoOlimpiada.OMI);
            if (o == null ||
                String.IsNullOrEmpty(estado) ||
                String.IsNullOrEmpty(tipo) ||
                String.IsNullOrEmpty(tipoAsistente) ||
                !tienePermisos(o.registroActivo, estado))
                return RedirectTo(Pagina.HOME);

            MiembroDelegacion md = null;
            MiembroDelegacion.TipoAsistente asistente = (MiembroDelegacion.TipoAsistente)Enum.Parse(typeof(MiembroDelegacion.TipoAsistente), tipoAsistente);
            TipoOlimpiada tipoOlimpiada = (TipoOlimpiada)Enum.Parse(typeof(TipoOlimpiada), tipo);
            TipoOlimpiada tipoO = TipoOlimpiada.NULL;
            if (!String.IsNullOrEmpty(claveOriginal))
            {
                tipoO = (TipoOlimpiada)Enum.Parse(typeof(TipoOlimpiada), tipoOriginal);
                var temp = MiembroDelegacion.obtenerMiembrosConClave(omi, tipoO, claveOriginal);
                if (temp.Count != 1)
                    return RedirectTo(Pagina.HOME);
                md = temp[0];
            }

            ViewBag.claveDisponible = claveSelect;
            ViewBag.estado = Estado.obtenerEstadoConClave(estado);
            ViewBag.md = md;
            ViewBag.nuevo = String.IsNullOrEmpty(claveOriginal);
            ViewBag.omi = o;
            ViewBag.tipo = tipoOlimpiada;
            ViewBag.estados = Estado.obtenerEstados();
            ViewBag.tipoAsistente = asistente;
            ViewBag.claveOriginal = claveOriginal;
            ViewBag.tipoOriginal = tipoO;
            limpiarErroresViewBag();
            ViewBag.resubmit = true;

            if (file != null)
            {
                var valida = Utilities.Archivos.esImagenValida(file, Peticion.TamañoFotoMaximo);
                if (valida != Utilities.Archivos.ResultadoImagen.VALIDA)
                {
                    ViewBag.errorImagen = valida.ToString().ToLower();
                    return View(p);
                }
            }

            if (!ModelState.IsValid)
                return View(p);

            if (String.IsNullOrEmpty(claveOriginal))
            {
                // Nuevo asistente
            }
            else
            {
                // Modificando asistente

                // Primero los datos de persona
                p.foto = guardaFoto(file, p.clave);
                Persona per = Persona.obtenerPersonaConClave(md.claveUsuario, completo: true, incluirDatosPrivados: true);
                p.clave = per.clave;
                p.guardarDatos(generarPeticiones: false, lugarGuardado: Persona.LugarGuardado.REGISTRO);

                // Luego el miembro delegacion
                md.tipoOlimpiada = tipoOlimpiada;
                md.estado = estado;
                md.clave = claveSelect;
                md.tipo = (MiembroDelegacion.TipoAsistente)Enum.Parse(typeof(MiembroDelegacion.TipoAsistente), tipoAsistente.ToString().ToUpper());
                md.guardarDatos(claveOriginal, tipoO);
            }

            ViewBag.guardado = true;
            return View(p);
        }

        private string guardaFoto(HttpPostedFileBase file, int clave)
        {
            if (file != null)
            {
                return Utilities.Archivos.guardaArchivo(file, clave.ToString(), Utilities.Archivos.FolderImagenes.USUARIOS);
            }

            return "";
        }
    }
}
