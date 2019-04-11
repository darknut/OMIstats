using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using OMIstats.Ajax;

namespace OMIstats.Models
{
    public class DetallePuntos
    {
        public string omi;
        public TipoOlimpiada tipoOlimpiada;
        public string clave;
        public int timestamp;
        public int dia;
        public List<float?> puntosProblemas;
        public float? puntosDia;

        private DetallePuntos()
        {
        }

        public DetallePuntos(string omi, TipoOlimpiada tipoOlimpiada, string clave, int timestamp, int dia, List<float?> puntos)
        {
            this.omi = omi;
            this.tipoOlimpiada = tipoOlimpiada;
            this.clave = clave;
            this.timestamp = timestamp;
            this.dia = dia;
            this.puntosProblemas = puntos;
            puntosDia = 0;

            foreach (float? t in puntos)
                if (t != null)
                    puntosDia += t;
        }

#if OMISTATS
        private static OverlayPuntos llenarDatos(DataRow row, int dia1, int dia2)
        {
            OverlayPuntos res = new OverlayPuntos();

            res.timestamp = (int) row["timestamp"];
            res.dia1 = new List<float?>();
            for (int i = 0; i < dia1; i++)
                res.dia1.Add(float.Parse(row["puntosD1P" + (i + 1)].ToString()));
            if (dia2 > 0)
            {
                res.totalDia1 = float.Parse(row["puntosD1"].ToString());
                res.dia2 = new List<float?>();
                for (int i = 0; i < dia2; i++)
                    res.dia2.Add( float.Parse(row["puntosD2P" + (i + 1)].ToString()));
                res.totalDia2 = float.Parse(row["puntosD2"].ToString());
            }
            res.total = float.Parse(row["puntos"].ToString());

            return res;
        }

        /// <summary>
        /// Obtiene la lista de resultados de un usuario en particular, de una olimpiada en particular
        /// </summary>
        /// <returns></returns>
        public static List<OverlayPuntos> cargarResultados(string omi, TipoOlimpiada tipoOlimpiada, string clave, int dia1, int dia2)
        {
            List<OverlayPuntos> lista = new List<OverlayPuntos>();

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from detallepuntos ");
            query.Append(" where clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(omi));
            query.Append(" and clave = ");
            query.Append(Utilities.Cadenas.comillas(clave));
            query.Append(" order by timestamp asc ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                lista.Add(llenarDatos(r, dia1, dia2));
            }

            return lista;
        }
#endif
        /// <summary>
        /// Guarda los datos del objeto en la base de datos
        /// </summary>
        public void guardar()
        {
            StringBuilder query = new StringBuilder();
            Utilities.Acceso db = new Utilities.Acceso();

            query.Append("insert into DetallePuntos values(");
            query.Append(Utilities.Cadenas.comillas(omi));
            query.Append(",");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(",");
            query.Append(Utilities.Cadenas.comillas(clave));
            query.Append(",");
            query.Append(timestamp);
            query.Append(",");
            query.Append(dia);
            query.Append(",");
            query.Append(this.puntosProblemas[0] == null ? "0" : this.puntosProblemas[0].ToString());
            query.Append(",");
            query.Append(this.puntosProblemas[1] == null ? "0" : this.puntosProblemas[1].ToString());
            query.Append(",");
            query.Append(this.puntosProblemas[2] == null ? "0" : this.puntosProblemas[2].ToString());
            query.Append(",");
            query.Append(this.puntosProblemas[3] == null ? "0" : this.puntosProblemas[3].ToString());
            query.Append(",");
            query.Append(this.puntosProblemas[4] == null ? "0" : this.puntosProblemas[4].ToString());
            query.Append(",");
            query.Append(this.puntosProblemas[5] == null ? "0" : this.puntosProblemas[5].ToString());
            query.Append(",");
            query.Append(this.puntosDia == null ? "0" : this.puntosDia.ToString());
            query.Append(")");

            db.EjecutarQuery(query.ToString());
        }
    }
}