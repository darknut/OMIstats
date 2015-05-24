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
        public void guardarPeticion()
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" insert into peticion values (");
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
            query.Append(")");

            db.EjecutarQuery(query.ToString());
        }

        /// <summary>
        /// Obtiene las peticiones del usuario mandado como parametro
        /// </summary>
        public static List<Peticion> obtenerPeticionesDeUsuario(Persona usuario)
        {
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
        /// Obtiene todas las peticiones de la base de datos
        /// </summary>
        public static List<Peticion> obtenerPeticiones()
        {
            List<Peticion> lista = new List<Peticion>();
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from peticion order by tipo, subtipo, usuario ");

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

            if (clave == 0)
                return null;

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
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append("delete peticion where clave = ");
            query.Append(clave);

            return !db.EjecutarQuery(query.ToString()).error;
        }
    }
}