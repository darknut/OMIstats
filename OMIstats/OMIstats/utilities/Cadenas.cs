using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace OMIstats.Utilities
{
    public class Cadenas
    {
        public static string comillas(string cadena)
        {
            if (String.IsNullOrEmpty(cadena))
                return "''";
            return "\'" + cadena.Replace("\'", "\'\'") + "\'";
        }

        /// <summary>
        /// Quita los caracteres especiales del string mandado como parámetro
        /// dejando solo letras minusculas y un espacio entre cada palabra
        /// </summary>
        public static string quitaEspeciales(string cadena)
        {
            if (String.IsNullOrEmpty(cadena))
                return "";

            StringBuilder s = new StringBuilder(cadena);

            s.Replace('á', 'a');
            s.Replace('é', 'e');
            s.Replace('í', 'i');
            s.Replace('ó', 'o');
            s.Replace('ú', 'u');
            s.Replace('Á', 'a');
            s.Replace('É', 'e');
            s.Replace('Í', 'i');
            s.Replace('Ó', 'o');
            s.Replace('Ú', 'u');
            s.Replace('ä', 'a');
            s.Replace('ë', 'e');
            s.Replace('ï', 'i');
            s.Replace('ö', 'o');
            s.Replace('ü', 'u');
            s.Replace('ñ', 'n');
            s.Replace('Ñ', 'n');
            s.Replace('\'', ' ');
            s.Replace('-', ' ');
            s.Replace('.', ' ');
            s.Replace('#', ' ');

            string r = s.ToString();

            while (r.IndexOf("  ") >= 0)
                r = r.Replace("  ", " ");

            return r.Trim().ToLower();
        }

        public static string quitaEspacios(string cadena)
        {
            if (String.IsNullOrEmpty(cadena))
                return "";

            while (cadena.IndexOf(" ") >= 0)
                cadena = cadena.Replace(" ", "");

            return cadena.Trim().ToLower();
        }
    }
}