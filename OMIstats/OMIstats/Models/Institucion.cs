using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace OMIstats.Models
{
    public class Institucion
    {
        public int clave { get; set; }

        public string nombre { get; set; }

        public string nombreCorto { get; set; }

        public string nombreURL { get; set; }

        public string URL { get; set; }

        public bool primaria { get; set; }

        public bool secundaria { get; set; }

        public bool preparatoria { get; set; }

        public bool universidad { get; set; }

        public bool publica { get; set; }

        public Institucion()
        {
            clave = 0;
            nombre = "";
            nombreCorto = "";
            nombreURL = "";
            URL = "";
            primaria = false;
            secundaria = false;
            preparatoria = false;
            universidad = false;
        }

        private void llenarDatos(DataRow datos)
        {
            clave = (int)datos["clave"];
            nombre = datos["nombre"].ToString().Trim();
            nombreCorto = datos["nombrecorto"].ToString().Trim();
            nombreURL = datos["nombreurl"].ToString().Trim();
            URL = datos["url"].ToString().Trim();
            primaria = (bool)datos["primaria"];
            secundaria = (bool)datos["secundaria"];
            preparatoria = (bool)datos["preparatoria"];
            universidad = (bool)datos["universidad"];
            publica = (bool)datos["publica"];
        }

        public static Institucion obtenerInstitucionConClave(int clave)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from institucion where clave = ");
            query.Append(clave);

            if (db.EjecutarQuery(query.ToString()).error)
                return null;

            DataTable table = db.getTable();
            if (table.Rows.Count != 1)
                return null;

            Institucion i = new Institucion();
            i.llenarDatos(table.Rows[0]);

            return i;
        }

        public static Institucion obtenerInstitucionConNombreCorto(string nombre)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from institucion where nombrecorto = ");
            query.Append(Utilities.Cadenas.comillas(nombre));

            if (db.EjecutarQuery(query.ToString()).error)
                return null;

            DataTable table = db.getTable();
            if (table.Rows.Count != 1)
                return null;

            Institucion i = new Institucion();
            i.llenarDatos(table.Rows[0]);

            return i;
        }

        public void nuevaInstitucion()
        {
            // -TODO- guardar institucion
        }
    }
}