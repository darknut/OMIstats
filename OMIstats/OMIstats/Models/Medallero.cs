using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using OMIstats.Utilities;

namespace OMIstats.Models
{
    public class Medallero : IComparable<Medallero>
    {
        public enum TipoMedallero
        {
            NULL,
            PERSONA,
            ESTADO,
            INSTITUCION,
            ASESOR,
            ESTADO_POR_OMI
        }

        private const string INVITADOS_EXTRA_KEY = "_I";

        public TipoOlimpiada tipoOlimpiada { get; set; }

        public TipoMedallero tipoMedallero { get; set; }

        public string clave { get; set; }

        public int oros { get; set; }

        public int platas { get; set; }

        public int bronces { get; set; }

        public int orosExtra { get; set; }

        public int platasExtra { get; set; }

        public int broncesExtra { get; set; }

        public int otros { get; set; }

        public string omi { get; set; }

        public float? puntos { get; set; }

        public float? promedio { get; set; }

        public int lugar { get; set; }

        // Variables auxiliares para conteo
#if OMISTATS
        private bool hayUNKs;
#endif
        public int count;
        private bool ordenarPorPuntos;

        public Medallero()
        {
            tipoOlimpiada = TipoOlimpiada.NULL;
            tipoMedallero = TipoMedallero.NULL;
            clave = "";
            oros = 0;
            platas = 0;
            bronces = 0;
            otros = 0;
            puntos = 0;
            promedio = 0;
            lugar = 0;

            omi = "";
#if OMISTATS
            hayUNKs = false;
#endif
            count = 0;
            ordenarPorPuntos = false;
        }

        public Medallero(TipoOlimpiada tipo): this()
        {
            tipoOlimpiada = tipo;
        }

        public void llenarDatos(DataRow datos)
        {
            tipoOlimpiada = DataRowParser.ToTipoOlimpiada(datos["clase"]);
            tipoMedallero = DataRowParser.ToTipoMedallero(datos["tipo"]);
            clave = DataRowParser.ToString(datos["clave"]);
            oros = DataRowParser.ToInt(datos["oro"]);
            platas = DataRowParser.ToInt(datos["plata"]);
            bronces = DataRowParser.ToInt(datos["bronce"]);
            otros = DataRowParser.ToInt(datos["otros"]);
            puntos = DataRowParser.ToFloat(datos["puntos"]);
            promedio = DataRowParser.ToFloat(datos["promedio"]);
            lugar = DataRowParser.ToInt(datos["lugar"]);

            omi = DataRowParser.ToString(datos["omi"]);
        }

        /// <summary>
        /// Obtiene el medallero de la base de datos
        /// </summary>
        /// <param name="tipoOlimpiada">El tipo de la olimpiada de la que se requiere el medallero</param>
        /// <param name="tipoMedallero">Si es estado, persona, institucion o asesor</param>
        /// <param name="clave">La clave del estado/persona/institucion/asesor</param>
        /// <param name="nullSiInexistente">Regresa null si no hay medallero y esta bandera es verdadera</param>
        /// <returns>Un objeto medallero con los datos deseados</returns>
        public static Medallero obtenerMedallas(TipoOlimpiada tipoOlimpiada, TipoMedallero tipoMedallero, string clave, bool nullSiInexistente = false)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from medallero where clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and tipo = ");
            query.Append((int)tipoMedallero);
            query.Append(" and clave = ");
            query.Append(Cadenas.comillas(clave));

            if (db.EjecutarQuery(query.ToString()).error)
                return null;

            DataTable table = db.getTable();

            Medallero m = new Medallero();
            m.tipoMedallero = tipoMedallero;
            m.tipoOlimpiada = tipoOlimpiada;
            m.clave = clave;

            if (table.Rows.Count > 0)
                m.llenarDatos(table.Rows[0]);
            else if (nullSiInexistente)
                return null;

            return m;
        }

        /// <summary>
        /// Obtiene todos los medalleros del tipo pedido
        /// </summary>
        /// <param name="tipoMedallero">Si es estado, persona, institucion o asesor</param>
        /// <param name="clave">La clave del estado/persona/institucion/asesor</param>
        /// <returns>Un objeto medalleros con los medalleros deseados</returns>
        public static Medalleros obtenerMedalleros(TipoMedallero tipoMedallero, string clave)
        {
            Medalleros m = new Medalleros();

            m.OMI = obtenerMedallas(TipoOlimpiada.OMI, tipoMedallero, clave, true);
            m.IOI = obtenerMedallas(TipoOlimpiada.IOI, tipoMedallero, clave, true);
            m.OMIS = obtenerMedallas(TipoOlimpiada.OMIS, tipoMedallero, clave, true);
            m.OMIP = obtenerMedallas(TipoOlimpiada.OMIP, tipoMedallero, clave, true);

            return m;
        }

        public bool guardarDatosEstados(bool hayInvitados, bool expectErrors = false)
        {
            if (hayInvitados)
            {
                bool temp = this.guardarDatos();
                if (temp)
                    return temp;
                this.oros = this.orosExtra;
                this.platas = this.platasExtra;
                this.bronces = this.broncesExtra;
                return this.guardarDatos(expectErrors, this.clave + INVITADOS_EXTRA_KEY);
            }
            else
            {
                this.oros += this.orosExtra;
                this.platas += this.platasExtra;
                this.bronces += this.broncesExtra;
                return this.guardarDatos();
            }
        }

        /// <summary>
        /// Guarda los datos en el objeto en la base de datos
        /// </summary>
        /// <returns>Regresa si se guardo o no</returns>
        public bool guardarDatos(bool expectErrors = false, string overwriteClave = null)
        {
            if (tipoMedallero == TipoMedallero.NULL || tipoOlimpiada == TipoOlimpiada.NULL || clave == "")
                return false;

            string c = overwriteClave == null ? this.clave : overwriteClave;

            return new Acceso().NuevoONoHagasNada("medallero",
                new Dictionary<string,object> {
                    { "clase", tipoOlimpiada.ToString().ToLower() },
                    { "tipo", (int)tipoMedallero },
                    { "clave", c },
                    { "omi", omi }
                }, new Dictionary<string, object> {
                    { "oro", oros },
                    { "plata", platas },
                    { "bronce", bronces },
                    { "otros", otros },
                    { "puntos", puntos },
                    { "promedio", promedio },
                    { "lugar", lugar },
                }).error;
        }

        /// <summary>
        /// Guarda el medallero de los estados
        /// haciendo los ajustes necesarios a las medallas extra
        /// </summary>
        public bool guardaMedallasEstado(bool hayInvitados)
        {
            if (hayInvitados)
            {
                bool temp = actualizar();
                if (!temp)
                    return temp;
                this.oros = this.orosExtra;
                this.platas = this.platasExtra;
                this.bronces = this.broncesExtra;
                return actualizar(this.clave + INVITADOS_EXTRA_KEY);
            }
            else
            {
                this.oros += this.orosExtra;
                this.platas += this.platasExtra;
                this.bronces += this.broncesExtra;
                return actualizar();
            }
        }

        /// <summary>
        /// Actualiza el medallero en la base de datos
        /// </summary>
        public bool actualizar(string overwriteClave = null)
        {
            if (tipoMedallero == TipoMedallero.NULL || tipoOlimpiada == TipoOlimpiada.NULL || clave == "")
                return false;

            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();
            string c = overwriteClave == null ? this.clave : overwriteClave;

            query.Append(" update medallero set oro = ");
            query.Append(oros);
            query.Append(", plata = ");
            query.Append(platas);
            query.Append(", bronce = ");
            query.Append(bronces);
            query.Append(", otros = ");
            query.Append(otros);
            query.Append(", puntos = ");
            query.Append(puntos);
            query.Append(", promedio = ");
            query.Append(promedio);
            query.Append(", lugar = ");
            query.Append(lugar);
            query.Append(" where clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and tipo = ");
            query.Append((int)tipoMedallero);
            query.Append(" and clave = ");
            query.Append(Cadenas.comillas(c));

            return !db.EjecutarQuery(query.ToString()).error;
        }

#if OMISTATS
        /// <summary>
        /// Usa las variables en el objeto para calcular las medallas basadas en lo que hay en la base de datos
        /// </summary>
        /// </param name="tipoOlimpiada">El tipo de olimpiada para el que se requieren los tipos</param>
        public static void calcularMedallas(TipoOlimpiada tipoOlimpiada, string olimpiada, bool ordenarPorPuntos, int competidoresBase)
        {
            if (tipoOlimpiada == TipoOlimpiada.NULL)
                return;

            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" delete medallero where clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and (tipo <> ");
            query.Append((int)TipoMedallero.ESTADO_POR_OMI);
            query.Append(" or clave like '%_");
            query.Append(olimpiada);
            query.Append("%')");

            // Primero borramos todo lo que está en la base de datos
            // a excepción de otros tipos de olimpiada u otros estado-olimpiada
            db.EjecutarQuery(query.ToString());

            // Obtenermos todos los resultados
            List<Resultados> resultados = Resultados.cargarResultados(null, tipoOlimpiada, cargarObjetos: true);

            // Diccionarios para los diferentes tipos de medalleros
            Dictionary<int, Medallero> personas = new Dictionary<int,Medallero>();
            Dictionary<int, Medallero> instituciones = new Dictionary<int,Medallero>();
            Dictionary<string, Medallero> estados = new Dictionary<string,Medallero>();
            Dictionary<string, Medallero> estadosPorOlimpiada = new Dictionary<string, Medallero>();

            // Recorremos todos los resultados agregando contadores
            foreach(Resultados resultado in resultados)
            {
                Medallero persona;
                Medallero institucion;
                Medallero estado;
                Medallero estadoPorOlimpiada = null;
                bool aplicaAOlimpiada = olimpiada == resultado.omi;
                Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(resultado.omi, resultado.tipoOlimpiada);

                string estadoPorOlimpiadaClave = resultado.estado + "_" + resultado.omi;

                if (!personas.TryGetValue(resultado.usuario, out persona))
                {
                    persona = new Medallero();
                    persona.clave = resultado.usuario.ToString();
                    persona.tipoOlimpiada = tipoOlimpiada;
                    persona.tipoMedallero = TipoMedallero.PERSONA;
                    personas.Add(resultado.usuario, persona);
                }

                if (resultado.escuela != null)
                {
                    if (!instituciones.TryGetValue(resultado.escuela.clave, out institucion))
                    {
                        institucion = new Medallero();
                        institucion.clave = resultado.escuela.clave.ToString();
                        institucion.tipoOlimpiada = tipoOlimpiada;
                        institucion.tipoMedallero = TipoMedallero.INSTITUCION;
                        instituciones.Add(resultado.escuela.clave, institucion);
                    }
                }
                else
                {
                    // Agregamos un dummy para evitar if's abajo
                    institucion = new Medallero();
                }

                Estado e = Estado.obtenerEstadoConClave(resultado.estado);
                if (!estados.TryGetValue(resultado.estado, out estado))
                {
                    estado = new Medallero();
                    estado.clave = resultado.estado;
                    estado.tipoOlimpiada = tipoOlimpiada;
                    estado.tipoMedallero = TipoMedallero.ESTADO;
                    estados.Add(resultado.estado, estado);
                }

                if (aplicaAOlimpiada)
                {
                    if (!estadosPorOlimpiada.TryGetValue(estadoPorOlimpiadaClave, out estadoPorOlimpiada))
                    {
                        estadoPorOlimpiada = new Medallero();
                        estadoPorOlimpiada.clave = estadoPorOlimpiadaClave;
                        estadoPorOlimpiada.tipoOlimpiada = tipoOlimpiada;
                        estadoPorOlimpiada.omi = resultado.omi;
                        estadoPorOlimpiada.ordenarPorPuntos = ordenarPorPuntos;
                        estadoPorOlimpiada.tipoMedallero = TipoMedallero.ESTADO_POR_OMI;
                        estadoPorOlimpiada.count = 0;
                        estadoPorOlimpiada.puntos = 0;
                        estadoPorOlimpiada.promedio = 0;
                        estadoPorOlimpiada.hayUNKs = false;
                        if (!e.extranjero)
                            estadosPorOlimpiada.Add(estadoPorOlimpiadaClave, estadoPorOlimpiada);
                    }
                }

                bool esInvitado = MiembroDelegacion.esInvitado(resultado.clave) ||
                                  MiembroDelegacion.esInvitadoOnline(resultado.clave, o.esOnline, o.competidoresBase);

                if (resultado.medalla != Resultados.TipoMedalla.DESCALIFICADO)
                {
                    switch (resultado.medalla)
                    {
                        case Resultados.TipoMedalla.ORO_3:
                        case Resultados.TipoMedalla.ORO_2:
                        case Resultados.TipoMedalla.ORO_1:
                        case Resultados.TipoMedalla.ORO:
                            {
                                persona.oros++;
                                if (!esInvitado)
                                    estado.oros++;
                                institucion.oros++;
                                if (aplicaAOlimpiada)
                                {
                                    if (esInvitado)
                                        estadoPorOlimpiada.orosExtra++;
                                    else
                                        estadoPorOlimpiada.oros++;
                                }
                                break;
                            }
                        case Resultados.TipoMedalla.PLATA:
                            {
                                persona.platas++;
                                if (!esInvitado)
                                    estado.platas++;
                                institucion.platas++;
                                if (aplicaAOlimpiada)
                                {
                                    if (esInvitado)
                                        estadoPorOlimpiada.platasExtra++;
                                    else
                                        estadoPorOlimpiada.platas++;
                                }
                                break;
                            }
                        case Resultados.TipoMedalla.BRONCE:
                            {
                                persona.bronces++;
                                if (!esInvitado)
                                    estado.bronces++;
                                institucion.bronces++;
                                if (aplicaAOlimpiada)
                                {
                                    if (esInvitado)
                                        estadoPorOlimpiada.broncesExtra++;
                                    else
                                        estadoPorOlimpiada.bronces++;
                                }
                                break;
                            }
                        default:
                            {
                                persona.otros++;
                                if (!esInvitado)
                                    estado.otros++;
                                institucion.otros++;
                                break;
                            }
                    }

                    if (aplicaAOlimpiada)
                    {
                        if (resultado.clave.StartsWith(Resultados.CLAVE_DESCONOCIDA))
                            estadoPorOlimpiada.hayUNKs = true;

                        // No se han guardado mas de 4 lugares
                        if (estadoPorOlimpiada.count < competidoresBase)
                        {
                            // Invitados no se cuentan en el total
                            if (!esInvitado)
                            {
                                // Si solo tenemos los datos de los medallistas, no podemos hacer nada con los puntos
                                if (o.noMedallistasConocidos)
                                {
                                    // En las OMIs con puntos desconocidos, se guarda en los puntos del día 2, los puntos de los estados
                                    if (o.puntosDesconocidos)
                                    {
                                        estadoPorOlimpiada.puntos += resultado.totalDia2;
                                    }
                                    else
                                    {
                                        estadoPorOlimpiada.count++;
                                        estadoPorOlimpiada.puntos += resultado.total;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Guardamos los contadores en la base de datos
            // Primero las personas
            foreach (Medallero persona in personas.Values)
                if (persona.clave != "0")
                    persona.guardarDatos();

            // Después las instituciones
            foreach (Medallero institucion in instituciones.Values)
                if (institucion.clave != "0")
                    institucion.guardarDatos();

            // Los estados (general)
            foreach (Medallero estado in estados.Values)
                estado.guardarDatos();

            // Finalmente, para los estados por olimpiada, hay que hacer un par de cosas
            List<Medallero> sortedEstados = new List<Medallero>(estadosPorOlimpiada.Values);
            if (sortedEstados.Count > 0)
            {
                string lastOMI = sortedEstados[0].omi;
                bool invalido = false;
                int firstEstadoInOmi = 0;
                // Los necesitamos ordenados primero por olimpiada
                sortedEstados.Sort();
                // Necesitamos reordenarlos por promedio
                for (int i = 0; i < sortedEstados.Count; i++)
                {
                    Medallero estado = sortedEstados[i];
                    estado.ajustarMedallas(competidoresBase);
                    if (estado.omi != lastOMI)
                    {
                        // Si algún estado en la olimpiada tiene un
                        // promedio invalido, ningún promedio es valido
                        if (invalido)
                        {
                            for (int j = firstEstadoInOmi; j < i; j++)
                                sortedEstados[j].promedio = 0;
                        }
                        firstEstadoInOmi = i;
                        invalido = false;
                        lastOMI = estado.omi;
                    }

                    if (!estado.hayUNKs && estado.count > 0)
                        estado.promedio = (float?)Math.Round((double)(estado.puntos / estado.count), 2);
                    invalido |= estado.promedioEsInvalido();
                }
                sortedEstados.Sort();

                lastOMI = "";
                int lugarActual = 0;
                Medallero ultimoEstado = null;

                // Vamos por cada estado para asignarles el lugar
                foreach (Medallero estado in sortedEstados)
                {
                    // Estamos recibiendo los estados de todas las olimpiadas
                    if (estado.omi != lastOMI)
                    {
                        lastOMI = estado.omi;
                        lugarActual = 0;
                        ultimoEstado = null;
                    }

                    lugarActual++;

                    // Revisamos si hay empates entre estados
                    if (ultimoEstado == null ||
                        ultimoEstado.oros != estado.oros ||
                        ultimoEstado.platas != estado.platas ||
                        ultimoEstado.bronces != estado.bronces ||
                        (int)Math.Round((double)ultimoEstado.puntos) != (int)Math.Round((double)estado.puntos))
                        estado.lugar = lugarActual;
                    else
                        estado.lugar = ultimoEstado.lugar;

                    ultimoEstado = estado;

                    Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(estado.omi, estado.tipoOlimpiada);
                    estado.guardarDatosEstados(o.invitados > 0);
                }
            }

            // Al final hacemos los ajustes hardcodeados
            hardcode(tipoOlimpiada, olimpiada);
        }
#endif

        /// <summary>
        /// Ajusta las medallas del medallero actual para que no haya más de
        /// 4 medallas en total
        /// </summary>
        public void ajustarMedallas(int competidoresBase)
        {
            int temp = 0;
            if (this.oros + this.platas + this.bronces > competidoresBase)
            {
                if (this.oros > competidoresBase)
                {
                    temp = this.oros;
                    this.oros = competidoresBase;
                    this.orosExtra = temp - this.oros;
                }
                if (this.oros + this.platas > competidoresBase)
                {
                    temp = this.platas;
                    this.platas = competidoresBase - this.oros;
                    this.platasExtra = temp - this.platas;
                }
                if (this.oros + this.platas + this.bronces > competidoresBase)
                {
                    temp = this.bronces;
                    this.bronces = competidoresBase - this.oros - this.platas;
                    this.broncesExtra = temp - this.bronces;
                }
            }
        }

        /// <summary>
        /// Obtiene la tabla de estados de la olimpiada mandada como parametro
        /// </summary>
        /// <param name="tipoOlimpiada">El tipo de olimpiada</param>
        /// <param name="olimpiada">La clave de la olimpiada</param>
        /// <param name="medalleroGeneral">El medallero donde se cuentan todas las medallas sin
        /// quitar los extra de la sede</param>
        /// <returns>La tabla ordenada de estados</returns>
        public static List<Medallero> obtenerTablaEstados(TipoOlimpiada tipoOlimpiada, string olimpiada, out Medallero medalleroGeneral)
        {
            List<Medallero> lista = new List<Medallero>();

            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from Medallero where tipo = ");
            query.Append((int)TipoMedallero.ESTADO_POR_OMI);
            query.Append(" and omi = ");
            query.Append(Cadenas.comillas(olimpiada));
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" order by lugar asc, clave asc ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            medalleroGeneral = new Medallero();
            Medallero lastMedallero = null;
            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(olimpiada, tipoOlimpiada);
            foreach (DataRow r in table.Rows)
            {
                Medallero m = new Medallero();
                m.llenarDatos(r);

                if (m.clave.EndsWith(INVITADOS_EXTRA_KEY))
                {
                    lastMedallero.orosExtra += m.oros;
                    lastMedallero.platasExtra += m.platas;
                    lastMedallero.broncesExtra += m.bronces;

                    medalleroGeneral.orosExtra += m.oros;
                    medalleroGeneral.platasExtra += m.platas;
                    medalleroGeneral.broncesExtra += m.bronces;
                }
                else
                {
                    // Después de esto solo nos importa la clave del estado, así que nos deshacemos del resto
                    m.clave = m.clave.Substring(0, 3);

                    medalleroGeneral.oros += m.oros;
                    medalleroGeneral.platas += m.platas;
                    medalleroGeneral.bronces += m.bronces;

                    // Solo quiero 4 medallas por estado en este caso
                    m.ajustarMedallas(o.competidoresBase);

                    lista.Add(m);

                    lastMedallero = m;
                }
            }

            return lista;
        }

#if OMISTATS

        public static List<Medallero> obtenerTablaEstadosSecreta(bool hayInvitados, string clave, TipoOlimpiada tipoOlimpiada)
        {
            OmegaUp poll = OmegaUp.obtenerParaOMI(clave, tipoOlimpiada);
            List<Resultados> resultados = Resultados.cargarResultadosSecretos(clave, tipoOlimpiada, poll.dia);
            Dictionary<string, Medallero> diccionario = new Dictionary<string, Medallero>();

            foreach (Resultados resultado in resultados)
            {
                Medallero m = null;
                bool esInvitado = MiembroDelegacion.esInvitadoOnline(resultado.clave, hayInvitados, 4);
                if (diccionario.ContainsKey(resultado.estado))
                {
                    m = diccionario[resultado.estado];
                }
                else
                {
                    m = new Medallero();
                    m.clave = resultado.estado;
                    diccionario.Add(resultado.estado, m);
                }
                switch (resultado.medalla)
                {
                    case Resultados.TipoMedalla.ORO:
                    case Resultados.TipoMedalla.ORO_1:
                    case Resultados.TipoMedalla.ORO_2:
                    case Resultados.TipoMedalla.ORO_3:
                        if (esInvitado)
                            m.orosExtra++;
                        else
                            m.oros++;
                        break;
                    case Resultados.TipoMedalla.PLATA:
                        if (esInvitado)
                            m.platasExtra++;
                        else
                            m.platas++;
                        break;
                    case Resultados.TipoMedalla.BRONCE:
                        if (esInvitado)
                            m.broncesExtra++;
                        else
                            m.bronces++;
                        break;
                }
                m.puntos += resultado.total;
                m.lugar++;
            }

            List<Medallero> medallero = new List<Medallero>();
            foreach (Medallero m in diccionario.Values)
            {
                m.promedio = m.puntos / m.lugar;
                m.lugar = 0;
                medallero.Add(m);
            }
            medallero.Sort();

            for (int i = 0; i < medallero.Count; i++)
            {
                medallero[i].lugar = i + 1;
            }

            return medallero;
        }

        /// <summary>
        /// Obtiene la tabla de estados generales, similar a las que se ve en wikipedia
        /// </summary>
        /// <param name="tipoOlimpiada">El tipo de olimpiada</param>
        /// <param name="cabeceras">Un arreglo donde cada elemento
        /// inidica si la olimpiada tiene resultados para cualquier estado</param>
        /// <returns>El diccionario de los estados</returns>
        public static Dictionary<string, Dictionary<string, Medallero>> obtenerTablaEstadosGeneral(TipoOlimpiada tipoOlimpiada, out bool[] cabeceras)
        {
            Dictionary<string, Dictionary<string, Medallero>> estados = new Dictionary<string, Dictionary<string, Medallero>>();

            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from Medallero where tipo = ");
            query.Append((int)TipoMedallero.ESTADO_POR_OMI);
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            // Un hashset para poder construir el arreglo de cabeceras
            HashSet<string> olimpiadasConResultados = new HashSet<string>();

            foreach (DataRow r in table.Rows)
            {
                // Obtengo los datos de la tabla a un objeto medallero
                Medallero m = new Medallero();
                m.llenarDatos(r);
                if (MiembroDelegacion.esInvitado(m.clave))
                    continue;
                string estado = m.clave.Substring(0, 3);

                // Obtengo el medallero del estado
                Dictionary<string, Medallero> medallero;
                if (!estados.TryGetValue(estado, out medallero))
                {
                    medallero = new Dictionary<string, Medallero>();
                    estados.Add(estado, medallero);
                }

                // Agrego el medallero con la olimpiada
                medallero.Add(m.omi, m);

                // Ponemos que la olimpiada tiene resultados en el hashset
                if (!olimpiadasConResultados.Contains(m.omi))
                    olimpiadasConResultados.Add(m.omi);
            }

            // Construimos el arreglo de cabeceras
            List<Olimpiada> olimpiadas = Olimpiada.obtenerOlimpiadas(tipoOlimpiada);
            cabeceras = new bool[olimpiadas.Count];

            for (int i = 0; i < olimpiadas.Count; i++)
                cabeceras[i] = olimpiadasConResultados.Contains(olimpiadas[i].numero);

            return estados;
        }
#endif

        /// <summary>
        /// Obtiene la tabla de estados generales
        /// Similar a obtenerMedallero de estados, pero incluye todos los estados
        /// </summary>
        /// <param name="tipoOlimpiada">El tipo de olimpiada</param>
        /// <returns>El diccionario de los estados</returns>
        public static Dictionary<string, Medallero> obtenerTablaEstadosGeneral(TipoOlimpiada tipoOlimpiada)
        {
            Dictionary<string, Medallero> estados = new Dictionary<string, Medallero>();

            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from Medallero where tipo = ");
            query.Append((int)TipoMedallero.ESTADO);
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                // Obtengo los datos de la tabla a un objeto medallero
                Medallero m = new Medallero();
                m.llenarDatos(r);

                // Agrego el medallero con la olimpiada
                estados.Add(m.clave, m);
            }

            return estados;
        }

        /// <summary>
        /// Obtiene los medalleros por olimpiada de un solo estado
        /// </summary>
        /// <param name="estado">El estado del cual se requieren los datos</param>
        /// <returns>La diccionario de los estados</returns>
        public static Dictionary<string, Medallero> obtenerDesempeñoEstado(string estado)
        {
            Dictionary<string, Medallero> estados = new Dictionary<string, Medallero>();

            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from Medallero where tipo = ");
            query.Append((int)TipoMedallero.ESTADO_POR_OMI);
            query.Append(" and left(clave,3) = ");
            query.Append(Cadenas.comillas(estado));
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(TipoOlimpiada.OMI.ToString().ToLower()));

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                // Obtengo los datos de la tabla a un objeto medallero
                Medallero m = new Medallero();
                m.llenarDatos(r);
                if (MiembroDelegacion.esInvitado(m.clave))
                    continue;

                // Agrego el medallero con la olimpiada
                estados.Add(m.omi, m);
            }

            return estados;
        }

        public bool promedioEsInvalido()
        {
            return promedio <= 0.001 &&
                    (puntos > 0 || bronces > 0 ||
                    platas > 0 || oros > 0);
        }

        public static bool hayPromedio(List<Medallero> lista)
        {
            for (int i = lista.Count - 1; i >= 0; i--)
            {
                if (lista[i].promedioEsInvalido())
                    return false;
            }
            return true;
        }

        public bool puntosSonInvalidos()
        {
            return puntos <= 0.001 &&
                    (bronces > 0 ||
                    platas > 0 || oros > 0);
        }

        public static bool hayPuntos(List<Medallero> lista)
        {
            for (int i = 0; i < lista.Count; i++)
            {
                if (lista[i].puntosSonInvalidos())
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Harcodea ciertos medalleros que sabemos los resultados
        /// pero no tenemos los números para que salgan correctamente
        /// </summary>
        public static void hardcode(TipoOlimpiada tipoOlimpiada, string olimpiada)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            // En la 10a OMI, hay que mover a GRO manualmente a 1er lugar
            if (tipoOlimpiada == TipoOlimpiada.OMI && olimpiada == "10")
            {
                query.Append(" update Medallero set lugar = lugar + 1 where clase = ");
                query.Append(Cadenas.comillas(TipoOlimpiada.OMI.ToString().ToLower()));
                query.Append(" and omi = \'10\' and lugar < 14 ");
                db.EjecutarQuery(query.ToString());

                query.Clear();
                query.Append(" update Medallero set lugar = 1 where clave = \'GRO_10\'");
                db.EjecutarQuery(query.ToString());
                query.Clear();
            }
        }

        public int CompareTo(Medallero obj)
        {
            if (this.omi == obj.omi)
            {
                if (ordenarPorPuntos)
                {
                    if (obj.promedio == this.promedio)
                    {
                        if (obj.puntos == this.puntos)
                        {
                            if (this.oros == obj.oros)
                            {
                                if (this.platas == obj.platas)
                                    return obj.bronces - this.bronces;
                                return obj.platas - this.platas;
                            }
                            return obj.oros - this.oros;
                        }
                        return (int)Math.Round((double)((obj.puntos * 100) - (this.puntos * 100)), 0);
                    }
                    return (int)Math.Round((double)((obj.promedio * 100) - (this.promedio * 100)), 0);
                }
                else
                {
                    if (this.oros == obj.oros)
                    {
                        if (this.platas == obj.platas)
                        {
                            if (this.bronces == obj.bronces)
                            {
                                if (obj.promedio == this.promedio)
                                    return (int)Math.Round((double)((obj.puntos * 100) - (this.puntos * 100)), 0);
                                return (int)Math.Round((double)((obj.promedio * 100) - (this.promedio * 100)), 0);
                            }
                            return obj.bronces - this.bronces;
                        }
                        return obj.platas - this.platas;
                    }
                    return obj.oros - this.oros;
                }
            }
            return this.omi.CompareTo(obj.omi);
        }
    }

    public class Medalleros
    {
        public Medallero OMI;
        public Medallero OMIS;
        public Medallero OMIP;
        public Medallero IOI;

        public Medallero medalleroDeTipo(TipoOlimpiada tipo)
        {
            switch (tipo)
            {
                case TipoOlimpiada.OMI:
                    return this.OMI;
                case TipoOlimpiada.OMIS:
                    return this.OMIS;
                case TipoOlimpiada.OMIP:
                    return this.OMIP;
                case TipoOlimpiada.IOI:
                    return this.IOI;
            }
            return null;
        }

        public TipoOlimpiada obtenerDefault(TipoOlimpiada tipo)
        {
            if (medalleroDeTipo(tipo) != null)
                return tipo;

            if (this.OMI != null)
                return TipoOlimpiada.OMI;

            if (this.IOI != null)
                return TipoOlimpiada.IOI;

            if (this.OMIS != null)
                return TipoOlimpiada.OMIS;

            if (this.OMIP != null)
                return TipoOlimpiada.OMIP;

            return TipoOlimpiada.OMI;
        }
    }
}