using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
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

        public ActionResult Select(string omi = null, TipoOlimpiada tipo = TipoOlimpiada.OMI, string GUID = null)
        {
            if (GUID != null)
                tryLogIn(GUID);

            if (omi == null)
                omi = Olimpiada.obtenerMasReciente(yaEmpezada: false).numero;

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, tipo);
            if (o == null || !(o.registroActivo || o.registroSedes))
            {
                Olimpiada op = Olimpiada.obtenerOlimpiadaConClave(omi, TipoOlimpiada.OMIPO);
                if (op != null && (op.registroActivo || op.registroSedes))
                {
                    tipo = TipoOlimpiada.OMIPO;
                    o = op;
                }
            }
            if (o == null || !tienePermisos(o.registroActivo || o.registroSedes))
                return RedirectTo(Pagina.HOME);

            Persona p = getUsuario();

            if (p.esSuperUsuario())
                return RedirectTo(Pagina.REGISTRO, new { tipo = tipo });

            List<Estado> estados = p.obtenerEstadosDeDelegado();
            if (estados.Count == 1)
                return RedirectTo(Pagina.REGISTRO, new { omi = omi, estado = estados[0].clave, tipo = tipo });
            ViewBag.estados = estados;
            ViewBag.tipo = o.tipoOlimpiada;

            return View();
        }

        //
        // GET: /Registro/Delegacion

        public ActionResult Delegacion(string omi = null, string estado = null, TipoOlimpiada tipo = TipoOlimpiada.OMI)
        {
            if (omi == null)
                omi = Olimpiada.obtenerMasReciente(yaEmpezada: false).numero;

            failSafeViewBag();
            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, tipo);
            ViewBag.omi = o == null ? new Olimpiada() : o;
            if (o == null || !tienePermisos(o.registroActivo || o.registroSedes, estado))
            {
                ViewBag.permisos = true;
                return View(new List<MiembroDelegacion>());
            }

            Persona p = getUsuario();
            ViewBag.invitaciones = false;

            if (!p.esSuperUsuario())
            {
                if (estado == null)
                    return RedirectTo(Pagina.HOME);
                Estado e = Estado.obtenerEstadoConClave(estado);
                ViewBag.estado = e;
                ViewBag.invitaciones =
                    Archivos.existeArchivo(Archivos.Folder.INVITACIONES, omi + "\\" + estado + "\\" +  e.ISO + "-1.pdf") ||
                    Archivos.existeArchivo(Archivos.Folder.INVITACIONES, omi + "\\" + estado + "\\S-" +  e.ISO + "-1.pdf") ||
                    Archivos.existeArchivo(Archivos.Folder.INVITACIONES, omi + "\\" + estado + "\\P-" +  e.ISO + "-1.pdf");
            }

            List<MiembroDelegacion> registrados = MiembroDelegacion.obtenerMiembrosDelegacion(omi, p.esSuperUsuario() ? null : estado, o.tipoOlimpiada);
            ViewBag.hayResultados = Resultados.hayResultadosParaOMI(o.numero);
            if (o.esOnline)
            {
                List<SedeOnline> sedes = SedeOnline.obtenerSedes(omi, p.esSuperUsuario() ? null : estado, tipo);
                Dictionary<int, List<MiembroDelegacion>> miembrosPorSede = new Dictionary<int, List<MiembroDelegacion>>();
                foreach (SedeOnline sede in sedes)
                {
                    List<MiembroDelegacion> miembrosEnSede = MiembroDelegacion.obtenerMiembrosEnSede(sede.clave);
                    miembrosPorSede.Add(sede.clave, miembrosEnSede);
                }
                ViewBag.miembrosPorSede = miembrosPorSede;
                ViewBag.sedes = sedes;
            }

            return View(registrados);
        }

        //
        // POST: /Registro/Buscar/

        [HttpPost]
        public JsonResult Buscar(string omi, TipoOlimpiada tipo, string query, string estado)
        {
            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, tipo);
            if (o == null || !tienePermisos(o.registroActivo, estado))
                Json("error");

            Persona p = getUsuario();

            return Json(MiembroDelegacion.buscarParaRegistro(omi, tipo, estado, query, p.esSuperUsuario()));
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
            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, tipo);
            if (o == null || !tienePermisos(o.registroActivo, estado) || Resultados.hayResultadosParaOMI(omi))
                return RedirectTo(Pagina.HOME);

            MiembroDelegacion md = MiembroDelegacion.obtenerMiembrosConClave(omi, tipo, clave)[0];
            Persona p = getUsuario();
            if (!p.esSuperUsuario() && md.estado != estado)
                return RedirectTo(Pagina.HOME);
            md.borrarMiembroDelegacion();

            // Se registra la telemetria
            Log.add(Log.TipoLog.REGISTRO, "Usuario " + getUsuario().nombreCompleto + " elimino al asistente con clave " +
                md.clave + " del estado " + md.estado + " en la categoría " + md.tipoOlimpiada.ToString());

            return RedirectTo(Pagina.REGISTRO, new { omi = omi, estado = estado, tipo = tipo });
        }

        private void failSafeViewBag()
        {
            ViewBag.errorInfo = "";
            ViewBag.tipo = TipoOlimpiada.NULL;
            ViewBag.tipoAsistente = MiembroDelegacion.TipoAsistente.NULL;
            ViewBag.hayResultados = false;
            ViewBag.resubmit = false;
            ViewBag.guardado = false;
            ViewBag.nombreEscuela = "";
            ViewBag.claveEscuela = 0;
            ViewBag.añoEscuela = 0;
            ViewBag.nivelEscuela = "";
            ViewBag.publica = true;
            ViewBag.permisos = false;
            ViewBag.invitaciones = false;
            ViewBag.omi = new Olimpiada();
        }

        //
        // GET: /Registro/Asistente

        public ActionResult Asistente(string omi, TipoOlimpiada tipo = TipoOlimpiada.NULL, string estado = null, string clave = null)
        {
            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, tipo == TipoOlimpiada.NULL ? TipoOlimpiada.OMI : tipo);
            if (o == null)
                return RedirectTo(Pagina.HOME);
            failSafeViewBag();
            ViewBag.omi = o;
            if (!tienePermisos(o.registroActivo || o.registroSedes, estado))
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
                if (!p.esSuperUsuario() && md.estado != estado)
                {
                    ViewBag.errorInfo = "permisos";
                    return View(new Persona());
                }
                ViewBag.claveDisponible = md.clave;
                ViewBag.tipoAsistente = md.tipo;
                ViewBag.estado = Estado.obtenerEstadoConClave(md.estado);
                ViewBag.nombreEscuela = md.nombreEscuela;
                ViewBag.claveEscuela = md.claveEscuela;
                ViewBag.añoEscuela = md.añoEscuela;
                ViewBag.nivelEscuela = md.nivelEscuela.ToString();
            }

            ViewBag.md = md;
            ViewBag.tipo = tipo;
            ViewBag.estados = Estado.obtenerEstados();
            limpiarErroresViewBag();
            ViewBag.resubmit = false;
            ViewBag.guardado = false;
            ViewBag.hayResultados = Resultados.hayResultadosParaOMI(o.numero);
            if (o.esOnline && !p.esSuperUsuario())
            {
                ViewBag.sedes = SedeOnline.obtenerSedes(o.numero, estado, tipo);
            }
            if (md != null && md.sede > 0)
                ViewBag.nombreSede = SedeOnline.obtenerSedeConClave(md.sede).nombre;

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
        public ActionResult Asistente(HttpPostedFileBase file, Persona p, string omi, string tipoOriginal,
            string estado, string claveSelect, int persona, string claveOriginal,
            int selectEscuela = 0, string nombreEscuela = null, int selectAnioEscolar = 0,
            Institucion.NivelInstitucion selectNivelEscolar = Institucion.NivelInstitucion.NULL,
            TipoOlimpiada tipo = TipoOlimpiada.NULL, bool selectPublica = true,
            MiembroDelegacion.TipoAsistente tipoAsistente = MiembroDelegacion.TipoAsistente.NULL, int sede = -1)
        {
            // Se valida que el usuario tenga permiso para realizar esta acción
            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, tipo == TipoOlimpiada.NULL ? TipoOlimpiada.OMI : tipo);
            if (String.IsNullOrEmpty(estado) ||
                tipo == TipoOlimpiada.NULL ||
                tipoAsistente == MiembroDelegacion.TipoAsistente.NULL ||
                o == null)
                return RedirectTo(Pagina.HOME);
            failSafeViewBag();
            ViewBag.omi = o;
            if (!tienePermisos(o.registroActivo || o.registroSedes, estado))
            {
                ViewBag.errorInfo = "permisos";
                return View(new Persona());
            }

            // Se valida que haya espacio para registrar otro competidor
            if (!esAdmin() &&
                 String.IsNullOrEmpty(claveOriginal) &&
                 tipoAsistente == MiembroDelegacion.TipoAsistente.COMPETIDOR &&
                 !puedeRegistrarOtroMas(o, tipo, estado))
            {
                ViewBag.errorInfo = "limite";
                return View(new Persona());
            }

            // Se valida que el miembro delegacion que se está modificando (en caso de update), exista
            MiembroDelegacion md = null;
            TipoOlimpiada tipoO = TipoOlimpiada.NULL;
            Institucion i = null;
            if (!String.IsNullOrEmpty(claveOriginal))
            {
                tipoO = DataRowParser.ToTipoOlimpiada(tipoOriginal);
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
                if (!p.esSuperUsuario() && md.estado != estado)
                {
                    ViewBag.errorInfo = "permisos";
                    return View(new Persona());
                }
            }

            // Se valida que la clave que se eligió sea valida para el estado
            Estado e = Estado.obtenerEstadoConClave(estado);
            if (claveSelect != null && tipoAsistente == MiembroDelegacion.TipoAsistente.COMPETIDOR && !claveSelect.StartsWith(e.ISO))
                claveSelect = "";
            if (persona != 0)
                p.clave = persona;

            // Se regresan todos los valores al viewbag en caso de error
            ViewBag.claveDisponible = claveSelect;
            ViewBag.estado = e;
            ViewBag.md = md;
            ViewBag.omi = o;
            ViewBag.tipo = tipo;
            ViewBag.estados = Estado.obtenerEstados();
            ViewBag.tipoAsistente = tipoAsistente;
            limpiarErroresViewBag();
            ViewBag.resubmit = true;
            bool hayResultados = Resultados.hayResultadosParaOMI(o.numero);
            ViewBag.hayResultados = hayResultados;
            if (o.esOnline && !p.esSuperUsuario())
            {
                ViewBag.sedes = SedeOnline.obtenerSedes(o.numero, estado, tipo);
            }
            if (tipoAsistente == MiembroDelegacion.TipoAsistente.COMPETIDOR)
            {
                ViewBag.escuelas = Institucion.obtenerEscuelasDeEstado(tipo, estado);
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
                    ViewBag.publica = selectPublica;
                }
            }

            // Validaciones extra de la foto
            if (file != null)
            {
                var valida = Archivos.esImagenValida(file, Peticion.TamañoFotoMaximo);
                if (valida != Archivos.ResultadoImagen.VALIDA)
                {
                    ViewBag.errorImagen = valida.ToString().ToLower();
                    return View(p);
                }
            }

            // Validaciones del modelo
            if (!ModelState.IsValid)
                return View(p);

            if ((tipo == TipoOlimpiada.OMIP || tipo == TipoOlimpiada.OMIS ||
                tipo == TipoOlimpiada.OMIPO || tipo == TipoOlimpiada.OMISO) &&
                tipoAsistente == MiembroDelegacion.TipoAsistente.COMPETIDOR)
            {
                p.omips = true;
            }

            List<MiembroDelegacion> miembrosExistentes = MiembroDelegacion.obtenerMiembrosDelegacion(omi, estado, tipo);
            bool registroCerrado = false;
            if (miembrosExistentes.Count > 0)
                registroCerrado = miembrosExistentes[0].cerrado;

            // Validaciones terminadas, guardamos persona y miembro delegacion
            // Primero en caso de que sea un nuevo miembro de la delegación
            if (md == null)
            {
                // Si la persona es 0, intentamos hacer match basándonos en el nombre, y solamente en el nombre
                if (persona == 0)
                {
                    Persona pe = Persona.obtenerPersonaConNombre(p.nombreCompleto);
                    if (pe != null)
                        persona = pe.clave;
                }

                // Nuevo asistente
                if (persona == 0)
                {
                    // Si no hay clave de persona previa, se agrega una nueva persona
                    p.nuevoUsuario();
                }
                else
                {
                    // Si tengo clave, se intenta conseguir
                    Persona per = Persona.obtenerPersonaConClave(persona);
                    if (per == null)
                    {
                        ViewBag.errorInfo = "persona";
                        return View(new Persona());
                    }

                    p.clave = per.clave;
                    p.oculta = per.oculta;
                    p.omips = per.omips;
                }

                if (tipoAsistente == MiembroDelegacion.TipoAsistente.COMPETIDOR)
                {
                    if (tipo == TipoOlimpiada.OMIPO || tipo == TipoOlimpiada.OMISO)
                    {
                        if (persona == 0)
                            p.oculta = true;
                    } else
                    {
                        p.oculta = false;
                    }
                }

                p.foto = guardaFoto(file, p.clave);
                p.guardarDatos(generarPeticiones: false, lugarGuardado: Persona.LugarGuardado.REGISTRO);

                // Se genera un nuevo miembro delegacion
                md = new MiembroDelegacion();
                md.olimpiada = o.numero;
                md.tipoOlimpiada = tipo;
                md.estado = estado;
                md.clave = claveSelect;
                md.tipo = tipoAsistente;
                md.claveUsuario = p.clave;
                md.sede = sede;
                md.cerrado = registroCerrado;
                md.nuevo();

                // Se registra la telemetria
                Log.add(Log.TipoLog.REGISTRO, "Usuario " + getUsuario().nombreCompleto + " registró a " +
                    p.nombreCompleto + " en el estado " + md.estado + " con clave " + md.clave +
                    " en la categoría " + md.tipoOlimpiada.ToString());
            }
            else
            {
                // Si ya hay resultados no podemos cambiar la clave, estado, o tipo de OMI
                if (hayResultados)
                {
                    tipo = md.tipoOlimpiada;
                    claveSelect = md.clave;
                    estado = md.estado;

                    // Adicionalmente si es competidor, no se piuede cambiar su tipo de asistencia
                    // ni volver a alguien mas competidor
                    if (md.tipo == MiembroDelegacion.TipoAsistente.COMPETIDOR ||
                        tipoAsistente == MiembroDelegacion.TipoAsistente.COMPETIDOR)
                        tipoAsistente = md.tipo;
                }

                // Modificando asistente
                // Primero los datos de persona
                Persona per = Persona.obtenerPersonaConClave(md.claveUsuario);
                if (tipoAsistente == MiembroDelegacion.TipoAsistente.COMPETIDOR)
                    per.oculta = false;
                p.clave = per.clave;
                p.oculta = per.oculta;
                p.omips = per.omips;
                p.foto = guardaFoto(file, p.clave);
                p.guardarDatos(generarPeticiones: false, lugarGuardado: Persona.LugarGuardado.REGISTRO);

                // Luego el miembro delegacion
                md.tipoOlimpiada = tipo;
                md.estado = estado;
                md.clave = claveSelect;
                md.tipo = tipoAsistente;
                md.sede = sede;
                md.cerrado = registroCerrado;
                md.guardarDatos(claveOriginal, tipoO);

                // Se registra la telemetria
                Log.add(Log.TipoLog.REGISTRO, "Usuario " + getUsuario().nombreCompleto + " actualizó a " +
                    p.nombreCompleto + " en el estado " + md.estado + " con clave " + md.clave +
                    " en la categoría " + md.tipoOlimpiada.ToString());
            }

            // Ahora se guarda la escuela
            if (tipoAsistente == MiembroDelegacion.TipoAsistente.COMPETIDOR)
            {
                // La escuela ya se consultó en la sección de viewbag, si es null, se llenó la sección de escuela nueva
                if (i == null)
                {
                    if (!String.IsNullOrEmpty(nombreEscuela))
                    {
                        // La escuela aun puede ya existir pero no la vieron y la volvieron a escribir,
                        // aqui tratamos de ver si ya existe
                        i = Institucion.buscarInstitucionConNombre(nombreEscuela);
                        if (i == null)
                        {
                            // Se genera una nueva escuela vacía y se asignan los datos que tenemos
                            i = new Institucion();
                            i.nuevaInstitucion();
                            i.nombre = nombreEscuela;
                            i.publica = selectPublica;
                        }
                    }
                }

                // Si en este punto tenemos una escuela, actualizamos los datos de la escuela y
                // actualizamos el miembro delegacion
                if (i != null)
                {
                    switch (selectNivelEscolar)
                    {
                        case Institucion.NivelInstitucion.PRIMARIA:
                            i.primaria = true;
                            break;
                        case Institucion.NivelInstitucion.SECUNDARIA:
                            i.secundaria = true;
                            break;
                        case Institucion.NivelInstitucion.PREPARATORIA:
                            i.preparatoria = true;
                            break;
                    }
                    i.guardar(generarPeticiones: false);

                    md.claveEscuela = i.clave;
                    md.nivelEscuela = selectNivelEscolar;
                    md.añoEscuela = selectAnioEscolar;
                    md.guardarDatosEscuela();
                }
            }

            ViewBag.guardado = true;
            return View(p);
        }

        private string guardaFoto(HttpPostedFileBase file, int clave)
        {
            if (file != null)
            {
                return Archivos.guardaArchivo(file, clave.ToString(), Archivos.Folder.USUARIOS, appendExtension: true, returnRelativeFolder: true);
            }

            return "";
        }

        private bool puedeRegistrarOtroMas(Olimpiada o, TipoOlimpiada tipo, string estado)
        {
            int maxUsuarios = o.getMaxParticipantesDeEstado(estado);
            List<MiembroDelegacion> mds = MiembroDelegacion.obtenerMiembrosDelegacion(o.numero, estado, tipo, MiembroDelegacion.TipoAsistente.COMPETIDOR);
            return (mds.Count < maxUsuarios);
        }

        //
        // GET: /Registro/GetCSV/

        public ActionResult GetCSV(string omi, TipoOlimpiada tipo, bool paraOmegaUp = false)
        {
            Persona p = getUsuario();
            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, TipoOlimpiada.OMI);
            if (!p.esSuperUsuario() || o == null)
                return RedirectTo(Pagina.HOME);

            StringBuilder texto = new StringBuilder();

            if (paraOmegaUp)
            {
                o = Olimpiada.obtenerOlimpiadaConClave(omi, tipo);
                if (o != null)
                {
                    texto.Append(o.obtenerTablaAsistentes(esParaOmegaUp: true));
                }
            }
            else if (tipo == TipoOlimpiada.OMIPO || tipo == TipoOlimpiada.OMISO)
            {
                o = Olimpiada.obtenerOlimpiadaConClave(omi, TipoOlimpiada.OMIPO);
                if (o != null)
                {
                    texto.Append(o.obtenerTablaAsistentes(esParaRegistro: true));
                }
                o = Olimpiada.obtenerOlimpiadaConClave(omi, TipoOlimpiada.OMISO);
                if (o != null)
                {
                    texto.Append(o.obtenerTablaAsistentes(esParaRegistro: true));
                }
            }
            else
            {
                texto.Append(o.obtenerTablaAsistentes(esParaRegistro: true, incluirCabeceras: true));
                o = Olimpiada.obtenerOlimpiadaConClave(omi, TipoOlimpiada.OMIP);
                if (o != null)
                {
                    texto.Append(o.obtenerTablaAsistentes(esParaRegistro: true));
                }
                o = Olimpiada.obtenerOlimpiadaConClave(omi, TipoOlimpiada.OMIS);
                if (o != null)
                {
                    texto.Append(o.obtenerTablaAsistentes(esParaRegistro: true));
                }
            }

            return File(Archivos.creaArchivoTexto(texto.ToString()), "text/csv", "asistentes.csv");
        }

        //
        // GET: /Registro/Sede

        public ActionResult Sede(string omi, string estado, TipoOlimpiada tipo, int clave = 0)
        {
            failSafeViewBag();
            Persona p = getUsuario();
            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, tipo);
            ViewBag.omi = o == null ? new Olimpiada() : o;

            if (o == null || !tienePermisos(o.registroActivo || o.registroSedes, estado) ||
                (!p.esSuperUsuario() && !o.registroSedes))
            {
                ViewBag.errorInfo = "permisos";
                return View(new SedeOnline());
            }

            SedeOnline so = null;
            if (clave > 0)
            {
                so = SedeOnline.obtenerSedeConClave(clave);
                if (so == null || so.estado != estado && !p.esSuperUsuario())
                {
                    ViewBag.errorInfo = "permisos";
                    return View(new SedeOnline());
                }
            }

            ViewBag.estado = Estado.obtenerEstadoConClave(estado);
            if (so == null)
                so = new SedeOnline();

            return View(so);
        }

        //
        // Post: /Registro/Sede

        [HttpPost]
        public ActionResult Sede(SedeOnline sede)
        {
            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(sede.omi, sede.tipoOlimpiada);
            if (o == null || !tienePermisos(o.registroActivo || o.registroSedes, sede.estado))
                return RedirectTo(Pagina.HOME);

            failSafeViewBag();
            ViewBag.omi = o;
            Persona p = getUsuario();

            if (!p.esSuperUsuario() && !o.registroSedes)
            {
                ViewBag.errorInfo = "permisos";
                return View(new SedeOnline());
            }
            if (sede.clave > 0)
            {
                SedeOnline so = SedeOnline.obtenerSedeConClave(sede.clave);
                if (so == null || so.estado != sede.estado && !p.esSuperUsuario())
                {
                    ViewBag.errorInfo = "permisos";
                    return View(new SedeOnline());
                }
            }

            ViewBag.omi = o;
            ViewBag.estado = Estado.obtenerEstadoConClave(sede.estado);

            // Validaciones del modelo
            if (!ModelState.IsValid)
                return View(sede);

            sede.guardar();
            ViewBag.guardado = true;

            if (sede.clave == 0)
                Log.add(Log.TipoLog.REGISTRO, "Usuario " + p.nombreCompleto + " agregó sede " + sede.nombre + " en el estado " + sede.estado);
            else
                Log.add(Log.TipoLog.REGISTRO, "Usuario " + p.nombreCompleto + " actualizó sede " + sede.nombre + " en el estado " + sede.estado);

            return View(sede);
        }

        //
        // GET: /Registro/EliminarSede

        public ActionResult EliminarSede(int clave)
        {
            SedeOnline so = SedeOnline.obtenerSedeConClave(clave);
            Persona p = getUsuario();
            Olimpiada o = null;
            if (so != null)
                o = Olimpiada.obtenerOlimpiadaConClave(so.omi, so.tipoOlimpiada);

            if (so == null || !tienePermisos(o.registroActivo || o.registroSedes, so.estado) ||
                (!p.esSuperUsuario() && !o.registroSedes))
                return RedirectTo(Pagina.HOME);

            List<MiembroDelegacion> miembros = MiembroDelegacion.obtenerMiembrosEnSede(clave);
            if (miembros.Count > 0)
                return RedirectTo(Pagina.HOME);

            so.borrar();

            // Se registra la telemetria
            Log.add(Log.TipoLog.REGISTRO, "Usuario " + getUsuario().nombreCompleto + " elimino la sede " +
                so.nombre + " del estado " + so.estado);

            return RedirectTo(Pagina.REGISTRO, new { omi = so.omi, estado = so.estado });
        }

        //
        // GET: /Registro/Terminar

        public ActionResult Terminar(string omi, string estado, TipoOlimpiada tipo)
        {
            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, tipo);
            if (o == null || !tienePermisos(o.registroActivo || o.registroSedes, estado))
                return RedirectTo(Pagina.HOME);


            MiembroDelegacion md = MiembroDelegacion.obtenerMiembrosDelegacion(omi, estado, tipo)[0];
            if (md.cerrado && !getUsuario().esSuperUsuario())
                return RedirectTo(Pagina.HOME);

            MiembroDelegacion.cerrarOAbrirRegistro(omi, estado, !md.cerrado, tipo);

            return RedirectTo(Pagina.REGISTRO, new { omi = omi, estado = estado, tipo = tipo });
        }

        //
        // GET: /Registro/GeneraInvitaciones

        public ActionResult GeneraInvitaciones(string omi)
        {
            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, TipoOlimpiada.OMI);
            if (o == null || !esAdmin())
                return RedirectTo(Pagina.HOME);

            ViewBag.invitaciones = MiembroDelegacion.generarInvitaciones(omi);

            return View();
        }

        //
        // GET: /Registro/Invitaciones

        public ActionResult Invitaciones(string omi, string estado)
        {
            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, TipoOlimpiada.OMI);
            if (o == null || !tienePermisos(o.registroActivo || o.registroSedes, estado))
                return RedirectTo(Pagina.HOME);

            return File(Archivos.comprimeArchivos(
               Archivos.Folder.INVITACIONES, omi + "\\" + estado),
               "application/zip", "Invitaciones.zip");
        }
    }
}
