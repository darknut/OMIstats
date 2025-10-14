using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OMIstats.Models;
using OMIstats.Utilities;

namespace OMIstats.Controllers
{
    public class ProfileController : BaseController
    {
        private const int AñoMinimo = 1950;
        private const int EdadMaxima = 20;

        private void ponFechasEnViewBag()
        {
            int maximo = MiembroDelegacion.primeraOMIPara(getUsuario());
            int minimo = MiembroDelegacion.ultimaOMIComoCompetidorPara(getUsuario());

            if (maximo == 0)
                maximo = DateTime.Today.Year;

            if (minimo == 0)
                minimo = AñoMinimo;
            else
                minimo -= EdadMaxima;

            ViewBag.maximo = maximo;
            ViewBag.minimo = minimo;
        }

        //
        // GET: /Profile/

        public ActionResult Index()
        {
            return RedirectTo(Pagina.VIEW_PROFILE);
        }

        //
        // GET: /Profile/view/

        public ActionResult view(string usuario = null, string clave = null, TipoOlimpiada tipo = TipoOlimpiada.OMI, string omi = null)
        {
            Persona p = null;
            limpiarErroresViewBag();
            bool isOwn = false;

            // QR's de competidor vienen en la forma usuario=null;clave=CMX-3;tipo=OMI;omi='30'
            if (usuario == null && clave != null)
            {
                List<MiembroDelegacion> md = MiembroDelegacion.obtenerMiembrosConClave(omi, tipo, clave, aproximarClave: true);
                if (md.Count == 0)
                    return RedirectTo(Pagina.ERROR, 404);
                if (md.Count > 1)
                    return RedirectTo(Pagina.DELEGACION, omi + ":" + md[0].estado);
                p = Persona.obtenerPersonaConClave(md[0].claveUsuario);
                usuario = p.usuario;
            }

            if (String.IsNullOrEmpty(usuario))
            {
                if (estaLoggeado())
                {
                    p = getUsuario();
                    isOwn = true;
                    ViewBag.tienePeticiones = p.tienePeticiones();
                }
                else
                {
                    guardarParams(Pagina.LOGIN, Pagina.VIEW_PROFILE, "");
                    return RedirectTo(Pagina.LOGIN);
                }
            }
            else
            {
                if (p == null)
                    p = Persona.obtenerPersonaDeUsuario(usuario);
                if (p != null)
                {
                    Persona u = getUsuario();
                    isOwn = p.usuario == u.usuario;
                    if (isOwn)
                        ViewBag.tienePeticiones = p.tienePeticiones();
                }
                else
                {
                    return RedirectTo(Pagina.ERROR, 404);
                }
            }

            // Estas variables de sesión tienen algo cuando se inicia sesión por primera vez y
            // se va a hacer el enlace de cuentas
            ViewBag.GUID = "";
            if (Session[GUID_STRING] != null && Session[GUID_USER].ToString() == p.clave.ToString())
                ViewBag.GUID = Session[GUID_STRING];
            Session[GUID_STRING] = null;
            Session[GUID_USER] = null;

            if (tipo == TipoOlimpiada.OMIS || tipo == TipoOlimpiada.OMIP || TableManager.isOMIPOS(tipo))
                tipo = TipoOlimpiada.OMI;

            Medalleros medalleros = Medallero.obtenerMedalleros(Medallero.TipoMedallero.PERSONA, p.clave.ToString());

            ViewBag.participaciones = Resultados.obtenerParticipacionesComoCompetidorPara(p.clave, tipo);
            ViewBag.asistencias = MiembroDelegacion.obtenerParticipaciones(p.clave, isOwn);
            ViewBag.medalleros = medalleros;
            ViewBag.tipo = tipo;
            return View(p);
        }

        //
        // GET: /Profile/Saved/

        public ActionResult Saved(string usuario = null)
        {
            string value = (string) obtenerParams(Pagina.SAVED_PROFILE);
            limpiarParams(Pagina.SAVED_PROFILE);

            if (value == null)
                return RedirectTo(Pagina.HOME);

            ViewBag.value = value;
            ViewBag.redirectUser = usuario;
            return View();
        }

        //
        // GET: /Profile/Edit/

        public ActionResult Edit(string usuario = null)
        {
            if (!estaLoggeado())
            {
                guardarParams(Pagina.LOGIN, Pagina.EDIT_PROFILE, "");
                return RedirectTo(Pagina.LOGIN);
            }

            Persona p = getUsuario();

            if (usuario != null && p.esSuperUsuario())
                p = Persona.obtenerPersonaDeUsuario(usuario);
            p.breakNombre();

            limpiarErroresViewBag();
            ponFechasEnViewBag();

            return View(p);
        }

        //
        // POST: /Profile/Edit/
        [HttpPost]
        public ActionResult Edit(HttpPostedFileBase file, Persona p, string tipoUsuario)
        {
            if (!estaLoggeado() || p == null)
                return RedirectTo(Pagina.HOME);

            Persona current = getUsuario();
            bool esSuperUsuario = current.esSuperUsuario();

            if (p.clave != current.clave)
            {
                if (!esSuperUsuario)
                    return RedirectTo(Pagina.ERROR, 403);
                current = Persona.obtenerPersonaConClave(p.clave);
            }

            if (!ModelState.IsValid)
                return Edit(current.usuario);

            limpiarErroresViewBag();

#if !DEBUG
            if (!esSuperUsuario && !revisaCaptcha())
            {
                ViewBag.errorCaptcha = true;
                return Edit(current.usuario);
            }
#endif

            // Validaciones foto
            if (file != null)
            {
                Archivos.ResultadoImagen resultado = Archivos.esImagenValida(file, Peticion.TamañoFotoMaximo);
                if (resultado != Archivos.ResultadoImagen.VALIDA)
                {
                    ViewBag.errorImagen = resultado.ToString().ToLower();
                    return Edit(current.usuario);
                }
            }

            // Validacion genero
            if (String.IsNullOrEmpty(p.genero) || p.genero.Equals("M"))
                p.genero = "M";
            else
                p.genero = "F";

            // Validación permisos
            if (esSuperUsuario && tipoUsuario != null && tipoUsuario != "")
            {
                Persona.TipoPermisos permisos;
                permisos = DataRowParser.ToTipoPermisos(tipoUsuario);
                if (permisos == Persona.TipoPermisos.ADMIN && !esAdmin())
                    return RedirectTo(Pagina.ERROR, 403);
                p.permisos = permisos;
            }
            else
            {
                p.permisos = current.permisos;
            }

            // Se copian los datos que no se pueden modificar
            p.clave = current.clave;
            if (!esAdmin())
            {
                p.ioiID = current.ioiID;
                p.oculta = current.oculta;
            }
            p.usuario = current.usuario;
            p.omips = current.omips;

            // Se guarda la imagen en disco
            if (file != null)
                p.foto = Archivos.guardaArchivo(file);

            // Se guardan los datos
            if (p.guardarDatos(generarPeticiones:!esSuperUsuario, currentValues: current))
            {
                if (!esSuperUsuario)
                    Log.add(Log.TipoLog.USUARIO, p.nombreCompleto + " actualizó sus datos");

                // Se modificaron los datos del usuario, tenemos que recargarlos en la variable de sesion
                recargarDatos();

                if (esSuperUsuario)
                {
                    if (file != null)
                    {
                        string oldFoto = p.foto;
                        p.foto =
                            Archivos.copiarArchivo(p.foto, Archivos.Folder.TEMPORAL,
                                                p.clave.ToString(), Archivos.Folder.USUARIOS);
                        Archivos.eliminarArchivo(oldFoto, Archivos.Folder.TEMPORAL);
                        p.guardarDatos();
                    }

                    guardarParams(Pagina.SAVED_PROFILE, OK);
                    return RedirectTo(Pagina.SAVED_PROFILE, current.usuario);
                }

                if (file != null || p.nombreCompleto != current.nombreCompleto)
                {
                    guardarParams(Pagina.SAVED_PROFILE, ADMIN);
                    return RedirectTo(Pagina.SAVED_PROFILE);
                }
                else
                {
                    guardarParams(Pagina.SAVED_PROFILE, OK);
                    return RedirectTo(Pagina.SAVED_PROFILE);
                }
            }
            else
            {
                guardarParams(Pagina.SAVED_PROFILE, ERROR);
                return RedirectTo(Pagina.SAVED_PROFILE);
            }
        }

        //
        // GET: /Profile/Diploma/

        public ActionResult Diploma(string omi, string clave, TipoOlimpiada clase = TipoOlimpiada.NULL, bool todos = false)
        {
            if (!estaLoggeado())
                return RedirectTo(Pagina.ERROR, 401);

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, TipoOlimpiada.OMI);
            if (o == null || !o.diplomasOnline)
                return RedirectTo(Pagina.ERROR, 404);

            Persona p = getUsuario();
            MiembroDelegacion md = MiembroDelegacion.obtenerMiembrosConClave(omi, clase, clave)[0];

            if (md.claveUsuario != p.clave)
                return RedirectTo(Pagina.ERROR, 401);

            if (todos && (md.tipo != MiembroDelegacion.TipoAsistente.LIDER &&
                        md.tipo != MiembroDelegacion.TipoAsistente.DELEGADO &&
                        md.tipo != MiembroDelegacion.TipoAsistente.SUBLIDER &&
                        md.tipo != MiembroDelegacion.TipoAsistente.DELELIDER))
                return RedirectTo(Pagina.ERROR, 401);

            if (md.tipo == MiembroDelegacion.TipoAsistente.COMPETIDOR)
            {
                if (clase == TipoOlimpiada.OMIP)
                    clave = "P-" + clave;
                if (clase == TipoOlimpiada.OMIS)
                    clave = "S-" + clave;
                if (clase == TipoOlimpiada.OMIPO)
                    clave = "P-" + clave + "-online";
                if (clase == TipoOlimpiada.OMISO)
                    clave = "S-" + clave + "-online";
            }

            if (!todos)
            {
                int numeroDeDiplomas = Archivos.cuantosExisten
                    (Archivos.Folder.DIPLOMAS, omi + "\\" + md.estado, clave);

                if (numeroDeDiplomas == 0)
                    return RedirectTo(Pagina.ERROR, 404);

                if (numeroDeDiplomas == 1)
                {
                    string contentFile = "application/pdf";
                    string url = Archivos.FOLDER_DIPLOMAS + "/" + omi + "/" + md.estado + "/" + clave + ".pdf";
                    string file = Server.MapPath(url);

                    if (!System.IO.File.Exists(file))
                        return RedirectTo(Pagina.ERROR, 404);

                    return File(file, contentFile, "Diploma.pdf");
                }
            }

            return File(Archivos.comprimeArchivos(
                Archivos.Folder.DIPLOMAS, omi + "\\" + md.estado, todos ? null : clave, todos ? null : ".pdf"),
                "application/zip", "Diplomas.zip");
        }

        //
        // GET: /Profile/Usurpar/

        public ActionResult Usurpar(int clave)
        {
            if (!esAdmin())
                return RedirectTo(Pagina.ERROR, 403);

            Persona p = Persona.obtenerPersonaConClave(clave);
            if (p == null)
                return RedirectTo(Pagina.ERROR, 404);

            setUsuario(p);
            return RedirectTo(Pagina.HOME);
        }
    }
}
