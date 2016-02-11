using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [MaxLength(50, ErrorMessage = "El tamaño máximo es de 50 caracteres")]
        public string nombre { get; set; }

        [MaxLength(100, ErrorMessage = "El tamaño máximo es de 100 caracteres")]
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

        /// <summary>
        /// Obtiene el problema de la base de datos.
        /// De no existir, se regresa un objeto nuevo (sin actualizar la base)
        /// </summary>
        /// <param name="omi">La omi del problema</param>
        /// <param name="dia">El día del problema</param>
        /// <param name="numero">El numero del problema</param>
        /// <returns>El objeto problema</returns>
        public static Problema obtenerProblema(string omi, int dia, int numero)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from problema where olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(omi));
            query.Append(" and dia = ");
            query.Append(dia);
            query.Append(" and numero = ");
            query.Append(numero);

            if (db.EjecutarQuery(query.ToString()).error)
                return null;

            DataTable table = db.getTable();
            Problema p = new Problema();
            if (table.Rows.Count == 0)
            {
                p.olimpiada = omi;
                p.dia = dia;
                p.numero = numero;
            }
            else
            {
                p.llenarDatos(table.Rows[0]);
            }
            return p;
        }

        /// <summary>
        /// Guarda los datos del objeto en la base de datos
        /// si el objeto no existe, lo crea.
        /// </summary>
        public void guardar()
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" insert into problema values( ");
            query.Append(Utilities.Cadenas.comillas(olimpiada));
            query.Append(", ");
            query.Append(dia);
            query.Append(", ");
            query.Append(numero);
            query.Append(", '', '', 0.0, 0, 0, 0)");

            db.EjecutarQuery(query.ToString());

            query.Clear();

            query.Append(" update problema set nombre = ");
            query.Append(Utilities.Cadenas.comillas(nombre));
            query.Append(", url = ");
            query.Append(Utilities.Cadenas.comillas(url));
            query.Append(", media = ");
            query.Append(media);
            query.Append(", mediana = ");
            query.Append(mediana);
            query.Append(", ceros = ");
            query.Append(ceros);
            query.Append(", perfectos = ");
            query.Append(perfectos);
            query.Append(" where olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(olimpiada));
            query.Append(" and dia = ");
            query.Append(dia);
            query.Append(" and numero = ");
            query.Append(numero);

            db.EjecutarQuery(query.ToString());
        }
    }
}