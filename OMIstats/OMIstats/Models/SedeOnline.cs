using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using OMIstats.Utilities;
using System.ComponentModel.DataAnnotations;

namespace OMIstats.Models
{
    public class SedeOnline
    {
        int clave;
        string estado;
        string omi;

        [Required(ErrorMessage = "Escribe el nombre")]
        [RegularExpression(@"^[a-zA-Z ñÑáéíóúÁÉÍÓÚäëïöü\.'-]*$", ErrorMessage = "Escribiste caracteres inválidos en el nombre")]
        [MaxLength(200, ErrorMessage = "El tamaño máximo es 200 caracteres")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "Escribe el nombre")]
        [RegularExpression(@"^[a-zA-Z ñÑáéíóúÁÉÍÓÚäëïöü\.'-]*$", ErrorMessage = "Escribiste caracteres inválidos en el nombre")]
        [MaxLength(200, ErrorMessage = "El tamaño máximo es 200 caracteres")]
        public string supervisor { get; set; }

        [RegularExpression(@"^[0-9\.]+$", ErrorMessage = "Escribe un teléfono válido, no incluyas guiones, espacios o paréntesis")]
        [MaxLength(12, ErrorMessage = "El tamaño máximo es de 12 caracteres, no incluyas guiones, espacios o paréntesis")]
        public string telefono { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$", ErrorMessage = "Escribe un correo electrónico válido")]
        [MaxLength(50, ErrorMessage = "El tamaño máximo es de 50 caracteres")]
        public string correo { get; set; }

        public SedeOnline()
        {
            clave = 0;
            estado = "";
            omi = "";
            nombre = "";
            supervisor = "";
            telefono = "";
            correo = "";
        }

        public void llenarDatos(DataRow r)
        {
            clave = (int)r["clave"];
            nombre = r["nombre"].ToString().Trim();
            estado = r["estado"].ToString().Trim();
            supervisor = r["supervisor"].ToString().Trim();
            telefono = r["telefono"].ToString().Trim();
            correo = r["correo"].ToString().Trim();
        }

        public void nuevo()
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" insert into SedeOnline values( ");
            query.Append(Cadenas.comillas(estado));
            query.Append(" ,");
            query.Append(Cadenas.comillas(omi));
            query.Append(" ,");
            query.Append(Cadenas.comillas(nombre));
            query.Append(" ,");
            query.Append(Cadenas.comillas(supervisor));
            query.Append(" ,");
            query.Append(Cadenas.comillas(telefono));
            query.Append(" ,");
            query.Append(Cadenas.comillas(correo));
            query.Append(")");

            db.EjecutarQuery(query.ToString());
        }

        public void update()
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" update SedeOnline set estado = ");
            query.Append(Cadenas.comillas(estado));
            query.Append(" , olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" , nombre = ");
            query.Append(Cadenas.comillas(nombre));
            query.Append(" , supervisor = ");
            query.Append(Cadenas.comillas(supervisor));
            query.Append(" , telefono = ");
            query.Append(Cadenas.comillas(telefono));
            query.Append(" , correo = ");
            query.Append(Cadenas.comillas(correo));
            query.Append(" where clave = ");
            query.Append(clave);

            db.EjecutarQuery(query.ToString());
        }
    }
}
