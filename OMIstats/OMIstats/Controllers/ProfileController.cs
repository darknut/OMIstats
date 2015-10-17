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

        #region Metodos privados

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

        #endregion

        //
        // GET: /Profile/

        public ActionResult Index()
        {
            return RedirectTo(Pagina.VIEW_PROFILE);
        }

        //
        // GET: /Profile/view/

        public ActionResult view(string usuario)
        {
            if (String.IsNullOrEmpty(usuario))
            {
                if (estaLoggeado())
                {
                    recargarDatos();
                    return View(getUsuario());
                }
                else
                    return RedirectTo(Pagina.ERROR, 401);
            }
            else
            {
                Persona p = Persona.obtenerPersonaDeUsuario(usuario);
                if (p != null)
                    return View(p);
                else
                    return RedirectTo(Pagina.ERROR, 404);
            }
        }

        //
        // GET: /Profile/Saved/

        public ActionResult Saved()
        {
            string value = (string) obtenerParams(Pagina.SAVED_PROFILE);
            limpiarParams(Pagina.SAVED_PROFILE);

            if (value == null)
                return RedirectTo(Pagina.HOME);

            ViewBag.value = value;
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

        public ActionResult Edit()
        {
            if (!estaLoggeado())
                return RedirectTo(Pagina.ERROR, 401);

            limpiarErroresViewBag();
            recargarDatos();
            ponFechasEnViewBag();

            return View(getUsuario());
        }

        //
        // POST: /Profile/Edit/
        [HttpPost]
        public ActionResult Edit(HttpPostedFileBase file, string password2, string password3, Persona p)
        {
            if (!estaLoggeado() || p == null)
                return RedirectTo(Pagina.ERROR, 401);

            if (!String.IsNullOrEmpty(p.password))
                ViewBag.passwordModificado = true;

            if (!ModelState.IsValid)
                return Edit();

            limpiarErroresViewBag();

            if (!esAdmin() && !revisaCaptcha())
            {
                ViewBag.errorCaptcha = true;
                return Edit();
            }

            Persona current = getUsuario();

            // Validaciones foto
            if (file != null)
            {
                Utilities.Archivos.ResultadoImagen resultado = Utilities.Archivos.esImagenValida(file, Peticion.TamañoFotoMaximo);
                if (resultado != Utilities.Archivos.ResultadoImagen.VALIDA)
                {
                    ViewBag.errorImagen = resultado.ToString().ToLower();
                    return Edit();
                }
            }

            // Validaciones nombre usuario
            Persona.DisponibilidadUsuario respUsuario = Persona.revisarNombreUsuarioDisponible(current, p.usuario);
            if (respUsuario != Persona.DisponibilidadUsuario.DISPONIBLE)
            {
                ViewBag.errorUsuario = respUsuario.ToString().ToLower();
                return Edit();
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
                    return Edit();
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
            current.recargarDatos();
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
                recargarDatos();

                if (esAdmin())
                {
                    if (file != null)
                    {
                        p.foto =
                            Utilities.Archivos.copiarArchivo(p.foto, Utilities.Archivos.FolderImagenes.TEMPORAL,
                                                p.clave.ToString(), Utilities.Archivos.FolderImagenes.USUARIOS);
                        p.guardarDatos();
                    }

                    guardarParams(Pagina.SAVED_PROFILE, OK);
                    return RedirectTo(Pagina.SAVED_PROFILE);
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
