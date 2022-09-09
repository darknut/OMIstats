using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace OMIstats.Utilities
{
    public class Cadenas
    {
        public static string comillas(string cadena, string comilla = "\'")
        {
            if (String.IsNullOrEmpty(cadena))
                return comilla + comilla;
            return comilla + cadena.Replace(comilla, comilla + comilla) + comilla;
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

            return quitaEspacioDoble(s.ToString()).Trim().ToLower();
        }

        public static string quitaEspacioDoble(string cadena)
        {
            if (cadena == null)
                return "";
            while (cadena.IndexOf("  ") >= 0)
                cadena = cadena.Replace("  ", " ");
            return cadena;
        }

        public static string quitaEspacios(string cadena)
        {
            if (String.IsNullOrEmpty(cadena))
                return "";

            while (cadena.IndexOf(" ") >= 0)
                cadena = cadena.Replace(" ", "");

            return cadena.Trim().ToLower();
        }

        public static bool esCorreo(string correo)
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$");
            return regex.IsMatch(correo);
        }

        public static string toDBString(object str)
        {
            if (str == null || str is DBNull)
                return "null";
            if (str is string)
                return comillas(str.ToString());
            return str.ToString();
        }
    }
}