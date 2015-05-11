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
            ViewBag.usuario = System.Web.HttpContext.Current.Session["usuario"];
        }
    }
}
