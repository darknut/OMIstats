using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace OMIstats.Utilities
{
    public class Archivos
    {
        public static readonly List<string> ExtensionesValidas = new List<string> { ".bmp", ".jpg", ".jpeg", ".gif", ".png" };
        public static readonly List<string> ExtensionesContenedoras = new List<string> { ".docx", ".pdf" };

        public const string FOLDER_TEMPORAL = "~/img/temp";
        public const string FOLDER_USUARIOS = "~/img/user";
        public const string FOLDER_ESTADOS = "~/img/estados";
        public const string FOLDER_OLIMPIADAS = "~/img/omi";
        public const string FOLDER_POSTERS = "~/img/posters";

        public const string HTML_PASSWORD = "~/private/cambioPassword.html";
        public const string HTML_BIENVENIDO = "~/private/bienvenido.html";
        public const string HTML_FIRMA = "~/private/firma.html";

        public const string FOTO_KAREL = "\\img\\karel.bmp";
        public const string FOTO_DOMI = "\\img\\domi.gif";
        public const string FOTO_OMI = "\\img\\omi.png";

        public enum ResultadoImagen
        {
            VALIDA = 0,
            IMAGEN_INVALIDA,
            IMAGEN_MUY_GRANDE
        }

        public enum FolderImagenes
        {
            TEMPORAL,
            ESTADOS,
            USUARIOS,
            OLIMPIADAS,
            POSTERS
        }

        public enum ArchivosHTML
        {
            PASSWORD,
            BIENVENIDO,
            FIRMA
        }

        public enum FotoInicial
        {
            KAREL,
            DOMI,
            OMI
        }

        /// <summary>
        /// Regresa si la imagen es válida o no.
        /// </summary>
        /// <param name="allowContainer">Si la imagen se permite estar contenida en un docx o pdf</param>
        public static ResultadoImagen esImagenValida(HttpPostedFileBase imagen, int tamaño = -1, bool allowContainer = false)
        {
            if (imagen == null || String.IsNullOrEmpty(imagen.FileName))
                return ResultadoImagen.IMAGEN_INVALIDA;

            string extension = Path.GetExtension(imagen.FileName).Trim();

            if (extension.Length < 2)
                return ResultadoImagen.IMAGEN_INVALIDA;

            if (allowContainer)
            {
                if (!ExtensionesValidas.Contains(extension.ToLower()) && !ExtensionesContenedoras.Contains(extension.ToLower()))
                    return ResultadoImagen.IMAGEN_INVALIDA;
            }
            else
            {
                if (!ExtensionesValidas.Contains(extension.ToLower()))
                    return ResultadoImagen.IMAGEN_INVALIDA;
            }

            if (tamaño != -1 && imagen.ContentLength > tamaño)
                return ResultadoImagen.IMAGEN_MUY_GRANDE;

            return ResultadoImagen.VALIDA;
        }

        private static string pathRelativo(FolderImagenes folder)
        {
            string s = "";
            switch (folder)
            {
                case FolderImagenes.TEMPORAL:
                    s = FOLDER_TEMPORAL;
                    break;
                case FolderImagenes.USUARIOS:
                    s = FOLDER_USUARIOS;
                    break;
                case FolderImagenes.ESTADOS:
                    s = FOLDER_ESTADOS;
                    break;
                case FolderImagenes.OLIMPIADAS:
                    s = FOLDER_OLIMPIADAS;
                    break;
                case FolderImagenes.POSTERS:
                    s = FOLDER_POSTERS;
                    break;
            }

            return s;
        }

        private static string pathAbsoluto(FolderImagenes folder)
        {
            string s = pathRelativo(folder);
            s = HttpContext.Current.Server.MapPath(s);

            return s;
        }

        /// <summary>
        /// Guarda la imagen en disco y devuelve el nombre del archivo
        /// </summary>
        /// <param name="archivo">La imagen mandada como http request</param>
        /// <param name="nombre">El nombre opcional de la imagen</param>
        /// <param name="folder">El folder donde colocar la imagen</param>
        /// <returns>El path relativo de la imagen</returns>
        public static string guardaArchivo(HttpPostedFileBase archivo, string nombre = "", FolderImagenes folder = FolderImagenes.TEMPORAL)
        {
            string extension = Path.GetExtension(archivo.FileName);
            string lugarEnDisco = pathAbsoluto(folder);

            if (String.IsNullOrEmpty(nombre))
                do
                {
                    nombre = Guid.NewGuid().ToString() + extension;
                } while (File.Exists(Path.Combine(lugarEnDisco, nombre)));

            archivo.SaveAs(Path.Combine(lugarEnDisco, nombre));

            return nombre;
        }

        /// <summary>
        /// Borra el archivo del disco
        /// </summary>
        public static void eliminarArchivo(string nombre, FolderImagenes folder)
        {
            string lugarEnDisco = pathAbsoluto(folder);
            if (File.Exists(Path.Combine(lugarEnDisco, nombre)))
                File.Delete(Path.Combine(lugarEnDisco, nombre));
        }

        /// <summary>
        /// Copia un archivo
        /// </summary>
        /// <returns>El path relativo al archivo destino</returns>
        public static string copiarArchivo(string nombreOrigen, FolderImagenes folderOrigen, string nombreDestino, FolderImagenes folderDestino)
        {
            string lugarOrigen = pathAbsoluto(folderOrigen);
            string lugarDestino = pathAbsoluto(folderDestino);

            if (Path.GetExtension(nombreDestino).Length < 2)
                nombreDestino += Path.GetExtension(nombreOrigen);

            File.Copy(Path.Combine(lugarOrigen, nombreOrigen),
                Path.Combine(lugarDestino, nombreDestino), overwrite:true);

            return Path.Combine(pathRelativo(folderDestino), nombreDestino);
        }

        public static string leerArchivoHTML(ArchivosHTML archivo)
        {
            string a = "";
            switch (archivo)
            {
                case ArchivosHTML.PASSWORD:
                    a = HTML_PASSWORD;
                    break;
                case ArchivosHTML.BIENVENIDO:
                    a = HTML_BIENVENIDO;
                    break;
                case ArchivosHTML.FIRMA:
                    a = HTML_FIRMA;
                    break;
            }
            a = HttpContext.Current.Server.MapPath(a);

            return File.ReadAllText(a);
        }

        public static string obtenerFotoInicial(FotoInicial foto)
        {
            switch (foto)
            {
                case FotoInicial.KAREL:
                    return FOTO_KAREL;
                case FotoInicial.DOMI:
                    return FOTO_DOMI;
                case FotoInicial.OMI:
                    return FOTO_OMI;
            }

            return "";
        }

        public static bool existeArchivo(FolderImagenes folder, string imagen)
        {
            string path = pathAbsoluto(folder);
            return File.Exists(Path.Combine(path, imagen));
        }
    }
}