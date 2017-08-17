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

        private const string IMG_ORO = "/img/oro.png";
        private const string IMG_PLATA = "/img/plata.png";
        private const string IMG_BRONCE = "/img/bronce.png";

        private const string A = "A";
        private const string B = "B";

        private const string ORO = "ORO";
        private const string ORO_1 = "ORO (I)";
        private const string ORO_2 = "ORO (II)";
        private const string ORO_3 = "ORO (III)";
        private const string NO_MEDALLA = "- - -";

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

        public void setCurrentOMI(Olimpiada omi)
        {
            this.currentOMI = omi;
        }

        public void setCurrentOMI()
        {
            currentOMI = Olimpiada.obtenerOlimpiadaConClave(currentResultados.omi, currentResultados.tipoOlimpiada);
        }

        public void setCurrentResultados(Resultados datos)
        {
            this.currentResultados = datos;
        }

        public string obtenerClaseCSS()
        {
            return obtenerClaseCSS(currentResultados.medalla);
        }

        public static string obtenerClaseCSS(Resultados.TipoMedalla medalla)
        {
            switch (medalla)
            {
                case Resultados.TipoMedalla.BRONCE:
                    return CLASE_BRONCE;
                case Resultados.TipoMedalla.PLATA:
                    return CLASE_PLATA;
                case OMIstats.Models.Resultados.TipoMedalla.ORO:
                case OMIstats.Models.Resultados.TipoMedalla.ORO_1:
                case OMIstats.Models.Resultados.TipoMedalla.ORO_2:
                case OMIstats.Models.Resultados.TipoMedalla.ORO_3:
                    return CLASE_ORO;
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

        public string medallaString(bool mostrarPrimeros = true)
        {
            switch (currentResultados.medalla)
            {
                case Resultados.TipoMedalla.ORO_1:
                    return mostrarPrimeros ? ORO_1 : ORO;
                case Resultados.TipoMedalla.ORO_2:
                    return mostrarPrimeros ? ORO_2 : ORO;
                case OMIstats.Models.Resultados.TipoMedalla.ORO_3:
                    return mostrarPrimeros ? ORO_3 : ORO;
                case OMIstats.Models.Resultados.TipoMedalla.ORO:
                case OMIstats.Models.Resultados.TipoMedalla.PLATA:
                case OMIstats.Models.Resultados.TipoMedalla.BRONCE:
                    return currentResultados.medalla.ToString();
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
            }
            return "";
        }

        public string enlaceOMI(bool nombreCompleto = false)
        {
            if (currentOMI.numero.EndsWith("b"))
            {
                if (nombreCompleto)
                    return "OMI Intermedia";
                else
                    return "OMII";
            }

            string enlace = "";
            if (currentOMI.tipoOlimpiada == Olimpiada.TipoOlimpiada.OMIP ||
                currentOMI.tipoOlimpiada == Olimpiada.TipoOlimpiada.OMIS)
            {
                enlace = currentOMI.omisActualNumber;
            }
            else
            {
                enlace = currentOMI.numero;
            }
            return enlace + "ª " + currentOMI.tipoOlimpiada.ToString();
        }
    }
}