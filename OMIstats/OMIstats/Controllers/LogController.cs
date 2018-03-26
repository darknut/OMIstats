using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OMIstats.Models;

namespace OMIstats.Controllers
{
    public class LogController : BaseController
    {
        //
        // GET: /Log/

        public ActionResult Index()
        {
            //return Redirect(Utilities.Server.direccionOMI());
            // Código para hacer mock
            string guid = Models.Usuario.MockUserLoggedIn(1);
            return RedirectToAction("In", "Log", new { GUID = guid });
        }

        //
        // GET: /Log/In/

        public ActionResult In(string GUID, bool force = false)
        {
            if (GUID == null || GUID.Equals(string.Empty))
                return RedirectTo(Pagina.HOME);

            ViewBag.GUIDerror = false;
            ViewBag.NoMatch = false;

            if (GUID != null)
            {
                Usuario usuario = Usuario.obtenerUsuarioConGUID(GUID);

                if (usuario == null)
                {
                    ViewBag.GUIDerror = true;
                    return View();
                }

                // Primero revisamos si el usuario ya está enlazado
                Persona persona = Persona.obtenerPersonaDeUsuario(usuario.Id.ToString());

                if (persona != null)
                {
                    // Log in exitoso
                    setUsuario(persona);
                    usuario.borrarGUID();

                    Object t = obtenerParams(Pagina.LOGIN);
                    limpiarParams(Pagina.LOGIN);
                    if (t != null)
                    {
                        KeyValuePair<Pagina, string> redireccion = (KeyValuePair<Pagina, string>)t;
                        return RedirectTo(redireccion.Key, redireccion.Value);
                    }
                    return RedirectTo(Pagina.VIEW_PROFILE);
                }

                // Obtenemos el mejor match del usuario de la base de datos
                persona = Persona.obtenerPersonaDeUsuario(usuario);

                if (persona == null)
                {
                    // No hay match, la persona no existe
                    ViewBag.NoMatch = true;
                    usuario.borrarGUID();
                    return View(usuario);
                }

                // Si el parámetro force viene incluido, simplemente guardamos los datos
                if (force)
                {
                    usuario.borrarGUID();
                    persona.usuario = usuario.Id.ToString();
                    persona.guardarDatos();
                    Log.add(Log.TipoLog.USUARIO, "Usuario linkeó su cuenta");
                    setUsuario(persona);
                }
                else
                {
                    Session[GUID_STRING] = GUID;
                    Session[GUID_USER] = persona.clave.ToString();
                }

                return RedirectTo(Pagina.VIEW_PROFILE, persona.usuario);
            }

            return RedirectTo(Pagina.HOME);
        }

        //
        // GET: /Log/Out/

        public ActionResult Out()
        {
            Session.Clear();
            setUsuario(new Persona(Persona.UsuarioNulo));
            return RedirectTo(Pagina.HOME);
        }
    }
}
