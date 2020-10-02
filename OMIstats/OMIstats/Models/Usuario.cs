using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using OMIstats.Utilities;

namespace OMIstats.Models
{   
    /// <summary>
    /// Clase para comunicarse con la base de datos de la OMI
    /// </summary>
    public class Usuario
    {
        public long Id { get; set; }

        public string Email { get; set; }

        public string Nombre { get; set; }

        public Usuario()
        {
            Id = 0;
            Email = "";
            Nombre = "";
        }

        private static string tableName(string table)
        {
#if DEBUG
            return "[" + table + "]";
#else
            return table;
#endif
        }

        /// <summary>
        /// Llena los datos de un usuario de la fila mandada como parametro
        /// </summary>
        /// <param name="datos">La fila con el origen de los datos</param>
        private void llenarDatos(DataRow datos)
        {
            Id = DataRowParser.ToLong(datos["Id"]);
            Email = DataRowParser.ToString(datos["Email"]);
            Nombre = DataRowParser.ToString(datos["NombreCompleto"]);
        }

        /// <summary>
        /// Loggea al usuario mandado como parámetro, como si la página de la OMI lo hubiera hecho
        /// </summary>
        /// <param name="userId">El usuario que se quiere loggear</param>
        /// <returns>El guid que se guardó en la base de datos</returns>
        public static string MockUserLoggedIn(int userId)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();
            string guid = Guid.NewGuid().ToString();

            query.Append(" update ");
            query.Append(tableName("Usuarios.Usuarios"));
            query.Append(" set GUID = ");
            query.Append(Cadenas.comillas(guid));
            query.Append(" where Id = ");
            query.Append(userId);

            db.EjecutarQuery(query.ToString(), Acceso.BaseDeDatos.OMI);
            return guid;
        }

        /// <summary>
        /// Borra el GUID del usuario en la base de datos
        /// </summary>
        public void borrarGUID()
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" update ");
            query.Append(tableName("Usuarios.Usuarios"));
            query.Append(" set GUID = ''");
            query.Append(" where Id = ");
            query.Append(Id);

            db.EjecutarQuery(query.ToString(), Acceso.BaseDeDatos.OMI);
        }

        /// <summary>
        /// Regresa el usuario en la base de datos con el GUID mandado como parámetro
        /// </summary>
        /// <param name="guid">El guid buscado</param>
        /// <returns>Un objeto Usuario con los datos en la base de datos</returns>
        public static Usuario obtenerUsuarioConGUID(string guid)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();
            Usuario usuario = new Usuario();

            query.Append(" select * from ");
            query.Append(tableName("Usuarios.Usuarios"));
            query.Append(" where GUID = ");
            query.Append(Cadenas.comillas(guid));

            db.EjecutarQuery(query.ToString(), Acceso.BaseDeDatos.OMI);

            DataTable table = db.getTable();

            if (table.Rows.Count == 0)
                return null;

            usuario.llenarDatos(table.Rows[0]);

            return usuario;
        }

        /// <summary>
        /// Busca en la base de datos de la OMI quiénes son delegados y les da permisos en este sitio
        /// </summary>
        public static void syncDelegados()
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();
            Usuario usuario = new Usuario();

            query.Append(" select Email, NombreCompleto from ");
            query.Append(tableName("usuarios.vwUsuarios"));
            query.Append(" where idGrupo='DEL'");

            db.EjecutarQuery(query.ToString(), Acceso.BaseDeDatos.OMI);
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                string correo = DataRowParser.ToString(r[0]);
                string nombre = DataRowParser.ToString(r[1]);

                Persona p = Persona.obtenerPersonaConCorreo(correo);
                if (p == null)
                {
                    p = Persona.obtenerPersonaConNombre(nombre);
                    if (p == null)
                    {
                        // No se encontró persona con nombre o correo, creamos una nueva
                        p = new Persona();
                        p.nombre = nombre;
                        p.correo = correo;
                        p.breakNombre();
                        p.nuevoUsuario(Archivos.FotoInicial.DOMI);
                    }
                }

                p.permisos = Persona.TipoPermisos.DELEGADO;
                p.guardarDatos();
            }
        }
    }
}