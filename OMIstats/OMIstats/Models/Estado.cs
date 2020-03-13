using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace OMIstats.Models
{
    public class Estado
    {
        private const string APPLICATION_ESTADOS = "Estados";

        public string clave { get; set; }
        public string nombre { get; set; }
        public Persona delegado { get; set; }

        [RegularExpression(@"^(https?:\/\/)?((([\w-]+)\.){1,})([\/\w\.-]+)(\?[\/\w\.-=%&]*)?$", ErrorMessage = "Escribe una URL válida")]
        [MaxLength(70, ErrorMessage = "El tamaño máximo es de 70 caracteres")]
        public string sitio { get; set; }

        // Las siguientes variables se necesitan porque
        // MVC no soporta modelos anidados
        public int claveDelegado { get; set; }
        public string usuarioDelegado { get; set; }

        [RegularExpression(@"^[a-zA-Z ñÑáéíóúÁÉÍÓÚäëïöü\.'-]*$", ErrorMessage = "Escribiste caracteres inválidos en el nombre")]
        [MaxLength(60, ErrorMessage = "El tamaño máximo es 60 caracteres")]
        public string nombreDelegado { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$", ErrorMessage = "Escribe un correo electrónico válido")]
        [MaxLength(50, ErrorMessage = "El tamaño máximo es de 50 caracteres")]
        public string mailDelegado { get; set; }

        [MaxLength(3, ErrorMessage = "El tamaño máximo es de 3 caracteres")]
        public string ISO { get; set; }

        private static Dictionary<string, Estado> cargarEstados()
        {
            Dictionary<string, Estado> lista = new Dictionary<string, Estado>();
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from estado order by nombre ");

            db.EjecutarQuery(query.ToString());

            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                Estado e = new Estado();
                e.llenarDatos(r);

                lista.Add(e.clave, e);
            }

            return lista;
        }

        /// <summary>
        /// Dado que los estados son pocos y a que se hacen muchas llamadas a la base para obtener estos objetos
        /// los cargamos una vez por aplicacion y los dejamos ahi
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, Estado> getEstados()
        {
            Dictionary<string, Estado> estados = (Dictionary<string, Estado>)HttpContext.Current.Application[APPLICATION_ESTADOS];

            if (estados == null)
            {
                estados = cargarEstados();
                HttpContext.Current.Application[APPLICATION_ESTADOS] = estados;
            }

            return estados;
        }

        public Estado()
        {
            clave = "";
            nombre = "";
            sitio = "";
            ISO = "";
            delegado = null;

            claveDelegado = Persona.UsuarioNulo;
            usuarioDelegado = "";
            nombreDelegado = "";
        }

        /// <summary>
        /// Regresa una lista con todos los estados
        /// </summary>
        /// <returns>La lista</returns>
        public static List<Estado> obtenerEstados()
        {
            return getEstados().Values.ToList();
        }

        private void llenarDatos(DataRow datos)
        {
            clave = datos["clave"].ToString().Trim();
            nombre = datos["nombre"].ToString().Trim();
            sitio = datos["sitio"].ToString().Trim();
            ISO = datos["iso"].ToString().Trim();

            claveDelegado = (int) (datos["delegado"] == DBNull.Value ? 0 : datos["delegado"]);
            delegado = Persona.obtenerPersonaConClave(claveDelegado);

            if (delegado != null)
            {
                nombreDelegado = delegado.nombre;
                usuarioDelegado = delegado.usuario;
                mailDelegado = delegado.correo;
            }
        }

        /// <summary>
        /// Devuelve el objeto estado asociado con la clave enviada como parametro
        /// </summary>
        /// <param name="clave">La clave del estado</param>
        /// <returns>El estado</returns>
        public static Estado obtenerEstadoConClave(string clave)
        {
            Dictionary<string, Estado> estados = getEstados();

            Estado e;

            estados.TryGetValue(clave.Trim(), out e);

            return e;
        }

        /// <summary>
        /// Guarda los datos del estado en la base de datos
        /// </summary>
        /// <remarks>Los únicos datos que se guardan son el sitio web y el delegado</remarks>
        /// <returns>Si se guardaron satisfactoriamente los datos</returns>
        public bool guardar()
        {
            // Borramos la referencia en la aplicacion para que el siguiente query recargue los datos
            HttpContext.Current.Application[APPLICATION_ESTADOS] = null;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" update estado set sitio = ");
            query.Append(Utilities.Cadenas.comillas(sitio));
            query.Append(" , iso = ");
            query.Append(Utilities.Cadenas.comillas(ISO));
            if (delegado != null)
            {
                query.Append(", delegado = ");
                query.Append(delegado.clave);
            }
            query.Append(" where clave = ");
            query.Append(Utilities.Cadenas.comillas(clave));

            return !db.EjecutarQuery(query.ToString()).error;
        }

        /// <summary>
        /// Regresa una lista de las olimpiadas en las que el estado instanciado
        /// fue la escuela sede
        /// </summary>
        /// <returns>La lista de olimpiadas</returns>
        public List<Olimpiada> obtenerOlimpiadasSede()
        {
            List<Olimpiada> list = new List<Olimpiada>();
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select numero from Olimpiada where estado = ");
            query.Append(Utilities.Cadenas.comillas(clave));
            query.Append(" and clase = ");
            // Mientras las OMIS y OMIPS no sean aparte, las sedes se cargan de OMIS
            query.Append(Utilities.Cadenas.comillas(TipoOlimpiada.OMI.ToString().ToLower()));

            db.EjecutarQuery(query.ToString());

            DataTable table = db.getTable();
            if (table.Rows.Count == 0)
                return list;

            foreach (DataRow r in table.Rows)
            {
                string numero = r[0].ToString();
                Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(numero, TipoOlimpiada.OMI);
                list.Add(o);
            }

            return list;
        }

        /// <summary>
        /// Revisa en la base de datos si el estado estuvo en la olimpiada mandada como parametro
        /// </summary>
        /// <param name="tipoOlimpiada">El tipo de olimpiada</param>
        /// <param name="olimpiada">La clave de la olimpiada</param>
        /// <returns></returns>
        public bool estadoVinoAOlimpiada(TipoOlimpiada tipoOlimpiada, string olimpiada)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select COUNT(*) from Resultados where clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(olimpiada));
            query.Append(" and estado = ");
            query.Append(Utilities.Cadenas.comillas(this.clave));

            db.EjecutarQuery(query.ToString());

            DataTable table = db.getTable();

            return ((int)table.Rows[0][0]) > 0;
        }

        /// <summary>
        /// Regresa el estado del delegado mandado como parametro
        /// </summary>
        /// <param name="tipoOlimpiada">El tipo de olimpiada</param>
        /// <param name="olimpiada">La clave de la olimpiada</param>
        /// <returns></returns>
        public static Estado obtenerEstadoDeDelegado(int clave)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select clave from Estado where delegado = ");
            query.Append(clave);

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            if (table.Rows.Count == 0)
                return null;

            return obtenerEstadoConClave(table.Rows[0][0].ToString().Trim());
        }
    }
}