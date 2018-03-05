using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMIstats.Models;

namespace OmegaUpPuller.WebRequest
{
    public class Scoreboard
    {
        private Dictionary<string, Resultados> resultados = null;
        private TipoOlimpiada tipoOlimpiada;
        private string olimpiada;
        private int dia;
        private int problemas;

        public Scoreboard(string olimpiada, TipoOlimpiada tipoOlimpiada, int dia, int problemas)
        {
            this.olimpiada = olimpiada;
            this.tipoOlimpiada = tipoOlimpiada;
            this.dia = dia;
            this.problemas = problemas;
        }

        public void actualiza(string clave, float[] resultados)
        {
        }

        public void ordena()
        {
        }

        public void guarda()
        {
        }
    }
}
