using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using OMIstats.Ajax;
using OMIstats.Utilities;

namespace OMIstats.Models
{
    public class DetallePuntos
    {
        public string omi;
        public TipoOlimpiada tipoOlimpiada;
        public string clave;
        public int timestamp;
        public int dia;
        public List<float?> puntosProblemas;
        public float? puntosDia;

        private DetallePuntos()
        {
        }

        public DetallePuntos(string omi, TipoOlimpiada tipoOlimpiada, string clave, int timestamp, int dia, List<float?> puntos)
        {
            this.omi = omi;
            this.tipoOlimpiada = tipoOlimpiada;
            this.clave = clave;
            this.timestamp = timestamp;
            this.dia = dia;
            this.puntosProblemas = puntos;
            puntosDia = 0;

            foreach (float? t in puntos)
                if (t != null)
                    puntosDia += t;
        }

#if OMISTATS
        private static void llenarDatos(DataRow row, OverlayPuntos puntos, int problemas)
        {
            puntos.timestamp.Add(DataRowParser.ToInt(row["timestamp"]));
            for (int i = 0; i < problemas; i++)
                puntos.problemas[i].Add(DataRowParser.ToFloat(row["puntosP" + (i + 1)]));
            puntos.puntos.Add(DataRowParser.ToFloat(row["puntosD"]));
        }

        private void llenarDatos(DataRow row)
        {
            dia = DataRowParser.ToInt(row["dia"]);
            clave = DataRowParser.ToString(row["clave"]);
            timestamp = DataRowParser.ToInt(row["timestamp"]);
            puntosProblemas = new List<float?>();
            for (int i = 0; i < 6; i++)
                puntosProblemas.Add(DataRowParser.ToFloat(row["puntosP" + (i + 1)]));
            puntosDia = DataRowParser.ToFloat(row["puntosD"]);
        }

        /// <summary>
        /// Obtiene la lista de resultados de un usuario en particular, de una olimpiada en particular
        /// </summary>
        /// <returns></returns>
        public static OverlayPuntos cargarResultados(string omi, TipoOlimpiada tipoOlimpiada, string clave, int dia, int problemas)
        {
            OverlayPuntos puntos = new OverlayPuntos();

            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from detallepuntos ");
            query.Append(" where clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" and clave = ");
            query.Append(Cadenas.comillas(clave));
            query.Append(" and dia = ");
            query.Append(dia);
            query.Append(" order by timestamp asc ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                llenarDatos(table.Rows[i], puntos, problemas);
            }

            if (puntos.timestamp[0] != 0)
            {
                puntos.timestamp.Insert(0, 0);
                for (int i = 0; i < problemas; i++)
                    puntos.problemas[i].Insert(0, 0);
                puntos.puntos.Insert(0, 0);
            }

            puntos.problemas = null;
            return puntos;
        }

        public static int obtenerTimestampMasReciente(string clave, TipoOlimpiada tipo, int dia)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select top 1 timestamp from detallepuntos ");
            query.Append(" where clase = ");
            query.Append(Cadenas.comillas(tipo.ToString().ToLower()));
            query.Append(" and olimpiada = ");
            query.Append(Cadenas.comillas(clave));
            query.Append(" and dia = ");
            query.Append(dia);
            query.Append(" order by timestamp desc ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            return DataRowParser.ToInt(table.Rows[0][0]);
        }

        public static Dictionary<string, DetallePuntos> obtenerPuntosConTimestamp(string clave, TipoOlimpiada tipo, int dia, int timestamp)
        {
            Dictionary<string, DetallePuntos> puntos = new Dictionary<string, DetallePuntos>();
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from detallepuntos ");
            query.Append(" where clase = ");
            query.Append(Cadenas.comillas(tipo.ToString().ToLower()));
            query.Append(" and olimpiada = ");
            query.Append(Cadenas.comillas(clave));
            query.Append(" and dia = ");
            query.Append(dia);
            query.Append(" and timestamp = ");
            query.Append(timestamp);

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                DetallePuntos dp = new DetallePuntos();
                dp.llenarDatos(table.Rows[i]);
                puntos.Add(dp.clave, dp);
            }

            return puntos;
        }
#endif
        /// <summary>
        /// Guarda los datos del objeto en la base de datos
        /// </summary>
        public void guardar()
        {
            StringBuilder query = new StringBuilder();
            Acceso db = new Acceso();

            query.Append("insert into DetallePuntos values(");
            query.Append(Cadenas.comillas(omi));
            query.Append(",");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(",");
            query.Append(Cadenas.comillas(clave));
            query.Append(",");
            query.Append(timestamp);
            query.Append(",");
            query.Append(dia);
            query.Append(",");
            query.Append(this.puntosProblemas[0] == null ? "0" : this.puntosProblemas[0].ToString());
            query.Append(",");
            query.Append(this.puntosProblemas[1] == null ? "0" : this.puntosProblemas[1].ToString());
            query.Append(",");
            query.Append(this.puntosProblemas[2] == null ? "0" : this.puntosProblemas[2].ToString());
            query.Append(",");
            query.Append(this.puntosProblemas[3] == null ? "0" : this.puntosProblemas[3].ToString());
            query.Append(",");
            query.Append(this.puntosProblemas[4] == null ? "0" : this.puntosProblemas[4].ToString());
            query.Append(",");
            query.Append(this.puntosProblemas[5] == null ? "0" : this.puntosProblemas[5].ToString());
            query.Append(",");
            query.Append(this.puntosDia == null ? "0" : this.puntosDia.ToString());
            query.Append(")");

            db.EjecutarQuery(query.ToString());
        }

        private static void borrar(string omi, string clase, string clave, int timestamp, int dia)
        {
            StringBuilder query = new StringBuilder();
            Acceso db = new Acceso();

            query.Append(" delete DetallePuntos where olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" and clase =  ");
            query.Append(Cadenas.comillas(clase));
            query.Append(" and clave =  ");
            query.Append(Cadenas.comillas(clave));
            query.Append(" and timestamp =  ");
            query.Append(timestamp);
            query.Append(" and dia =  ");
            query.Append(dia);

            db.EjecutarQuery(query.ToString());
        }

        public static void clean(string omi, TipoOlimpiada tipo, int dia)
        {
            StringBuilder query = new StringBuilder();
            Acceso db = new Acceso();

            query.Append(" select * from DetallePuntos where olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipo.ToString().ToLower()));
            query.Append(" and dia = ");
            query.Append(dia);
            query.Append(" order by clave, timestamp asc ");

            db.EjecutarQuery(query.ToString());

            DataTable table = db.getTable();

            bool first = false;
            DetallePuntos anterior = new DetallePuntos();
            DetallePuntos actual = new DetallePuntos();
            foreach (DataRow r in table.Rows)
            {
                actual.puntosDia = DataRowParser.ToFloat(r["puntosD"]);
                actual.timestamp = DataRowParser.ToInt(r["timestamp"]);
                actual.clave = DataRowParser.ToString(r["clave"]);

                if (actual.clave != anterior.clave)
                {
                    first = true;
                }
                else
                {
                    if (actual.puntosDia == anterior.puntosDia)
                    {
                        if (!first)
                            borrar(omi, tipo.ToString().ToLower(), anterior.clave, anterior.timestamp, dia);
                        first = false;
                    }
                    else
                    {
                        first = true;
                    }
                }

                anterior.puntosDia = actual.puntosDia;
                anterior.timestamp = actual.timestamp;
                anterior.clave = actual.clave;
            }
        }

        public static void trim(string omi, TipoOlimpiada tipo, int tiempo, int dia = 1)
        {
            if (dia > 2)
                return;

            StringBuilder query = new StringBuilder();
            Acceso db = new Acceso();

            // Primero obtenemos una lista de todos los timestamps mas grandes
            query.Append(" select clave, MAX(timestamp) from DetallePuntos where olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipo.ToString().ToLower()));
            query.Append(" and dia = ");
            query.Append(dia);
            query.Append(" group by clave ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                string clave = DataRowParser.ToString(r[0]);
                int timestamp = DataRowParser.ToInt(r[1]);

                // Si el último timestamp es diferente del tiempo que tenemos...
                if (timestamp != tiempo)
                {
                    // ...borramos todos las entradas superiores y menores al que tenemos
                    query.Clear();
                    query.Append(" delete DetallePuntos where olimpiada = ");
                    query.Append(Cadenas.comillas(omi));
                    query.Append(" and clase = ");
                    query.Append(Cadenas.comillas(tipo.ToString().ToLower()));
                    query.Append(" and dia = ");
                    query.Append(dia);
                    query.Append(" and clave = ");
                    query.Append(Cadenas.comillas(clave));
                    query.Append(" and timestamp >= ");
                    query.Append(tiempo);
                    query.Append(" and timestamp <> ");
                    query.Append(timestamp);

                    db.EjecutarQuery(query.ToString());

                    // ... y actualizamos el que tenemos para que tenga ese timestamp
                    query.Clear();
                    query.Append(" update DetallePuntos set timestamp = ");
                    query.Append(tiempo);
                    query.Append(" where olimpiada = ");
                    query.Append(Cadenas.comillas(omi));
                    query.Append(" and clase = ");
                    query.Append(Cadenas.comillas(tipo.ToString().ToLower()));
                    query.Append(" and dia = ");
                    query.Append(dia);
                    query.Append(" and clave = ");
                    query.Append(Cadenas.comillas(clave));
                    query.Append(" and timestamp = ");
                    query.Append(timestamp);

                    db.EjecutarQuery(query.ToString());
                }
            }

            // Finalmente hacemos lo mismo con dia 2
            trim(omi, tipo, tiempo, dia + 1);
        }

        /// <summary>
        /// Actualiza la última entrada en la tabla para el concursante mandado como parámetro
        /// </summary>
        public static void actualizarUltimo(string omi, TipoOlimpiada tipo, int dia, string clave, List<float?>puntos, float? total)
        {
            StringBuilder query = new StringBuilder();
            Acceso db = new Acceso();

            // Primero obtenemos el timestamp mas grande
            query.Append(" select MAX(timestamp) from DetallePuntos where olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipo.ToString().ToLower()));
            query.Append(" and dia = ");
            query.Append(dia);
            query.Append(" and clave = ");
            query.Append(Cadenas.comillas(clave));

            db.EjecutarQuery(query.ToString());

            DataTable table = db.getTable();

            if (table.Rows.Count == 0)
                return;

            int timestamp = DataRowParser.ToInt(table.Rows[0][0]);
            query.Clear();

            // Ahora actualizamos los puntos
            query.Append("update DetallePuntos set puntosP1 = ");
            query.Append(puntos[0] == null ? "0" : puntos[0].ToString());
            query.Append(", puntosP2 = ");
            query.Append(puntos[1] == null ? "0" : puntos[1].ToString());
            query.Append(", puntosP3 = ");
            query.Append(puntos[2] == null ? "0" : puntos[2].ToString());
            query.Append(", puntosP4 = ");
            query.Append(puntos[3] == null ? "0" : puntos[3].ToString());
            query.Append(", puntosP5 = ");
            query.Append(puntos[4] == null ? "0" : puntos[4].ToString());
            query.Append(", puntosP6 = ");
            query.Append(puntos[5] == null ? "0" : puntos[5].ToString());
            query.Append(", puntosD = ");
            query.Append(total == null ? "0" : total.ToString());
            query.Append(" where olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipo.ToString().ToLower()));
            query.Append(" and dia = ");
            query.Append(dia);
            query.Append(" and clave = ");
            query.Append(Cadenas.comillas(clave));
            query.Append(" and timestamp = ");
            query.Append(timestamp);

            db.EjecutarQuery(query.ToString());
        }
#if OMISTATS
        public static void TempUpdate(string omi, TipoOlimpiada tipo, int dia, string clave, int problema, int score)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();
            DetallePuntos dp = new DetallePuntos();

            query.Append(" select * from detallepuntos ");
            query.Append(" where clase = ");
            query.Append(Cadenas.comillas(tipo.ToString().ToLower()));
            query.Append(" and olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" and clave = ");
            query.Append(Cadenas.comillas(clave));
            query.Append(" and dia = ");
            query.Append(dia);
            query.Append(" and timestamp = 0");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            if (table.Rows.Count == 0)
            {
                dp.omi = omi;
                dp.tipoOlimpiada = tipo;
                dp.dia = dia;
                dp.clave = clave;
                dp.puntosProblemas = new List<float?>();
                for (int i = 0; i < 6; i++)
                    dp.puntosProblemas.Add(null);

                query.Append("insert into DetallePuntos(olimpiada, clase, clave, dia, timestamp) values(");
                query.Append(Cadenas.comillas(omi));
                query.Append(",");
                query.Append(Cadenas.comillas(tipo.ToString().ToLower()));
                query.Append(",");
                query.Append(Cadenas.comillas(clave));
                query.Append(",");
                query.Append(dia);
                query.Append(",0)");

                db.EjecutarQuery(query.ToString());
            }
            else
            {
                dp.llenarDatos(table.Rows[0]);
            }
            dp.puntosProblemas[problema - 1] = score;

            dp.puntosDia = (dp.puntosProblemas[0] == null ? 0 : dp.puntosProblemas[0]) +
                           (dp.puntosProblemas[1] == null ? 0 : dp.puntosProblemas[1]) +
                           (dp.puntosProblemas[2] == null ? 0 : dp.puntosProblemas[2]) +
                           (dp.puntosProblemas[3] == null ? 0 : dp.puntosProblemas[3]);

            query.Clear();
            query.Append("update DetallePuntos set puntosP1 = ");
            query.Append(dp.puntosProblemas[0] == null ? "null" : dp.puntosProblemas[0].ToString());
            query.Append(", puntosP2 =");
            query.Append(dp.puntosProblemas[1] == null ? "null" : dp.puntosProblemas[1].ToString());
            query.Append(", puntosP3 =");
            query.Append(dp.puntosProblemas[2] == null ? "null" : dp.puntosProblemas[2].ToString());
            query.Append(", puntosP4 = ");
            query.Append(dp.puntosProblemas[3] == null ? "null" : dp.puntosProblemas[3].ToString());
            query.Append(", puntosD = ");
            query.Append(dp.puntosDia);
            query.Append(" where olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipo.ToString().ToLower()));
            query.Append(" and clave = ");
            query.Append(Cadenas.comillas(clave));
            query.Append(" and timestamp = 0 and dia = ");
            query.Append(dia);

            db.EjecutarQuery(query.ToString());
        }

        private static int compara(Resultados x, Resultados y)
        {
            float x1 = 0, y1 = 0;

            if (x == null)
                x1 = -1;
            else
                x1 = (float)x.total;

            if (y == null)
                y1 = -1;
            else
                y1 = (float)y.total;

            return y1.CompareTo(x1);
        }

        public static void TempMedallas(string omi, TipoOlimpiada tipo, bool guardarFinal, int dia)
        {
            Dictionary<string, Resultados> resultados = new Dictionary<string, Resultados>();
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from miembrodelegacion ");
            query.Append(" where clase = ");
            query.Append(Cadenas.comillas(tipo.ToString().ToLower()));
            query.Append(" and olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" and tipo = 'competidor'");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            int invitados = 0;
            int concursantes = 0;

            foreach (DataRow r in table.Rows)
            {
                MiembroDelegacion md = new MiembroDelegacion();
                md.llenarDatos(r, incluirEscuela: false, incluirPersona: false);

                Resultados res = new Resultados();
                res.omi = omi;
                res.tipoOlimpiada = tipo;
                res.usuario  = md.claveUsuario;
                res.clave = md.clave;
                res.estado = md.estado;
                res.invitado = MiembroDelegacion.esInvitadoOnline(res.clave, true);

                res.dia1[0] = null;
                res.dia1[1] = null;
                res.dia1[2] = null;
                res.dia1[3] = null;

                res.dia2[0] = null;
                res.dia2[1] = null;
                res.dia2[2] = null;
                res.dia2[3] = null;

                if (res.invitado)
                    invitados++;
                concursantes++;

                resultados.Add(res.clave, res);
            }

            query.Clear();
            query.Append(" select * from detallepuntos ");
            query.Append(" where clase = ");
            query.Append(Cadenas.comillas(tipo.ToString().ToLower()));
            query.Append(" and olimpiada = ");
            query.Append(Cadenas.comillas(omi));

            db.EjecutarQuery(query.ToString());
            table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                DetallePuntos dp = new DetallePuntos();
                dp.llenarDatos(r);

                if (!resultados.ContainsKey(dp.clave))
                    continue;
                Resultados res = resultados[dp.clave];

                List<float?> d = null;
                if (dp.dia == 1)
                {
                    d = res.dia1;
                    res.totalDia1 = dp.puntosDia;
                }
                else
                {
                    d = res.dia2;
                    res.totalDia2 = dp.puntosDia;
                }

                for (int i = 0; i < 4; i++)
                    d[i] = dp.puntosProblemas[i];

                res.total = res.totalDia1 + res.totalDia2;
            }

            // Ordenamos los resultados
            List<Resultados> list = resultados.Values.ToList();
            list.Sort(compara);

            int[] cortes;
            Dictionary<string, Medallero> medalleroEstados = new Dictionary<string,Medallero>();

            if (tipo == TipoOlimpiada.OMI)
            {
                // Para las OMI se siguen las reglas de los doceavos
                cortes = new int[] {
                    (int) Math.Ceiling((concursantes - invitados) / 12.0),
                    (int) Math.Ceiling((concursantes - invitados) / 4.0),
                    (int) Math.Ceiling((concursantes - invitados) / 2.0),
                    concursantes + 1
                };
            }
            else
            {
                // Para OMIP y OMIS, se siguen las reglas de los cincos
                cortes = new int[] {
                    5,
                    10,
                    15,
                    concursantes + 1
                };
            }

            int lugar = 0;
            int lastPoints = -1;
            int empatados = 0;
            int premioActual = 0;
            int counterMedalla = 1;
            int counterLugar = 1;

            Resultados.TipoMedalla[] medallas = new Resultados.TipoMedalla[] {
                Resultados.TipoMedalla.ORO,
                Resultados.TipoMedalla.PLATA,
                Resultados.TipoMedalla.BRONCE,
                Resultados.TipoMedalla.NADA
            };

            // Asignamos lugares y medallas
            for (int i = 0; i < list.Count; i++)
            {
                Resultados r = list[i];
                if (r == null)
                    break;

                // Se acordó que para el calculo de medallas, los puntos se iban a redondear
                int currentPoints = (int)Math.Round((decimal)r.total, 0);
                if (currentPoints == lastPoints)
                {
                    empatados++;
                }
                else
                {
                    lugar = counterLugar;
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
                    // Se calcula la nueva medalla
                    while (cortes[premioActual] < counterMedalla && empatados == 0)
                        premioActual++;

                    r.medalla = medallas[premioActual];
                }

                // Para las OMI también calculamos los estados
                if (tipo == TipoOlimpiada.OMI)
                {
                    Medallero m;
                    if (!medalleroEstados.TryGetValue(r.estado, out m))
                    {
                        m = new Medallero();
                        m.tipoOlimpiada = tipo;
                        m.tipoMedallero = Medallero.TipoMedallero.ESTADO_POR_OMI;
                        m.clave = r.estado + "_" + omi;
                        m.omi = omi;

                        medalleroEstados.Add(r.estado, m);

                        m.guardarDatosEstados(true);
                    }

                    // Si es invitado no cuentan los puntos
                    if (!r.invitado)
                    {
                        m.count++;
                        if (m.count <= Olimpiada.COMPETIDORES_BASE)
                            m.puntos += r.total;
                    }

                    // Pero sí las medallas
                    switch (r.medalla)
                    {
                        case Resultados.TipoMedalla.ORO:
                            {
                                if (r.invitado)
                                    m.orosExtra++;
                                else
                                    m.oros++;
                                break;
                            }
                        case Resultados.TipoMedalla.PLATA:
                            {
                                if (r.invitado)
                                    m.platasExtra++;
                                else
                                    m.platas++;
                                break;
                            }
                        case Resultados.TipoMedalla.BRONCE:
                            {
                                if (r.invitado)
                                    m.broncesExtra++;
                                else
                                    m.bronces++;
                                break;
                            }
                    }
                }

                // Para el calculo de medallas, no contamos invitados, así que no incrementamos el counter actual
                if (!r.invitado)
                    counterMedalla++;
                counterLugar++;

                // Finalmente guardamos la linea en la base de datos
                DetalleLugar detalleLugar = new DetalleLugar(omi, tipo, r.clave, 0, dia, r.medalla, r.lugar);
                detalleLugar.guardar();

                if (guardarFinal)
                {
                    r.guardar();
                }
            }

            // Si estamos escondiendo los puntos, no hace falta continuar
            if (!guardarFinal)
                return;

            // Actualizamos el número de problemas
            Olimpiada.guardaProblemas(omi, tipo, tipo == TipoOlimpiada.OMI ? 4 : 3, dia);

            // Para OMIPS ya terminamos los cálculos
            if (tipo != TipoOlimpiada.OMI)
                return;

            // Ordenamos también el medallero de los estados
            List<Medallero> sortedEstados = new List<Medallero>(medalleroEstados.Values);

            // Primero calculamos los promedios
            foreach (Medallero estado in sortedEstados)
            {
                // Arreglamos el estado sede
                int competidores = estado.count;
                if (competidores > Olimpiada.COMPETIDORES_BASE)
                {
                    competidores = Olimpiada.COMPETIDORES_BASE;
                    estado.ajustarMedallas();
                }
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

                estado.guardaMedallasEstado(invitados > 0);
            }
        }
#endif
    }
}