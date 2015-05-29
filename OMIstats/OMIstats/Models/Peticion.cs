using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace OMIstats.Models
{
    public class Peticion
    {
        public int clave { get; set; }
        public string tipo { get; set; }
        public string subtipo { get; set; }
        public Persona usuario { get; set; }
        public string datos1 { get; set; }
        public string datos2 { get; set; }
        public string datos3 { get; set; }

        public Peticion()
        {
            tipo = "";
            subtipo = "";
            usuario = null;
            datos1 = "";
            datos2 = "";
            datos3 = "";
        }

        /// <summary>
        /// Guarda los datos de una nueva petición en la base de datos
        /// </summary>
        /// <returns>Regresa si la peticion se insertó correctamente</returns>
        public bool guardarPeticion()
        {
            if (String.IsNullOrEmpty(tipo) || String.IsNullOrEmpty(subtipo))
                return false;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" declare @inserted table(clave int); ");
            query.Append(" insert into peticion output inserted.clave into @inserted values (");
            query.Append(Utilities.Cadenas.comillas(tipo));
            query.Append(", ");
            query.Append(Utilities.Cadenas.comillas(subtipo));
            query.Append(", ");
            if (usuario != null)
                query.Append(usuario.clave);
            else
                query.Append("0");
            query.Append(", ");
            query.Append(Utilities.Cadenas.comillas(datos1));
            query.Append(", ");
            query.Append(Utilities.Cadenas.comillas(datos2));
            query.Append(", ");
            query.Append(Utilities.Cadenas.comillas(datos3));
            query.Append("); select clave from @inserted");

            if (db.EjecutarQuery(query.ToString()).error)
                return false;

            DataTable table = db.getTable();
            if (table.Rows.Count != 1)
                return false;
            clave = (int)table.Rows[0][0];

            if (usuario != null && tipo.Equals("usuario") && subtipo.Equals("password"))
                return Utilities.Correo.enviarPeticionPassword(clave, datos1, usuario.correo);

            return true;
        }

        /// <summary>
        /// Obtiene las peticiones del usuario mandado como parametro
        /// </summary>
        public static List<Peticion> obtenerPeticionesDeUsuario(Persona usuario)
        {
            if (usuario == null)
                return null;

            List<Peticion> lista = new List<Peticion>();
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from peticion where usuario = ");
            query.Append(usuario.clave);
            query.Append(" order by tipo, subtipo ");

            if (db.EjecutarQuery(query.ToString()).error)
                return lista;

            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                Peticion p = new Peticion();
                p.llenarDatos(r);
                p.usuario = usuario;

                lista.Add(p);
            }

            return lista;
        }

        /// <summary>
        /// Obtiene las primeras 30 peticiones de la base de datos
        /// Ignora las peticiones de cambio de password
        /// </summary>
        public static List<Peticion> obtenerPeticiones()
        {
            List<Peticion> lista = new List<Peticion>();
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select top 30 * from peticion where subtipo <> 'password' order by tipo, subtipo, usuario ");

            if (db.EjecutarQuery(query.ToString()).error)
                return lista;

            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                Peticion p = new Peticion();
                p.llenarDatos(r, true);

                lista.Add(p);
            }

            return lista;
        }

        /// <summary>
        /// Regresa el total de de peticiones en la base de datos
        /// Ignora las peticiones de cambio de password
        /// </summary>
        public static int cuentaPeticiones()
        {
            List<Peticion> lista = new List<Peticion>();
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select count(*) from peticion where subtipo <> 'password'");

            if (db.EjecutarQuery(query.ToString()).error)
                return 0;

            DataTable table = db.getTable();
            if (table.Rows.Count != 1)
                return 0;

            return (int) table.Rows[0][0];
        }

        private void llenarDatos(DataRow datos, bool cargarUsuario = false)
        {
            clave = (int) datos["clave"];
            tipo = datos["tipo"].ToString();
            subtipo = datos["subtipo"].ToString();
            datos1 = datos["datos1"].ToString();
            datos2 = datos["datos2"].ToString();
            datos3 = datos["datos3"].ToString();

            if (cargarUsuario)
                usuario = Persona.obtenerPersonaConClave((int)datos["usuario"]);
        }

        /// <summary>
        /// Obtiene la peticion de la base de datos con la clave mandada como parametro
        /// </summary>
        public static Peticion obtenerPeticionConClave(int clave)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from peticion where clave = ");
            query.Append(clave);

            if (db.EjecutarQuery(query.ToString()).error)
                return null;

            DataTable table = db.getTable();
            if (table.Rows.Count != 1)
                return null;

            Peticion p = new Peticion();
            p.llenarDatos(table.Rows[0], true);

            return p;
        }

        /// <summary>
        /// Elimina la petición de la base de datos
        /// </summary>
        /// <returns>Si se eliminó correctamente la petición</returns>
        public bool eliminarPeticion()
        {
            if (String.IsNullOrEmpty(tipo) || String.IsNullOrEmpty(subtipo))
                return false;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append("delete peticion where clave = ");
            query.Append(clave);

            if (db.EjecutarQuery(query.ToString()).error)
                return false;

            if (tipo.Equals("usuario") && subtipo.Equals("foto"))
                Utilities.Archivos.eliminarArchivo(datos1, Utilities.Archivos.FolderImagenes.TEMPORAL);

            return true;
        }

        /// <summary>
        /// Acepta la peticion y actualiza las tablas correspondientes
        /// </summary>
        public void aceptarPeticion()
        {
            if (String.IsNullOrEmpty(tipo) || String.IsNullOrEmpty(subtipo))
                return;

            if (tipo.Equals("usuario"))
            {
                if (subtipo.Equals("nombre"))
                    usuario.nombre = datos1;

                if (subtipo.Equals("foto"))
                    usuario.foto =
                        Utilities.Archivos.copiarArchivo(datos1, Utilities.Archivos.FolderImagenes.TEMPORAL,
                                            usuario.clave.ToString(), Utilities.Archivos.FolderImagenes.USUARIOS);
                usuario.guardarDatos();
            }

            eliminarPeticion();
        }
    }
}