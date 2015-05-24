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
            int maximo = MiembroDelegacion.primeraOMIPara((Persona)Session["usuario"]);
            int minimo = MiembroDelegacion.ultimaOMIComoCompetidorPara((Persona)Session["usuario"]);

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
            return RedirectToAction("view");
        }

        //
        // GET: /Profile/view/

        public ActionResult view(string usuario)
        {
            if (String.IsNullOrEmpty(usuario))
            {
                if (Persona.isLoggedIn(Session["usuario"]))
                {
                    ((Persona)Session["usuario"]).recargarDatos();
                    return View((Persona)Session["usuario"]);
                }
                else
                    return RedirectToAction("Index", "Home");
            }
            else
            {
                Persona p = Persona.obtenerPersonaDeUsuario(usuario);
                if (p != null)
                    return View(p);
                else
                    return RedirectToAction("Index", "Home");
            }
        }

        //
        // GET: /Profile/Edit/

        public ActionResult Edit()
        {
            if (!Persona.isLoggedIn(Session["usuario"]))
                return RedirectToAction("Index", "Home");

            ((Persona)Session["usuario"]).recargarDatos();
            ponFechasEnViewBag();

            return View((Persona)Session["usuario"]);
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
            if (!Persona.isLoggedIn(Session["usuario"]))
                return Json("error");

            string respuesta = Persona.revisarNombreUsuarioDisponible((Persona)Session["usuario"], usuario).ToString().ToLower();

            return Json(respuesta);
        }

        //
        // POST: /Profile/Edit/
        [HttpPost]
        public ActionResult Edit(HttpPostedFileBase file, string password2, string password3, Persona p)
        {
            if (!Persona.isLoggedIn(Session["usuario"]) || p == null)
                return RedirectToAction("Index", "Home");

            if (!String.IsNullOrEmpty(p.password))
                @ViewBag.passwordModificado = true;

            if (!ModelState.IsValid)
                return Edit();

            bool needsAdmin = false;
            limpiaErroresViewBag();

            Persona current = (Persona)Session["usuario"];

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
                ((Persona)Session["usuario"]).recargarDatos();

                // Guardando los request especiales
                if (file != null)
                {
                    string imagen = Utilities.Archivos.guardaArchivo(file, "", Utilities.Archivos.FolderImagenes.TEMPORAL);
                    Peticion pet = new Peticion();
                    pet.tipo = "usuario";
                    pet.subtipo = "foto";
                    pet.usuario = (Persona)Session["usuario"];
                    pet.datos1 = imagen;
                    pet.guardarPeticion();
                    needsAdmin = true;
                }

                if (!nuevoNombre.Equals(((Persona)Session["usuario"]).nombre))
                {
                    Peticion pet = new Peticion();
                    pet.tipo = "usuario";
                    pet.subtipo = "nombre";
                    pet.usuario = (Persona)Session["usuario"];
                    pet.datos1 = nuevoNombre;
                    pet.guardarPeticion();
                    needsAdmin = true;
                }

                if (needsAdmin)
                    return RedirectToAction("Saved", "Profile", new { value = "admin" });
                else
                    return RedirectToAction("Saved", "Profile", new { value = "ok" });
            }
            else
            {
                return RedirectToAction("Saved", "Profile", new { value = "error" });
            }
        }
    }
}
