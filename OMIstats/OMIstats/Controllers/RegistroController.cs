using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OMIstats.Models;
using OMIstats.Utilities;

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
                omi = Olimpiada.obtenerMasReciente(yaEmpezada: false).numero;

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
                omi = Olimpiada.obtenerMasReciente(yaEmpezada: false).numero;

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
            ViewBag.hayResultados = Resultados.hayResultadosParaOMI(o.numero);
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
        // POST: /Registro/Escuelas/

        [HttpPost]
        public JsonResult Escuelas(TipoOlimpiada tipo, string estado)
        {
            List<object> lista = new List<object>();
            lista.Add(tipo.ToString());
            lista.Add(estado);
            lista.Add(Institucion.obtenerEscuelasDeEstado(tipo, estado));
            return Json(lista);
        }

        //
        // GET: /Registro/Eliminar

        public ActionResult Eliminar(string omi, TipoOlimpiada tipo, string estado, string clave)
        {
            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, TipoOlimpiada.OMI);
            if (o == null || !tienePermisos(o.registroActivo, estado) || Resultados.hayResultadosParaOMI(omi))
                return RedirectTo(Pagina.HOME);

            MiembroDelegacion md = MiembroDelegacion.obtenerMiembrosConClave(omi, tipo, clave)[0];
            if (md.estado != estado)
                return RedirectTo(Pagina.HOME);
            md.borrarMiembroDelegacion();

            return RedirectTo(Pagina.REGISTRO, new { omi = omi, estado = estado });
        }

        private void failSafeViewBag()
        {
            ViewBag.nuevo = false;
            ViewBag.errorInfo = "";
            ViewBag.tipo = TipoOlimpiada.NULL;
            ViewBag.tipoAsistente = MiembroDelegacion.TipoAsistente.NULL;
            ViewBag.tipoOriginal = TipoOlimpiada.NULL;
            ViewBag.hayResultados = false;
            ViewBag.resubmit = false;
            ViewBag.guardado = false;
            ViewBag.nombreEscuela = "";
            ViewBag.claveEscuela = 0;
            ViewBag.añoEscuela = 0;
            ViewBag.nivelEscuela = "";
        }

        //
        // GET: /Registro/Asistente

        public ActionResult Asistente(string omi, TipoOlimpiada tipo = TipoOlimpiada.NULL, string estado = null, string clave = null)
        {
            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, TipoOlimpiada.OMI);
            if (o == null)
                return RedirectTo(Pagina.HOME);
            failSafeViewBag();
            ViewBag.omi = o;
            if (!tienePermisos(o.registroActivo, estado))
            {
                ViewBag.errorInfo = "permisos";
                return View(new Persona());
            }

            Persona p = getUsuario();

            if (!p.esSuperUsuario())
            {
                if (estado == null ||
                    (String.IsNullOrEmpty(clave) &&
                     tipo != TipoOlimpiada.NULL &&
                     !puedeRegistrarOtroMas(o, tipo, estado)))
                {
                    ViewBag.errorInfo = "limite";
                    return View(new Persona());
                }

                ViewBag.estado = Estado.obtenerEstadoConClave(estado);
            }

            MiembroDelegacion md = null;
            TipoOlimpiada tipoOriginal = TipoOlimpiada.NULL;
            if (clave == null)
            {
                if (!p.esSuperUsuario() && tipo != TipoOlimpiada.NULL)
                    ViewBag.claveDisponible = MiembroDelegacion.obtenerPrimerClaveDisponible(omi, tipo, estado);
                ViewBag.tipoAsistente = MiembroDelegacion.TipoAsistente.NULL;
            }
            else
            {
                var temp = MiembroDelegacion.obtenerMiembrosConClave(omi, tipo, clave);
                if (temp.Count == 0)
                {
                    ViewBag.errorInfo = "invalido";
                    return View(new Persona());
                }
                if (temp.Count > 1)
                {
                    ViewBag.errorInfo = "duplicado";
                    return View(new Persona());
                }
                md = temp[0];
                ViewBag.claveOriginal = md.clave;
                ViewBag.claveDisponible = md.clave;
                ViewBag.tipoAsistente = md.tipo;
                ViewBag.estado = Estado.obtenerEstadoConClave(md.estado);
                ViewBag.nombreEscuela = md.nombreEscuela;
                ViewBag.claveEscuela = md.claveEscuela;
                ViewBag.añoEscuela = md.añoEscuela;
                ViewBag.nivelEscuela = md.nivelEscuela.ToString();
                tipoOriginal = md.tipoOlimpiada;
            }

            ViewBag.md = md;
            ViewBag.nuevo = (clave == null);
            ViewBag.tipo = tipo;
            ViewBag.estados = Estado.obtenerEstados();
            ViewBag.tipoOriginal = tipoOriginal;
            limpiarErroresViewBag();
            ViewBag.resubmit = false;
            ViewBag.guardado = false;
            ViewBag.hayResultados = Resultados.hayResultadosParaOMI(o.numero);

            p = md == null ? new Persona() : Persona.obtenerPersonaConClave(md.claveUsuario, completo: true, incluirDatosPrivados: true);
            p.breakNombre();

            List<Ajax.BuscarEscuelas> escuelas = null;
            if (md != null && md.tipo == MiembroDelegacion.TipoAsistente.COMPETIDOR)
            {
                escuelas = Institucion.obtenerEscuelasDeEstado(md.tipoOlimpiada, md.estado);
            }
            else if (estado != null)
            {
                if (tipo != TipoOlimpiada.NULL)
                    escuelas = Institucion.obtenerEscuelasDeEstado(tipo, estado);
            }
            ViewBag.escuelas = escuelas;

            return View(p);
        }

        //
        // POST: /Registro/Asistente
        [HttpPost]
        public ActionResult Asistente(HttpPostedFileBase file, Persona p, string omi, string tipo, string tipoAsistente,
            string tipoOriginal, string estado, string claveSelect, string persona, string claveOriginal,
            int selectEscuela, string nombreEscuela, int selectAnioEscolar, Institucion.NivelInstitucion selectNivelEscolar = Institucion.NivelInstitucion.NULL)
        {
            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, TipoOlimpiada.OMI);
            if (o == null)
                return RedirectTo(Pagina.HOME);
            failSafeViewBag();
            ViewBag.omi = o;
            if (String.IsNullOrEmpty(estado) ||
                String.IsNullOrEmpty(tipo) ||
                String.IsNullOrEmpty(tipoAsistente) ||
                !tienePermisos(o.registroActivo, estado))
            {
                ViewBag.errorInfo = "permisos";
                return View(new Persona());
            }

            MiembroDelegacion.TipoAsistente asistente = EnumParser.ToTipoAsistente(tipoAsistente);
            TipoOlimpiada tipoOlimpiada = EnumParser.ToTipoOlimpiada(tipo);
            if (!esAdmin() &&
                 String.IsNullOrEmpty(claveOriginal) &&
                 asistente == MiembroDelegacion.TipoAsistente.COMPETIDOR &&
                 !puedeRegistrarOtroMas(o, tipoOlimpiada, estado))
            {
                ViewBag.errorInfo = "limite";
                return View(new Persona());
            }

            MiembroDelegacion md = null;
            TipoOlimpiada tipoO = TipoOlimpiada.NULL;
            Institucion i = null;
            if (!String.IsNullOrEmpty(claveOriginal))
            {
                tipoO = EnumParser.ToTipoOlimpiada(tipoOriginal);
                var temp = MiembroDelegacion.obtenerMiembrosConClave(omi, tipoO, claveOriginal);
                if (temp.Count == 0)
                {
                    ViewBag.errorInfo = "invalido";
                    return View(new Persona());
                }
                if (temp.Count > 1)
                {
                    ViewBag.errorInfo = "duplicado";
                    return View(new Persona());
                }
                md = temp[0];
            }

            Estado e = Estado.obtenerEstadoConClave(estado);
            if (claveSelect != null && asistente == MiembroDelegacion.TipoAsistente.COMPETIDOR && !claveSelect.StartsWith(e.ISO))
                claveSelect = "";
            if (persona != "0")
                p.clave = int.Parse(persona);
            ViewBag.claveDisponible = claveSelect;
            ViewBag.estado = e;
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
            ViewBag.hayResultados = Resultados.hayResultadosParaOMI(o.numero);

            if (asistente == MiembroDelegacion.TipoAsistente.COMPETIDOR)
            {
                ViewBag.escuelas = Institucion.obtenerEscuelasDeEstado(tipoOlimpiada, estado);
                ViewBag.claveEscuela = selectEscuela;
                ViewBag.añoEscuela = selectAnioEscolar;
                ViewBag.nivelEscuela = selectNivelEscolar.ToString();

                if (selectEscuela > 0)
                {
                    i = Institucion.obtenerInstitucionConClave(selectEscuela);
                    if (i == null)
                    {
                        ViewBag.nombreEscuela = "";
                        ViewBag.claveEscuela = 0;
                    }
                    else
                        ViewBag.nombreEscuela = i.nombre;
                }
                else
                {
                    ViewBag.nombreEscuela = nombreEscuela;
                }
            }

            if (file != null)
            {
                var valida = Archivos.esImagenValida(file, Peticion.TamañoFotoMaximo);
                if (valida != Archivos.ResultadoImagen.VALIDA)
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
                if (persona == "0")
                {
                    // Si no hay clave de persona previa, se agrega una nueva persona
                    p.nuevoUsuario();
                }
                else
                {
                    // Si tengo clave, se intenta conseguir
                    Persona per = Persona.obtenerPersonaConClave(int.Parse(persona));
                    if (per == null)
                    {
                        ViewBag.errorInfo = "persona";
                        return View(new Persona());
                    }

                    p.clave = per.clave;
                }

                string fotoURL = guardaFoto(file, p.clave);
                if (!String.IsNullOrEmpty(fotoURL))
                    p.foto = fotoURL;
                p.guardarDatos(generarPeticiones: false, lugarGuardado: Persona.LugarGuardado.REGISTRO);

                // Se genera un nuevo miembro delegacion
                md = new MiembroDelegacion();
                md.olimpiada = o.numero;
                md.tipoOlimpiada = tipoOlimpiada;
                md.estado = estado;
                md.clave = claveSelect;
                md.tipo = asistente;
                md.claveUsuario = p.clave;
                md.nuevo();
            }
            else
            {
                // Si ya hay resultados no podemos cambiar la clave, estado, o tipo de OMI
                if (Resultados.hayResultadosParaOMI(o.numero))
                {
                    tipoOlimpiada = md.tipoOlimpiada;
                    claveSelect = md.clave;
                    estado = md.estado;

                    // Adicionalmente si es competidor, no se piuede cambiar su tipo de asistencia
                    // ni volver a alguien mas competidor
                    if (md.tipo == MiembroDelegacion.TipoAsistente.COMPETIDOR ||
                        asistente == MiembroDelegacion.TipoAsistente.COMPETIDOR)
                        asistente = md.tipo;
                }

                // Modificando asistente
                // Primero los datos de persona
                Persona per = Persona.obtenerPersonaConClave(md.claveUsuario);
                p.clave = per.clave;
                p.foto = guardaFoto(file, p.clave);
                p.guardarDatos(generarPeticiones: false, lugarGuardado: Persona.LugarGuardado.REGISTRO);

                // Luego el miembro delegacion
                md.tipoOlimpiada = tipoOlimpiada;
                md.estado = estado;
                md.clave = claveSelect;
                md.tipo = asistente;
                md.guardarDatos(claveOriginal, tipoO);
            }

            ViewBag.guardado = true;
            return View(p);
        }

        private string guardaFoto(HttpPostedFileBase file, int clave)
        {
            if (file != null)
            {
                return Archivos.guardaArchivo(file, clave.ToString(), Archivos.FolderImagenes.USUARIOS, appendExtension: true, returnRelativeFolder: true);
            }

            return "";
        }

        private bool puedeRegistrarOtroMas(Olimpiada o, TipoOlimpiada tipo, string estado)
        {
            int maxUsuarios = o.getMaxParticipantesDeEstado(estado);
            List<MiembroDelegacion> mds = MiembroDelegacion.obtenerMiembrosDelegacion(o.numero, estado, tipo, MiembroDelegacion.TipoAsistente.COMPETIDOR);
            return (mds.Count < maxUsuarios);
        }
    }
}
