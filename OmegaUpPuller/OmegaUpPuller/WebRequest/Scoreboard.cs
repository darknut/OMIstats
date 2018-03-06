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

        private static Resultados.TipoMedalla[] medallas = new Resultados.TipoMedalla[] {
                Resultados.TipoMedalla.ORO,
                Resultados.TipoMedalla.PLATA,
                Resultados.TipoMedalla.BRONCE,
                Resultados.TipoMedalla.NADA
            };

        private int[] cortes;

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
            // Ordenamos los resutlados
            List<Resultados> list = this.resultados.Values.ToList();
            list.Sort(compara);

            // Si no hemos calculado los cortes, o el numero de concursantes cambió,
            // calculamos los cortes
            if (this.cortes == null || this.cortes[3] <= this.concursantes)
            {
                if (this.tipoOlimpiada == TipoOlimpiada.OMI)
                {
                    // Para las OMI se siguen las reglas de los doceavos
                    this.cortes = new int[] {
                        (int) Math.Ceiling(this.concursantes / 12.0),
                        (int) Math.Ceiling(this.concursantes / 4.0),
                        (int) Math.Ceiling(this.concursantes / 2.0),
                        this.concursantes + 1
                    };
                }
                else
                {
                    // Para OMIP y OMIS, se siguen las reglas de los tercios
                    this.cortes = new int[] {
                        (int) Math.Ceiling(this.concursantes / 3.0),
                        (int) Math.Ceiling(this.concursantes / 1.5),
                        this.concursantes + 1,
                        this.concursantes + 1
                    };
                }
            }

            int lugar = 0;
            int lastPoints = -1;
            int empatados = 0;
            int premioActual = 0;

            // Asignamos lugares y medallas
            for (int counter = 1; counter <= list.Count; counter++)
            {
                Resultados r = list[counter - 1];
                if (r == null)
                    break;

                // Se acordó que para el calculo de medallas, los puntos se iban a redondear
                int currentPoints = (int) Math.Round((decimal)r.total, 0);
                if (currentPoints == lastPoints)
                {
                    empatados++;
                }
                else
                {
                    lugar = counter;
                    empatados = 0;
                }

                r.lugar = lugar;
                lastPoints = currentPoints;

                // Si no hay puntos, no hay medallas
                if (currentPoints == 0)
                {
                    r.medalla = Resultados.TipoMedalla.NADA;
                }
                else
                {
                    // Se verifica si hay que cambiar de premio
                    if (this.cortes[premioActual] < counter && empatados == 0)
                        premioActual++;

                    r.medalla = medallas[premioActual];
                }

                // Finalmente guardamos la linea en la base de datos
                r.guardar();
            }
        }
    }
}
