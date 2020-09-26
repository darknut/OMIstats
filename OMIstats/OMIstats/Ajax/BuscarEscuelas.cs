using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using OMIstats.Utilities;

namespace OMIstats.Ajax
{
    public class BuscarEscuelas
    {
        public int clave;
        public string nombre;

        public void llenarDatos(DataRow datos)
        {
            clave = DataRowParser.ToInt(datos["clave"]);
            nombre = DataRowParser.ToString(datos["nombre"]);
        }
    }
}
