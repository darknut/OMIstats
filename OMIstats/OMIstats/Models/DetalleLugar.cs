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
    public class DetalleLugar
    {
        public string omi;
        public TipoOlimpiada tipoOlimpiada;
        public string clave;
        public int timestamp;
        public int dia;
        public Resultados.TipoMedalla medalla;
        public int lugar;

        private DetalleLugar()
        {
        }

        public DetalleLugar(string omi, TipoOlimpiada tipoOlimpiada, string clave, int timestamp, int dia, Resultados.TipoMedalla medalla, int lugar)
        {
            this.omi = omi;
            this.tipoOlimpiada = tipoOlimpiada;
            this.clave = clave;
            this.timestamp = timestamp;
            this.dia = dia;
            this.medalla = medalla;
            this.lugar = lugar;
        }
#if OMISTATS
        private static void llenarDatos(DataRow row, OverlayLugares lugares)
        {
            lugares.lugar.Add(DataRowParser.ToInt(row["lugar"]));
            lugares.timestamp.Add(DataRowParser.ToInt(row["timestamp"]));
            lugares.medalla.Add(DataRowParser.ToInt(row["medalla"]));
        }

        private void llenarDatos(DataRow row)
        {
            clave = DataRowParser.ToString(row["clave"]);
            lugar = DataRowParser.ToInt(row["lugar"]);
            medalla = DataRowParser.ToTipoMedalla(row["medalla"]);
        }

        public static OverlayLugares cargarResultados(string omi, TipoOlimpiada tipoOlimpiada, int dia, string clave)
        {
            OverlayLugares lugares = new OverlayLugares();

            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from detallelugar ");
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

            foreach (DataRow r in table.Rows)
            {
                llenarDatos(r, lugares);
            }

            if (lugares.timestamp[0] != 0 && dia == 1)
            {
                lugares.timestamp.Insert(0, 0);
                lugares.lugar.Insert(0, 0);
                lugares.medalla.Insert(0, 7);
            }

            return lugares;
        }

        public static Dictionary<string, DetalleLugar> obtenerLugaresConTimestamp(string clave, TipoOlimpiada tipo, int dia, int timestamp)
        {
            Dictionary<string, DetalleLugar> lugares = new Dictionary<string, DetalleLugar>();
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from detallelugar ");
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
                DetalleLugar dl = new DetalleLugar();
                dl.llenarDatos(table.Rows[i]);
                lugares.Add(dl.clave, dl);
            }

            return lugares;
        }
#endif
        /// <summary>
        /// Guarda los datos del objeto en la base de datos
        /// </summary>
        public void guardar()
        {
            StringBuilder query = new StringBuilder();
            Acceso db = new Acceso();

            query.Append("insert into DetalleLugar values(");
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
            query.Append((int)medalla);
            query.Append(",");
            query.Append(lugar);
            query.Append(")");

            db.EjecutarQuery(query.ToString());
        }

        private static void borrar(string omi, string clase, string clave, int timestamp, int dia)
        {
            StringBuilder query = new StringBuilder();
            Acceso db = new Acceso();

            query.Append(" delete DetalleLugar where olimpiada = ");
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

            query.Append(" select * from DetalleLugar where olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipo.ToString().ToLower()));
            query.Append(" and dia = ");
            query.Append(dia);
            query.Append(" order by clave, timestamp asc ");

            db.EjecutarQuery(query.ToString());

            DataTable table = db.getTable();

            bool first = false;
            DetalleLugar anterior = new DetalleLugar();
            DetalleLugar actual = new DetalleLugar();
            foreach (DataRow r in table.Rows)
            {
                actual.lugar = DataRowParser.ToInt(r["lugar"]);
                actual.timestamp = DataRowParser.ToInt(r["timestamp"]);
                actual.medalla = DataRowParser.ToTipoMedalla(r["medalla"]);
                actual.clave = DataRowParser.ToString(r["clave"]);

                if (actual.clave != anterior.clave)
                {
                    first = true;
                }
                else
                {
                    if (actual.medalla == anterior.medalla &&
                        actual.lugar == anterior.lugar)
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

                anterior.lugar = actual.lugar;
                anterior.timestamp = actual.timestamp;
                anterior.medalla = actual.medalla;
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
            query.Append(" select clave, MAX(timestamp) from DetalleLugar where olimpiada = ");
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
                    query.Append(" delete DetalleLugar where olimpiada = ");
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
                    query.Append(" update DetalleLugar set timestamp = ");
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
        public static void actualizarUltimo(string omi, TipoOlimpiada tipo, int dia, string clave, int lugar, Resultados.TipoMedalla medalla)
        {
            StringBuilder query = new StringBuilder();
            Acceso db = new Acceso();

            // Primero obtenemos el timestamp mas grande
            query.Append(" select MAX(timestamp) from DetalleLugar where olimpiada = ");
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
            query.Append("update DetalleLugar set lugar = ");
            query.Append(lugar);
            if (medalla != Resultados.TipoMedalla.DESCALIFICADO)
            {
                query.Append(", medalla = ");
                query.Append((int)medalla);
            }
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