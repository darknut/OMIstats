using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using OMIstats.Utilities;

namespace OmegaUpPuller
{
    class Program
    {
        private static void Setup()
        {
            Acceso.CADENA_CONEXION = ConfigurationManager.AppSettings["conexion"];
        }

        static void Main(string[] args)
        {
            Acceso db = new Acceso();
        }
    }
}
