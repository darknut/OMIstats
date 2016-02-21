using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace OMIstats.Models
{
    public class Resultados
    {
        public enum TipoMedalla
        {
            NULL,
            ORO_1,
            ORO_2,
            ORO_3,
            ORO,
            PLATA,
            BRONCE,
            NADA,
        }

        public enum TipoError
        {
            OK,
            FALTAN_CAMPOS,
            CLAVE_INEXISTENTE,
            ESPERABA_NUMEROS
        }

        // Este objeto debe de ser contenido por un objeto olimpiada,
        // por eso no cargamos un objeto olimpiada aqui

        public int usuario;
        public string estado;
        public string clave;
        public List<int> dia1;
        public int totalDia1;
        public List<int> dia2;
        public int totalDia2;
        public int total;
        public TipoMedalla medalla;
        public bool publico;
        public string ioi;

        private bool eliminar;

        public Resultados()
        {
            usuario = 0;
            estado = "";
            clave = "";
            dia1 = null;
            totalDia1 = 0;
            dia2 = null;
            totalDia2 = 0;
            total = 0;
            medalla = TipoMedalla.NULL;
            publico = false;
            ioi = "";
        }

        private void llenarDatos(DataRow row)
        {
            dia1 = new List<int>();
            dia1.Add(0);
            dia1.Add(0);
            dia1.Add(0);
            dia1.Add(0);
            dia1.Add(0);
            dia1.Add(0);

            dia2 = new List<int>();
            dia2.Add(0);
            dia2.Add(0);
            dia2.Add(0);
            dia2.Add(0);
            dia2.Add(0);
            dia2.Add(0);

            usuario = (int)row["concursante"];
            clave = row["clave"].ToString().Trim();
            estado = row["estado"].ToString().Trim();
            dia1[0] = (int)row["puntosD1P1"];
            dia1[1] = (int)row["puntosD1P2"];
            dia1[2] = (int)row["puntosD1P3"];
            dia1[3] = (int)row["puntosD1P4"];
            dia1[4] = (int)row["puntosD1P5"];
            dia1[5] = (int)row["puntosD1P6"];
            totalDia1 = (int)row["puntosD1"];
            dia2[0] = (int)row["puntosD2P1"];
            dia2[1] = (int)row["puntosD2P2"];
            dia2[2] = (int)row["puntosD2P3"];
            dia2[3] = (int)row["puntosD2P4"];
            dia2[4] = (int)row["puntosD2P5"];
            dia2[5] = (int)row["puntosD2P6"];
            totalDia2 = (int)row["puntosD2"];
            total = (int)row["puntos"];
            medalla = (TipoMedalla)Enum.Parse(typeof(TipoMedalla), row["medalla"].ToString().ToUpper());
            publico = (bool)row["publico"];
            ioi = row["ioi"].ToString().Trim();
        }

        /// <summary>
        /// Regresa los resultados de la olimpiada mandada como parametro
        /// </summary>
        /// <param name="omi">La olimpiada en cuestión</param>
        /// <param name="tipoOlimpiada">El tipo de olimpiada</param>
        /// <returns>Una lista con los resultados</returns>
        public static List<Resultados> cargarResultados(string omi, Olimpiada.TipoOlimpiada tipoOlimpiada)
        {
            List<Resultados> lista = new List<Resultados>();
            if (omi == null)
                return null;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from resultados ");
            query.Append(" where olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(omi));
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" order by puntos desc, clave asc");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                Resultados res = new Resultados();
                res.llenarDatos(r);

                lista.Add(res);
            }

            return lista;
        }

        /// <summary>
        /// Obtiene un string separado con comas, con todos los datos en el objeto
        /// </summary>
        /// <param name="problemasDia1">El número de problemas en el día 1</param>
        /// <param name="problemasDia1">El número de problemas en el día 2</param>
        /// <returns>El string para la tabla</returns>
        public string obtenerLineaAdmin(int problemasDia1, int problemasDia2)
        {
            StringBuilder s = new StringBuilder();

            s.Append(clave);
            s.Append(", ");

            for (int i = 0; i < problemasDia1; i++)
            {
                s.Append(dia1[i]);
                s.Append(", ");
            }

            s.Append(totalDia1);
            s.Append(", ");

            for (int i = 0; i < problemasDia2; i++)
            {
                s.Append(dia2[i]);
                s.Append(", ");
            }

            s.Append(totalDia2);
            s.Append(", ");
            s.Append(total);
            s.Append(", ");
            s.Append(medalla.ToString().ToLower());
            s.Append(", ");
            s.Append(ioi);

            return s.ToString();
        }
    }
}