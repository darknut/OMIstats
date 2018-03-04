using OMIstats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OMIstats.Controllers
{
    public class AdminController : BaseController
    {
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            return RedirectTo(Pagina.ERROR, 404);
        }

        //
        // GET: /Admin/Scoreboard/

        public ActionResult Scoreboard()
        {
            if (!estaLoggeado())
            {
                guardarParams(Pagina.LOGIN, Pagina.ADMIN_SCOREBOARD);
                return RedirectTo(Pagina.LOGIN);
            }

            if (!esAdmin())
                return RedirectTo(Pagina.ERROR, 401);

            ViewBag.status = OmegaUp.obtenerInstrucciones(OmegaUp.Instruccion.STATUS);
            ViewBag.polls = OmegaUp.obtenerInstrucciones(OmegaUp.Instruccion.POLL);

            return View();
        }

        //
        // Post: /Admin/Scoreboard/

        [HttpPost]
        public ActionResult Scoreboard(OmegaUp poll)
        {
            if (!esAdmin())
                return RedirectTo(Pagina.ERROR, 401);

            poll.instruccion = OmegaUp.Instruccion.POLL;
            poll.guardarNuevo();

            return RedirectTo(Pagina.ADMIN_SCOREBOARD);
        }

        //
        // GET: /Admin/StartScoreboard/

        public ActionResult StartScoreboard()
        {
            if (!esAdmin())
                return RedirectTo(Pagina.ERROR, 401);

            OmegaUp.StartScoreboard();

            return RedirectTo(Pagina.ADMIN_SCOREBOARD);
        }

        //
        // GET: /Admin/Change/

        public ActionResult Change(string usuario)
        {
            if (!estaLoggeado())
            {
                guardarParams(Pagina.LOGIN, Pagina.ADMIN_CHANGE, usuario);
                return RedirectTo(Pagina.LOGIN);
            }

            if(!esAdmin())
                return RedirectTo(Pagina.ERROR, 401);

            Persona p = Persona.obtenerPersonaDeUsuario(usuario);
            if (p == null)
                return RedirectTo(Pagina.ERROR, 404);

            Persona u = getUsuario();
            if (u.usuario == p.usuario)
                return RedirectTo(Pagina.ERROR, 401);

            limpiarErroresViewBag();

            return View(p);
        }

        //
        // POST: /Admin/Change/

        [HttpPost]
        public ActionResult Change(Persona p)
        {
            if (!esAdmin() || p == null)
                return RedirectTo(Pagina.HOME);

            limpiarErroresViewBag();

            Persona cambiar = Persona.obtenerPersonaDeUsuario(p.usuario);
            Persona admin = Persona.obtenerPersonaConClave(getUsuario().clave);

            cambiar.admin = !cambiar.admin;
            cambiar.guardarDatos();

            ViewBag.guardado = true;

            return View(cambiar);
        }

        //
        // GET: /Admin/Zombies/

        public ActionResult Zombies()
        {
            if (!esAdmin())
                return RedirectTo(Pagina.HOME);

            Institucion.borrarZombies();
            Persona.borrarZombies();

            return RedirectTo(Pagina.MANAGE_REQUEST);
        }

        //
        // GET: /Admin/Unlink/

        public ActionResult Unlink(string usuario)
        {
            if (!estaLoggeado())
            {
                guardarParams(Pagina.LOGIN, Pagina.ADMIN_UNLINK, usuario);
                return RedirectTo(Pagina.LOGIN);
            }

            if (!esAdmin())
                return RedirectTo(Pagina.ERROR, 401);

            Persona p = Persona.obtenerPersonaDeUsuario(usuario);
            if (p == null)
                return RedirectTo(Pagina.ERROR, 404);

            Persona u = getUsuario();
            if (u.usuario == p.usuario)
                return RedirectTo(Pagina.ERROR, 401);

            p.usuario = "_" + p.clave;
            p.guardarDatos();

            limpiarErroresViewBag();

            return RedirectTo(Pagina.VIEW_PROFILE, p.usuario);
        }
    }
}
