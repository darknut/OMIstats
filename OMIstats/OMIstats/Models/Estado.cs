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

        public Estado()
        {
            clave = "";
            nombre = "";
            sitio = "";
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
            List<Estado> lista = new List<Estado>();
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from estado ");

            if (db.EjecutarQuery(query.ToString()).error)
                return lista;

            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                Estado e = new Estado();
                e.llenarDatos(r);

                lista.Add(e);
            }

            return lista;
        }

        private void llenarDatos(DataRow datos)
        {
            clave = datos["clave"].ToString().Trim();
            nombre = datos["nombre"].ToString().Trim();
            sitio = datos["sitio"].ToString().Trim();

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
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from estado where clave = ");
            query.Append(Utilities.Cadenas.comillas(clave));

            if (db.EjecutarQuery(query.ToString()).error)
                return null;

            DataTable table = db.getTable();
            if (table.Rows.Count != 1)
                return null;

            Estado e = new Estado();
            e.llenarDatos(table.Rows[0]);

            return e;
        }

        /// <summary>
        /// Guarda los datos del estado en la base de datos
        /// </summary>
        /// <remarks>Los únicos datos que se guardan son el sitio web y el delegado</remarks>
        /// <returns>Si se guardaron satisfactoriamente los datos</returns>
        public bool guardar()
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" update estado set sitio = ");
            query.Append(Utilities.Cadenas.comillas(sitio));
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
    }
}