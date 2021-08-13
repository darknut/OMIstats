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

            List<OmegaUp> status = OmegaUp.obtenerInstrucciones(OmegaUp.Instruccion.STATUS);
            ViewBag.polls = OmegaUp.obtenerInstrucciones();
            ViewBag.status = status;
            ViewBag.hide = OmegaUp.obtenerInstrucciones(OmegaUp.Instruccion.HIDE);

            if (status.Count == 0)
                OmegaUp.RunnerStarted = false;

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

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(poll.olimpiada, poll.tipoOlimpiada);
            if (o != null)
            {
                o.liveResults = true;

                if (o.esOnline)
                    OmegaUp.nuevaInstruccion(OmegaUp.Instruccion.HIDE, true);
            }

            return RedirectTo(Pagina.ADMIN_SCOREBOARD);
        }

        //
        // GET: /Admin/StartScoreboard/

        public ActionResult StartScoreboard()
        {
            if (!esAdmin())
                return RedirectTo(Pagina.ERROR, 401);

            OmegaUp.StartScoreboard();
            OmegaUp.RunnerStarted = true;

            return RedirectTo(Pagina.ADMIN_SCOREBOARD);
        }

        //
        // GET: /Admin/KillScoreboard/

        public ActionResult KillScoreboard()
        {
            if (!esAdmin())
                return RedirectTo(Pagina.ERROR, 401);
            OmegaUp.nuevaInstruccion(OmegaUp.Instruccion.KILL, true);
            return RedirectTo(Pagina.ADMIN_SCOREBOARD);
        }

        //
        // GET: /Admin/HideScoreboard/

        public ActionResult HideScoreboard()
        {
            if (!esAdmin())
                return RedirectTo(Pagina.ERROR, 401);
            OmegaUp.nuevaInstruccion(OmegaUp.Instruccion.HIDE, true);
            return RedirectTo(Pagina.ADMIN_SCOREBOARD);
        }

        //
        // GET: /Admin/BorrarScoreboard/

        public ActionResult BorrarScoreboard(int clave = 0)
        {
            if (!esAdmin())
                return RedirectTo(Pagina.ERROR, 401);

            if (clave == 0)
            {
                OmegaUp.borrarTodo();
                Olimpiada.resetOMIs(TipoOlimpiada.OMI);
                Olimpiada.resetOMIs(TipoOlimpiada.OMIS);
                Olimpiada.resetOMIs(TipoOlimpiada.OMIP);
                OmegaUp.RunnerStarted = false;
            }
            else
            {
                OmegaUp om = OmegaUp.obtenerConClave(clave);
                OmegaUp.borrarConClave(clave);

                if (om.instruccion == OmegaUp.Instruccion.POLL)
                {
                    OmegaUp om2 = OmegaUp.obtenerParaOMI(om.olimpiada, om.tipoOlimpiada);

                    if (om2 == null)
                    {
                        Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(om.olimpiada, om.tipoOlimpiada);
                        o.liveResults = false;
                    }
                }
                else if (om.instruccion == OmegaUp.Instruccion.KILL)
                    OmegaUp.RunnerStarted = false;
            }

            return RedirectTo(Pagina.ADMIN_SCOREBOARD);
        }

        //
        // GET: /Admin/ResetOMI/

        public ActionResult ResetOMI()
        {
            if (!esAdmin())
                return RedirectTo(Pagina.ERROR, 401);

            Olimpiada.resetOMIs(TipoOlimpiada.OMI);
            Olimpiada.resetOMIs(TipoOlimpiada.OMIS);
            Olimpiada.resetOMIs(TipoOlimpiada.OMIP);

            List<OmegaUp> status = OmegaUp.obtenerInstrucciones(OmegaUp.Instruccion.STATUS);
            OmegaUp.RunnerStarted = (status.Count > 0);

            return RedirectTo(Pagina.ADMIN_SCOREBOARD);
        }

        //
        // GET: /Admin/Trim/

        public ActionResult Trim(TipoOlimpiada tipo, int tiempo, int dia, string omi = "")
        {
            if (!esAdmin())
                return RedirectTo(Pagina.ERROR, 401);

            Olimpiada o;

            if (String.IsNullOrEmpty(omi))
                o = Olimpiada.obtenerMasReciente();
            else
                o = Olimpiada.obtenerOlimpiadaConClave(omi, TipoOlimpiada.OMI);

            if (o == null)
                return RedirectTo(Pagina.ERROR, 401);

            DetalleLugar.clean(o.numero, tipo, dia);
            DetalleLugar.trim(o.numero, tipo, tiempo, dia);

            DetallePuntos.clean(o.numero, tipo, dia);
            DetallePuntos.trim(o.numero, tipo, tiempo, dia);

            return RedirectTo(Pagina.ADMIN_SCOREBOARD);
        }

        //
        // GET: /Admin/Logs/

        public ActionResult Logs(int count = 0, Log.TipoLog tipo = Log.TipoLog.NULL)
        {
            if (!esAdmin())
                return RedirectTo(Pagina.ERROR, 401);

            string logs = "";

            foreach (Log l in Log.get(count, tipo))
                logs += l.timestamp + "\t" + l.tipo + "\t" + l.log + "\n";

            ViewBag.logs = logs;
            ViewBag.count = count;
            ViewBag.tipo = tipo;

            return View();
        }

        //
        // GET: /Admin/ClearLogs/

        public ActionResult ClearLogs()
        {
            if (!esAdmin())
                return RedirectTo(Pagina.ERROR, 401);

            Log.clear();

            return RedirectTo(Pagina.ADMIN_LOGS); ;
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
        // GET: /Admin/ResetPermisos/

        public ActionResult ResetPermisos()
        {
            if (!esAdmin())
                return RedirectTo(Pagina.HOME);

            Persona.resetPermisos();

            return RedirectTo(Pagina.MANAGE_REQUEST);
        }

        //
        // GET: /Admin/SyncDelegados/

        public ActionResult SyncDelegados()
        {
            if (!esAdmin())
                return RedirectTo(Pagina.HOME);

            Usuario.syncDelegados();

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
