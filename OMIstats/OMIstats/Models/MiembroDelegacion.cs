using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace OMIstats.Models
{
    public class MiembroDelegacion
    {
        public enum TipoAsistente
        {
            COMPETIDOR,
            ASESOR,
            LIDER,
            DELEGADO,
            COMI,
            COLO,
            INVITADO
        }

        public enum TipoMedalla
        {
            ORO,
            PLATA,
            BRONCE,
            NADA
        }

        // Este objeto debe de ser contenido por un objeto olimpiada,
        // por eso no cargamos un objeto olimpiada aqui

        public Persona miembro;
        public Institucion escuela;
        public string usuario;
        public string genero;
        public string nombreEscuela;
        public bool escuelaPublica;
        public Institucion.NivelInstitucion nivelEscuela;
        public int añoEscuela;
        public string clave;
        public string estado;
        public TipoAsistente tipo;
        public TipoMedalla medalla;

        /// <summary>
        /// Regresa el año de la primera OMI para la persona mandada como parametro
        /// Si no se encuentra nada en la base de datos, se devuelve 0
        /// </summary>
        public static int primeraOMIPara(Persona p)
        {
            if (p == null)
                return 0;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select o.año from MiembroDelegacion as md ");
            query.Append(" inner join olimpiada as o on md.olimpiada = o.numero ");
            query.Append(" where md.persona = ");
            query.Append(p.clave);
            query.Append(" order by md.olimpiada asc ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            if (table.Rows.Count == 0)
                return 0;

            return Int32.Parse(table.Rows[0][0].ToString().Trim());
        }

        /// <summary>
        /// Regresa el año de la ultima OMI como competirdor para la persona mandada como parámetro
        /// De no encontrarse ninguna, se devuelve 0
        /// </summary>
        public static int ultimaOMIComoCompetidorPara(Persona p)
        {
            if (p == null)
                return 0;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select o.año from MiembroDelegacion as md ");
            query.Append(" inner join olimpiada as o on md.olimpiada = o.numero ");
            query.Append(" where md.persona = ");
            query.Append(p.clave);
            query.Append(" and md.tipo = \'competidor\' ");
            query.Append(" order by md.olimpiada desc ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            if (table.Rows.Count == 0)
                return 0;

            return Int32.Parse(table.Rows[0][0].ToString().Trim());
        }

        /// <summary>
        /// Regresa todos los asistentes de la olimpiada mandada como parámetro
        /// </summary>
        /// <param name="omi">La omi de la que se necesitan los asistentes</param>
        /// <returns>Una lista con los asistentes de la OMI</returns>
        public static List<MiembroDelegacion> cargarAsistentesOMI(string omi)
        {
            List<MiembroDelegacion> list = new List<MiembroDelegacion>();
            if (omi == null)
                return null;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from MiembroDelegacion ");
            query.Append(" where olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(omi));
            query.Append(" order by clave ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            // -TODO- Cargar los miembros de la delegacion

            return list;
        }
    }
}