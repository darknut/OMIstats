using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using OMIstats.Ajax;
using OMIstats.Utilities;

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
        private static void llenarDatos(DataRow row, OverlayPuntos puntos, int problemas)
        {
            puntos.timestamp.Add(DataRowParser.ToInt(row["timestamp"]));
            for (int i = 0; i < problemas; i++)
                puntos.problemas[i].Add(DataRowParser.ToFloat(row["puntosP" + (i + 1)]));
            puntos.puntos.Add(DataRowParser.ToFloat(row["puntosD"]));
        }

        /// <summary>
        /// Obtiene la lista de resultados de un usuario en particular, de una olimpiada en particular
        /// </summary>
        /// <returns></returns>
        public static OverlayPuntos cargarResultados(string omi, TipoOlimpiada tipoOlimpiada, string clave, int dia, int problemas)
        {
            OverlayPuntos puntos = new OverlayPuntos();

            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from detallepuntos ");
            query.Append(" where clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" and clave = ");
            query.Append(Cadenas.comillas(clave));
            query.Append(" and dia = ");
            query.Append(dia);
            query.Append(" order by timestamp asc ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                llenarDatos(table.Rows[i], puntos, problemas);
            }

            if (puntos.timestamp[0] != 0)
            {
                puntos.timestamp.Insert(0, 0);
                for (int i = 0; i < problemas; i++)
                    puntos.problemas[i].Insert(0, 0);
                puntos.puntos.Insert(0, 0);
            }

            puntos.problemas = null;
            return puntos;
        }
#endif
        /// <summary>
        /// Guarda los datos del objeto en la base de datos
        /// </summary>
        public void guardar()
        {
            StringBuilder query = new StringBuilder();
            Acceso db = new Acceso();

            query.Append("insert into DetallePuntos values(");
            query.Append(Cadenas.comillas(omi));
            query.Append(",");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(",");
            query.Append(Cadenas.comillas(clave));
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

        private static void borrar(string omi, string clase, string clave, int timestamp, int dia)
        {
            StringBuilder query = new StringBuilder();
            Acceso db = new Acceso();

            query.Append(" delete DetallePuntos where olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" and clase =  ");
            query.Append(Cadenas.comillas(clase));
            query.Append(" and clave =  ");
            query.Append(Cadenas.comillas(clave));
            query.Append(" and timestamp =  ");
            query.Append(timestamp);
            query.Append(" and dia =  ");
            query.Append(dia);

            db.EjecutarQuery(query.ToString());
        }

        public static void clean(string omi)
        {
            StringBuilder query = new StringBuilder();
            Acceso db = new Acceso();

            query.Append(" select * from DetallePuntos where olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" order by clase, clave, dia, timestamp asc ");

            db.EjecutarQuery(query.ToString());

            DataTable table = db.getTable();

            bool first = false;
            DetallePuntos anterior = new DetallePuntos();
            DetallePuntos actual = new DetallePuntos();
            foreach (DataRow r in table.Rows)
            {
                actual.puntosDia = DataRowParser.ToFloat(r["puntosD"]);
                actual.timestamp = DataRowParser.ToInt(r["timestamp"]);
                actual.dia = DataRowParser.ToInt(r["dia"]);
                actual.clave = DataRowParser.ToString(r["clave"]);
                actual.tipoOlimpiada = DataRowParser.ToTipoOlimpiada(r["clase"]);

                if (actual.tipoOlimpiada != anterior.tipoOlimpiada ||
                    actual.clave != anterior.clave ||
                    actual.dia != anterior.dia)
                {
                    first = true;
                }
                else
                {
                    if (actual.puntosDia == anterior.puntosDia)
                    {
                        if (!first)
                            borrar(omi, anterior.tipoOlimpiada.ToString().ToLower(), anterior.clave, anterior.timestamp, anterior.dia);
                        first = false;
                    }
                    else
                    {
                        first = true;
                    }

                }

                anterior.puntosDia = actual.puntosDia;
                anterior.timestamp = actual.timestamp;
                anterior.dia = actual.dia;
                anterior.clave = actual.clave;
                anterior.tipoOlimpiada = actual.tipoOlimpiada;
            }
        }
    }
}