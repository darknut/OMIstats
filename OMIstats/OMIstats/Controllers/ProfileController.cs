using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OMIstats.Models;

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

        public ActionResult view(string usuario, Olimpiada.TipoOlimpiada tipo = Olimpiada.TipoOlimpiada.OMI)
        {
            Persona p;
            limpiarErroresViewBag();

            if (String.IsNullOrEmpty(usuario))
            {
                if (estaLoggeado())
                {
                    p = getUsuario();
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
                p = Persona.obtenerPersonaDeUsuario(usuario);
                if (p != null)
                {
                    Persona u = getUsuario();
                    if (p.usuario == u.usuario)
                        ViewBag.tienePeticiones = p.tienePeticiones();
                }
                else
                {
                    return RedirectTo(Pagina.ERROR, 404);
                }
            }

            if (tipo == Olimpiada.TipoOlimpiada.OMIS || tipo == Olimpiada.TipoOlimpiada.OMIP)
                tipo = Olimpiada.TipoOlimpiada.OMI;

            Medalleros medalleros = Medallero.obtenerMedalleros(Medallero.TipoMedallero.PERSONA, p.clave.ToString());

            ViewBag.participaciones = Resultados.obtenerParticipacionesComoCompetidorPara(p.clave, tipo);
            ViewBag.asistencias = MiembroDelegacion.obtenerParticipaciones(p.clave, tipo);
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
        // POST: /Profile/Check/

        [HttpPost]
        public JsonResult Check(string usuario)
        {
            if (!estaLoggeado())
                return Json(ERROR);

            string respuesta = Persona.revisarNombreUsuarioDisponible(getUsuario(), usuario).ToString().ToLower();

            return Json(respuesta);
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

            Persona p;

            if (usuario != null && esAdmin())
                p = Persona.obtenerPersonaDeUsuario(usuario);
            else
                p = getUsuario();

            limpiarErroresViewBag();
            ponFechasEnViewBag();

            return View(p);
        }

        //
        // POST: /Profile/Edit/
        [HttpPost]
        public ActionResult Edit(HttpPostedFileBase file, string password2, string password3, Persona p)
        {
            if (!estaLoggeado() || p == null)
                return RedirectTo(Pagina.HOME);

            if (!String.IsNullOrEmpty(p.password))
                ViewBag.passwordModificado = true;

            Persona current = getUsuario();

            if (p.clave != current.clave)
            {
                if (!esAdmin())
                    return RedirectTo(Pagina.ERROR, 403);
                current = Persona.obtenerPersonaConClave(p.clave);
            }

            if (!ModelState.IsValid)
                return Edit(current.usuario);

            limpiarErroresViewBag();

            if (!esAdmin() && !revisaCaptcha())
            {
                ViewBag.errorCaptcha = true;
                return Edit(current.usuario);
            }

            // Validaciones foto
            if (file != null)
            {
                Utilities.Archivos.ResultadoImagen resultado = Utilities.Archivos.esImagenValida(file, Peticion.TamañoFotoMaximo);
                if (resultado != Utilities.Archivos.ResultadoImagen.VALIDA)
                {
                    ViewBag.errorImagen = resultado.ToString().ToLower();
                    return Edit(current.usuario);
                }
            }

            // Validaciones nombre usuario
            Persona.DisponibilidadUsuario respUsuario = Persona.revisarNombreUsuarioDisponible(current, p.usuario);
            if (respUsuario != Persona.DisponibilidadUsuario.DISPONIBLE)
            {
                if (respUsuario == Persona.DisponibilidadUsuario.VACIO && esAdmin())
                {
                    p.usuario = p.clave.ToString();
                }
                else
                {
                    ViewBag.errorUsuario = respUsuario.ToString().ToLower();
                    return Edit(current.usuario);
                }
            }

            // Validaciones password
            if (!String.IsNullOrEmpty(p.password) ||
                !String.IsNullOrEmpty(password2) ||
                !String.IsNullOrEmpty(password3))
            {
                Persona temp = new Persona();
                temp.usuario = current.usuario;
                temp.password = p.password;

                Persona.ErrorPassword errorPassword = temp.verificaPasswords(password2, password3);
                if (errorPassword != Persona.ErrorPassword.OK)
                {
                    ViewBag.errorPassword = errorPassword.ToString().ToLower();
                    return Edit(current.usuario);
                }

                p.password = password2;
            }
            else
            {
                p.password = "";
            }

            // Validacion genero
            if (String.IsNullOrEmpty(p.genero) || p.genero.Equals("M"))
                p.genero = "M";
            else
                p.genero = "F";

            // Se copian los datos que no se pueden modificar
            p.admin = current.admin;
            p.clave = current.clave;
            p.ioiID = current.ioiID;

            // Se guarda la imagen en disco
            if (file != null)
                p.foto = Utilities.Archivos.guardaArchivo(file);

            // Si el nombre es el mismo, no se actualiza
            if (p.nombre.Equals(current.nombre))
                p.nombre = "";

            // Se guardan los datos
            if (p.guardarDatos(generarPeticiones:!esAdmin()))
            {
                // Se modificaron los datos del usuario, tenemos que recargarlos en la variable de sesion
                recargarDatos();

                if (esAdmin())
                {
                    if (file != null)
                    {
                        string oldFoto = p.foto;
                        p.foto =
                            Utilities.Archivos.copiarArchivo(p.foto, Utilities.Archivos.FolderImagenes.TEMPORAL,
                                                p.clave.ToString(), Utilities.Archivos.FolderImagenes.USUARIOS);
                        Utilities.Archivos.eliminarArchivo(oldFoto, Utilities.Archivos.FolderImagenes.TEMPORAL);
                        p.guardarDatos();
                    }

                    guardarParams(Pagina.SAVED_PROFILE, OK);
                    return RedirectTo(Pagina.SAVED_PROFILE, current.usuario);
                }

                if (file != null || p.nombre.Length > 0)
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
    }
}
