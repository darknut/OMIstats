using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace OMIstats.Models
{
    public class Persona
    {
        public int clave = -1;
        public string nombre;
        public string nacimiento;
        public string facebook;
        public string twitter;
        public string sitio;
        public string correo;
        public string usuario;
        public bool admin;
        public char genero;
        public string foto;

        /// <summary>
        /// Solo de entrada, no se obtiene de la base de datos
        /// </summary>
        public string password;

        /// <summary>
        /// Usando los datos en las variables de instancia
        /// se intenta hacer log in, si los datos de acceso son correctos
        /// el resto de los datos en la instancia se llena
        /// </summary>
        /// <returns></returns>
        public bool logIn()
        {
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
        /// <param name="completo">Si es true, saca todos los datos de la fila, de ser false, solo nombre y clave</param>
        private static void llenarDatos(Persona persona, DataRow datos, bool completo = true)
        {
            persona.clave = (int) datos["clave"];
            persona.nombre = datos["nombre"].ToString();
            persona.password = "";

            if (completo)
            {
                persona.usuario = datos["usuario"].ToString();
                persona.nacimiento = datos["nacimiento"].ToString();
                persona.facebook = datos["facebook"].ToString();
                persona.twitter = datos["twitter"].ToString();
                persona.sitio = datos["sitio"].ToString();
                persona.correo = datos["correo"].ToString();
                persona.admin = datos["admin"].ToString().Equals("1");
                persona.genero = (char) datos["genero"];
                persona.foto = datos["foto"].ToString();
            }
        }
    }
}