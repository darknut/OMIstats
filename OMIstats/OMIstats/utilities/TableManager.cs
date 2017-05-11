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

        private const string CLASE_BRONCE = "table-td-bronce";
        private const string CLASE_PLATA = "table-td-plata";
        private const string CLASE_ORO = "table-td-oro";

        private const string A = "A";
        private const string B = "B";

        private const string ORO = "ORO";
        private const string ORO_1 = "ORO (I)";
        private const string ORO_2 = "ORO (II)";
        private const string ORO_3 = "ORO (III)";

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
            switch (currentResultados.medalla)
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
                     currentResultados.usuario == this.claveUsuario);
        }

        public bool faltaNombre()
        {
            return currentResultados.clave.StartsWith(Resultados.NOMBRE_FALTANTE);
        }

        public bool faltaClave()
        {
            return currentResultados.clave.StartsWith(Resultados.CLAVE_FALTANTE);
        }

        public bool esClaveDesconocida()
        {
            return currentResultados.clave.StartsWith(Resultados.CLAVE_DESCONOCIDA);
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
            }

            return String.Empty;
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
    }
}