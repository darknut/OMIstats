using OMIstats.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace OMIstats.Models
{
    public enum RetoStatus
    {
        PENDING,
        ACCEPTED,
        REJECTED,
        OUTDATED,
    }

    public class RetoPersona
    {
        public int clave;
        public string olimpiada;
        public int reto;
        public int persona;
        public RetoStatus status;
        public long timestamp;
        public string foto;

        private void llenarDatos(DataRow r, bool onlyTop = false)
        {
            reto = DataRowParser.ToInt(r["reto"]);
            persona = DataRowParser.ToInt(r["persona"]);
            timestamp = DataRowParser.ToLong(r["timestamp"]);
            if (!onlyTop)
            {
                clave = DataRowParser.ToInt(r["clave"]);
                olimpiada = DataRowParser.ToString(r["olimpiada"]);
                status = DataRowParser.ToRetoStatus(r["status"]);
                foto = DataRowParser.ToString(r["foto"]);
            }
        }

        public static RetoPersona obtenerMasReciente(string omi, int persona, int reto)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from retopersona where olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" and persona = ");
            query.Append(persona);
            query.Append(" and reto = ");
            query.Append(reto);
            query.Append(" order by timestamp desc ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            if (table.Rows.Count == 0)
                return null;

            RetoPersona rp = new RetoPersona();
            rp.llenarDatos(table.Rows[0]);

            return rp;
        }

        /// <summary>
        /// Sube una foto para responder al reto especificado y lo agrega a la base de datos
        /// </summary>
        /// <returns>El nombre creado en el server para la imagen</returns>
        public static string subirFoto(HttpPostedFileBase file, string omi, int persona, int reto, long inicioOMI)
        {
            string name = null;
            try
            {
                name = Utilities.Archivos.guardaArchivo(file, folder: Archivos.Folder.RETO);
            }
            catch (Exception e)
            {
                Log.add(Log.TipoLog.RETO, e.ToString());
                return null;
            }

            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" insert into retopersona values (");
            query.Append(Cadenas.comillas(omi));
            query.Append(" ,");
            query.Append(reto);
            query.Append(" ,");
            query.Append(persona);
            query.Append(" ,0,");
            query.Append(Convert.ToInt64(new TimeSpan(DateTime.Now.Ticks - inicioOMI).TotalSeconds));
            query.Append(" ,");
            query.Append(Cadenas.comillas(name));
            query.Append(")");

            if (db.EjecutarQuery(query.ToString()).error)
            {
                Log.add(Log.TipoLog.RETO, "Query error");
                return null;
            }

            return name;
        }

        /// <summary>
        /// Obtiene cuántos retos faltan por evaluar
        /// </summary>
        public static int obtenerRetosPorEvaluar(string omi)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select count(*) from retopersona where olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" and status = ");
            query.Append((int)RetoStatus.PENDING);

            db.EjecutarQuery(query.ToString());
            return DataRowParser.ToInt(db.getTable().Rows[0][0]);
        }

        /// <summary>
        /// Regresa el primer RetoPersona que no se haya evaluado
        /// </summary>
        public static RetoPersona obtenerPrimerRetoPorEvaluar(string omi)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from retopersona where olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" and status = ");
            query.Append((int)RetoStatus.PENDING);
            query.Append(" order by timestamp desc ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();
            if (table.Rows.Count == 0)
                return null;

            RetoPersona rp = new RetoPersona();
            rp.llenarDatos(table.Rows[0]);
            return rp;
        }

        /// <summary>
        /// Obtiene el retopersona mandado como parametro
        /// </summary>
        public static RetoPersona obtenerRetoConClave(int reto)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from RetoPersona where clave = ");
            query.Append(reto);

            db.EjecutarQuery(query.ToString());

            DataTable table = db.getTable();

            if (table.Rows.Count == 0)
                return null;

            RetoPersona r = new RetoPersona();
            r.llenarDatos(table.Rows[0]);

            return r;
        }

        /// <summary>
        /// Actualiza el retopersona con el status mandado como parametro y
        /// pone el status OUTDATED a todas los envios para este reto
        /// que se enviaron antes
        /// </summary>
        public void cambiarStatusEInvalidarAnteriores(RetoStatus retoStatus)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" update RetoPersona set status = ");
            query.Append((int)retoStatus);
            query.Append(" where clave = ");
            query.Append(clave);

            db.EjecutarQuery(query.ToString());

            query.Clear();
            query.Append(" update RetoPersona set status = ");
            query.Append((int)RetoStatus.OUTDATED);
            query.Append(" where olimpiada = ");
            query.Append(Cadenas.comillas(olimpiada));
            query.Append(" and reto = ");
            query.Append(reto);
            query.Append(" and persona = ");
            query.Append(persona);
            query.Append(" and timestamp < ");
            query.Append(timestamp);

            db.EjecutarQuery(query.ToString());
        }

        /// <summary>
        /// Obtiene la lista de ganadores hasta el momento (top 5)
        /// </summary>
        public static List<RetoPersona> obtenerResultados(string omi)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();
            List<RetoPersona> lista = new List<RetoPersona>();

            query.Append(" select top 5 persona, COUNT(reto) as reto, SUM(timestamp) as timestamp from RetoPersona ");
            query.Append(" where status = ");
            query.Append((int)RetoStatus.ACCEPTED);
            query.Append(" and olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" group by persona order by reto desc, timestamp asc ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                RetoPersona rp = new RetoPersona();
                rp.llenarDatos(r, onlyTop: true);
                lista.Add(rp);
            }
            return lista;
        }
    }
}