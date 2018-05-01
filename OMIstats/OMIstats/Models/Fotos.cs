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
    public class Album
    {
        public static string ACCESS_TOKEN;

        [Required(ErrorMessage = "Campo requerido")]
        [MaxLength(50, ErrorMessage = "El tamaño máximo es 50 caracteres")]
        public string id { get; set; }

        public string olimpiada { get; set; }

        public TipoOlimpiada tipoOlimpiada { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int orden { get; set; }

        public string nombre { get; set; }

        public int fotos { get; set; }

        public string portada { get; set; }

        public bool update { get; set; }

        public Album()
        {
            id = "";
            olimpiada = "";
            tipoOlimpiada = TipoOlimpiada.NULL;
            orden = 0;
            nombre = "";
            fotos = 0;
            portada = "";
        }

        private void llenarDatos(DataRow r)
        {
            id = r["id"].ToString().Trim();
            olimpiada = r["olimpiada"].ToString().Trim();
            tipoOlimpiada = (TipoOlimpiada)Enum.Parse(typeof(TipoOlimpiada), r["clase"].ToString().ToUpper());
            orden = (int)r["orden"];
            fotos = (int)r["fotos"];
            nombre = r["nombre"].ToString().Trim();
            portada = r["portada"].ToString().Trim();
        }

        public static Album obtenerAlbum(string id)
        {
            Album al = new Album();

            if (String.IsNullOrEmpty(id))
                return al;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from album where id = ");
            query.Append(Utilities.Cadenas.comillas(id));

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            if (table.Rows.Count == 0)
                return al;

            al.llenarDatos(table.Rows[0]);

            return al;
        }

        private void tryNew()
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" insert into album (id) values( ");
            query.Append(Utilities.Cadenas.comillas(id));
            query.Append(" )");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();
        }

        public void guardarDatos()
        {
            tryNew();

            if (update)
                updateAlbum();

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" update album set olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(olimpiada));
            query.Append(", clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(", orden = ");
            query.Append(orden);
            query.Append(", nombre = ");
            query.Append(Utilities.Cadenas.comillas(nombre));
            query.Append(", fotos = ");
            query.Append(fotos);
            query.Append(", portada = ");
            query.Append(Utilities.Cadenas.comillas(portada));
            query.Append(" where id = ");
            query.Append(Utilities.Cadenas.comillas(id));

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();
        }

        private void updateAlbum()
        {
        }
    }
}