using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OMIstats.Models;

namespace OMIstats.Controllers
{
    public class LogInController : Controller
    {
        //
        // GET: /LogIn/

        public ActionResult Index()
        {
            Persona p = new Persona();

            return View(p);
        }

    }
}
