using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;

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

        public TipoOlimpiada tipoOlimpiada { get; set; }

        public TipoMedallero tipoMedallero { get; set; }

        public string clave { get; set; }

        public int oros { get; set; }

        public int platas { get; set; }

        public int bronces { get; set; }

        public int otros { get; set; }

        public string omi { get; set; }

        public float? puntos { get; set; }

        public float? promedio { get; set; }

        public int lugar { get; set; }

        // Variables auxiliares para conteo

        private bool hayUNKs;
        public int count;

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

            hayUNKs = false;
            count = 0;
        }

        public Medallero(TipoOlimpiada tipo): this()
        {
            tipoOlimpiada = tipo;
        }

        private void llenarDatos(DataRow datos)
        {
            tipoOlimpiada = (TipoOlimpiada)Enum.Parse(typeof(TipoOlimpiada), datos["clase"].ToString().ToUpper());
            tipoMedallero = (TipoMedallero)Enum.Parse(typeof(TipoMedallero), datos["tipo"].ToString().ToUpper());
            clave = datos["clave"].ToString().Trim();
            oros = (int)datos["oro"];
            platas = (int)datos["plata"];
            bronces = (int)datos["bronce"];
            otros = (int)datos["otros"];
            puntos = float.Parse(datos["puntos"].ToString());
            promedio = float.Parse(datos["promedio"].ToString());
            lugar = (int)datos["lugar"];

            omi = datos["omi"].ToString().Trim();
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
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from medallero where clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and tipo = ");
            query.Append((int)tipoMedallero);
            query.Append(" and clave = ");
            query.Append(Utilities.Cadenas.comillas(clave));

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

        /// <summary>
        /// Guarda los datos en el objeto en la base de datos
        /// </summary>
        /// <returns>Regresa si se guardo o no</returns>
        public bool guardarDatos()
        {
            if (tipoMedallero == TipoMedallero.NULL || tipoOlimpiada == TipoOlimpiada.NULL || clave == "")
                return false;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" insert into medallero values( ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(", ");
            query.Append((int)tipoMedallero);
            query.Append(", ");
            query.Append(Utilities.Cadenas.comillas(clave));
            query.Append(", ");
            query.Append(oros);
            query.Append(", ");
            query.Append(platas);
            query.Append(", ");
            query.Append(bronces);
            query.Append(", ");
            query.Append(otros);
            query.Append(", ");
            query.Append(Utilities.Cadenas.comillas(omi));
            query.Append(", ");
            query.Append(puntos);
            query.Append(", ");
            query.Append(promedio);
            query.Append(", ");
            query.Append(lugar);
            query.Append(")");

            return !db.EjecutarQuery(query.ToString()).error;
        }

        /// <summary>
        /// Actualiza el medallero en la base de datos
        /// </summary>
        public bool actualizar()
        {
            if (tipoMedallero == TipoMedallero.NULL || tipoOlimpiada == TipoOlimpiada.NULL || clave == "")
                return false;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

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
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and tipo = ");
            query.Append((int)tipoMedallero);
            query.Append(" and clave = ");
            query.Append(Utilities.Cadenas.comillas(clave));

            return !db.EjecutarQuery(query.ToString()).error;
        }

        /// <summary>
        /// Usa las variables en el objeto para calcular las medallas basadas en lo que hay en la base de datos
        /// </summary>
        /// </param name="tipoOlimpiada">El tipo de olimpiada para el que se requieren los tipos</param>
        public static void calcularMedallas(TipoOlimpiada tipoOlimpiada)
        {
            if (tipoOlimpiada == TipoOlimpiada.NULL)
                return;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" delete medallero where clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));

            // Primero borramos todo lo que está en la base de datos
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
                Medallero estadoPorOlimpiada;

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

                if (!estados.TryGetValue(resultado.estado, out estado))
                {
                    estado = new Medallero();
                    estado.clave = resultado.estado;
                    estado.tipoOlimpiada = tipoOlimpiada;
                    estado.tipoMedallero = TipoMedallero.ESTADO;
                    estados.Add(resultado.estado, estado);
                }

                if (!estadosPorOlimpiada.TryGetValue(estadoPorOlimpiadaClave, out estadoPorOlimpiada))
                {
                    estadoPorOlimpiada = new Medallero();
                    estadoPorOlimpiada.clave = estadoPorOlimpiadaClave;
                    estadoPorOlimpiada.tipoOlimpiada = tipoOlimpiada;
                    estadoPorOlimpiada.omi = resultado.omi;
                    estadoPorOlimpiada.tipoMedallero = TipoMedallero.ESTADO_POR_OMI;
                    estadoPorOlimpiada.count = 0;
                    estadoPorOlimpiada.puntos = 0;
                    estadoPorOlimpiada.promedio = 0;
                    estadoPorOlimpiada.hayUNKs = false;
                    estadosPorOlimpiada.Add(estadoPorOlimpiadaClave, estadoPorOlimpiada);
                }

                switch (resultado.medalla)
                {
                    case Resultados.TipoMedalla.ORO_3:
                    case Resultados.TipoMedalla.ORO_2:
                    case Resultados.TipoMedalla.ORO_1:
                    case Resultados.TipoMedalla.ORO:
                        {
                            persona.oros++;
                            estado.oros++;
                            institucion.oros++;
                            estadoPorOlimpiada.oros++;
                            break;
                        }
                    case Resultados.TipoMedalla.PLATA:
                        {
                            persona.platas++;
                            estado.platas++;
                            institucion.platas++;
                            estadoPorOlimpiada.platas++;
                            break;
                        }
                    case Resultados.TipoMedalla.BRONCE:
                        {
                            persona.bronces++;
                            estado.bronces++;
                            institucion.bronces++;
                            estadoPorOlimpiada.bronces++;
                            break;
                        }
                    default:
                        {
                            persona.otros++;
                            estado.otros++;
                            institucion.otros++;
                            break;
                        }
                }

                if (resultado.clave.StartsWith(Resultados.CLAVE_DESCONOCIDA))
                    estadoPorOlimpiada.hayUNKs = true;

                // No se han guardado mas de 4 lugares
                if (estadoPorOlimpiada.count < 4)
                {
                    // En algunas olimpiadas, hubo invitados que se pusieron en el medallero, estos no se cuentan en el total
                    if (!resultado.clave.EndsWith("I"))
                    {
                        Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(resultado.omi, resultado.tipoOlimpiada);

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

            // Guardamos los contadores en la base de datos
            foreach (Medallero persona in personas.Values)
                if (persona.clave != "0")
                    persona.guardarDatos();

            foreach (Medallero institucion in instituciones.Values)
                if (institucion.clave != "0")
                    institucion.guardarDatos();

            foreach (Medallero estado in estados.Values)
                estado.guardarDatos();

            List<Medallero> sortedEstados = new List<Medallero>(estadosPorOlimpiada.Values);
            sortedEstados.Sort();
            string lastOMI = "";
            int lugarActual = 0;
            Medallero ultimoEstado = null;

            foreach (Medallero estado in sortedEstados)
            {
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

                if (!estado.hayUNKs && estado.count > 0)
                    estado.promedio = (float?) Math.Round((double)(estado.puntos / estado.count), 2);

                estado.guardarDatos();
            }
        }

        /// <summary>
        /// Ajusta las medallas del medallero actual para que no haya más de
        /// 4 medallas en total
        /// </summary>
        public void ajustarMedallas()
        {
            if (this.oros + this.platas + this.bronces > 4)
            {
                if (this.oros > 4)
                    this.oros = 4;
                if (this.oros + this.platas > 4)
                    this.platas = 4 - this.oros;
                if (this.oros + this.platas + this.bronces > 4)
                    this.bronces = 4 - this.oros - this.platas;
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

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from Medallero where tipo = ");
            query.Append((int)TipoMedallero.ESTADO_POR_OMI);
            query.Append(" and omi = ");
            query.Append(Utilities.Cadenas.comillas(olimpiada));
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" order by lugar asc ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            medalleroGeneral = new Medallero();
            foreach (DataRow r in table.Rows)
            {
                Medallero m = new Medallero();
                m.llenarDatos(r);
                // Después de esto solo nos importa la clave del estado, así que nos deshacemos del resto
                m.clave = m.clave.Substring(0, 3);

                medalleroGeneral.oros += m.oros;
                medalleroGeneral.platas += m.platas;
                medalleroGeneral.bronces += m.bronces;

                // Solo quiero 4 medallas por estado en este caso
                m.ajustarMedallas();

                lista.Add(m);
            }

            return lista;
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

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from Medallero where tipo = ");
            query.Append((int)TipoMedallero.ESTADO_POR_OMI);
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            // Un hashset para poder construir el arreglo de cabeceras
            HashSet<string> olimpiadasConResultados = new HashSet<string>();

            foreach (DataRow r in table.Rows)
            {
                // Obtengo los datos de la tabla a un objeto medallero
                Medallero m = new Medallero();
                m.llenarDatos(r);
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

        /// <summary>
        /// Obtiene la tabla de estados generales
        /// Similar a obtenerMedallero de estados, pero incluye todos los estados
        /// </summary>
        /// <param name="tipoOlimpiada">El tipo de olimpiada</param>
        /// <returns>El diccionario de los estados</returns>
        public static Dictionary<string, Medallero> obtenerTablaEstadosGeneral(TipoOlimpiada tipoOlimpiada)
        {
            Dictionary<string, Medallero> estados = new Dictionary<string, Medallero>();

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from Medallero where tipo = ");
            query.Append((int)TipoMedallero.ESTADO);
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));

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

        public static bool hayPromedio(List<Medallero> lista)
        {
            for (int i = lista.Count - 1; i >= 0; i--)
            {
                if ((int)Math.Round((double)lista[i].promedio) == 0 &&
                    (lista[i].puntos > 0 || lista[i].bronces > 0 ||
                    lista[i].platas > 0 || lista[i].oros > 0))
                    return false;
            }
            return true;
        }

        public static bool hayPuntos(List<Medallero> lista)
        {
            for (int i = 0; i < lista.Count; i++)
            {
                if ((int)Math.Round((double)lista[i].puntos) == 0 &&
                    (lista[i].bronces > 0 ||
                    lista[i].platas > 0 || lista[i].oros > 0))
                    return false;
            }
            return true;
        }

        public int CompareTo(Medallero obj)
        {
            if (this.omi == obj.omi)
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