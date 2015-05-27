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

        private void limpiaErroresViewBag()
        {
            ViewBag.errorImagen = "";
            ViewBag.errorUsuario = "";
            ViewBag.errorPassword = "";
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
                    return RedirectTo(Pagina.HOME);
            }
            else
            {
                Persona p = Persona.obtenerPersonaDeUsuario(usuario);
                if (p != null)
                    return View(p);
                else
                    return RedirectTo(Pagina.HOME);
            }
        }

        //
        // GET: /Profile/Saved/

        public ActionResult Saved(string value)
        {
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
                return RedirectTo(Pagina.HOME);

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
                return RedirectTo(Pagina.HOME);

            if (!String.IsNullOrEmpty(p.password))
                ViewBag.passwordModificado = true;

            if (!ModelState.IsValid)
                return Edit();

            bool needsAdmin = false;
            limpiaErroresViewBag();

            Persona current = getUsuario();

            // Validaciones foto
            if (file != null)
            {
                Utilities.Archivos.ResultadoImagen resultado = Utilities.Archivos.esImagenValida(file);
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
            // Foto y nombre se vuelven "" porque se actualizan por un Admin
            string nuevoNombre = p.nombre;
            p.foto = "";
            p.nombre = "";

            // Se guardan los datos
            if (p.guardarDatos())
            {
                recargarDatos();

                // Guardando los request especiales
                if (file != null)
                {
                    string imagen = Utilities.Archivos.guardaArchivo(file, "", Utilities.Archivos.FolderImagenes.TEMPORAL);
                    Peticion pet = new Peticion();
                    pet.tipo = "usuario";
                    pet.subtipo = "foto";
                    pet.usuario = getUsuario();
                    pet.datos1 = imagen;
                    pet.guardarPeticion();
                    needsAdmin = true;
                }

                if (!nuevoNombre.Equals(getUsuario().nombre))
                {
                    Peticion pet = new Peticion();
                    pet.tipo = "usuario";
                    pet.subtipo = "nombre";
                    pet.usuario = getUsuario();
                    pet.datos1 = nuevoNombre;
                    pet.guardarPeticion();
                    needsAdmin = true;
                }

                if (needsAdmin)
                    return RedirectTo(Pagina.SAVED_PROFILE, new { value = "admin" });
                else
                    return RedirectTo(Pagina.SAVED_PROFILE, new { value = "ok" });
            }
            else
            {
                return RedirectTo(Pagina.SAVED_PROFILE, new { value = "error" });
            }
        }
    }
}
