using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace OMIstats.Models
{
    public class Problema
    {
        public string olimpiada { get; set; }

        public int dia { get; set; }

        public int numero { get; set; }

        public string nombre { get; set; }

        public string url { get; set; }

        public float media { get; set; }

        public int mediana { get; set; }

        public int ceros { get; set; }

        public int perfectos { get; set; }

        private void llenarDatos(DataRow datos)
        {
            olimpiada = datos["olimpiada"].ToString().Trim();
            dia = (int)datos["dia"];
            numero = (int)datos["numero"];
            nombre = datos["nombre"].ToString().Trim();
            url = datos["url"].ToString().Trim();
            media = float.Parse(datos["media"].ToString().Trim());
            perfectos = (int)datos["perfectos"];
            ceros = (int)datos["ceros"];
            mediana = (int)datos["mediana"];
        }

        /// <summary>
        /// Regresa la lista de problemas de la omi y dia
        /// </summary>
        /// <param name="omi">La omi de los problemas</param>
        /// <param name="dia">El dia de los problemas</param>
        /// <returns>La lista de problemas</returns>
        /// <remarks>Siempre se regresara un arreglo con 4 elementos,
        /// de haber menos problemas, el resto de los elmentos tendrá null</remarks>
        public static List<Problema> obtenerProblemasDeOMI(string omi, int dia)
        {
            List<Problema> problemas = new List<Problema>();
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            problemas.Add(null);
            problemas.Add(null);
            problemas.Add(null);
            problemas.Add(null);

            query.Append(" select * from problema where olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(omi));
            query.Append(" and dia = ");
            query.Append(dia);
            query.Append(" order by numero asc ");

            if (db.EjecutarQuery(query.ToString()).error)
                return null;

            DataTable table = db.getTable();
            foreach (DataRow r in table.Rows)
            {
                Problema p = new Problema();
                p.llenarDatos(r);

                problemas[p.numero - 1] = p;
            }

            return problemas;
        }
    }
}