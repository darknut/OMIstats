using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OMIstats.Models
{
    public class SearchResult
    {
        public Dictionary<TipoOlimpiada, Medallero> medalleros;
        public Persona persona;
        public HashSet<string> estados;
        public HashSet<string> participaciones;

        public SearchResult(Persona p)
        {
            persona = p;
        }
    }
}