using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

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
            PASSWORD,
            BIENVENIDO,
            ACCESO,
            GENERAL,
            DUDA,
            QUEJA,
            SUGERENCIA,
            PREGUNTA,
            COMENTARIO
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

            if (tipo == TipoPeticion.USUARIO &&
                (subtipo == TipoPeticion.PASSWORD || subtipo == TipoPeticion.BIENVENIDO))
                datos1 = Guid.NewGuid().ToString();

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" declare @inserted table(clave int); ");
            query.Append(" insert into peticion output inserted.clave into @inserted values (");
            query.Append(Utilities.Cadenas.comillas(tipo.ToString().ToLower()));
            query.Append(", ");
            query.Append(Utilities.Cadenas.comillas(subtipo.ToString().ToLower()));
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

            if (usuario != null && tipo == TipoPeticion.USUARIO)
            {
                if (subtipo == TipoPeticion.PASSWORD)
                    return Utilities.Correo.enviarPeticionPassword(clave, datos1, usuario.correo);
                if (subtipo == TipoPeticion.BIENVENIDO)
                    return Utilities.Correo.enviarPeticionBienvenido(clave, datos1, usuario.correo);
            }

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

            query.Append(" select top 30 * from peticion where subtipo <> 'password' and subtipo <> 'bienvenido'");
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
        /// Ignora las peticiones de cambio de password
        /// </summary>
        public static int cuentaPeticiones()
        {
            List<Peticion> lista = new List<Peticion>();
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select count(*) from peticion where subtipo <> 'password' and subtipo <> 'bienvenido'");

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
            tipo = (TipoPeticion) Enum.Parse(typeof(TipoPeticion), datos["tipo"].ToString().Trim().ToUpper());
            subtipo = (TipoPeticion) Enum.Parse(typeof(TipoPeticion), datos["subtipo"].ToString().Trim().ToUpper());
            datos1 = datos["datos1"].ToString().Trim();
            datos2 = datos["datos2"].ToString().Trim();
            datos3 = datos["datos3"].ToString().Trim();

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
            if (tipo == TipoPeticion.NULL || subtipo == TipoPeticion.NULL)
                return false;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append("delete peticion where clave = ");
            query.Append(clave);

            if (db.EjecutarQuery(query.ToString()).error)
                return false;

            if (tipo == TipoPeticion.USUARIO &&
                (subtipo == TipoPeticion.FOTO || subtipo == TipoPeticion.ACCESO))
                Utilities.Archivos.eliminarArchivo(datos1, Utilities.Archivos.FolderImagenes.TEMPORAL);

            return true;
        }

        /// <summary>
        /// Elimina las peticiones 'ocultas' del usuario
        /// Estas son las de peticion de acceso o de cambio de password
        /// Deben de borrarse cuando al usuario se le da un nuevo password
        /// </summary>
        private bool eliminarPeticionesPassword()
        {
            List<Peticion> peticiones = obtenerPeticionesDeUsuario(usuario);

            foreach (Peticion p in peticiones)
            {
                if (p.tipo == TipoPeticion.USUARIO &&
                    (p.subtipo == TipoPeticion.PASSWORD ||
                     p.subtipo == TipoPeticion.BIENVENIDO ||
                     p.subtipo == TipoPeticion.ACCESO))
                    if (!p.eliminarPeticion())
                        return false;
            }

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

                if (subtipo == TipoPeticion.FOTO)
                    usuario.foto =
                        Utilities.Archivos.copiarArchivo(datos1, Utilities.Archivos.FolderImagenes.TEMPORAL,
                                            usuario.clave.ToString(), Utilities.Archivos.FolderImagenes.USUARIOS);

                if (subtipo == TipoPeticion.PASSWORD || subtipo == TipoPeticion.BIENVENIDO)
                {
                    usuario.password = System.Web.Security.Membership.GeneratePassword(8, 3);
                    if (!eliminarPeticionesPassword())
                        return false;
                }

                if (subtipo == TipoPeticion.ACCESO)
                {
                    usuario.correo = datos2;
                    Peticion pe = new Peticion();
                    pe.tipo = TipoPeticion.USUARIO;
                    pe.subtipo = TipoPeticion.BIENVENIDO;
                    pe.usuario = usuario;
                    if (!pe.guardarPeticion())
                        return false;
                }

                if (!usuario.guardarDatos())
                    return false;
            }
            else if (tipo == TipoPeticion.GENERAL)
            {
                if (!Utilities.Correo.enviarRespuestaPeiticionGeneral(
                    datos2, datos3, usuario.correo, subtipo))
                    return false;
            }

            if (!eliminarPeticion())
                return false;

            return true;
        }

        /// <summary>
        /// Cuando un usuario hace log in, borramos todas las peticiones de
        /// cambio de password que hay de su usuario
        /// </summary>
        public static void borraPeticionesPassword(Persona usuario)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append("delete peticion where tipo = 'usuario'");
            query.Append(" and (subtipo = 'password' or subtipo = 'bienvenido') and usuario =  ");
            query.Append(usuario.clave);

            db.EjecutarQuery(query.ToString());
        }
    }
}