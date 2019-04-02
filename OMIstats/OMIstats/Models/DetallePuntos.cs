using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

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

        private void llenarDatos(DataRow row)
        {
            omi = row["olimpiada"].ToString().Trim();
            clave = row["clave"].ToString().Trim();
            timestamp = (int) row["timestamp"];
            for (int i = 0; i < 6; i++)
                if (row["puntosD1P" + (i + 1)] == DBNull.Value)
                    dia1[i] = null;
                else
                    dia1[i] = float.Parse(row["puntosD1P" + (i + 1)].ToString());
            totalDia1 = float.Parse(row["puntosD1"].ToString());
            for (int i = 0; i < 6; i++)
                if (row["puntosD2P" + (i + 1)] == DBNull.Value)
                    dia2[i] = null;
                else
                    dia2[i] = float.Parse(row["puntosD2P" + (i + 1)].ToString());
            totalDia2 = float.Parse(row["puntosD2"].ToString());
            total = float.Parse(row["puntos"].ToString());
        }

        /// <summary>
        /// Obtiene la lista de resultados de un usuario en particular, de una olimpiada en particular
        /// </summary>
        /// <returns></returns>
        private static List<DetallePuntos> cargarResultadosDeUsuario(string omi, TipoOlimpiada tipoOlimpiada, string clave)
        {
            List<DetallePuntos> lista = new List<DetallePuntos>();

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
                DetallePuntos res = new DetallePuntos();
                res.tipoOlimpiada = tipoOlimpiada;
                res.llenarDatos(r);

                lista.Add(res);
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
            query.Append(this.dia1[0] == null ? "null" : this.dia1[0].ToString());
            query.Append(",");
            query.Append(this.dia1[1] == null ? "null" : this.dia1[1].ToString());
            query.Append(",");
            query.Append(this.dia1[2] == null ? "null" : this.dia1[2].ToString());
            query.Append(",");
            query.Append(this.dia1[3] == null ? "null" : this.dia1[3].ToString());
            query.Append(",");
            query.Append(this.dia1[4] == null ? "null" : this.dia1[4].ToString());
            query.Append(",");
            query.Append(this.dia1[5] == null ? "null" : this.dia1[5].ToString());
            query.Append(",");
            query.Append(this.totalDia1 == null ? "null" : this.totalDia1.ToString());
            query.Append(",");
            query.Append(this.dia2[0] == null ? "null" : this.dia2[0].ToString());
            query.Append(",");
            query.Append(this.dia2[1] == null ? "null" : this.dia2[1].ToString());
            query.Append(",");
            query.Append(this.dia2[2] == null ? "null" : this.dia2[2].ToString());
            query.Append(",");
            query.Append(this.dia2[3] == null ? "null" : this.dia2[3].ToString());
            query.Append(",");
            query.Append(this.dia2[4] == null ? "null" : this.dia2[4].ToString());
            query.Append(",");
            query.Append(this.dia2[5] == null ? "null" : this.dia2[5].ToString());
            query.Append(",");
            query.Append(this.totalDia2 == null ? "null" : this.totalDia2.ToString());
            query.Append(",");
            query.Append(this.total == null ? "null" : this.total.ToString());
            query.Append(")");

            db.EjecutarQuery(query.ToString());
        }
    }
}