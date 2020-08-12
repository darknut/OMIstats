using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace OMIstats.Ajax
{
    public class BuscarEscuelas
    {
        public int clave;
        public string nombre;

        public void llenarDatos(DataRow datos)
        {
            clave = (int)datos["clave"];
            nombre = datos["nombre"].ToString().Trim();
        }
    }
}
