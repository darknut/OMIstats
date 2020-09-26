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
    public class Peticion
    {
        public const int TamañoFotoMaximo = 1024 * 300;
        public const int TamañoPeticionMaximo = 1024 * 1024;
        public const int TamañoDatos3 = 300;

        public int clave { get; set; }
        public TipoPeticion tipo { get; set; }
        public TipoPeticion subtipo { get; set; }
        public Persona usuario { get; set; }
        [MaxLength(100, ErrorMessage = "El tamaño máximo es 100 caracteres")]
        public string datos1 { get; set; }
        [MaxLength(100, ErrorMessage = "El tamaño máximo es 100 caracteres")]
        public string datos2 { get; set; }
        [MaxLength(TamañoDatos3, ErrorMessage = "El tamaño máximo es 300 caracteres")]
        public string datos3 { get; set; }

        public enum TipoPeticion
        {
            NULL,
            USUARIO,
            NOMBRE,
            FOTO,
            GENERAL,
            DUDA,
            QUEJA,
            SUGERENCIA,
            PREGUNTA,
            COMENTARIO,
            LOGIN,
            NO_ERROR,
            NO_ESTOY,
            ERROR,
            BAD_LINK,
            INCOMPLETO,
            DUPLICADO,
            NO_SOY_YO,
            PUNTOS,
            APELLIDOPATERNO,
            APELLIDOMATERNO
        }

        public Peticion()
        {
            tipo = TipoPeticion.NULL;
            subtipo = TipoPeticion.NULL;
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
            if (tipo == TipoPeticion.NULL || subtipo == TipoPeticion.NULL)
                return false;

            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" declare @inserted table(clave int); ");
            query.Append(" insert into peticion output inserted.clave into @inserted values (");
            query.Append(Cadenas.comillas(tipo.ToString().ToLower()));
            query.Append(", ");
            query.Append(Cadenas.comillas(subtipo.ToString().ToLower()));
            query.Append(", ");
            if (usuario != null)
                query.Append(usuario.clave);
            else
                query.Append("0");
            query.Append(", ");
            query.Append(Cadenas.comillas(datos1));
            query.Append(", ");
            query.Append(Cadenas.comillas(datos2));
            query.Append(", ");
            query.Append(Cadenas.comillas(datos3));
            query.Append("); select clave from @inserted");

            if (db.EjecutarQuery(query.ToString()).error)
                return false;

            DataTable table = db.getTable();
            if (table.Rows.Count != 1)
                return false;
            clave = (int)table.Rows[0][0];

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
            Acceso db = new Acceso();
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
        /// </summary>
        public static List<Peticion> obtenerPeticiones()
        {
            List<Peticion> lista = new List<Peticion>();
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select top 30 * from peticion ");
            query.Append(" order by tipo, subtipo, usuario ");

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
        /// </summary>
        public static int cuentaPeticiones()
        {
            List<Peticion> lista = new List<Peticion>();
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select count(*) from peticion ");

            if (db.EjecutarQuery(query.ToString()).error)
                return 0;

            DataTable table = db.getTable();
            if (table.Rows.Count != 1)
                return 0;

            return (int) table.Rows[0][0];
        }

        private void llenarDatos(DataRow datos, bool cargarUsuario = false)
        {
            clave = DataRowParser.ToInt(datos["clave"]);
            tipo = DataRowParser.ToTipoPeticion(datos["tipo"]);
            subtipo = DataRowParser.ToTipoPeticion(datos["subtipo"]);
            datos1 = DataRowParser.ToString(datos["datos1"]);
            datos2 = DataRowParser.ToString(datos["datos2"]);
            datos3 = DataRowParser.ToString(datos["datos3"]);

            if (cargarUsuario)
                usuario = Persona.obtenerPersonaConClave(DataRowParser.ToInt(datos["usuario"]));
        }

        /// <summary>
        /// Obtiene la peticion de la base de datos con la clave mandada como parametro
        /// </summary>
        public static Peticion obtenerPeticionConClave(int clave)
        {
            Acceso db = new Acceso();
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
            if (tipo == TipoPeticion.NULL || subtipo == TipoPeticion.NULL)
                return false;

            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append("delete peticion where clave = ");
            query.Append(clave);

            if (db.EjecutarQuery(query.ToString()).error)
                return false;

            if (tipo == TipoPeticion.USUARIO && subtipo == TipoPeticion.FOTO)
                Archivos.eliminarArchivo(datos1, Archivos.FolderImagenes.TEMPORAL);

            return true;
        }

        /// <summary>
        /// Acepta la peticion y actualiza las tablas correspondientes
        /// </summary>
        public bool aceptarPeticion()
        {
            if (tipo == TipoPeticion.NULL || subtipo == TipoPeticion.NULL)
                return false;

            if (tipo == TipoPeticion.USUARIO)
            {
                if (subtipo == TipoPeticion.NOMBRE)
                    usuario.nombre = datos1;

                if (subtipo == TipoPeticion.APELLIDOPATERNO)
                    usuario.apellidoPaterno = datos1;

                if (subtipo == TipoPeticion.APELLIDOMATERNO)
                    usuario.apellidoMaterno = datos1;

                if (subtipo == TipoPeticion.FOTO)
                    usuario.foto =
                        Archivos.copiarArchivo(datos1, Archivos.FolderImagenes.TEMPORAL,
                                    usuario.clave.ToString(), Archivos.FolderImagenes.USUARIOS);

                if (!usuario.guardarDatos())
                    return false;
            }

            if (!eliminarPeticion())
                return false;

            return true;
        }
    }
}