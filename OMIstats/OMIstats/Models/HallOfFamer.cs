using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace OMIstats.Models
{
    public class HallOfFamer
    {
        public Persona persona { get; set; }
        public List<KeyValuePair<Olimpiada, Resultados.TipoMedalla>> medallas = null;
        public int lugar { get; set; }
        public HashSet<string> estados;

        private int oros;
        private int platas;
        private int bronces;

        /// <summary>
        /// Obtiene los multimedallistas de la olimpiada mandada como parámetro
        /// </summary>
        /// <param name="tipoOlimpiada">El tipo de olimpiada</param>
        /// <param name="cabeceras">El número de cabeceras que tendrá la tabla</param>
        /// <param name="excluirNoOros">true si solamente debemos incluir aquellos
        /// medallistas con al menos 1 oro</param>
        /// <param name="estado">Si debemos filtrar a cierto estado en particular</param>
        /// <returns>La lista de competidores con múltiples medallas</returns>
        public static List<HallOfFamer> obtenerMultimedallistas(out int cabeceras, TipoOlimpiada tipoOlimpiada = TipoOlimpiada.OMI, bool excluirNoOros = true, string estado = null)
        {
            List<HallOfFamer> medallistas = new List<HallOfFamer>();

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select clave from Medallero where tipo = ");
            query.Append((int)Medallero.TipoMedallero.PERSONA);
            query.Append(" and ");
            if (excluirNoOros)
                query.Append(" oro > 0 and ");
            query.Append(" (oro + plata + bronce) > 1 and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" order by oro desc, plata desc, bronce desc");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();
            HallOfFamer lastHof = null;

            cabeceras = 0;
            int lugar = 0;
            foreach (DataRow r in table.Rows)
            {
                HallOfFamer hof = new HallOfFamer();
                hof.llenarDatos(int.Parse(r["clave"].ToString()));

                if (estado != null)
                {
                    if (!hof.estados.Contains(estado))
                        continue;
                }

                lugar++;
                if (lastHof == null || lastHof.oros != hof.oros ||
                    lastHof.platas != hof.platas || lastHof.bronces != hof.bronces)
                    hof.lugar = lugar;
                else
                    hof.lugar = lastHof.lugar;

                if (hof.medallas.Count > cabeceras)
                    cabeceras = hof.medallas.Count;

                lastHof = hof;
                medallistas.Add(hof);
            }

            return medallistas;
        }

        private void llenarDatos(int usuario, TipoOlimpiada tipoOlimpiada = TipoOlimpiada.OMI)
        {
            // Primero obtenemos la persona
            this.persona = Persona.obtenerPersonaConClave(usuario);

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select olimpiada, medalla, estado from Resultados where concursante = ");
            query.Append(usuario);
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" order by medalla");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            medallas = new List<KeyValuePair<Olimpiada, Resultados.TipoMedalla>>();
            estados = new HashSet<string>();
            foreach (DataRow r in table.Rows)
            {
                Resultados.TipoMedalla medalla = (Resultados.TipoMedalla)Enum.Parse(typeof(Resultados.TipoMedalla), r["medalla"].ToString().ToUpper());

                if (medalla == Resultados.TipoMedalla.NADA)
                    continue;

                Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(r["olimpiada"].ToString(), tipoOlimpiada);
                medallas.Add(new KeyValuePair<Olimpiada,Resultados.TipoMedalla>(o, medalla));

                string estado = r["estado"].ToString().Trim();
                if (!estados.Contains(estado))
                    estados.Add(estado);

                if (medalla == Resultados.TipoMedalla.BRONCE)
                    bronces++;
                else if (medalla == Resultados.TipoMedalla.PLATA)
                    platas++;
                else
                    oros++;
            }
        }
    }
}