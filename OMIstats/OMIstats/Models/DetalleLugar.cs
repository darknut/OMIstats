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
        private static OverlayLugares llenarDatos(DataRow row)
        {
            OverlayLugares res = new OverlayLugares();
            res.lugar = (int)row["lugar"];
            res.timestamp = (int)row["timestamp"];
            res.medalla = (int)row["medalla"];

            return res;
        }

        public static List<OverlayLugares> cargarResultados(string omi, TipoOlimpiada tipoOlimpiada, string clave)
        {
            List<OverlayLugares> lista = new List<OverlayLugares>();

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from detallelugar ");
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
                lista.Add(llenarDatos(r));
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
    }
}