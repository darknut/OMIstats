using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OMIstats.Controllers
{
    public class HallOfFameController : BaseController
    {
        //
        // GET: /HallOfFame/

        public ActionResult Index()
        {
            return View();
        }

    }
}
