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
        public static readonly int TamañoMaximo = 1024 * 300;

        public const string FOLDER_TEMPORAL = "~/img/temp";

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
            USUARIOS
        }

        public static ResultadoImagen esImagenValida(HttpPostedFileBase imagen)
        {
            if (imagen == null || String.IsNullOrEmpty(imagen.FileName))
                return ResultadoImagen.IMAGEN_INVALIDA;

            string extension = Path.GetExtension(imagen.FileName).Trim();

            if (extension.Length < 2)
                return ResultadoImagen.IMAGEN_INVALIDA;

            if (!ExtensionesValidas.Contains(extension.ToLower()))
                return ResultadoImagen.IMAGEN_INVALIDA;

            if (imagen.ContentLength > TamañoMaximo)
                return ResultadoImagen.IMAGEN_MUY_GRANDE;

            return ResultadoImagen.VALIDA;
        }

        /// <summary>
        /// Guarda la imagen en disco y devuelve el nombre del archivo
        /// </summary>
        /// <param name="imagen">La imagen mandada como http request</param>
        /// <param name="nombre">El nombre opcional de la imagen</param>
        /// <param name="folder">El folder donde colocar la imagen</param>
        /// <returns>El path relativo de la imagen</returns>
        public static string guardaImagen(HttpPostedFileBase imagen, string nombre = "", FolderImagenes folder = FolderImagenes.TEMPORAL)
        {
            string extension = Path.GetExtension(imagen.FileName);
            string lugarEnDisco = "";
            switch (folder)
            {
                case FolderImagenes.TEMPORAL:
                    lugarEnDisco = FOLDER_TEMPORAL;
                    break;
            }
            lugarEnDisco = HttpContext.Current.Server.MapPath(lugarEnDisco);

            if (String.IsNullOrEmpty(nombre))
                do
                {
                    nombre = Guid.NewGuid().ToString() + extension;
                } while (File.Exists(Path.Combine(lugarEnDisco, nombre)));

            imagen.SaveAs(Path.Combine(lugarEnDisco, nombre));

            return nombre;
        }
    }
}