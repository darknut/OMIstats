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

        public static string guardaImagen(HttpPostedFileBase imagen, string nombre, FolderImagenes folder)
        {
            ////var fileName = Path.GetFileName(file.FileName);
            ////var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
            ////file.SaveAs(path);
            return "";
        }
    }
}