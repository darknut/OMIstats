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

            return View((Persona)Session["usuario"]);
        }
    }
}
