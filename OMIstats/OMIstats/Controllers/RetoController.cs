using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OMIstats.Models;

namespace OMIstats.Controllers
{
    public class RetoController : BaseController
    {
        //
        // GET: /Reto/

        public ActionResult Index()
        {
            ViewBag.esAdmin = esAdmin();
            Olimpiada o = Olimpiada.obtenerMasReciente();
            ViewBag.activo = Reto.isRetoActivo(o.numero);
            if (!estaLoggeado())
            {
                guardarParams(Pagina.LOGIN, Pagina.RETO);
                ViewBag.logeado = false;
                return View();
            }
            ViewBag.logeado = true;

            Persona p = getUsuario();
            MiembroDelegacion md = MiembroDelegacion.obtenerMiembroDePersona(p.clave, o.numero);

            return View(md);
        }

        //
        // GET: /Reto/Manage/

        public ActionResult Manage(string omi = null)
        {
            Olimpiada o = null;
            if (omi == null)
                o = Olimpiada.obtenerMasReciente();
            else
                o = Olimpiada.obtenerOlimpiadaConClave(omi, TipoOlimpiada.OMI);

            if (!esAdmin() || o == null)
                return RedirectTo(Pagina.ERROR, 404);

            ViewBag.omi = o;
            int porEvaluar = RetoPersona.obtenerRetosPorEvaluar(o.numero);
            ViewBag.porEvaluar = porEvaluar;
            ViewBag.sigReto = Reto.obtenerPrimerRetoPorEvaluar(o.numero);
            if (porEvaluar == 0)
                ViewBag.results = RetoPersona.obtenerResultados(o.numero);

            return View(Reto.obtenerRetosDeOlimpiada(o.numero));
        }

        private ActionResult AceptarOCancerlar(int reto, RetoStatus status)
        {
            if (!esAdmin())
                return RedirectTo(Pagina.ERROR, 404);

            RetoPersona rp = RetoPersona.obtenerRetoConClave(reto);

            if (rp == null)
                return RedirectTo(Pagina.ERROR, 404);

            rp.cambiarStatusEInvalidarAnteriores(status);

            return RedirectTo(Pagina.RETO_MANAGE);
        }

        //
        // GET: /Reto/Aceptar/

        public ActionResult Aceptar(int reto)
        {
            return AceptarOCancerlar(reto, RetoStatus.ACCEPTED);
        }

        //
        // GET: /Reto/Rechazar/

        public ActionResult Rechazar(int reto)
        {
            return AceptarOCancerlar(reto, RetoStatus.REJECTED);
        }

        //
        // GET: /Reto/Edit/

        public ActionResult Edit(string omi, int reto = 0)
        {
            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, TipoOlimpiada.OMI);

            if (!esAdmin() || o == null)
                return RedirectTo(Pagina.ERROR, 404);

            ViewBag.omi = o;
            Reto r = null;
            if (reto == 0)
                r = new Reto(omi);
            else
                r = Reto.obtenerRetoConClave(reto);

            return View(r);
        }

        //
        // POST: /Reto/Edit/
        [HttpPost]
        public ActionResult Edit(Reto r)
        {
            if (!esAdmin())
                return RedirectTo(Pagina.ERROR, 404);

            ViewBag.omi = Olimpiada.obtenerOlimpiadaConClave(r.olimpiada, TipoOlimpiada.OMI);

            if (!ModelState.IsValid)
                return View(r);

            r.guardar();

            return RedirectTo(Pagina.RETO_MANAGE, r.olimpiada);
        }

        //
        // GET: /Reto/Borrar/

        public ActionResult Borrar(string omi, int reto)
        {
            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, TipoOlimpiada.OMI);
            Reto r = Reto.obtenerRetoConClave(reto);

            if (!esAdmin() || o == null || r == null)
                return RedirectTo(Pagina.ERROR, 404);

            r.borrar();

            return RedirectTo(Pagina.RETO_MANAGE, r.olimpiada);
        }

        //
        // GET: /Reto/SwitchActivo/

        public ActionResult SwitchActivo(string omi)
        {
            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, TipoOlimpiada.OMI);
            if (!esAdmin() || o == null)
                return RedirectTo(Pagina.ERROR, 404);

            Reto.switchActivo(omi);

            return RedirectTo(Pagina.RETO_MANAGE, omi);
        }

        //
        // GET: /Reto/SwitchCerrado/

        public ActionResult SwitchCerrado(string omi)
        {
            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, TipoOlimpiada.OMI);
            if (!esAdmin() || o == null)
                return RedirectTo(Pagina.ERROR, 404);

            Reto.switchCerrado(omi);

            return RedirectTo(Pagina.RETO_MANAGE, omi);
        }

        //
        // GET: /Reto/Lista

        public ActionResult Lista()
        {
            if (!estaLoggeado())
            {
                guardarParams(Pagina.LOGIN, Pagina.RETO_LISTA);
                return RedirectTo(Pagina.LOGIN);
            }

            Persona p = getUsuario();
            Olimpiada o = Olimpiada.obtenerMasReciente();
            MiembroDelegacion md = MiembroDelegacion.obtenerMiembroDePersona(p.clave, o.numero);

            bool activo = Reto.isRetoActivo(o.numero);

            if (!activo || !(md.tipo == MiembroDelegacion.TipoAsistente.COMPETIDOR || esAdmin()))
            {
                return RedirectTo(Pagina.RETO);
            }

            ViewBag.cerrado = Reto.isRetoCerrado(o.numero);
            ViewBag.omi = o;
            ViewBag.persona = p;

            return View(Reto.obtenerRetosDeOlimpiada(o.numero, p.clave));
        }

        //
        // POST: /Registro/Pregunta/

        [HttpPost]
        public JsonResult Pregunta(HttpPostedFileBase file, int reto)
        {
            if (!estaLoggeado())
                return Json("login");

            Persona p = getUsuario();
            Olimpiada o = Olimpiada.obtenerMasReciente();
            MiembroDelegacion md = MiembroDelegacion.obtenerMiembroDePersona(p.clave, o.numero);

            bool activo = Reto.isRetoActivo(o.numero);

            if (!activo || !(md.tipo == MiembroDelegacion.TipoAsistente.COMPETIDOR || esAdmin()))
                return Json("error");

            Reto r = Reto.obtenerRetoConClave(reto);
            if (r == null || r.olimpiada != o.numero)
                return Json("error");

            if (r.cerrado)
                return Json("cerrado");

            string response = RetoPersona.subirFoto(file, o.numero, p.clave, reto, o.inicio.Ticks);
            return Json(response ?? "error");
        }
    }
}
