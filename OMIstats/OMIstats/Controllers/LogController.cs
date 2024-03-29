﻿using System;
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
#if DEBUG
            // En Debug mode hacemos mock
            string guid = Models.Usuario.MockUserLoggedIn(1);
            return RedirectToAction("In", "Log", new { GUID = guid });
#else
            return Redirect(Utilities.Server.direccionOMI());
#endif
        }

        //
        // GET: /Log/In/

        public ActionResult In(string GUID, bool force = false)
        {
            if (String.IsNullOrEmpty(GUID))
            {
                Log.add(Log.TipoLog.USUARIO, "Se mandó un GUID vacío o nulo");
                return RedirectTo(Pagina.HOME);
            }

            ViewBag.GUIDerror = false;
            ViewBag.NoMatch = false;

            Usuario usuario = Usuario.obtenerUsuarioConGUID(GUID);

            if (usuario == null)
            {
                if (estaLoggeado())
                    return RedirectTo(Pagina.HOME);

                Log.add(Log.TipoLog.USUARIO, "GUID no existe");
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

                if (t is Pagina)
                    return RedirectTo((Pagina)t);
                if (t is KeyValuePair<Pagina, string>)
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
                Log.add(Log.TipoLog.USUARIO, "Usuario con nombre " + usuario.Nombre + " y correo " + usuario.Email + " no se encontró en el sistema");
                usuario.borrarGUID();
                return View(usuario);
            }

            // Si el parámetro force viene incluido, simplemente guardamos los datos
            if (force)
            {
                usuario.borrarGUID();
                persona.usuario = usuario.Id.ToString();
                persona.guardarDatos();
                Log.add(Log.TipoLog.USUARIO, persona.nombreCompleto + " linkeó su cuenta");
                setUsuario(persona);
            }
            else
            {
                // Si no, redirigmos a view profile para que confirmen que son ellos
                Log.add(Log.TipoLog.USUARIO, "Posible match para " + usuario.Nombre);
                Session[GUID_STRING] = GUID;
                Session[GUID_USER] = persona.clave.ToString();
            }

            return RedirectTo(Pagina.VIEW_PROFILE, persona.usuario);
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
