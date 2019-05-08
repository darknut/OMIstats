using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using OMIstats.Ajax;

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
            lugares.lugar.Add((int)row["lugar"]);
            lugares.timestamp.Add((int)row["timestamp"]);
            lugares.medalla.Add((int)row["medalla"]);
        }

        public static OverlayLugares cargarResultados(string omi, TipoOlimpiada tipoOlimpiada, int dia, string clave)
        {
            OverlayLugares lugares = new OverlayLugares();

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from detallelugar ");
            query.Append(" where clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(omi));
            query.Append(" and clave = ");
            query.Append(Utilities.Cadenas.comillas(clave));
            query.Append(" and dia = ");
            query.Append(dia);
            query.Append(" order by timestamp asc ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                llenarDatos(r, lugares);
            }

            if (lugares.timestamp[0] != 0)
            {
                lugares.timestamp.Insert(0, 0);
                lugares.lugar.Insert(0, 0);
                lugares.medalla.Insert(0, 7);
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
            Utilities.Acceso db = new Utilities.Acceso();

            query.Append("insert into DetalleLugar values(");
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
            query.Append((int)medalla);
            query.Append(",");
            query.Append(lugar);
            query.Append(")");

            db.EjecutarQuery(query.ToString());
        }

        private static void borrar(string omi, string clase, string clave, int timestamp, int dia)
        {
            StringBuilder query = new StringBuilder();
            Utilities.Acceso db = new Utilities.Acceso();

            query.Append(" delete DetalleLugar where olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(omi));
            query.Append(" and clase =  ");
            query.Append(Utilities.Cadenas.comillas(clase));
            query.Append(" and clave =  ");
            query.Append(Utilities.Cadenas.comillas(clave));
            query.Append(" and timestamp =  ");
            query.Append(timestamp);
            query.Append(" and dia =  ");
            query.Append(dia);

            db.EjecutarQuery(query.ToString());
        }

        public static void clean(string omi)
        {
            StringBuilder query = new StringBuilder();
            Utilities.Acceso db = new Utilities.Acceso();

            query.Append(" select * from DetalleLugar where olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(omi));
            query.Append(" order by clase, clave, dia, timestamp asc ");

            db.EjecutarQuery(query.ToString());

            DataTable table = db.getTable();

            bool first = false;
            DetalleLugar anterior = new DetalleLugar();
            DetalleLugar actual = new DetalleLugar();
            foreach (DataRow r in table.Rows)
            {
                actual.lugar = (int)r["lugar"];
                actual.timestamp = (int)r["timestamp"];
                actual.medalla = (Resultados.TipoMedalla)Enum.Parse(typeof(Resultados.TipoMedalla), r["medalla"].ToString());
                actual.dia = (int)r["dia"];
                actual.clave = r["clave"].ToString();
                actual.tipoOlimpiada = (TipoOlimpiada)Enum.Parse(typeof(TipoOlimpiada), r["clase"].ToString().ToUpper());

                if (actual.tipoOlimpiada != anterior.tipoOlimpiada ||
                    actual.clave != anterior.clave ||
                    actual.dia != anterior.dia)
                {
                    first = true;
                }
                else
                {
                    if (actual.medalla == anterior.medalla &&
                        actual.lugar == anterior.lugar)
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

                anterior.lugar = actual.lugar;
                anterior.timestamp = actual.timestamp;
                anterior.medalla = actual.medalla;
                anterior.dia = actual.dia;
                anterior.clave = actual.clave;
                anterior.tipoOlimpiada = actual.tipoOlimpiada;
            }
        }
    }
}