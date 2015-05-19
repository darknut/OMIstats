using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OMIstats.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {
            // Se usa System.Web en vez de Session porque a tiempo de construcción, Session aún no esta populada
            ViewBag.usuario = System.Web.HttpContext.Current.Session["usuario"];
        }
    }
}
