using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace OMIstats.Models
{
    public class Persona
    {
        public const int TamañoUsuarioMaximo = 20;

        public int clave { get; set; }

        [Required(ErrorMessage = "Escribe tu nombre")]
        [RegularExpression(@"^[a-zA-Z ñÑáéíóúÁÉÍÓÚü\.'-]*$", ErrorMessage = "Escribiste caracteres inválidos en tu nombre")]
        [MaxLength(60, ErrorMessage = "El tamaño máximo es 60 caracteres")]
        public string nombre { get; set; }

        public DateTime nacimiento { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9\.]+$", ErrorMessage = "Escribe un nombre de usuario válido")]
        [MaxLength(50, ErrorMessage = "El tamaño máximo es de 50 caracteres")]
        public string facebook { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Escribe un nombre de usuario válido")]
        [MaxLength(15, ErrorMessage = "El tamaño máximo es de 15 caracteres")]
        public string twitter { get; set; }

        [RegularExpression(@"^(https?:\/\/)?((([\w-]+)\.){2,})([\/\w\.-]+)(\?[\/\w\.-=%&]*)?$", ErrorMessage = "Escribe una URL válida")]
        [MaxLength(100, ErrorMessage = "El tamaño máximo es de 100 caracteres")]
        public string sitio { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$", ErrorMessage = "Escribe un correo electrónico válido")]
        [MaxLength(50, ErrorMessage = "El tamaño máximo es de 50 caracteres")]
        public string correo { get; set; }

        public string usuario { get; set; }

        public bool admin { get; set; }

        public string genero { get; set; }

        public string foto { get; set; }

        public int ioiID { get; set; }

        /// <summary>
        /// Solo de entrada, no se obtiene de la base de datos
        /// </summary>
        public string password { get; set; }

        public enum DisponibilidadUsuario
        {
            DISPONIBLE = 0,
            NUMBER,
            ALFANUMERIC,
            TAKEN,
            SIZE,
            ERROR
        }

        public enum ErrorPassword
        {
            OK = 0,
            PASSWORD_INVALIDO,
            PASSWORD_VACIO,
            PASSWORD_DIFERENTE
        }

        public Persona()
        {
            clave = 0;
            nombre = "";
            nacimiento = new DateTime(1900, 1, 1);
            facebook = "";
            twitter = "";
            sitio = "";
            correo = "";
            usuario = "";
            admin = false;
            genero = " ";
            foto = "";
            ioiID = 0;
            password = "";
        }

        /// <summary>
        /// Usando los datos en las variables de instancia
        /// se intenta hacer log in, si los datos de acceso son correctos
        /// el resto de los datos en la instancia se llena
        /// </summary>
        /// <returns>Si el login fue exitoso</returns>
        public bool logIn(bool datosCompletos = true)
        {
            if (String.IsNullOrEmpty(usuario) || String.IsNullOrEmpty(password))
                return false;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append("select * from persona where usuario = ");
            query.Append(Utilities.Cadenas.comillas(usuario.ToLower()));
            query.Append(" and password = HASHBYTES(\'SHA1\', ");
            query.Append(Utilities.Cadenas.comillas(password));
            query.Append(")");

            if (db.EjecutarQuery(query.ToString()).error)
                return false;

            DataTable table = db.getTable();
            if (table.Rows.Count != 1)
                return false;

            llenarDatos(this, table.Rows[0], completo:datosCompletos);

            return true;
        }

        /// <summary>
        /// Llena los datos de una persona de la fila mandada como parametro
        /// </summary>
        /// <param name="persona">El objeto donde se guardaran los datos</param>
        /// <param name="datos">La fila con el origen de los datos</param>
        /// <param name="completo">Si es true, saca todos los datos de la fila, de ser false, solo nombre, usuario y clave</param>
        private static void llenarDatos(Persona persona, DataRow datos, bool completo = true)
        {
            persona.clave = (int) datos["clave"];
            persona.nombre = datos["nombre"].ToString().Trim();
            persona.usuario = datos["usuario"].ToString().Trim();
            persona.password = "";

            if (completo)
            {
                persona.nacimiento = Utilities.Fechas.stringToDate(datos["nacimiento"].ToString().Trim());
                persona.facebook = datos["facebook"].ToString().Trim();
                persona.twitter = datos["twitter"].ToString().Trim();
                persona.sitio = datos["sitio"].ToString().Trim();
                persona.correo = datos["correo"].ToString().Trim();
                persona.admin = (bool) datos["admin"];
                persona.genero = datos["genero"].ToString();
                persona.foto = datos["foto"].ToString().Trim();
                persona.ioiID = (int) datos["ioiID"];
            }
        }

        /// <summary>
        /// Revisa si hay un usuario loggeado
        /// </summary>
        public static bool isLoggedIn(Object obj)
        {
            if (obj == null)
                return false;

            Persona p = (Persona)obj;

            return (p.clave != 0);
        }

        /// <summary>
        /// Regresa el objeto persona asociado con el nombre de usuario mandado como parámetro
        /// </summary>
        public static Persona obtenerPersonaDeUsuario(string usuario)
        {
            if (String.IsNullOrEmpty(usuario))
                return null;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append("select * from persona where usuario = ");
            query.Append(Utilities.Cadenas.comillas(usuario.ToLower()));

            if (db.EjecutarQuery(query.ToString()).error)
                return null;

            DataTable table = db.getTable();
            if (table.Rows.Count != 1)
                return null;

            Persona p = new Persona();
            llenarDatos(p, table.Rows[0]);

            return p;
        }

        /// <summary>
        /// Lee de nuevo los datos del usuario de la base de datos y los actualiza en el objeto
        /// </summary>
        public void recargarDatos()
        {
            if (clave == 0)
                return;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append("select * from persona where clave = ");
            query.Append(clave);

            if (db.EjecutarQuery(query.ToString()).error)
                return;

            DataTable table = db.getTable();
            if (table.Rows.Count != 1)
                return;

            llenarDatos(this, table.Rows[0]);
        }

        /// <summary>
        /// Regresa los datos de los admins del sitio
        /// como una lista de personas
        /// </summary>
        public static List<Persona> obtenerAdmins()
        {
            List<Persona> admins = new List<Persona>();

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append("select * from persona where admin = 1");

            if (db.EjecutarQuery(query.ToString()).error)
                return null;

            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                Persona p = new Persona();
                llenarDatos(p, r, completo:false);
                admins.Add(p);
            }

            return admins;
        }

        /// <summary>
        /// Revisa en la base de datos si el nombre de usuario está disponible y si es un nombre válido
        /// </summary>
        /// <remarks>Este campo se valida a mano porque se valida en una llamada AJAX</remarks>
        /// <returns>
        /// ok: el nombre esta disponible
        /// number: el nombre empieza con numero y es invalido
        /// alfanumeric: el nombre no es alfanumerico
        /// taken: el nombre no esta disponible
        /// </returns>
        public static DisponibilidadUsuario revisarNombreUsuarioDisponible(Persona p, string usuario)
        {
            if (p == null || usuario == null)
                return DisponibilidadUsuario.ERROR;

            usuario = usuario.Trim().ToLower();

            if (usuario.Length == 0 || usuario.Length > TamañoUsuarioMaximo)
                return DisponibilidadUsuario.SIZE;

            if (Regex.IsMatch(usuario, "^\\d"))
                return DisponibilidadUsuario.NUMBER;

            if (p.usuario.Equals(usuario))
                return DisponibilidadUsuario.DISPONIBLE;

            if (!Regex.IsMatch(usuario, "^[a-zA-Z0-9]*$"))
                return DisponibilidadUsuario.ALFANUMERIC;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from Persona ");
            query.Append(" where usuario = ");
            query.Append(Utilities.Cadenas.comillas(usuario));
            query.Append(" and clave <> ");
            query.Append(p.clave);

            db.EjecutarQuery(query.ToString());

            DataTable table = db.getTable();
            if (table.Rows.Count == 0)
                return DisponibilidadUsuario.DISPONIBLE;

            return DisponibilidadUsuario.TAKEN;
        }

        /// <summary>
        /// Verifica que el nuevo password sea válido
        /// </summary>
        public ErrorPassword verificaPasswords(string password1, string password2)
        {
            if (!logIn(datosCompletos: false))
                return ErrorPassword.PASSWORD_INVALIDO;

            if (String.IsNullOrEmpty(password1))
                return ErrorPassword.PASSWORD_VACIO;

            if (!password1.Equals(password2))
                return ErrorPassword.PASSWORD_DIFERENTE;

            return ErrorPassword.OK;
        }

        /// <summary>
        /// Guarda los datos en la base de datos
        /// </summary>
        /// <returns>Si el update se ejecutó correctamente</returns>
        public bool guardarDatos()
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" update persona set ");

            if (nombre.Length > 0)
            {
                query.Append(" nombre = ");
                query.Append(Utilities.Cadenas.comillas(nombre));
                query.Append(",");

                query.Append(" nombreHash = HASHBYTES(\'SHA1\', ");
                query.Append(Utilities.Cadenas.comillas(Utilities.Cadenas.quitaEspeciales(nombre)));
                query.Append("),");
            }

            query.Append(" facebook = ");
            query.Append(Utilities.Cadenas.comillas(facebook));
            query.Append(",");

            query.Append(" twitter = ");
            query.Append(Utilities.Cadenas.comillas(twitter));
            query.Append(",");

            query.Append(" sitio = ");
            query.Append(Utilities.Cadenas.comillas(sitio));
            query.Append(",");

            query.Append(" usuario = ");
            query.Append(Utilities.Cadenas.comillas(usuario));
            query.Append(",");

            if (password.Length > 0)
            {
                query.Append(" [password] = HASHBYTES(\'SHA1\', ");
                query.Append(Utilities.Cadenas.comillas(password));
                query.Append("),");
            }

            query.Append(" [admin] = ");
            query.Append(admin ? "1" : "0");
            query.Append(",");

            query.Append(" genero = ");
            query.Append(Utilities.Cadenas.comillas(genero));
            query.Append(",");

            if (foto.Length > 0)
            {
                query.Append(" foto = ");
                query.Append(Utilities.Cadenas.comillas(foto));
                query.Append(",");
            }

            query.Append(" correo = ");
            query.Append(Utilities.Cadenas.comillas(correo));
            query.Append(",");

            query.Append(" nacimiento = ");
            query.Append(Utilities.Cadenas.comillas(Utilities.Fechas.dateToString(nacimiento)));
            query.Append(",");

            query.Append(" ioiID = ");
            query.Append(ioiID);

            query.Append(" where clave = ");
            query.Append(clave);

            return !db.EjecutarQuery(query.ToString()).error;
        }
    }
}