using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;


namespace OMIstats.Models
{
    public class Fotos
    {
        public int clave { get; set; }

        public string olimpiada { get; set; }

        public TipoOlimpiada tipoOlimpiada { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int orden { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [MaxLength(50, ErrorMessage = "El tamaño máximo es 50 caracteres")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [MaxLength(200, ErrorMessage = "El tamaño máximo es 200 caracteres")]
        public string url { get; set; }

        public Fotos()
        {
            clave = 0;
            olimpiada = "";
            tipoOlimpiada = TipoOlimpiada.NULL;
            orden = 0;
            nombre = "";
            url = "";
        }

        private void llenarDatos(DataRow r)
        {
            clave = (int)r["clave"];
            olimpiada = r["olimpiada"].ToString().Trim();
            tipoOlimpiada = (TipoOlimpiada)Enum.Parse(typeof(TipoOlimpiada), r["clase"].ToString().ToUpper());
            orden = (int)r["orden"];
            url = r["url"].ToString().Trim();
            nombre = r["nombre"].ToString().Trim();
        }

        public static Fotos obtenerFotos(int id)
        {
            Fotos f = new Fotos();

            if (id == 0)
                return f;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from Fotos where clave = ");
            query.Append(id);

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            if (table.Rows.Count == 0)
                return f;

            f.llenarDatos(table.Rows[0]);

            return f;
        }

        private void nuevo()
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" declare @inserted table(clave int); ");
            query.Append(" insert into fotos (orden) output inserted.clave into @inserted values( ");
            query.Append(" 0); select clave from @inserted ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            if (table.Rows.Count == 1)
                clave = (int)table.Rows[0][0];
        }

        public void guardarDatos()
        {
            if (clave == 0)
                nuevo();

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" update fotos set olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(olimpiada));
            query.Append(", clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(", orden = ");
            query.Append(orden);
            query.Append(", nombre = ");
            query.Append(Utilities.Cadenas.comillas(nombre));
            query.Append(", url = ");
            query.Append(Utilities.Cadenas.comillas(url));
            query.Append(" where clave = ");
            query.Append(clave);

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();
        }
    }
}