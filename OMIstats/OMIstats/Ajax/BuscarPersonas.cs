using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OMIstats.Models;

namespace OMIstats.Ajax
{
    public class BuscarPersonas
    {
        public string nombre;
        public int clave;

        public BuscarPersonas(Persona persona)
        {
            nombre = persona.nombre;
            clave = persona.clave;
        }
    }
}