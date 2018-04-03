using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OMIstats.Utilities
{
    public class Fechas
    {
        private static readonly DateTime UNIX_TIME = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static string friendlyString(DateTime fecha)
        {
            string s = fecha.Day + " de ";
            switch (fecha.Month)
            {
                case 1:
                    s += "enero";
                    break;
                case 2:
                    s += "febero";
                    break;
                case 3:
                    s += "marzo";
                    break;
                case 4:
                    s += "abril";
                    break;
                case 5:
                    s += "mayo";
                    break;
                case 6:
                    s += "junio";
                    break;
                case 7:
                    s += "julio";
                    break;
                case 8:
                    s += "agosto";
                    break;
                case 9:
                    s += "septiembre";
                    break;
                case 10:
                    s += "octubre";
                    break;
                case 11:
                    s += "noviembre";
                    break;
                case 12:
                    s += "diciembre";
                    break;
            }

            return s;
        }

        public static DateTime stringToDate(string fecha)
        {
            if (String.IsNullOrEmpty(fecha))
                return new DateTime(1900, 1, 1);

            int dia, mes, año;
            string[] numeros = fecha.Split('/');
            Int32.TryParse(numeros[0], out dia);
            Int32.TryParse(numeros[1], out mes);
            Int32.TryParse(numeros[2], out año);

            return new DateTime(año, mes, dia);
        }

        public static string dateToString(DateTime fecha)
        {
            if (fecha == null)
                return "01/01/1900";

            return fecha.Day.ToString("00") + "/" +
                   fecha.Month.ToString("00") + "/" +
                   fecha.Year.ToString();
        }

        public static DateTime FromUnixTime(long seconds)
        {
            return UNIX_TIME.AddSeconds(seconds);
        }
    }
}