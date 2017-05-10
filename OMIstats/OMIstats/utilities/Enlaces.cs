using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OMIstats.Utilities
{
    public class RedirectData
    {
        public string actionName;
        public string controllerName;
        public object routeValues;

        public RedirectData(string controllerName, string actionName, object routeValues)
            : this(actionName, controllerName)
        {
            this.routeValues = routeValues;
        }

        public RedirectData(string controllerName, string actionName)
        {
            this.actionName = actionName;
            this.controllerName = controllerName;
            this.routeValues = null;
        }
    }

    public class Enlaces
    {
        private const string REQUEST = "Request";
        private const string VIEW = "view";
        private const string ESTADO = "Estado";
        private const string EDIT = "Edit";
        private const string MANAGE = "Manage";
        private const string PROFILE = "Profile";
        private const string SAVED = "Saved";
        private const string ESCUELA = "Escuela";
        private const string OLIMPIADA = "Olimpiada";
        private const string OLIMPIADAS = "Olimpiadas";
        private const string ATTENDEES = "Attendees";
        private const string RESULTS_TABLE = "ResultsTable";
        private const string LOG = "Log";
        private const string IN = "In";
        private const string INDEX = "Index";
        private const string ERROR = "Error";
        private const string CHANGE = "Change";
        private const string ADMIN = "Admin";
        private const string RESET_PASSWORD = "ResetPassword";
        private const string PROBLEMA = "Problema";
        private const string HOME = "Home";

        public static RedirectData RedirectTo(Pagina pagina, object opciones = null)
        {
            switch (pagina)
            {
                case Pagina.VIEW_REQUEST:
                    return new RedirectData(REQUEST, VIEW);
                case Pagina.EDIT_ESTADO:
                    if (opciones != null)
                        return new RedirectData(ESTADO, EDIT, new { estado = opciones.ToString() });
                    return RedirectTo(Pagina.ERROR, 404);
                case Pagina.MANAGE_REQUEST:
                    return new RedirectData(REQUEST, MANAGE);
                case Pagina.SAVED_PROFILE:
                    return new RedirectData(PROFILE, SAVED);
                case Pagina.SAVED_ESCUELA:
                    if (opciones == null)
                        return RedirectTo(Pagina.ERROR, 404);
                    return new RedirectData(ESCUELA, SAVED, new { url = opciones.ToString() });
                case Pagina.EDIT_PROFILE:
                    return new RedirectData(PROFILE, EDIT);
                case Pagina.EDIT_OLIMPIADA:
                    if (opciones != null)
                        return new RedirectData(OLIMPIADA, EDIT, new { clave = opciones.ToString() });
                    return RedirectTo(Pagina.ERROR, 404);
                case Pagina.ATTENDEES_OMI:
                    if (opciones != null)
                        return new RedirectData(OLIMPIADA, ATTENDEES, new { clave = opciones.ToString() });
                    return RedirectTo(Pagina.ERROR, 404);
                case Pagina.RESULTS_TABLE:
                    if (opciones != null)
                        return new RedirectData(OLIMPIADA, RESULTS_TABLE, new { clave = opciones.ToString() });
                    return RedirectTo(Pagina.ERROR, 404);
                case Pagina.EDIT_ESCUELA:
                    if (opciones != null)
                        return new RedirectData(ESCUELA, EDIT, new { url = opciones.ToString() });
                    return RedirectTo(Pagina.ERROR, 404);
                case Pagina.VIEW_PROFILE:
                    if (opciones != null)
                        return new RedirectData(PROFILE, VIEW, new { usuario = opciones.ToString() });
                    return new RedirectData(PROFILE, VIEW);
                case Pagina.LOGIN:
                    return new RedirectData(LOG, IN);
                case Pagina.ERROR:
                    if (opciones != null)
                        return new RedirectData(ERROR, INDEX, new { code = opciones.ToString() });
                    return new RedirectData(ERROR, INDEX);
                case Pagina.ADMIN_CHANGE:
                    if (opciones != null)
                        return new RedirectData(ADMIN, CHANGE, new { usuario = opciones.ToString() });
                    return RedirectTo(Pagina.ERROR, 404);
                case Pagina.ADMIN_RESET_PASSWORD:
                    if (opciones != null)
                        return new RedirectData(ADMIN, RESET_PASSWORD, new { usuario = opciones.ToString() });
                    return RedirectTo(Pagina.ERROR, 404);
                case Pagina.OLIMPIADAS:
                    return new RedirectData(OLIMPIADAS, INDEX);
                case Pagina.PROBLEMA:
                    if (opciones != null)
                    {
                        string[] param = opciones.ToString().Split(':');
                        if (param.Length == 3)
                            return new RedirectData(PROBLEMA, EDIT, new { omi = param[0], dia = param[1], numero = param[2] });
                    }
                    return RedirectTo(Pagina.ERROR, 404);
                case Pagina.OLIMPIADA:
                    if (opciones != null)
                        return new RedirectData(OLIMPIADA, INDEX, new { clave = opciones.ToString() });
                    return RedirectTo(Pagina.ERROR, 404);
                case Pagina.HOME:
                default:
                    return new RedirectData(HOME, INDEX);
            }
        }
    }
}