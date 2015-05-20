using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OMIstats.Utilities
{
    public class Fechas
    {
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
    }
}