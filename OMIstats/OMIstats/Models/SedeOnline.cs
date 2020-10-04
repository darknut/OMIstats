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
        public int clave { get; set; }
        public string estado { get; set; }
        public string omi { get; set; }

        [Required(ErrorMessage = "Escribe el nombre")]
        [RegularExpression(@"^[a-zA-Z0-9 ñÑáéíóúÁÉÍÓÚäëïöü#\.'-]*$", ErrorMessage = "Escribiste caracteres inválidos")]
        [MaxLength(200, ErrorMessage = "El tamaño máximo es 200 caracteres")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "Escribe el nombre")]
        [RegularExpression(@"^[a-zA-Z ñÑáéíóúÁÉÍÓÚäëïöü\.'-]*$", ErrorMessage = "Escribiste caracteres inválidos en el nombre")]
        [MaxLength(200, ErrorMessage = "El tamaño máximo es 200 caracteres")]
        public string supervisor { get; set; }

        [Required(ErrorMessage = "Escribe el teléfono")]
        [RegularExpression(@"^[0-9\.]+$", ErrorMessage = "Escribe un teléfono válido, no incluyas guiones, espacios o paréntesis")]
        [MaxLength(12, ErrorMessage = "El tamaño máximo es de 12 caracteres, no incluyas guiones, espacios o paréntesis")]
        public string telefono { get; set; }

        [Required(ErrorMessage = "Escribe el correo electrónico")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$", ErrorMessage = "Escribe un correo electrónico válido")]
        [MaxLength(50, ErrorMessage = "El tamaño máximo es de 50 caracteres")]
        public string correo { get; set; }

        [RegularExpression(@"^[a-zA-Z ñÑáéíóúÁÉÍÓÚäëïöü\.'-]*$", ErrorMessage = "Escribiste caracteres inválidos en el nombre")]
        [MaxLength(200, ErrorMessage = "El tamaño máximo es 200 caracteres")]
        public string supervisor2 { get; set; }

        [RegularExpression(@"^[0-9\.]+$", ErrorMessage = "Escribe un teléfono válido, no incluyas guiones, espacios o paréntesis")]
        [MaxLength(12, ErrorMessage = "El tamaño máximo es de 12 caracteres, no incluyas guiones, espacios o paréntesis")]
        public string telefono2 { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$", ErrorMessage = "Escribe un correo electrónico válido")]
        [MaxLength(50, ErrorMessage = "El tamaño máximo es de 50 caracteres")]
        public string correo2 { get; set; }

        [RegularExpression(@"^[a-zA-Z ñÑáéíóúÁÉÍÓÚäëïöü\.'-]*$", ErrorMessage = "Escribiste caracteres inválidos en el nombre")]
        [MaxLength(200, ErrorMessage = "El tamaño máximo es 200 caracteres")]
        public string supervisor3 { get; set; }

        [RegularExpression(@"^[0-9\.]+$", ErrorMessage = "Escribe un teléfono válido, no incluyas guiones, espacios o paréntesis")]
        [MaxLength(12, ErrorMessage = "El tamaño máximo es de 12 caracteres, no incluyas guiones, espacios o paréntesis")]
        public string telefono3 { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$", ErrorMessage = "Escribe un correo electrónico válido")]
        [MaxLength(50, ErrorMessage = "El tamaño máximo es de 50 caracteres")]
        public string correo3 { get; set; }

        public SedeOnline()
        {
            clave = 0;
            estado = "";
            omi = "";
            nombre = "";
            supervisor = "";
            telefono = "";
            correo = "";
            supervisor2 = "";
            telefono2 = "";
            correo2 = "";
            supervisor3 = "";
            telefono3 = "";
            correo3 = "";
        }

        public void llenarDatos(DataRow r)
        {
            clave = DataRowParser.ToInt(r["clave"]);
            nombre = DataRowParser.ToString(r["nombre"]);
            omi = DataRowParser.ToString(r["olimpiada"]);
            estado = DataRowParser.ToString(r["estado"]);
            supervisor = DataRowParser.ToString(r["supervisor"]);
            telefono = DataRowParser.ToString(r["telefono"]);
            correo = DataRowParser.ToString(r["correo"]);
            supervisor2 = DataRowParser.ToString(r["supervisor2"]);
            telefono2 = DataRowParser.ToString(r["telefono2"]);
            correo2 = DataRowParser.ToString(r["correo2"]);
            supervisor3 = DataRowParser.ToString(r["supervisor3"]);
            telefono3 = DataRowParser.ToString(r["telefono3"]);
            correo3 = DataRowParser.ToString(r["correo3"]);
        }

        private void nuevo()
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
            query.Append(" ,");
            query.Append(Cadenas.comillas(supervisor2));
            query.Append(" ,");
            query.Append(Cadenas.comillas(telefono2));
            query.Append(" ,");
            query.Append(Cadenas.comillas(correo2));
            query.Append(" ,");
            query.Append(Cadenas.comillas(supervisor3));
            query.Append(" ,");
            query.Append(Cadenas.comillas(telefono3));
            query.Append(" ,");
            query.Append(Cadenas.comillas(correo3));
            query.Append(")");

            db.EjecutarQuery(query.ToString());
        }

        private void update()
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
            query.Append(" , supervisor2 = ");
            query.Append(Cadenas.comillas(supervisor2));
            query.Append(" , telefono2 = ");
            query.Append(Cadenas.comillas(telefono2));
            query.Append(" , correo2 = ");
            query.Append(Cadenas.comillas(correo2));
            query.Append(" , supervisor3 = ");
            query.Append(Cadenas.comillas(supervisor3));
            query.Append(" , telefono3 = ");
            query.Append(Cadenas.comillas(telefono3));
            query.Append(" , correo3 = ");
            query.Append(Cadenas.comillas(correo3));
            query.Append(" where clave = ");
            query.Append(clave);

            db.EjecutarQuery(query.ToString());
        }

        public void guardar()
        {
            if (clave == 0)
                nuevo();
            else
                update();
        }

        public static List<SedeOnline> obtenerSedes(string omi, string estado)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();
            List<SedeOnline> list = new List<SedeOnline>();

            query.Append(" select * from SedeOnline where olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            if (!String.IsNullOrEmpty(estado))
            {
                query.Append(" and estado = ");
                query.Append(Cadenas.comillas(estado));
            }
            query.Append(" order by estado asc ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                SedeOnline so = new SedeOnline();
                so.llenarDatos(r);
                list.Add(so);
            }

            return list;
        }

        public static SedeOnline obtenerSedeConClave(int clave)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from SedeOnline where clave = ");
            query.Append(clave);

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();
            SedeOnline so = null;

            if (table.Rows.Count > 0)
            {
                so = new SedeOnline();
                so.llenarDatos(table.Rows[0]);
            }

            return so;
        }

        public void borrar()
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" delete SedeOnline where clave = ");
            query.Append(clave);

            db.EjecutarQuery(query.ToString());
        }

        private static void generarDiplomas(string omi, string X, string baseURL, string estado, string supervisor, int i, StringBuilder lineas)
        {
            lineas.Append(estado);
            lineas.Append("\\Supervisor");
            lineas.Append(i);
            lineas.Append(".pdf,");
            lineas.Append(supervisor);
            lineas.Append(",");
            lineas.Append(X);
            lineas.Append(",Supervisor");
            lineas.Append(Estado.obtenerEstadoConClave(estado).obtenerNombreConPrefijo());
            lineas.Append(",");
            lineas.Append(baseURL);
            lineas.Append("/Profile/OMI/");
            lineas.Append(omi);
            lineas.Append("/");
            lineas.Append(estado);
            lineas.Append(",OMI\n");
        }

        public static string generarDiplomas(string omi, string X, string baseURL)
        {
            StringBuilder lineas = new StringBuilder();

            List<SedeOnline> sedes = obtenerSedes(omi, null);

            int i = 1;
            foreach (SedeOnline sede in sedes)
            {
                generarDiplomas(omi, X, baseURL, sede.estado, sede.supervisor, i++, lineas);
                if (!String.IsNullOrEmpty(sede.supervisor2))
                    generarDiplomas(omi, X, baseURL, sede.estado, sede.supervisor2, i++, lineas);
                if (!String.IsNullOrEmpty(sede.supervisor3))
                    generarDiplomas(omi, X, baseURL, sede.estado, sede.supervisor3, i++, lineas);
            }

            return lineas.ToString();
        }
    }
}
