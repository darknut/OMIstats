using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OMIstats.Models;

namespace OMIstats.Utilities
{
    public class TableManager
    {
        private Olimpiada currentOMI;
        private bool admin;
        private int claveUsuario;
        private Resultados currentResultados;

        private const string CLASE_BRONCE = "fondoBronce";
        private const string CLASE_PLATA = "fondoPlata";
        private const string CLASE_ORO = "fondoOro";
        private const string CLASE_PENDIENTE = "fondoPendiente";
        private const string CLASE_CLASIFICADO = "fondoPendiente";
        private const string CLASE_EMPATE = "fondoEmpate";
        private const string CLASE_MENCION = "fondoMencion";

        private const string IMG_ORO = "/img/oro.png";
        private const string IMG_PLATA = "/img/plata.png";
        private const string IMG_BRONCE = "/img/bronce.png";
        private const string IMG_MENCION = "/img/diploma.png";

        private const string A = "A";
        private const string B = "B";
        private const string YOUNG = "Y";

        private const string ORO = "ORO";
        private const string ORO_1 = "ORO (I)";
        private const string ORO_2 = "ORO (II)";
        private const string ORO_3 = "ORO (III)";
        private const string NO_MEDALLA = "- - -";
        private const string PENDIENTE = "PENDIENTE";

        public TableManager(): this(false, null)
        {
        }

        public TableManager(bool admin, int? claveUsuario)
        {
            this.admin = admin;

            if (claveUsuario == null)
                this.claveUsuario = Persona.UsuarioNulo;
            else
                this.claveUsuario = (int)claveUsuario;

            currentOMI = null;
            currentResultados = null;
        }

        public Olimpiada getCurrentOMI()
        {
            return this.currentOMI;
        }

        public void setCurrentOMI(Olimpiada omi)
        {
            this.currentOMI = omi;
        }

        public void setCurrentOMI()
        {
            currentOMI = Olimpiada.obtenerOlimpiadaConClave(currentResultados.omi, currentResultados.tipoOlimpiada);
        }

        public string getIOIStatsLink(int id)
        {
            return "http://stats.ioinformatics.org/people/" + id;
        }

        public string getIOIStatsLinkForPerson()
        {
            int IOI = 1988;
            try
            {
                IOI += int.Parse(currentOMI.relacion);
            } catch (Exception)
            {
                return "";
            }

            return "http://stats.ioinformatics.org/delegations/MEX/" + IOI;
        }

        public void setCurrentResultados(Resultados datos)
        {
            this.currentResultados = datos;
        }

        public string obtenerClaseCSS(bool mostrarPendientes = true)
        {
            if (currentOMI.liveResults && mostrarPendientes)
                return CLASE_PENDIENTE;
            return obtenerClaseCSS(currentResultados.medalla);
        }

        public static string obtenerClaseCSS(Medalleros medalleros, TipoOlimpiada tipo)
        {
            if (tipo == TipoOlimpiada.OMI)
            {
                if (medalleros.OMI.oros > 0)
                    return CLASE_ORO;
                if (medalleros.OMI.platas > 0)
                    return CLASE_PLATA;
                if (medalleros.OMI.bronces > 0)
                    return CLASE_BRONCE;
                return String.Empty;
            }
            if (tipo == TipoOlimpiada.OMIS)
            {
                if (medalleros.OMIS.oros + medalleros.OMIP.oros > 0)
                    return CLASE_ORO;
                if (medalleros.OMIS.platas + medalleros.OMIP.platas > 0)
                    return CLASE_PLATA;
                if (medalleros.OMIS.bronces + medalleros.OMIP.bronces > 0)
                    return CLASE_BRONCE;
                return String.Empty;
            }
            return String.Empty;
        }

        public static string obtenerClaseCSS(Resultados.TipoMedalla medalla, bool top3 = false)
        {
            switch (medalla)
            {
                case Resultados.TipoMedalla.BRONCE:
                    return CLASE_BRONCE;
                case Resultados.TipoMedalla.PLATA:
                    return CLASE_PLATA;
                case Resultados.TipoMedalla.ORO:
                case Resultados.TipoMedalla.ORO_1:
                    return CLASE_ORO;
                case Resultados.TipoMedalla.ORO_2:
                    if (top3)
                        return CLASE_PLATA;
                    return CLASE_ORO;
                case Resultados.TipoMedalla.ORO_3:
                    if (top3)
                        return CLASE_BRONCE;
                    return CLASE_ORO;
                case Resultados.TipoMedalla.CLASIFICADO:
                    return CLASE_CLASIFICADO;
                case Resultados.TipoMedalla.EMPATE:
                    return CLASE_EMPATE;
                case Resultados.TipoMedalla.MENCION:
                    return CLASE_MENCION;
            }

            return String.Empty;
        }

        public bool mostrarLinea(bool overrideMostrarResultadosTotales = false)
        {
            return (overrideMostrarResultadosTotales || currentOMI.mostrarResultadosTotales) &&
                    (this.admin || currentOMI.datosPublicos || currentResultados.publico ||
                     (currentResultados.medalla != Resultados.TipoMedalla.NADA &&
                      currentResultados.medalla != Resultados.TipoMedalla.NULL) ||
                     currentResultados.usuario == this.claveUsuario ||
                     this.mostrarLogoIOI());
        }

        public bool faltaNombre()
        {
            return currentResultados.clave.StartsWith(Resultados.NOMBRE_FALTANTE);
        }

        public bool faltaClave()
        {
            return faltaClave(currentResultados.clave);
        }

        public static bool faltaClave(string clave)
        {
            return clave.StartsWith(Resultados.CLAVE_FALTANTE);
        }

        public bool esClaveDesconocida()
        {
            return esClaveDesconocida(currentResultados.clave);
        }

        public static bool esClaveDesconocida(string clave)
        {
            return clave.StartsWith(Resultados.CLAVE_DESCONOCIDA);
        }

        public bool mostrarLogoIOI()
        {
            return currentResultados.ioi == A || currentResultados.ioi == B;
        }

        public string medallaString(bool mostrarPrimeros = true, bool mostrarPendientes = true)
        {
            if (currentOMI.liveResults && mostrarPendientes)
                return PENDIENTE;

            switch (currentResultados.medalla)
            {
                case Resultados.TipoMedalla.ORO_1:
                    return mostrarPrimeros ? ORO_1 : ORO;
                case Resultados.TipoMedalla.ORO_2:
                    return mostrarPrimeros ? ORO_2 : ORO;
                case Resultados.TipoMedalla.ORO_3:
                    return mostrarPrimeros ? ORO_3 : ORO;
                case Resultados.TipoMedalla.ORO:
                case Resultados.TipoMedalla.PLATA:
                case Resultados.TipoMedalla.BRONCE:
                case Resultados.TipoMedalla.EMPATE:
                    return currentResultados.medalla.ToString();
                case Resultados.TipoMedalla.CLASIFICADO:
                    if (currentResultados.persona.genero == "F")
                        return "CLASIFICADA";
                    return "CLASIFICADO";
                case Resultados.TipoMedalla.MENCION:
                    return "MENCIÓN HONORÍFICA";
                default:
                    return NO_MEDALLA;
            }
        }

        public string puntosProblema(int dia, int problema)
        {
            if (dia == 1)
                return puntos(currentResultados.dia1, problema);
            return puntos(currentResultados.dia2, problema);
        }

        private string puntos(List<float?> problemas, int i)
        {
            return problemas[i] == null ? Resultados.NULL_POINTS : problemas[i].ToString();
        }

        public static string obtenerImagenMedalla(Resultados.TipoMedalla medalla)
        {
            switch (medalla)
            {
                case Resultados.TipoMedalla.ORO:
                case Resultados.TipoMedalla.ORO_1:
                case Resultados.TipoMedalla.ORO_2:
                case Resultados.TipoMedalla.ORO_3:
                    return IMG_ORO;
                case Resultados.TipoMedalla.PLATA:
                    return IMG_PLATA;
                case Resultados.TipoMedalla.BRONCE:
                    return IMG_BRONCE;
                case Resultados.TipoMedalla.MENCION:
                    return IMG_MENCION;
            }
            return "";
        }

        public string enlaceOMI(bool nombreCompleto = false, string olimpiada = null, bool incluirCiudad = false)
        {
            if (olimpiada == null)
                olimpiada = currentOMI.numero;

            if (olimpiada.EndsWith("b"))
            {
                if (nombreCompleto)
                    return "OMI Intermedia";
                else
                    return "OMII";
            }

            string enlace = "";
            if (currentOMI != null &&
                (currentOMI.tipoOlimpiada == TipoOlimpiada.OMIP ||
                 currentOMI.tipoOlimpiada == TipoOlimpiada.OMIS ||
                 currentOMI.tipoOlimpiada == TipoOlimpiada.OMIPO ||
                 currentOMI.tipoOlimpiada == TipoOlimpiada.OMISO))
            {
                enlace = currentOMI.omisActualNumber;
            }
            else
            {
                enlace = olimpiada;
            }

            string nombre = "OMI";
            if (currentOMI != null)
            {
                if (currentOMI.tipoOlimpiada == TipoOlimpiada.OMIPO)
                    nombre = "OMIP Online";
                else if (currentOMI.tipoOlimpiada == TipoOlimpiada.OMISO)
                {
                    if (currentOMI.año < 2024)
                        nombre = "OMIS Online";
                    else
                        nombre = "OMIPS Online";
                }
                else if (currentOMI.tipoOlimpiada == TipoOlimpiada.OMIS && currentOMI.año >= 2024)
                    nombre = "OMIPS";
                else
                    nombre = currentOMI.tipoOlimpiada.ToString();
            }
            enlace += "ª " + nombre;
            if (incluirCiudad && currentOMI != null && !currentOMI.esOnline)
            {
                enlace += ": " + (currentOMI.claveEstado == "MDF" ? "" : currentOMI.ciudad + ", ");
            }
            return enlace;
        }

        public bool tieneDiplomas()
        {
            if (currentOMI == null)
                return false;
            return currentOMI.diplomasOnline;
        }

        public bool tieneDiplomasOMIPOS()
        {
            return tieneDiplomas() && currentOMI.año >= 2022;
        }

        public static string participaciones(MiembroDelegacion md, TipoOlimpiada tipo)
        {
            string result = "";

            int count = md.numeroParticipaciones[tipo];
            switch (count)
            {
                case 1:
                    result += "Primera";
                    break;
                case 2:
                    result += "Segunda";
                    break;
                case 3:
                    result += "Tercera";
                    break;
                case 4:
                    result += "Cuarta";
                    break;
                case 5:
                    result += "Quinta";
                    break;
                case 6:
                    result += "Sexta";
                    break;
                case 7:
                    result += "Séptima";
                    break;
            }
            result += " participación";

            if (tipo != TipoOlimpiada.OMIP &&
                (md.numeroParticipaciones.ContainsKey(TipoOlimpiada.OMIP) &&
                    md.numeroParticipaciones[TipoOlimpiada.OMIP] > 0 ||
                 md.numeroParticipaciones.ContainsKey(TipoOlimpiada.OMIS) &&
                    md.numeroParticipaciones[TipoOlimpiada.OMIS] > 0 &&
                    tipo == TipoOlimpiada.OMI))
                result += " en esta categoría";

            return result;
        }

        public static bool isOMIPOS(TipoOlimpiada tipo)
        {
            return tipo == TipoOlimpiada.OMIPO || tipo == TipoOlimpiada.OMISO;
        }

        public static string getPreEstado(string estado, bool includeA = true)
        {
            if (estado == "MEX")
                return includeA ? "al" : "el";
            if (estado == "MDF")
                return includeA ? "a la" : "la";
            return (includeA ? "al" : "el") + " estado de";
        }

        public static bool IsDebug()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}