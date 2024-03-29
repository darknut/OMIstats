﻿using System;
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
using OMIstats.Utilities;

namespace OMIstats.Models
{
    public class Foto
    {
        public string album { get; set; }
        public string id { get; set; }
        public int orden { get; set; }
        public string imagen { get; set; }
        public string url { get; set; }

        public Foto()
        {
            album = "";
            id = "";
            orden = 0;
            imagen = "";
            url = "";
        }

        private void llenarDatos(DataRow r)
        {
            album = DataRowParser.ToString(r["album"]);
            id = DataRowParser.ToString(r["id"]);
            orden = DataRowParser.ToInt(r["orden"]);
            imagen = DataRowParser.ToString(r["imagen"]);
            url = DataRowParser.ToString(r["url"]);
        }

        /// <summary>
        /// Borra todas las fotos del album mandado como parametro
        /// </summary>
        /// <param name="album">El álbum a borrar</param>
        public static void borrarDeAlbum(string album)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" delete foto where album = ");
            query.Append(Cadenas.comillas(album));

            db.EjecutarQuery(query.ToString());
        }

        /// <summary>
        /// Guarda una nueva foto en la base de datos con los datos en el objeto
        /// </summary>
        public void guardar()
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" insert into foto(album, id, orden, imagen, url) values(");
            query.Append(Cadenas.comillas(album));
            query.Append(", ");
            query.Append(Cadenas.comillas(id));
            query.Append(", ");
            query.Append(orden);
            query.Append(", ");
            query.Append(Cadenas.comillas(imagen));
            query.Append(", ");
            query.Append(Cadenas.comillas(url));
            query.Append(")");

            db.EjecutarQuery(query.ToString());
        }

        public static List<Foto> obtenerFotosDeAlbum(string album)
        {
            List<Foto> fotos = new List<Foto>();
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from foto where album = ");
            query.Append(Cadenas.comillas(album));
            query.Append(" order by orden ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow row in table.Rows)
            {
                Foto f = new Foto();
                f.llenarDatos(row);
                fotos.Add(f);
            }

            return fotos;
        }
    }

    public class Album
    {
        public static string ACCESS_TOKEN;
        public const string BASE_FACEBOOK_URL = "https://graph.facebook.com/{1}{2}?access_token={0}{3}";
        public const string AFTER_METADATA = "&after={0}";
        public const string ALBUM_METADATA = "&fields=cover_photo,count,name";
        public const string ALBUM_FOTOS = "/photos";
        public const string FOTOS_METADATA = "&fields=images,link";
        public const string NOMBRE_ALBUM = "name";
        public const string COUNT_ALBUM = "count";
        public const string PORTADA_ALBUM = "cover_photo";
        public const string ID = "id";
        public const string IMAGES = "images";
        public const int THUMBNAIL_SIZE = 225;
        public const string HEIGHT = "height";
        public const string WIDTH = "width";
        public const string URL = "source";
        public const string DATA = "data";
        public const string PAGING = "paging";
        public const string CURSORS = "cursors";
        public const string AFTER = "after";
        public const string LINK = "link";

        private const string ALBUM_GRAL = "0";
        private const int DAYS_UPDATE = 7;
        private const int MAX_UPDATES = 150;

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

        public bool newsletter { get; set; }

        public DateTime lastUpdated { get; set; }

        private static int currentCalls = 0;

        public Album()
        {
            id = "";
            olimpiada = "";
            tipoOlimpiada = TipoOlimpiada.NULL;
            orden = 0;
            nombre = "";
            fotos = 0;
            portada = "";
            newsletter = false;
            lastUpdated = new DateTime(1900, 1, 1);
        }

        private void llenarDatos(DataRow r)
        {
            id = DataRowParser.ToString(r["id"]);
            lastUpdated = DataRowParser.ToDateTime(r["lastUpdated"]);
            orden = DataRowParser.ToInt(r["orden"]);

            // Los datos generales no guardan nada mas
            if (id == ALBUM_GRAL)
            {
                return;
            }

            olimpiada = DataRowParser.ToString(r["olimpiada"]);
            tipoOlimpiada = DataRowParser.ToTipoOlimpiada(r["clase"]);
            fotos = DataRowParser.ToInt(r["fotos"]);
            nombre = DataRowParser.ToString(r["nombre"]);
            portada = DataRowParser.ToString(r["portada"]);
            newsletter = DataRowParser.ToBool(r["newsletter"]);
        }

        /// <summary>
        /// Obtiene una lista de todas las olimpiadas con albumes de fotos
        /// </summary>
        /// <param name="tipo">El tipo de olimpiada</param>
        /// <returns>La lista de olimpiadas</returns>
        public static HashSet<string> obtenerOlimpiadasConAlbumes(TipoOlimpiada tipo = TipoOlimpiada.OMI)
        {
            HashSet<string> olimpiadas = new HashSet<string>();
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select distinct(olimpiada) from album where clase = ");
            query.Append(Cadenas.comillas(tipo.ToString().ToLower()));

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow row in table.Rows)
            {
                olimpiadas.Add(DataRowParser.ToString(row[0]));
            }

            return olimpiadas;
        }

        /// <summary>
        /// Obtiene el album con la id especificada
        /// </summary>
        /// <param name="id">La id del album</param>
        /// <returns>El objeto album requerido</returns>
        public static Album obtenerAlbum(string id)
        {
            Album al = new Album();

            if (String.IsNullOrEmpty(id))
                return al;

            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from album where id = ");
            query.Append(Cadenas.comillas(id));

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            if (table.Rows.Count == 0)
                return al;

            al.llenarDatos(table.Rows[0]);

            // Si ya tiene más de una semana que
            // cacheamos el album, lo refrescamos
            if (id != ALBUM_GRAL && puedeActualizar() && al.lastUpdated.AddDays(DAYS_UPDATE) < DateTime.Today && !al.newsletter)
            {
                Log.add(Log.TipoLog.FACEBOOK, "Álbum " + al.id + " actualizado por inactividad");
                al.updateAlbum();
                al.guardarDatos();
            }

            return al;
        }

        private static bool puedeActualizar()
        {
            if (currentCalls > 0)
                return false;

            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select orden, lastUpdated from album where id = '0' ");
            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            try
            {
                int count = (int)table.Rows[0]["orden"];
                DateTime lastUpdated = Fechas.stringToDate(table.Rows[0]["lastUpdated"].ToString().Trim());

                return (lastUpdated < DateTime.Today || count < MAX_UPDATES);
            }
            catch (Exception)
            {
                // El valor no está en la base de datos, lo agregamos
                query.Append(" insert into Album (id, orden, lastUpdated) values ('0', 0, ");
                query.Append(Cadenas.comillas(Fechas.dateToString(DateTime.Today)));
                query.Append(" )");
                db.EjecutarQuery(query.ToString());
            }
            return true;
        }

        /// <summary>
        /// Obtiene todos los albumes de la olimpiada
        /// </summary>
        /// <param name="olimpiada">La olimpiada de la que se quieren los albumes</param>
        /// <param name="tipo">El tipo de olipiada</param>
        /// <returns>La lista de álbumes</returns>
        public static List<Album> obtenerAlbumsDeOlimpiada(string olimpiada, TipoOlimpiada tipo)
        {
            List<Album> albums = new List<Album>();

            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from album where olimpiada = ");
            query.Append(Cadenas.comillas(olimpiada));
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipo.ToString().ToLower()));
            query.Append(" order by orden ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow row in table.Rows)
            {
                Album al = new Album();
                al.llenarDatos(row);
                albums.Add(al);
            }

            return albums;
        }

        private void tryNew()
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" insert into album (id) values( ");
            query.Append(Cadenas.comillas(id));
            query.Append(" )");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();
        }

        public void guardarDatos()
        {
            tryNew();

            if (!newsletter && update)
                updateAlbum();

            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" update album set lastUpdated = ");
            query.Append(Cadenas.comillas(Fechas.dateToString(lastUpdated)));
            query.Append(", orden = ");
            query.Append(orden);
            if (id != ALBUM_GRAL)
            {
                query.Append(", olimpiada = ");
                query.Append(Cadenas.comillas(olimpiada));
                query.Append(", clase = ");
                query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
                query.Append(", nombre = ");
                query.Append(Cadenas.comillas(nombre));
                query.Append(", fotos = ");
                query.Append(fotos);
                query.Append(", portada = ");
                query.Append(Cadenas.comillas(portada));
                query.Append(", newsletter = ");
                query.Append(Cadenas.boolToInt(newsletter));
            }
            query.Append(" where id = ");
            query.Append(Cadenas.comillas(id));

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();
        }

        private Dictionary<string, object> call(string api)
        {
            HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(api);
            request.Method = "GET";
            request.UserAgent = "Other";
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.UseDefaultCredentials = true;
            request.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            request.Referer = "https://www.olimpiadadeinformatica.org.mx/Resultados";

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                currentCalls++;

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
            catch (Exception e)
            {
                if (e is WebException)
                {
                    var ex = (WebException)e;
                    var response = ex.Response;
                    var dataStream = response.GetResponseStream();
                    var reader = new StreamReader(dataStream);
                    var details = reader.ReadToEnd();
                    Models.Log.add(Log.TipoLog.FACEBOOK, details);
                }
                else
                    Models.Log.add(Log.TipoLog.FACEBOOK, e.ToString());
                return null;
            }
        }

        private string obtenerFoto(Dictionary<string, object> foto, bool ignorarVerticales = false)
        {
            ArrayList images = (ArrayList)foto[IMAGES];
            int bestSize = 0;
            string bestURL = "";
            string fallbackURL = "";

            foreach (Dictionary<string, object> sizeObj in images)
            {
                int height = (int)sizeObj[HEIGHT];
                int width = (int)sizeObj[WIDTH];

                if (ignorarVerticales && (height > width || width * 0.6 > height))
                    continue;

                if (height >= THUMBNAIL_SIZE)
                {
                    if (height < bestSize || bestSize == 0)
                    {
                        bestSize = height;
                        bestURL = (string)sizeObj[URL];
                    }
                }

                fallbackURL = (string)sizeObj[URL];
            }

            if (bestURL == "")
                return fallbackURL;

            return bestURL;
        }

        private void updateAlbum()
        {
            string fallbackPortada = "";
            try
            {
                currentCalls = 1; // Bloqueamos a otros procesos con esta bandera

                // Sacamos el metadata del album
                Dictionary<string, object> response = call(String.Format(BASE_FACEBOOK_URL, ACCESS_TOKEN, id, "", ALBUM_METADATA));
                nombre = (string)response[NOMBRE_ALBUM];
                fotos = (int)response[COUNT_ALBUM];
                string coverId = (string)((Dictionary<string, object>)response[PORTADA_ALBUM])[ID];

                // Luego sacamos el metadata de la portada
                response = call(String.Format(BASE_FACEBOOK_URL, ACCESS_TOKEN, coverId, "", FOTOS_METADATA));
                portada = obtenerFoto(response, ignorarVerticales: true);

                // Borramos las fotos del album
                Foto.borrarDeAlbum(id);

                string after = "";
                int ordenFoto = 0;
                // Finalmente, obtenemos las fotos del album
                while (true)
                {
                    string url = String.Format(BASE_FACEBOOK_URL, ACCESS_TOKEN, id, ALBUM_FOTOS, FOTOS_METADATA);
                    if (after.Length > 0)
                        url += String.Format(AFTER_METADATA, after);
                    response = call(url);

                    ArrayList data = (ArrayList)response[DATA];
                    if (data.Count == 0)
                        break;

                    // Se agregan las fotos
                    foreach (Dictionary<string, object> foto in data)
                    {
                        Foto f = new Foto();
                        f.album = id;
                        f.id = (string)foto[ID];
                        f.orden = ++ordenFoto;
                        f.url = (string)foto[LINK];
                        f.imagen = obtenerFoto(foto);

                        if (fallbackPortada.Length == 0)
                            fallbackPortada = f.imagen;

                        if (portada.Length == 0)
                            portada = obtenerFoto(foto, ignorarVerticales: true);

                        f.guardar();
                    }

                    Dictionary<string, object> paging = (Dictionary<string, object>)response[PAGING];
                    Dictionary<string, object> cursors = (Dictionary<string, object>)paging[CURSORS];
                    after = (string)cursors[AFTER];
                }
            }
            catch (Exception e)
            {
                Log.add(Log.TipoLog.FACEBOOK, e.ToString());
            }
            finally
            {
                if (portada != null)
                {
                    if (portada.Length == 0)
                        portada = fallbackPortada;

                    // Actualizamos las actualizaciones en general
                    Album al = Album.obtenerAlbum(ALBUM_GRAL);

                    if (al.lastUpdated < DateTime.Today)
                    {
                        al.lastUpdated = DateTime.Today;
                        al.orden = currentCalls;
                    }
                    else
                    {
                        al.orden += currentCalls;
                    }

                    // Le quitamos 1 del extra que le pusimos al principio
                    al.orden--;

                    // Actualizamos los calls en el sistema
                    al.guardarDatos();

                    // También actualizamos la fecha del album que estamos actualizando
                    lastUpdated = DateTime.Today;
                    currentCalls = 0;
                }
            }
        }
    }
}