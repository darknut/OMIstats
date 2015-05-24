using System;
using System.Collections.Generic;
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
    }
}