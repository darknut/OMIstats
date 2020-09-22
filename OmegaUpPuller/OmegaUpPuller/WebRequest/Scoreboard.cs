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
        private Dictionary<string, Medallero> medalleroEstados = null;

        private static Resultados.TipoMedalla[] medallas = new Resultados.TipoMedalla[] {
                Resultados.TipoMedalla.ORO,
                Resultados.TipoMedalla.PLATA,
                Resultados.TipoMedalla.BRONCE,
                Resultados.TipoMedalla.NADA
            };

        private int[] cortes;
        public bool hidden;

        public Scoreboard(string olimpiada, TipoOlimpiada tipoOlimpiada, int dia, int problemas)
        {
            this.olimpiada = olimpiada;
            this.tipoOlimpiada = tipoOlimpiada;
            this.dia = dia;
            this.problemas = problemas;
            this.concursantes = 0;
            this.hidden = false;

            inicializaResultados();

            medalleroEstados = new Dictionary<string, Medallero>();

            if (Program.HIDE)
                this.hidden = true;
            else
                this.guardaProblemas();
        }

        private void inicializaResultados()
        {
            resultados = new Dictionary<string, Resultados>();
            List<Resultados> r = Resultados.cargarResultados(olimpiada, tipoOlimpiada, cargarObjetos: false);
            foreach (var resultado in r)
            {
                resultados.Add(resultado.clave, resultado);
            }
            this.concursantes = resultados.Count;
        }

        private void reseteaMedalleroEstados()
        {
            if (tipoOlimpiada != TipoOlimpiada.OMI)
                return;

            foreach (Medallero m in medalleroEstados.Values)
            {
                m.oros = 0;
                m.platas = 0;
                m.bronces = 0;
                m.otros = 0;
                m.puntos = 0;
                m.promedio = 0;
                m.lugar = 0;
                m.count = 0;
            }
        }

        private Medallero agregaEstado(string estado)
        {
            Medallero m = new Medallero();
            m.tipoOlimpiada = this.tipoOlimpiada;
            m.tipoMedallero = Medallero.TipoMedallero.ESTADO_POR_OMI;
            m.clave = estado + "_" + this.olimpiada;
            m.omi = this.olimpiada;

            medalleroEstados.Add(estado, m);
            m.guardarDatos();

            return m;
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
                res.estado = miembros[0].estado;
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

        public void ordena(int timestamp, int dia)
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
                    // Para OMIP y OMIS, se siguen las reglas de los cincos
                    this.cortes = new int[] {
                        5,
                        10,
                        15,
                        this.concursantes + 1
                    };
                }
            }

            int lugar = 0;
            int lastPoints = -1;
            int empatados = 0;
            int premioActual = 0;
            this.reseteaMedalleroEstados();

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
                    // Si no hay puntos, tienes el último lugar
                    r.lugar = concursantes;
                }
                else
                {
                    // Se verifica si hay que cambiar de premio
                    while (this.cortes[premioActual] < counter && empatados == 0)
                        premioActual++;

                    r.medalla = medallas[premioActual];
                }


                // Para las OMI también calculamos los estados
                if (tipoOlimpiada == TipoOlimpiada.OMI)
                {
                    Medallero m;
                    if (!medalleroEstados.TryGetValue(r.estado, out m))
                        m = this.agregaEstado(r.estado);

                    m.count++;

                    if (m.count <= 4)
                        m.puntos += r.total;

                    switch (r.medalla)
                    {
                        case Resultados.TipoMedalla.ORO:
                            {
                                m.oros++;
                                break;
                            }
                        case Resultados.TipoMedalla.PLATA:
                            {
                                m.platas++;
                                break;
                            }
                        case Resultados.TipoMedalla.BRONCE:
                            {
                                m.bronces++;
                                break;
                            }
                    }
                }

                // Finalmente guardamos la linea en la base de datos
                r.guardar(detalles: true, timestamp: timestamp, dia: dia, soloDetalles: Program.HIDE);
            }

            // Para OMIPS ya terminamos los cálculos
            if (tipoOlimpiada != TipoOlimpiada.OMI)
                return;

            // Si estamos escondiendo los puntos, no hace falta continuar
            if (Program.HIDE)
                return;

            // Ordenamos también el medallero de los estados
            List<Medallero> sortedEstados = new List<Medallero>(medalleroEstados.Values);

            // Primero calculamos los promedios
            foreach(Medallero estado in sortedEstados)
            {
                // Arreglamos el estado sede
                int competidores = estado.count;
                if (competidores > 4)
                    competidores = 4;
                estado.promedio = (float?)Math.Round((double)(estado.puntos / competidores), 2);
            }

            // Luego ordenamos
            Medallero ultimoEstado = null;
            sortedEstados.Sort();
            lugar = 0;

            // Finalmente, asignamos lugares
            for (int i = 0; i < sortedEstados.Count; i++)
            {
                Medallero estado = sortedEstados[i];
                lugar++;

                // Revisamos si hay empates entre estados
                if (ultimoEstado == null ||
                    ultimoEstado.oros != estado.oros ||
                    ultimoEstado.platas != estado.platas ||
                    ultimoEstado.bronces != estado.bronces ||
                    (int)Math.Round((double)ultimoEstado.puntos) != (int)Math.Round((double)estado.puntos))
                    estado.lugar = lugar;
                else
                    estado.lugar = ultimoEstado.lugar;

                ultimoEstado = estado;

                estado.actualizar();
            }
        }

        /// <summary>
        /// Actualiza la tabla olimpiada una vez que se ha borrado la instrucción de ocultar
        /// </summary>
        public void guardaProblemas()
        {
            Olimpiada.guardaProblemas(olimpiada, tipoOlimpiada, problemas, dia);
        }
    }
}
