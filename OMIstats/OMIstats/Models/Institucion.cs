using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace OMIstats.Models
{
    public class Institucion
    {
        public enum NivelInstitucion
        {
            NULL,
            PRIMARIA,
            SECUNDARIA,
            PREPARATORIA,
            UNIVERSIDAD
        }

        public int clave { get; set; }

        [Required(ErrorMessage = "Escribe el nombre de la escuela")]
        [RegularExpression(@"^[a-zA-Z0-9 ñÑáéíóúÁÉÍÓÚäëïöü#\.'-]*$", ErrorMessage = "Escribiste caracteres inválidos")]
        [MaxLength(100, ErrorMessage = "El tamaño máximo es 100 caracteres")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "Escribe el nombre corto de la escuela")]
        [RegularExpression(@"^[a-zA-Z0-9 ñÑáéíóúÁÉÍÓÚäëïöü#\.'-]*$", ErrorMessage = "Escribiste caracteres inválidos")]
        [MaxLength(20, ErrorMessage = "El tamaño máximo es 20 caracteres")]
        public string nombreCorto { get; set; }

        [Required(ErrorMessage = "Escribe el nombre común de la escuela")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Solo se permiten letras y números. No espacios.")]
        [MaxLength(10, ErrorMessage = "El tamaño máximo es 10 caracteres")]
        public string nombreURL { get; set; }

        [RegularExpression(@"^(https?:\/\/).*$", ErrorMessage = "Escribe una dirección de internet")]
        [MaxLength(50, ErrorMessage = "El tamaño máximo es 50 caracteres")]
        public string pagina { get; set; }

        public bool primaria { get; set; }

        public bool secundaria { get; set; }

        public bool preparatoria { get; set; }

        public bool universidad { get; set; }

        public bool publica { get; set; }

        public string logo { get; set; }

        public Institucion()
        {
            clave = 0;
            nombre = "";
            nombreCorto = "";
            nombreURL = "";
            pagina = "";
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
            pagina = datos["url"].ToString().Trim();
            primaria = (bool)datos["primaria"];
            secundaria = (bool)datos["secundaria"];
            preparatoria = (bool)datos["preparatoria"];
            universidad = (bool)datos["universidad"];
            publica = (bool)datos["publica"];

            if (Utilities.Archivos.existeArchivo(Utilities.Archivos.FolderImagenes.ESCUELAS,
                                                clave + ".png"))
                logo = clave + ".png";
            else
                logo = "omi.png";
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
        /// Regresa la institucion con el nombre url mandado como parametro
        /// </summary>
        /// <param name="nombre">El nombre url de la institucion</param>
        /// <returns>El objeto institucion</returns>
        public static Institucion obtenerInstitucionConNombreURL(string url)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from institucion where nombreurl = ");
            query.Append(Utilities.Cadenas.comillas(url));

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
        /// Regresa la institucion con el nombre mandado como parametro
        /// Este método busca en todos los campos de nombre: nombre, nombrecorto y nombreurl
        /// </summary>
        /// <param name="nombre">El nombre de la institucion</param>
        /// <returns>El objeto institucion</returns>
        public static Institucion buscarInstitucionConNombre(string nombre)
        {
            Institucion i = obtenerInstitucionConNombreCorto(nombre);

            if (i != null)
                return i;

            i = obtenerInstitucionConNombreURL(nombre);

            if (i != null)
                return i;

            string hash = Utilities.Cadenas.quitaEspeciales(nombre);
            hash = Utilities.Cadenas.quitaEspacios(hash);

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from institucion where nombrehash = HASHBYTES(\'SHA1\', ");
            query.Append(Utilities.Cadenas.comillas(hash));
            query.Append(")");

            if (db.EjecutarQuery(query.ToString()).error)
                return null;

            DataTable table = db.getTable();
            if (table.Rows.Count != 1)
                return null;

            i = new Institucion();
            i.llenarDatos(table.Rows[0]);

            return i;
        }

        /// <summary>
        /// Guarda los datos en este objeto a la base de datos
        /// </summary>
        private bool guardarDatos()
        {
            if (nombre.Length == 0)
                return false;

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

            Institucion temp = obtenerInstitucionConNombreURL(nombreURL);
            if (!(temp == null || temp.clave == clave))
            {
                int counter = 0;
                int caracters = 1;
                int nextIncrement = 10;
                while (true)
                {
                    if (nombreURL.Length > (10 - caracters))
                        nombreURL = nombreURL.Substring(0, 10 - caracters);

                    if (obtenerInstitucionConNombreURL(nombreURL + counter.ToString()) == null)
                        break;
                    counter++;

                    if (counter == nextIncrement)
                    {
                        caracters++;
                        nextIncrement *= 10;
                    }
                }
                nombreURL += counter.ToString();
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
            query.Append(Utilities.Cadenas.comillas(pagina));
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

            return !db.EjecutarQuery(query.ToString()).error;
        }

        /// <summary>
        /// Guarda una nueva institucion en la base de datos
        /// </summary>
        public bool nuevaInstitucion()
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" declare @inserted table(clave int); ");
            query.Append(" insert into institucion output inserted.clave into @inserted values (");
            query.Append("'', '', '', '', '', 0, 0, 0, 0, 0);");
            query.Append(" select clave from @inserted");

            db.EjecutarQuery(query.ToString());

            DataTable table = db.getTable();
            if (table.Rows.Count != 1)
                return false;
            clave = (int)table.Rows[0][0];

            return guardarDatos();
        }

        /// <summary>
        /// Regresa una lista de las olimpiadas en las que la escuela instanciada
        /// fue la escuela sede
        /// </summary>
        /// <returns>La lista de olimpiadas</returns>
        public List<Olimpiada> obtenerOlimpiadasSede()
        {
            List<Olimpiada> list = new List<Olimpiada>();
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select numero from Olimpiada where escuela = ");
            query.Append(clave);

            db.EjecutarQuery(query.ToString());

            DataTable table = db.getTable();
            if (table.Rows.Count == 0)
                return list;

            foreach (DataRow r in table.Rows)
            {
                string numero = r[0].ToString();
                Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(numero, Olimpiada.TipoOlimpiada.OMI);
                list.Add(o);
            }

            return list;
        }

        /// <summary>
        /// Guarda los datos en la base de datos
        /// </summary>
        /// <param name="generarPeticiones">Si se deben de generar peticiones 
        /// o guardar directamente los datos</param>
        /// <returns>Si se guardaron los datos satisfactoriamente</returns>
        public bool guardar(bool generarPeticiones)
        {
            if (generarPeticiones)
            {
                // -TODO- Generar peticiones
                return true;
            }
            else
            {
                return guardarDatos();
            }
        }
    }
}