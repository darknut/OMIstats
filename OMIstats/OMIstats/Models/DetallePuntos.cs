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
        public List<float?> dia1;
        public float? totalDia1;
        public List<float?> dia2;
        public float? totalDia2;
        public float? total;

        private DetallePuntos()
        {
        }

        public DetallePuntos(string omi, TipoOlimpiada tipoOlimpiada, string clave, int timestamp, List<float?> dia1, List<float?> dia2)
        {
            this.omi = omi;
            this.tipoOlimpiada = tipoOlimpiada;
            this.clave = clave;
            this.timestamp = timestamp;
            this.dia1 = dia1;
            this.dia2 = dia2;
            totalDia1 = 0;
            totalDia2 = 0;
            total = 0;

            foreach (float? t in dia1)
            {
                if (t != null)
                {
                    totalDia1 += t;
                    total += t;
                }
            }

            foreach (float? t in dia2)
            {
                if (t != null)
                {
                    totalDia2 += t;
                    total += t;
                }
            }
        }

        private static OverlayAjax llenarDatos(DataRow row, int dia1, int dia2)
        {
            OverlayAjax res = new OverlayAjax();

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
        public static List<OverlayAjax> cargarResultados(string omi, TipoOlimpiada tipoOlimpiada, string clave, int dia1, int dia2)
        {
            List<OverlayAjax> lista = new List<OverlayAjax>();

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
            query.Append(this.dia1[0] == null ? "0" : this.dia1[0].ToString());
            query.Append(",");
            query.Append(this.dia1[1] == null ? "0" : this.dia1[1].ToString());
            query.Append(",");
            query.Append(this.dia1[2] == null ? "0" : this.dia1[2].ToString());
            query.Append(",");
            query.Append(this.dia1[3] == null ? "0" : this.dia1[3].ToString());
            query.Append(",");
            query.Append(this.dia1[4] == null ? "0" : this.dia1[4].ToString());
            query.Append(",");
            query.Append(this.dia1[5] == null ? "0" : this.dia1[5].ToString());
            query.Append(",");
            query.Append(this.totalDia1 == null ? "0" : this.totalDia1.ToString());
            query.Append(",");
            query.Append(this.dia2[0] == null ? "0" : this.dia2[0].ToString());
            query.Append(",");
            query.Append(this.dia2[1] == null ? "0" : this.dia2[1].ToString());
            query.Append(",");
            query.Append(this.dia2[2] == null ? "0" : this.dia2[2].ToString());
            query.Append(",");
            query.Append(this.dia2[3] == null ? "0" : this.dia2[3].ToString());
            query.Append(",");
            query.Append(this.dia2[4] == null ? "0" : this.dia2[4].ToString());
            query.Append(",");
            query.Append(this.dia2[5] == null ? "0" : this.dia2[5].ToString());
            query.Append(",");
            query.Append(this.totalDia2 == null ? "0" : this.totalDia2.ToString());
            query.Append(",");
            query.Append(this.total == null ? "0" : this.total.ToString());
            query.Append(")");

            db.EjecutarQuery(query.ToString());
        }
    }
}