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
            if (usuario == null || usuario.Length == 0)
            {
                if (Persona.isLoggedIn(Session["usuario"]))
                    return View((Persona)Session["usuario"]);
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

            return View((Persona)Session["usuario"]);
        }

        //
        // POST: /Profile/Edit/
        [HttpPost]
        public ActionResult Edit(HttpPostedFileBase file, Persona p)
        {
            ////if (file.ContentLength > 0)
            ////{
            ////    var fileName = Path.GetFileName(file.FileName);
            ////    var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
            ////    file.SaveAs(path);
            ////}
            // -TODO- Verificar si persona dentro de session cambia cuando regresa por aqui

            return View((Persona)Session["usuario"]);
        }
    }
}
