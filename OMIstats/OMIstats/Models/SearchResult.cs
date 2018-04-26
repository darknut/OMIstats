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
        public List<string> estados;
        public List<string> participaciones;

        public SearchResult(Persona p)
        {
            persona = p;

            Medalleros ms = Medallero.obtenerMedalleros(Medallero.TipoMedallero.PERSONA, p.clave.ToString());
            medalleros = new Dictionary<TipoOlimpiada, Medallero>();
            foreach (TipoOlimpiada tipo in Enum.GetValues(typeof(TipoOlimpiada)))
            {
                Medallero m = ms.medalleroDeTipo(tipo);
                if (m != null)
                    medalleros.Add(tipo, m);
            }
            estados = p.consultarEstados();
        }
    }
}