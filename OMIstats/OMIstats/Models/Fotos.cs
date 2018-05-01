using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;


namespace OMIstats.Models
{
    public class Album
    {
        public static string ACCESS_TOKEN;
        public const string BASE_FACEBOOK_URL = "https://graph.facebook.com/{1}{2}?access_token={0}{3}";
        public const string ALBUM_METADATA = "&fields=cover_photo,count,name";
        public const string FOTOS_METADATA = "&fields=images";
        public const string NOMBRE_ALBUM = "name";
        public const string COUNT_ALBUM = "count";
        public const string PORTADA_ALBUM = "cover_photo";
        public const string ID = "id";
        public const string IMAGES = "images";
        public const int THUMBNAIL_SIZE = 225;
        public const string SIZE = "height";
        public const string URL = "source";

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

        private Dictionary<string, object> call(string api)
        {
            HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(api);
            request.Method = "GET";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                string content = string.Empty;
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        content = sr.ReadToEnd();
                    }
                }

                return new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(content);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string obtenerFoto(Dictionary<string, object> foto)
        {
            ArrayList images = (ArrayList)foto[IMAGES];
            int bestSize = 0;
            string bestURL = "";

            foreach (Dictionary<string, object> sizeObj in images)
            {
                int size = (int)sizeObj[SIZE];
                if (size >= THUMBNAIL_SIZE)
                {
                    if (bestSize < size)
                    {
                        bestSize = size;
                        bestURL = (string)sizeObj[URL];
                    }
                }
            }

            return bestURL;
        }

        private void updateAlbum()
        {
            // Sacamos el metadata del album
            Dictionary<string, object> response = call(String.Format(BASE_FACEBOOK_URL, ACCESS_TOKEN, id, "", ALBUM_METADATA));
            nombre = (string)response[NOMBRE_ALBUM];
            fotos = (int)response[COUNT_ALBUM];
            string coverId = (string)((Dictionary<string, object>)response[PORTADA_ALBUM])[ID];

            // Luego sacamos el metadata de la portada
            response = call(String.Format(BASE_FACEBOOK_URL, ACCESS_TOKEN, coverId, "", FOTOS_METADATA));
            portada = obtenerFoto(response);

            // Finalmente, obtenemos las fotos del album
        }
    }
}