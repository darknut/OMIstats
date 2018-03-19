using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMIstats.Models
{
    public class Olimpiada
    {
        public bool noMedallistasConocidos;
        public bool puntosDesconocidos;

        public static void guardaProblemas(string olimpiada, TipoOlimpiada tipoOlimpiada, int problemas, int dia)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" update olimpiada set datospublicos = 1 ");
            if (dia == 1)
                query.Append(", problemasDia1 = ");
            else
                query.Append(", problemasDia2 = ");
            query.Append(problemas);
            query.Append(", mostrarResultadosPorDia = ");
            query.Append(dia == 1 ? "0" : "1");
            query.Append(", mostrarResultadosPorProblema = 1 ");
            query.Append(", mostrarResultadosTotales = 1 ");
            query.Append(" where numero = ");
            query.Append(Utilities.Cadenas.comillas(olimpiada));
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));

            db.EjecutarQuery(query.ToString());
        }

        public static Olimpiada obtenerOlimpiadaConClave(string clave, TipoOlimpiada tipo)
        {
            throw new InvalidOperationException("obtenerOlimpiadaConClave no permitido en este contexto");
        }
    }
}
