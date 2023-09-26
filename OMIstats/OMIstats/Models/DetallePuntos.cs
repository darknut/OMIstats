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

        private void llenarDatos(DataRow row)
        {
            dia = DataRowParser.ToInt(row["dia"]);
            clave = DataRowParser.ToString(row["clave"]);
            timestamp = DataRowParser.ToInt(row["timestamp"]);
            puntosProblemas = new List<float?>();
            for (int i = 0; i < 6; i++)
                puntosProblemas.Add(DataRowParser.ToFloat(row["puntosP" + (i + 1)]));
            puntosDia = DataRowParser.ToFloat(row["puntosD"]);
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

        public static int obtenerTimestampMasReciente(string clave, TipoOlimpiada tipo, int dia)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select top 1 timestamp from detallepuntos ");
            query.Append(" where clase = ");
            query.Append(Cadenas.comillas(tipo.ToString().ToLower()));
            query.Append(" and olimpiada = ");
            query.Append(Cadenas.comillas(clave));
            query.Append(" and dia = ");
            query.Append(dia);
            query.Append(" order by timestamp desc ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            return DataRowParser.ToInt(table.Rows[0][0]);
        }

        public static Dictionary<string, DetallePuntos> obtenerPuntosConTimestamp(string clave, TipoOlimpiada tipo, int dia, int timestamp)
        {
            Dictionary<string, DetallePuntos> puntos = new Dictionary<string, DetallePuntos>();
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from detallepuntos ");
            query.Append(" where clase = ");
            query.Append(Cadenas.comillas(tipo.ToString().ToLower()));
            query.Append(" and olimpiada = ");
            query.Append(Cadenas.comillas(clave));
            query.Append(" and dia = ");
            query.Append(dia);
            query.Append(" and timestamp = ");
            query.Append(timestamp);

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                DetallePuntos dp = new DetallePuntos();
                dp.llenarDatos(table.Rows[i]);
                puntos.Add(dp.clave, dp);
            }

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
            query.Append(Cadenas.toStringOrDefault(this.puntosProblemas[0]));
            query.Append(",");
            query.Append(Cadenas.toStringOrDefault(this.puntosProblemas[1]));
            query.Append(",");
            query.Append(Cadenas.toStringOrDefault(this.puntosProblemas[2]));
            query.Append(",");
            query.Append(Cadenas.toStringOrDefault(this.puntosProblemas[3]));
            query.Append(",");
            query.Append(Cadenas.toStringOrDefault(this.puntosProblemas[4]));
            query.Append(",");
            query.Append(Cadenas.toStringOrDefault(this.puntosProblemas[5]));
            query.Append(",");
            query.Append(Cadenas.toStringOrDefault(this.puntosDia));
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

        public static void clean(string omi, TipoOlimpiada tipo, int dia)
        {
            StringBuilder query = new StringBuilder();
            Acceso db = new Acceso();

            query.Append(" select * from DetallePuntos where olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipo.ToString().ToLower()));
            query.Append(" and dia = ");
            query.Append(dia);
            query.Append(" order by clave, timestamp asc ");

            db.EjecutarQuery(query.ToString());

            DataTable table = db.getTable();

            bool first = false;
            DetallePuntos anterior = new DetallePuntos();
            DetallePuntos actual = new DetallePuntos();
            foreach (DataRow r in table.Rows)
            {
                actual.puntosDia = DataRowParser.ToFloat(r["puntosD"]);
                actual.timestamp = DataRowParser.ToInt(r["timestamp"]);
                actual.clave = DataRowParser.ToString(r["clave"]);

                if (actual.clave != anterior.clave)
                {
                    first = true;
                }
                else
                {
                    if (actual.puntosDia == anterior.puntosDia)
                    {
                        if (!first)
                            borrar(omi, tipo.ToString().ToLower(), anterior.clave, anterior.timestamp, dia);
                        first = false;
                    }
                    else
                    {
                        first = true;
                    }
                }

                anterior.puntosDia = actual.puntosDia;
                anterior.timestamp = actual.timestamp;
                anterior.clave = actual.clave;
            }
        }

        public static void trim(string omi, TipoOlimpiada tipo, int tiempo, int dia = 1)
        {
            if (dia > 2)
                return;

            StringBuilder query = new StringBuilder();
            Acceso db = new Acceso();

            // Primero obtenemos una lista de todos los timestamps mas grandes
            query.Append(" select clave, MAX(timestamp) from DetallePuntos where olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipo.ToString().ToLower()));
            query.Append(" and dia = ");
            query.Append(dia);
            query.Append(" group by clave ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                string clave = DataRowParser.ToString(r[0]);
                int timestamp = DataRowParser.ToInt(r[1]);

                // Si el último timestamp es diferente del tiempo que tenemos...
                if (timestamp != tiempo)
                {
                    // ...borramos todos las entradas superiores y menores al que tenemos
                    query.Clear();
                    query.Append(" delete DetallePuntos where olimpiada = ");
                    query.Append(Cadenas.comillas(omi));
                    query.Append(" and clase = ");
                    query.Append(Cadenas.comillas(tipo.ToString().ToLower()));
                    query.Append(" and dia = ");
                    query.Append(dia);
                    query.Append(" and clave = ");
                    query.Append(Cadenas.comillas(clave));
                    query.Append(" and timestamp >= ");
                    query.Append(tiempo);
                    query.Append(" and timestamp <> ");
                    query.Append(timestamp);

                    db.EjecutarQuery(query.ToString());

                    // ... y actualizamos el que tenemos para que tenga ese timestamp
                    query.Clear();
                    query.Append(" update DetallePuntos set timestamp = ");
                    query.Append(tiempo);
                    query.Append(" where olimpiada = ");
                    query.Append(Cadenas.comillas(omi));
                    query.Append(" and clase = ");
                    query.Append(Cadenas.comillas(tipo.ToString().ToLower()));
                    query.Append(" and dia = ");
                    query.Append(dia);
                    query.Append(" and clave = ");
                    query.Append(Cadenas.comillas(clave));
                    query.Append(" and timestamp = ");
                    query.Append(timestamp);

                    db.EjecutarQuery(query.ToString());
                }
            }

            // Finalmente hacemos lo mismo con dia 2
            trim(omi, tipo, tiempo, dia + 1);
        }

        /// <summary>
        /// Actualiza la última entrada en la tabla para el concursante mandado como parámetro
        /// </summary>
        public static void actualizarUltimo(string omi, TipoOlimpiada tipo, int dia, string clave, List<float?>puntos, float? total)
        {
            StringBuilder query = new StringBuilder();
            Acceso db = new Acceso();

            // Primero obtenemos el timestamp mas grande
            query.Append(" select MAX(timestamp) from DetallePuntos where olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipo.ToString().ToLower()));
            query.Append(" and dia = ");
            query.Append(dia);
            query.Append(" and clave = ");
            query.Append(Cadenas.comillas(clave));

            db.EjecutarQuery(query.ToString());

            DataTable table = db.getTable();

            if (table.Rows.Count == 0)
                return;

            int timestamp = DataRowParser.ToInt(table.Rows[0][0]);
            query.Clear();

            // Ahora actualizamos los puntos
            query.Append("update DetallePuntos set puntosP1 = ");
            query.Append(Cadenas.toStringOrDefault(puntos[0]));
            query.Append(", puntosP2 = ");
            query.Append(Cadenas.toStringOrDefault(puntos[1]));
            query.Append(", puntosP3 = ");
            query.Append(Cadenas.toStringOrDefault(puntos[2]));
            query.Append(", puntosP4 = ");
            query.Append(Cadenas.toStringOrDefault(puntos[3]));
            query.Append(", puntosP5 = ");
            query.Append(Cadenas.toStringOrDefault(puntos[4]));
            query.Append(", puntosP6 = ");
            query.Append(Cadenas.toStringOrDefault(puntos[5]));
            query.Append(", puntosD = ");
            query.Append(Cadenas.toStringOrDefault(total));
            query.Append(" where olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipo.ToString().ToLower()));
            query.Append(" and dia = ");
            query.Append(dia);
            query.Append(" and clave = ");
            query.Append(Cadenas.comillas(clave));
            query.Append(" and timestamp = ");
            query.Append(timestamp);

            db.EjecutarQuery(query.ToString());
        }
    }
}