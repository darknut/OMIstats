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
        public const int UsuarioNulo = -1;

        public int clave { get; set; }

        [Required(ErrorMessage = "Escribe tu nombre")]
        [RegularExpression(@"^[a-zA-Z ñÑáéíóúÁÉÍÓÚäëïöü\.'-]*$", ErrorMessage = "Escribiste caracteres inválidos en tu nombre")]
        [MaxLength(60, ErrorMessage = "El tamaño máximo es 60 caracteres")]
        public string nombre { get; set; }

        public DateTime nacimiento { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9\.]+$", ErrorMessage = "Escribe un nombre de usuario válido")]
        [MaxLength(50, ErrorMessage = "El tamaño máximo es de 50 caracteres")]
        public string facebook { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Escribe un nombre de usuario válido")]
        [MaxLength(15, ErrorMessage = "El tamaño máximo es de 15 caracteres")]
        public string twitter { get; set; }

        [RegularExpression(@"^(https?:\/\/)?((([\w-]+)\.){1,})([\/\w\.-]+)(\?[\/\w\.-=%&]*)?$", ErrorMessage = "Escribe una URL válida")]
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

        public Persona(int clave): this()
        {
            this.clave = clave;
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
            genero = "M";
            foto = "";
            ioiID = 0;
        }

        /// <summary>
        /// Llena los datos de una persona de la fila mandada como parametro
        /// </summary>
        /// <param name="datos">La fila con el origen de los datos</param>
        /// <param name="completo">Si es true, saca todos los datos de la fila, de ser false, solo nombre, usuario y clave</param>
        private void llenarDatos(DataRow datos, bool completo = true)
        {
            clave = (int) datos["clave"];
            nombre = datos["nombre"].ToString().Trim();
            usuario = datos["usuario"].ToString().Trim();

            if (completo)
            {
                nacimiento = Utilities.Fechas.stringToDate(datos["nacimiento"].ToString().Trim());
                facebook = datos["facebook"].ToString().Trim();
                twitter = datos["twitter"].ToString().Trim();
                sitio = datos["sitio"].ToString().Trim();
                correo = datos["correo"].ToString().Trim();
                admin = (bool) datos["admin"];
                genero = datos["genero"].ToString();
                foto = datos["foto"].ToString().Trim();
                ioiID = (int) datos["ioiID"];
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

            return (p.clave > 0);
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
            p.llenarDatos(table.Rows[0]);

            return p;
        }

        /// <summary>
        /// Regresa el objeto persona asociado con el nombre mandado como parámetro
        /// </summary>
        public static Persona obtenerPersonaConNombre(string nombre)
        {
            if (String.IsNullOrEmpty(nombre))
                return null;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append("select * from persona where nombreHash = HASHBYTES(\'SHA1\', ");
            query.Append(Utilities.Cadenas.comillas(Utilities.Cadenas.quitaEspeciales(nombre)));
            query.Append(")");

            if (db.EjecutarQuery(query.ToString()).error)
                return null;

            DataTable table = db.getTable();
            if (table.Rows.Count == 0)
                return null;

            Persona p = new Persona();
            p.llenarDatos(table.Rows[0]);

            return p;
        }

        /// <summary>
        /// Regresa la persona asociada con la clave mandada como parametro
        /// </summary>
        public static Persona obtenerPersonaConClave(int clave)
        {
            Persona p = new Persona();
            p.clave = clave;
            if (p.recargarDatos())
                return p;
            else
                return null;
        }

        /// <summary>
        /// Lee de nuevo los datos del usuario de la base de datos y los actualiza en el objeto
        /// </summary>
        /// <returns>Si se recargaron satisfactoriamente los datos</returns>
        public bool recargarDatos()
        {
            if (clave == 0)
                return false;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append("select * from persona where clave = ");
            query.Append(clave);

            if (db.EjecutarQuery(query.ToString()).error)
                return false;

            DataTable table = db.getTable();
            if (table.Rows.Count != 1)
                return false;

            llenarDatos(table.Rows[0]);

            return true;
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
                return admins;

            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                Persona p = new Persona();
                p.llenarDatos(r, completo:false);
                admins.Add(p);
            }

            return admins;
        }

        /// <summary>
        /// Guarda los datos en la base de datos
        /// </summary>
        /// <param name="generarPeticiones">Si nombre y foto deben de guardarse
        /// directamente en la base de datos o si se deben generar peticiones</param>
        /// <returns>Si el update se ejecutó correctamente</returns>
        public bool guardarDatos(bool generarPeticiones = false)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" update persona set ");

            if (!String.IsNullOrEmpty(nombre))
            {
                if (generarPeticiones)
                {
                    Peticion pet = new Peticion();
                    pet.tipo = Peticion.TipoPeticion.USUARIO;
                    pet.subtipo = Peticion.TipoPeticion.NOMBRE;
                    pet.usuario = this;
                    pet.datos1 = nombre;
                    pet.guardarPeticion();
                }
                else
                {
                    query.Append(" nombre = ");
                    query.Append(Utilities.Cadenas.comillas(nombre));
                    query.Append(",");

                    query.Append(" nombreHash = HASHBYTES(\'SHA1\', ");
                    query.Append(Utilities.Cadenas.comillas(Utilities.Cadenas.quitaEspeciales(nombre)));
                    query.Append("),");
                }
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

            query.Append(" [admin] = ");
            query.Append(admin ? "1" : "0");
            query.Append(",");

            query.Append(" genero = ");
            query.Append(Utilities.Cadenas.comillas(genero));
            query.Append(",");

            if (!String.IsNullOrEmpty(foto))
            {
                if (generarPeticiones)
                {
                    Peticion pet = new Peticion();
                    pet.tipo = Peticion.TipoPeticion.USUARIO;
                    pet.subtipo = Peticion.TipoPeticion.FOTO;
                    pet.usuario = this;
                    pet.datos1 = foto;
                    pet.guardarPeticion();
                }
                else
                {
                    query.Append(" foto = ");
                    query.Append(Utilities.Cadenas.comillas(foto));
                    query.Append(",");
                }
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

        /// <summary>
        /// Regresa si el usuario tiene peticiones que él mismo puede ver
        /// </summary>
        /// <returns></returns>
        public bool tienePeticiones()
        {
            List<Peticion> peticiones = Peticion.obtenerPeticionesDeUsuario(this);

            foreach (Peticion p in peticiones)
                if (p.tipo == Peticion.TipoPeticion.USUARIO &&
                    (p.subtipo == Peticion.TipoPeticion.FOTO ||
                     p.subtipo == Peticion.TipoPeticion.NOMBRE))
                    return true;

            return false;
        }

        /// <summary>
        /// Crea un nuevo usuario con los datos en el objeto
        /// </summary>
        public void nuevoUsuario(Utilities.Archivos.FotoInicial fotoInicial = Utilities.Archivos.FotoInicial.KAREL)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" declare @inserted table(clave int); ");
            query.Append(" insert into persona (nombre) output inserted.clave into @inserted values( ");
            query.Append(Utilities.Cadenas.comillas(nombre));
            query.Append("); select clave from @inserted ");

            if (db.EjecutarQuery(query.ToString()).error)
                return;

            DataTable table = db.getTable();
            if (table.Rows.Count != 1)
                return;
            clave = (int)table.Rows[0][0];
            usuario = "_" + clave.ToString();
            foto = Utilities.Archivos.obtenerFotoInicial(fotoInicial);

            guardarDatos();
        }

        /// <summary>
        /// Borra de la base de datos todos los competidores sin ningún rol en la OMI
        /// </summary>
        public static void borrarZombies()
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" delete persona where clave in ( ");
            query.Append(" select clave from Persona where clave ");
            query.Append(" not in (select distinct(Persona) from MiembroDelegacion) and clave not in ");
            query.Append(" (select delegado from Estado where delegado is not null))");

            db.EjecutarQuery(query.ToString());
        }

        /// <summary>
        /// A partir de la persona obtenida de la base de datos de la OMI, hacemos la mejor aproximacion
        /// para obtener el objeto deseado
        /// </summary>
        /// <param name="usuario">El usuario obtenido de la base de la OMI</param>
        /// <returns>La persona correspondiente en nuestra base</returns>
        public static Persona obtenerPersonaDeUsuario(Usuario usuario)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            // Primero revisamos los CURP

            if (usuario.CURP != null && !usuario.CURP.Equals(String.Empty))
            {
                query.Append("");
            }

            return null;
        }
    }
}