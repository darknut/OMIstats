using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace OMIstats.Ajax
{
    public class BuscarEscuelas
    {
        public string clave;
        public string nombre;

        public void llenarDatos(DataRow datos)
        {
            clave = datos["clave"].ToString().Trim();
            nombre = datos["nombre"].ToString().Trim();
        }
    }
}
