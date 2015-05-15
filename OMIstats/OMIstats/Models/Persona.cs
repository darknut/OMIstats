using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace OMIstats.Models
{
    public class Persona
    {
        public int clave { get; set; }
        public string nombre { get; set; }
        public DateTime nacimiento { get; set; }
        public string facebook { get; set; }
        public string twitter { get; set; }
        public string sitio { get; set; }
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
        public bool logIn()
        {
            if (usuario == null || password == null)
                return false;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append("select * from persona where usuario = ");
            query.Append(Utilities.Cadenas.comillas(usuario));
            query.Append(" and password = HASHBYTES(\'SHA1\', ");
            query.Append(Utilities.Cadenas.comillas(password));
            query.Append(")");

            if (db.EjecutarQuery(query.ToString()).error)
                return false;

            DataTable table = db.getTable();
            if (table.Rows.Count != 1)
                return false;

            llenarDatos(this, table.Rows[0]);

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
            if (usuario == null || usuario.Length == 0)
                return null;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append("select * from persona where usuario = ");
            query.Append(Utilities.Cadenas.comillas(usuario));

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
    }
}