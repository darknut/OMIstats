using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace OMIstats.Models
{
    public class CachedResult
    {
        public int lugar;
        public string clave;
        public List<float?> puntos;
        public float totalDia;
        public float total;
        public string medalla;

        private void llenarDatos(DataRow row, int dia, int problemas)
        {
            puntos = new List<float?>();

            lugar = (int)row["lugar"];
            clave = row["clave"].ToString().Trim();
            for (int i = 1; i <= problemas; i++)
                if (row["puntosD" + dia + "P" + i] == DBNull.Value)
                    puntos.Add(null);
                else
                    puntos.Add(float.Parse(row["puntosD" + dia + "P" + i].ToString()));
            totalDia = float.Parse(row["puntosD" + dia].ToString());
            total = float.Parse(row["puntos"].ToString());
            medalla = Enum.Parse(typeof(Resultados.TipoMedalla), row["medalla"].ToString()).ToString();
        }

        /// <summary>
        /// Regresa los resultados de la olimpiada mandada como parametro
        /// </summary>
        /// <param name="omi">La olimpiada en cuestión</param>
        /// <param name="tipoOlimpiada">El tipo de olimpiada</param>
        /// <param name="dia">El dia del cual se quieren los resultados</param>
        /// <returns>Una lista con los resultados</returns>
        public static List<CachedResult> cargarResultados(string omi, TipoOlimpiada tipoOlimpiada, int dia, int problemas)
        {
            List<CachedResult> lista = new List<CachedResult>();

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from resultados ");
            query.Append(" where clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(omi));
            query.Append(" order by puntos desc, clave asc");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                CachedResult res = new CachedResult();
                res.llenarDatos(r, dia, problemas);

                lista.Add(res);
            }

            return lista;
        }
    }

    public class AjaxResponse
    {
        public enum Status
        {
            UPDATED,
            NOT_CHANGED,
            FINISHED,
            ERROR
        }

        public List<CachedResult> resultados;
        public int secondsSinceUpdate;
        public int timeToFinish;
        public string status;
    }
}