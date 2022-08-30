using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using OMIstats.Utilities;

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
        [MaxLength(100, ErrorMessage = "El tamaño máximo es 100 caracteres")]
        public string pagina { get; set; }

        public bool primaria { get; set; }

        public bool secundaria { get; set; }

        public bool preparatoria { get; set; }

        public bool universidad { get; set; }

        public bool publica { get; set; }

        public string logo { get; set; }

        // Usado en la vista de escuelas para saber
        // de qué estados es parte la escuela
        public List<string> estados;

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

            estados = null;
        }

        private void llenarDatos(DataRow datos)
        {
            clave = DataRowParser.ToInt(datos["clave"]);
            nombre = DataRowParser.ToString(datos["nombre"]);
            nombreCorto = DataRowParser.ToString(datos["nombrecorto"]);
            nombreURL = DataRowParser.ToString(datos["nombreurl"]);
            pagina = DataRowParser.ToString(datos["url"]);
            primaria = DataRowParser.ToBool(datos["primaria"]);
            secundaria = DataRowParser.ToBool(datos["secundaria"]);
            preparatoria = DataRowParser.ToBool(datos["preparatoria"]);
            universidad = DataRowParser.ToBool(datos["universidad"]);
            publica = DataRowParser.ToBool(datos["publica"]);

            if (Archivos.existeArchivo(Archivos.Folder.ESCUELAS,
                                                clave + ".png"))
                logo = clave + ".png";
            else
                logo = Archivos.OMI_LOGO;
        }

        /// <summary>
        /// Obtiene la insticucion con la clave mandada como parametro
        /// </summary>
        /// <param name="clave">La clave de la institucion</param>
        /// <returns>El objeto institucion</returns>
        public static Institucion obtenerInstitucionConClave(int clave)
        {
            Acceso db = new Acceso();
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
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from institucion where nombrecorto = ");
            query.Append(Cadenas.comillas(nombre));

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
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from institucion where nombreurl = ");
            query.Append(Cadenas.comillas(url));

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

            string hash = Cadenas.quitaEspeciales(nombre);
            hash = Cadenas.quitaEspacios(hash);

            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from institucion where nombrehash = HASHBYTES(\'SHA1\', ");
            query.Append(Cadenas.comillas(hash));
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
        /// Obtiene la lista de las mejores escuelas con medallas
        /// </summary>
        /// <param name="tipoOlimpiada">El tipo de olimpiada</param>
        /// <param name="estado">El estado que queremos filtrar</param>
        /// <returns>La lista de instituciones</returns>
        public static List<KeyValuePair<Institucion, Medallero>> obtenerMejoresEscuelas(string estado, TipoOlimpiada tipoOlimpiada)
        {
            List<KeyValuePair<Institucion, Medallero>> escuelas = new List<KeyValuePair<Institucion, Medallero>>();

            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from Medallero where tipo = ");
            query.Append((int) Medallero.TipoMedallero.INSTITUCION);
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            if (estado == null)
                query.Append(" and oro > 1 ");
            else
                query.Append(" and (oro + plata + bronce) > 1 ");
            query.Append(" order by oro desc, plata desc, bronce desc ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                Medallero m = new Medallero();
                m.llenarDatos(r);
                Institucion i = Institucion.obtenerInstitucionConClave(int.Parse(m.clave));
                i.consultarEstadosDeInstitucion(tipoOlimpiada);

                if (estado == null || i.estados.Contains(estado))
                    escuelas.Add(new KeyValuePair<Institucion,Medallero>(i, m));
            }

            return escuelas;
        }

        /// <summary>
        /// Calcula los estados de los cuales hay participantes para la escuela
        /// </summary>
        /// <param name="tipoOlimpiada">El tipo de olimpiada</param>
        private void consultarEstadosDeInstitucion(TipoOlimpiada tipoOlimpiada)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select distinct(estado) from MiembroDelegacion where institucion = ");
            query.Append(this.clave);
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            estados = new List<string>();
            foreach (DataRow r in table.Rows)
            {
                string estado = DataRowParser.ToString(r[0]);
                estados.Add(estado);
            }
        }

        /// <summary>
        /// Regresa todas las escuelas que han representado a un estado
        /// </summary>
        /// <param name="tipoOlimpiada">El tipo de olimpiada</param>
        /// <param name="estado">El estado que se quiere consultar</param>
        public static List<Ajax.BuscarEscuelas> obtenerEscuelasDeEstado(TipoOlimpiada tipoOlimpiada, string estado)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();
            List<Ajax.BuscarEscuelas> escuelas = new List<Ajax.BuscarEscuelas>();

            query.Append(" select * from Institucion where clave in ");
            query.Append(" (select distinct(institucion) from MiembroDelegacion as md ");
            query.Append(" where md.estado =  ");
            query.Append(Cadenas.comillas(estado));
            if (tipoOlimpiada == TipoOlimpiada.OMIP || tipoOlimpiada == TipoOlimpiada.OMIPO)
            {
                query.Append(" and (md.clase =  ");
                query.Append(Cadenas.comillas(TipoOlimpiada.OMIP.ToString().ToLower()));
                query.Append(" or md.clase =  ");
                query.Append(Cadenas.comillas(TipoOlimpiada.OMIPO.ToString().ToLower()));
                query.Append(" ) ");
            }
            else if (tipoOlimpiada == TipoOlimpiada.OMIS || tipoOlimpiada == TipoOlimpiada.OMISO)
            {
                query.Append(" and (md.clase =  ");
                query.Append(Cadenas.comillas(TipoOlimpiada.OMIS.ToString().ToLower()));
                query.Append(" or md.clase =  ");
                query.Append(Cadenas.comillas(TipoOlimpiada.OMISO.ToString().ToLower()));
                query.Append(" ) ");
            }
            else
            {
                query.Append(" and md.clase =  ");
                query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            }
            query.Append(" ) order by nombre ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                Ajax.BuscarEscuelas be = new Ajax.BuscarEscuelas();
                be.llenarDatos(r);
                escuelas.Add(be);
            }
            return escuelas;
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
                nombreURL = Cadenas.quitaEspeciales(nombreURL);
                nombreURL = Cadenas.quitaEspacios(nombreURL);
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

            temp = obtenerInstitucionConNombreCorto(nombreCorto);
            if (!(temp == null || temp.clave == clave))
            {
                int counter = 0;
                int caracters = 1;
                int nextIncrement = 10;
                while (true)
                {
                    if (nombreCorto.Length > (20 - caracters))
                        nombreCorto = nombreCorto.Substring(0, 20 - caracters);

                    if (obtenerInstitucionConNombreCorto(nombreCorto + counter.ToString()) == null)
                        break;
                    counter++;

                    if (counter == nextIncrement)
                    {
                        caracters++;
                        nextIncrement *= 10;
                    }
                }
                nombreCorto += counter.ToString();
            }

            string hash = Cadenas.quitaEspeciales(nombre);
            hash = Cadenas.quitaEspacios(hash);

            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" update institucion set nombre = ");
            query.Append(Cadenas.comillas(nombre));
            query.Append(", nombrecorto = ");
            query.Append(Cadenas.comillas(nombreCorto));
            query.Append(", nombreurl = ");
            query.Append(Cadenas.comillas(nombreURL));
            query.Append(", url = ");
            query.Append(Cadenas.comillas(pagina));
            query.Append(", nombrehash = HASHBYTES(\'SHA1\', ");
            query.Append(Cadenas.comillas(hash));
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
            Acceso db = new Acceso();
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
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select numero from Olimpiada where escuela = ");
            query.Append(clave);
            query.Append(" and clase = ");
            // Mientras las OMIS y OMIPS no sean aparte, las sedes se cargan de OMIS
            query.Append(Cadenas.comillas(TipoOlimpiada.OMI.ToString().ToLower()));

            db.EjecutarQuery(query.ToString());

            DataTable table = db.getTable();
            if (table.Rows.Count == 0)
                return list;

            foreach (DataRow r in table.Rows)
            {
                string numero = DataRowParser.ToString(r[0]);
                Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(numero, TipoOlimpiada.OMI);
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

        /// <summary>
        /// Borra de la base de datos todas las instituciones sin competidores o olimpiadas asignadas
        /// </summary>
        public static void borrarZombies()
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" delete institucion where clave in ( ");
            query.Append(" select clave from Institucion where clave ");
            query.Append(" not in (select escuela from Olimpiada) and clave not in ");
            query.Append(" (select institucion from MiembroDelegacion where institucion is not null))");

            db.EjecutarQuery(query.ToString());
        }
    }
}