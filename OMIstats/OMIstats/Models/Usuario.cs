using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace OMIstats.Models
{   
    /// <summary>
    /// Clase para comunicarse con la base de datos de la OMI
    /// </summary>
    public class Usuario
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string Foto { get; set; }

        public string Nombre { get; set; }

        public string CURP { get; set; }

        public static string MockUserLoggedIn(int userId)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();
            string guid = Guid.NewGuid().ToString();

            query.Append(" update [Usuarios.Usuarios] set GUID = ");
            query.Append(Utilities.Cadenas.comillas(guid));
            query.Append(" where Id = ");
            query.Append(userId);

            db.EjecutarQuery(query.ToString(), Utilities.Acceso.BaseDeDatos.OMI);
            return guid;
        }
    }
}