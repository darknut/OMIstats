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

        public string usuario;
        public string nombreAsistente;
        public string fechaNacimiento;
        public string correo;
        public string genero;
        public string nombreEscuela;
        public bool escuelaPublica;
        public Institucion.NivelInstitucion nivelEscuela;
        public int añoEscuela;
        public string clave;
        public string estado;
        public TipoAsistente tipo;
        public TipoMedalla medalla;

        private void llenarDatos(DataRow row)
        {
            usuario = row["usuario"].ToString().Trim();
            nombreAsistente = row["nombre"].ToString().Trim();
            estado = row["estado"].ToString().Trim();
            tipo = (TipoAsistente) Enum.Parse(typeof (TipoAsistente), row["tipo"].ToString().ToUpper());
            clave = row["clave"].ToString().Trim();
            fechaNacimiento = row["nacimiento"].ToString().Trim();
            genero = row["genero"].ToString().Trim();
            correo = row["correo"].ToString().Trim();
            nombreEscuela = row["nombreCorto"].ToString().Trim();
            nivelEscuela = (Institucion.NivelInstitucion)row["nivel"];
            añoEscuela = (int)row["año"];
            escuelaPublica = (bool)row["publica"];
        }

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
            List<MiembroDelegacion> lista = new List<MiembroDelegacion>();
            if (omi == null)
                return null;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select p.usuario, p.nombre, md.estado, md.tipo, md.clave,");
            query.Append(" p.nacimiento, p.genero, p.correo, i.nombreCorto, md.nivel,");
            query.Append(" md.año, i.publica from miembrodelegacion as md");
            query.Append(" inner join Persona as p on p.clave = md.persona ");
            query.Append(" inner join Institucion as i on i.clave = md.institucion");
            query.Append(" where md.olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(omi));
            query.Append(" order by md.clave ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                MiembroDelegacion md = new MiembroDelegacion();
                md.llenarDatos(r);

                lista.Add(md);
            }

            return lista;
        }

        /// <summary>
        /// Regresa los datos en este objeto como un string separado por comas
        /// para que los admins puedan ver los datos en una tabla
        /// </summary>
        /// <returns>Los datos separados por coma</returns>
        public string obtenerLineaAdmin()
        {
            StringBuilder s = new StringBuilder();

            s.Append(usuario);
            s.Append(", ");
            s.Append(nombreAsistente);
            s.Append(", ");
            s.Append(estado);
            s.Append(", ");
            s.Append(tipo.ToString().ToLower());
            s.Append(", ");
            s.Append(clave);
            s.Append(", ");
            s.Append(fechaNacimiento);
            s.Append(", ");
            s.Append(genero);
            s.Append(", ");
            s.Append(correo);
            s.Append(", ");
            s.Append(nombreEscuela);
            s.Append(", ");
            s.Append(nivelEscuela.ToString().ToLower());
            s.Append(", ");
            s.Append(añoEscuela);
            s.Append(", ");
            s.Append(escuelaPublica ? "publica" : "privada");

            return s.ToString();
        }

        /// <summary>
        /// Guarda la linea mandada como parametro en la base de datos
        /// </summary>
        /// <param name="omi">La clave de la olimpiada</param>
        /// <param name="linea">Los datos tabulados por comas</param>
        /// <returns>Si hubo un error, devuelve true</returns>
        public static bool guardarLineaAdmin(string omi, string linea)
        {
            return false;
        }
    }
}