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
            participaciones = p.consultarParticipaciones();

            if (medalleros.Count == 0 && estados.Count == 0 && participaciones.Count == 0)
            {
                // En este caso, estamos tratando con un delegado que no ha ido a olimpiadas o un zombie
                Estado estado = Estado.obtenerEstadoDeDelegado(p.clave);
                if (estado != null)
                {
                    participaciones.Add(MiembroDelegacion.TipoAsistente.DELEGADO.ToString());
                    estados.Add(estado.clave);
                }
            }
        }
    }
}