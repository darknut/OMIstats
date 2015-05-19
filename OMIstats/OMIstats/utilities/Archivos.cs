using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OMIstats.Utilities
{
    public class Archivos
    {
        public static readonly List<string> ExtensionesValidas = new List<string> { "bmp", "jpg", "jpeg", "gif", "png" };
        public static readonly int TamañoMaximo = 1024 * 300;

        public enum ResultadoImagen
        {
            VALIDA = 0,
            INVALIDA = 1,
            BYTES = 2
        }

        public enum FolderImagenes
        {
            TEMPORAL,
            ESTADOS,
            USUARIOS
        }

        public static ResultadoImagen esImagenValida(HttpPostedFileBase imagen)
        {
            string[] nombre = imagen.FileName.Split('.');

            if (nombre.Length < 2)
                return ResultadoImagen.INVALIDA;

            if (!ExtensionesValidas.Contains(nombre[1].Trim().ToLower()))
                return ResultadoImagen.INVALIDA;

            if (imagen.ContentLength > TamañoMaximo)
                return ResultadoImagen.BYTES;

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