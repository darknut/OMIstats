using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OMIstats.Models
{   
    /// <summary>
    /// Clase para comunicarse con la base de datos de la OMI
    /// </summary>
    public class Usuario
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string Foto { get; set; }

        public string Nombre { get; set; }

        public string CURP { get; set; }
    }
}