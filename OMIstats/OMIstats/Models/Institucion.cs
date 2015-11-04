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

        /// <summary>
        /// Obtiene la insticucion con la clave mandada como parametro
        /// </summary>
        /// <param name="clave">La clave de la institucion</param>
        /// <returns>El objeto institucion</returns>
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

        /// <summary>
        /// Regresa la institucion con el nombre corto mandado como parametro
        /// </summary>
        /// <param name="nombre">El nombre corto de la institucion</param>
        /// <returns>El objeto institucion</returns>
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

        /// <summary>
        /// Guarda los datos en este objeto a la base de datos
        /// </summary>
        public void guardarDatos()
        {
            if (nombreCorto.Length == 0)
            {
                nombreCorto = nombre;
                if (nombreCorto.Length > 20)
                    nombreCorto = nombreCorto.Substring(0, 20);
            }

            if (nombreURL.Length == 0)
            {
                nombreURL = nombreCorto;
                nombreURL = Utilities.Cadenas.quitaEspeciales(nombreURL);
                nombreURL = Utilities.Cadenas.quitaEspacios(nombreURL);
                if (nombreURL.Length > 10)
                    nombreURL = nombreURL.Substring(0, 10);
            }

            string hash = Utilities.Cadenas.quitaEspeciales(nombre);
            hash = Utilities.Cadenas.quitaEspacios(hash);

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" update institucion set nombre = ");
            query.Append(Utilities.Cadenas.comillas(nombre));
            query.Append(", nombrecorto = ");
            query.Append(Utilities.Cadenas.comillas(nombreCorto));
            query.Append(", nombreurl = ");
            query.Append(Utilities.Cadenas.comillas(nombreURL));
            query.Append(", url = ");
            query.Append(Utilities.Cadenas.comillas(URL));
            query.Append(", nombrehash = HASHBYTES(\'SHA1\', ");
            query.Append(Utilities.Cadenas.comillas(hash));
            query.Append("), primaria = ");
            query.Append(primaria ? "1" : "0");
            query.Append(", secundaria = ");
            query.Append(secundaria ? "1" : "0");
            query.Append(", preparatoria = ");
            query.Append(preparatoria ? "1" : "0");
            query.Append(", universidad = ");
            query.Append(universidad ? "1" : "0");
            query.Append(", publica = ");
            query.Append(publica ? "1" : "0");
            query.Append(" where clave = ");
            query.Append(clave);

            db.EjecutarQuery(query.ToString());
        }

        /// <summary>
        /// Guarda una nueva institucion en la base de datos
        /// </summary>
        public void nuevaInstitucion()
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" declare @inserted table(clave int); ");
            query.Append(" insert into institucion output inserted.clave into @inserted values (");
            query.Append("'', '', '', '', '', 0, 0, 0, 0, 0);");
            query.Append(" select clave from @inserted");

            DataTable table = db.getTable();
            if (table.Rows.Count != 1)
                return;
            clave = (int)table.Rows[0][0];

            guardarDatos();
        }
    }
}