using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace OMIstats.Models
{
    public class DetalleLugar
    {
        public string omi;
        public TipoOlimpiada tipoOlimpiada;
        public string clave;
        public int timestamp;
        public Resultados.TipoMedalla medalla;
        public int lugar;

        public DetalleLugar()
        {
            omi = "";
            tipoOlimpiada = TipoOlimpiada.NULL;
            clave = "";
            timestamp = 0;
            medalla = Resultados.TipoMedalla.NULL;
            lugar = 0;
        }

        private void llenarDatos(DataRow row)
        {
            lugar = (int)row["lugar"];
            omi = row["olimpiada"].ToString().Trim();
            clave = row["clave"].ToString().Trim();
            timestamp = (int)row["timestamp"];
            medalla = (Resultados.TipoMedalla)Enum.Parse(typeof(Resultados.TipoMedalla), row["medalla"].ToString());
        }

        private static List<DetalleLugar> cargarResultadosDeUsuario(string omi, TipoOlimpiada tipoOlimpiada, string clave)
        {
            List<DetalleLugar> lista = new List<DetalleLugar>();

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
                DetalleLugar res = new DetalleLugar();
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

            query.Append("insert into DetalleLugar values(");
            query.Append(Utilities.Cadenas.comillas(omi));
            query.Append(",");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(",");
            query.Append(Utilities.Cadenas.comillas(clave));
            query.Append(",");
            query.Append(timestamp);
            query.Append(",");
            query.Append((int)medalla);
            query.Append(",");
            query.Append(lugar);
            query.Append(")");

            db.EjecutarQuery(query.ToString());
        }
    }
}