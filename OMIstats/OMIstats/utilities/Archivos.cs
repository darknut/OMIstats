using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.IO.Compression;
using System.Text;

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
        public const string FOLDER_ESCUELAS = "~/img/escuelas";
        public const string FOLDER_NEWSLETTERS = "~/img/news";
        public const string FOLDER_DIPLOMAS = "~/private/diplomas";
        public const string FOLDER_INVITACIONES = "~/private/invitaciones";
        public const string FOLDER_RETO = "~/private/reto";
        public const string FOLDER_DEFAULT = "~/img";

        public const string OMI_LOGO = "omi.png";

        public const string FOTO_KAREL = "~/img/karel.bmp";
        public const string FOTO_DOMI = "~/img/domi.gif";
        public const string FOTO_OMI = "~/img/" + OMI_LOGO;

        private static byte[] BOM = {239, 187, 191};

        public enum ResultadoImagen
        {
            VALIDA = 0,
            IMAGEN_INVALIDA,
            IMAGEN_MUY_GRANDE
        }

        public enum Folder
        {
            TEMPORAL,
            ESTADOS,
            USUARIOS,
            OLIMPIADAS,
            POSTERS,
            ESCUELAS,
            NEWSLETTERS,
            DIPLOMAS,
            INVITACIONES,
            RETO
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

        private static string pathRelativo(Folder folder)
        {
            switch (folder)
            {
                case Folder.TEMPORAL:
                    return FOLDER_TEMPORAL;
                case Folder.USUARIOS:
                    return FOLDER_USUARIOS;
                case Folder.ESTADOS:
                    return FOLDER_ESTADOS;
                case Folder.OLIMPIADAS:
                    return FOLDER_OLIMPIADAS;
                case Folder.POSTERS:
                    return FOLDER_POSTERS;
                case Folder.ESCUELAS:
                    return FOLDER_ESCUELAS;
                case Folder.NEWSLETTERS:
                    return FOLDER_NEWSLETTERS;
                case Folder.DIPLOMAS:
                    return FOLDER_DIPLOMAS;
                case Folder.INVITACIONES:
                    return FOLDER_INVITACIONES;
                case Folder.RETO:
                    return FOLDER_RETO;
                default:
                    return FOLDER_DEFAULT;
            }
        }

        private static string pathAbsoluto(Folder folder)
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
        public static string guardaArchivo(HttpPostedFileBase archivo, string nombre = "", Folder folder = Folder.TEMPORAL, bool appendExtension = false, bool returnRelativeFolder = false)
        {
            string extension = Path.GetExtension(archivo.FileName);
            string lugarEnDisco = pathAbsoluto(folder);

            if (String.IsNullOrEmpty(nombre))
                nombre = Guid.NewGuid().ToString() + extension;
            if (appendExtension)
                nombre += extension;

            if (!Directory.Exists(lugarEnDisco))
                Directory.CreateDirectory(lugarEnDisco);

            try
            {
                archivo.SaveAs(Path.Combine(lugarEnDisco, nombre));
            }
            catch (Exception e)
            {
                Models.Log.add(Models.Log.TipoLog.EXCEPTIONS, e.ToString());
                throw e;
            }

            if (returnRelativeFolder)
                return Path.Combine(pathRelativo(folder), nombre);

            return nombre;
        }

        /// <summary>
        /// Borra el archivo del disco
        /// </summary>
        public static void eliminarArchivo(string nombre, Folder folder)
        {
            string lugarEnDisco = pathAbsoluto(folder);
            if (File.Exists(Path.Combine(lugarEnDisco, nombre)))
            {
                try
                {
                    File.Delete(Path.Combine(lugarEnDisco, nombre));
                }
                catch (Exception e)
                {
                    Models.Log.add(Models.Log.TipoLog.EXCEPTIONS, e.ToString());
                    throw e;
                }
            }
        }

        /// <summary>
        /// Copia un archivo
        /// </summary>
        /// <returns>El path relativo al archivo destino</returns>
        public static string copiarArchivo(string nombreOrigen, Folder folderOrigen, string nombreDestino, Folder folderDestino)
        {
            string lugarOrigen = pathAbsoluto(folderOrigen);
            string lugarDestino = pathAbsoluto(folderDestino);

            if (Path.GetExtension(nombreDestino).Length < 2)
                nombreDestino += Path.GetExtension(nombreOrigen);

            try
            {
                File.Copy(Path.Combine(lugarOrigen, nombreOrigen),
                    Path.Combine(lugarDestino, nombreDestino), overwrite: true);
            }
            catch (Exception e)
            {
                Models.Log.add(Models.Log.TipoLog.EXCEPTIONS, e.ToString());
                throw e;
            }

            return Path.Combine(pathRelativo(folderDestino), nombreDestino);
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

        public static bool existeArchivo(Folder folder, string imagen)
        {
            string path = pathAbsoluto(folder);
            return File.Exists(Path.Combine(path, imagen));
        }

        public static int cuantosExisten(Folder folder, string subfolder, string nombre)
        {
            try
            {
                string path = pathAbsoluto(folder);
                var archivos = Directory.GetFiles(Path.Combine(path, subfolder), nombre + "*");
                return archivos.Count();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static byte[] comprimeArchivos(Folder folder, string subfolder, string nombre = null)
        {
            string path = pathAbsoluto(folder);
            var archivos = Directory.GetFiles(Path.Combine(path, subfolder), nombre == null ? "*" : nombre + "*");
            byte[] bytes = null;

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var archivo in archivos)
                    {
                        var nombreArchivo = archivo.Split('\\').Last();
                        // No incluimos los diplomas del comi en los zip
                        if (nombreArchivo.StartsWith("COMI") || nombreArchivo.StartsWith("COLO"))
                            continue;
                        var file = archive.CreateEntry(nombreArchivo);

                        using (var outputStream = file.Open())
                        {
                            using (var streamWriter = new BinaryWriter(outputStream))
                            {
                                streamWriter.Write(File.ReadAllBytes(archivo));
                            }
                        }
                    }
                }

                bytes = memoryStream.ToArray();
            }

            return bytes;
        }

        public static byte[] creaArchivoTexto(string texto)
        {
            List<byte> list = new List<byte>();
            list.AddRange(BOM);
            list.AddRange(Encoding.UTF8.GetBytes(texto));
            return list.ToArray();
        }
    }
}