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
        private int concursantes;

        public Scoreboard(string olimpiada, TipoOlimpiada tipoOlimpiada, int dia, int problemas)
        {
            this.olimpiada = olimpiada;
            this.tipoOlimpiada = tipoOlimpiada;
            this.dia = dia;
            this.problemas = problemas;
            this.concursantes = 0;

            inicializaResultados();
        }

        private void inicializaResultados()
        {
            resultados = new Dictionary<string, Resultados>();
            List<Resultados> r = Resultados.cargarResultados(olimpiada, tipoOlimpiada, cargarObjetos: false);
            foreach (var resultado in r)
            {
                resultados.Add(resultado.clave, resultado);
            }
        }

        public void actualiza(string clave, decimal?[] resultados)
        {
            Resultados res;
            if (!this.resultados.TryGetValue(clave, out res))
            {
                List<MiembroDelegacion> miembros = MiembroDelegacion.obtenerMiembrosConClave(this.olimpiada, this.tipoOlimpiada, clave);
                if (miembros.Count != 1)
                {
                    this.resultados.Add(clave, null);
                    return;
                }

                res = new Resultados();
                res.tipoOlimpiada = this.tipoOlimpiada;
                res.omi = this.olimpiada;
                res.usuario = miembros[0].claveUsuario;
                res.clave = clave;
                res.publico = true;
                concursantes++;
                this.resultados.Add(clave, res);
            }

            if (res == null)
                return;

            List<float?> arreglo;
            if (dia == 1)
                arreglo = res.dia1;
            else
                arreglo = res.dia2;

            float? total = 0;
            for (int i = 0; i < this.problemas; i++)
            {
                arreglo[i] = (float?)resultados[i];

                if(arreglo[i] != null)
                    total += arreglo[i];
            }

            if (dia == 1)
                res.totalDia1 = total;
            else
                res.totalDia2 = total;

            res.total = res.totalDia1 + res.totalDia2;
        }

        public static int compara(Resultados x, Resultados y)
        {
            float x1 = 0, y1 = 0;

            if (x == null)
                x1 = -1;
            else
                x1 = (float)x.total;

            if (y == null)
                y1 = -1;
            else
                y1 = (float) y.total;

            return y1.CompareTo(x1);
        }

        public void ordena()
        {
            List<Resultados> list = this.resultados.Values.ToList();

            list.Sort(compara);
        }

        public void guarda()
        {
        }
    }
}
