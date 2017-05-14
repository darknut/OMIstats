using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;

namespace OMIstats.Models
{
    public class Medallero
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

        public Olimpiada.TipoOlimpiada tipoOlimpiada { get; set; }

        public TipoMedallero tipoMedallero { get; set; }

        public string clave { get; set; }

        public int oros { get; set; }

        public int platas { get; set; }

        public int bronces { get; set; }

        public float? otros { get; set; }

        public string omi { get; set; }

        public Medallero()
        {
            tipoOlimpiada = Olimpiada.TipoOlimpiada.NULL;
            tipoMedallero = TipoMedallero.NULL;
            clave = "";
            oros = 0;
            platas = 0;
            bronces = 0;
            otros = 0;

            omi = "";
        }

        private void llenarDatos(DataRow datos)
        {
            tipoOlimpiada = (Olimpiada.TipoOlimpiada)Enum.Parse(typeof(Olimpiada.TipoOlimpiada), datos["clase"].ToString().ToUpper());
            tipoMedallero = (TipoMedallero)Enum.Parse(typeof(TipoMedallero), datos["tipo"].ToString().ToUpper());
            clave = datos["clave"].ToString().Trim();
            oros = (int)datos["oro"];
            platas = (int)datos["plata"];
            bronces = (int)datos["bronce"];
            otros = float.Parse(datos["otros"].ToString());

            omi = datos["omi"].ToString().Trim();
        }

        /// <summary>
        /// Obtiene el medallero de la base de datos
        /// </summary>
        /// <param name="tipoOlimpiada">El tipo de la olimpiada de la que se requiere el medallero</param>
        /// <param name="tipoMedallero">Si es estado, persona, institucion o asesor</param>
        /// <param name="clave">La clave del estado/persona/institucion/asesor</param>
        /// <returns>Un objeto medallero con los datos deseados</returns>
        public static Medallero obtenerMedallas(Olimpiada.TipoOlimpiada tipoOlimpiada, TipoMedallero tipoMedallero, string clave)
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

            return m;
        }

        /// <summary>
        /// Guarda los datos en el objeto en la base de datos
        /// </summary>
        /// <returns>Regresa si se guardo o no</returns>
        public bool guardarDatos()
        {
            if (tipoMedallero == TipoMedallero.NULL || tipoOlimpiada == Olimpiada.TipoOlimpiada.NULL || clave == "")
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
            query.Append(")");

            return !db.EjecutarQuery(query.ToString()).error;
        }

        /// <summary>
        /// Usa las variables en el objeto para calcular las medallas basadas en lo que hay en la base de datos
        /// </summary>
        /// </param name="tipoOlimpiada">El tipo de olimpiada para el que se requieren los tipos</param>
        public static void calcularMedallas(Olimpiada.TipoOlimpiada tipoOlimpiada)
        {
            if (tipoOlimpiada == Olimpiada.TipoOlimpiada.NULL)
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
                    // Usaremos esta variable para contar cuantos competidores llevamos y asi solo contar los mejores 4
                    estadoPorOlimpiada.tipoMedallero = TipoMedallero.NULL;
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

                // No se han guardado mas de 4 lugares
                if (((int)estadoPorOlimpiada.tipoMedallero) < 4)
                {
                    // En algunas olimpiadas, hubo invitados que se pusieron en el medallero, estos no se cuentan en el total
                    if (!resultado.clave.EndsWith("I"))
                    {
                        Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(resultado.omi, resultado.tipoOlimpiada);

                        // En las OMIs con puntos desconocidos, se guarda en los puntos del día 2, los puntos de los estados
                        if (o.puntosDesconocidos)
                        {
                            estadoPorOlimpiada.otros += resultado.totalDia2;
                        }
                        else
                        {
                            estadoPorOlimpiada.tipoMedallero++;
                            estadoPorOlimpiada.otros += resultado.total;
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

            foreach (Medallero estado in estadosPorOlimpiada.Values)
            {
                // Ajustamos los estados que tienen mas de cuatro medallas
                if (estado.oros + estado.platas + estado.bronces > 4)
                {
                    if (estado.oros > 4)
                        estado.oros = 4;
                    if (estado.oros + estado.platas > 4)
                        estado.platas = 4 - estado.oros;
                    if (estado.oros + estado.platas + estado.bronces > 4)
                        estado.bronces = 4 - estado.oros - estado.platas;
                }
                // Antes de guardar, pones el tipo medallero que usamos para otra cosa arriba
                estado.tipoMedallero = TipoMedallero.ESTADO_POR_OMI;
                estado.guardarDatos();
            }
        }

        /// <summary>
        /// Obtiene las medallas de la delegación mandada como parámetro
        /// </summary>
        /// <param name="delegacion">La delegación de la cual se contaran las medallas</param>
        /// <returns>Una lista con las medallas</returns>
        public static Medallero contarMedallas(List<MiembroDelegacion> delegacion)
        {
            Medallero m = new Medallero();

            foreach (MiembroDelegacion md in delegacion)
            {
                switch (md.medalla)
                {
                    case Resultados.TipoMedalla.ORO:
                    case Resultados.TipoMedalla.ORO_1:
                    case Resultados.TipoMedalla.ORO_2:
                    case Resultados.TipoMedalla.ORO_3:
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

            return m;
        }

        /// <summary>
        /// Obtiene la tabla de estados de la olimpiada mandada como parametro
        /// </summary>
        /// <param name="tipoOlimpiada">El tipo de olimpiada</param>
        /// <param name="olimpiada">La clave de la olimpiada</param>
        /// <returns>La tabla ordenada de estados</returns>
        public static List<Medallero> obtenerTablaEstados(Olimpiada.TipoOlimpiada tipoOlimpiada, string olimpiada)
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
            query.Append(" order by otros desc ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                Medallero m = new Medallero();
                m.llenarDatos(r);
                // Después de esto solo nos importa la clave del estado, así que nos deshacemos del resto
                m.clave = m.clave.Substring(0, 3);

                lista.Add(m);
            }

            return lista;
        }
    }
}