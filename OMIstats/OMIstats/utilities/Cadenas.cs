using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OMIstats.Utilities
{
    public class Cadenas
    {
        public static string comillas(string cadena)
        {
            return "\'" + cadena.Replace("\'", "\'\'") + "\'";
        }
    }
}