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
        private static string ICON_OMEGAUP = "/img/omega.png";
        private static string ICON_PDF = "/img/pdf.png";
        private static string ICON_KAREL = "/img/karelotitlan.png";
        private static string ICON_OTRO = "/img/link.png";

        public string olimpiada { get; set; }

        public TipoOlimpiada tipoOlimpiada { get; set; }

        public int dia { get; set; }

        public int numero { get; set; }

        [MaxLength(50, ErrorMessage = "El tamaño máximo es de 50 caracteres")]
        public string nombre { get; set; }

        [MaxLength(100, ErrorMessage = "El tamaño máximo es de 100 caracteres")]
        public string url { get; set; }

        [MaxLength(150, ErrorMessage = "El tamaño máximo es de 150 caracteres")]
        public string casos { get; set; }

        [MaxLength(150, ErrorMessage = "El tamaño máximo es de 150 caracteres")]
        public string codigo { get; set; }

        public float media { get; set; }

        public float mediana { get; set; }

        public int ceros { get; set; }

        public int perfectos { get; set; }

        public string getURLIcon()
        {
            if (url.Contains("omegaup"))
                return ICON_OMEGAUP;
            if (url.Contains("pdf"))
                return ICON_PDF;
            if (url.Contains("karelotitlan"))
                return ICON_KAREL;

            return ICON_OTRO;
        }

        private void llenarDatos(DataRow datos)
        {
            olimpiada = datos["olimpiada"].ToString().Trim();
            tipoOlimpiada = (TipoOlimpiada)Enum.Parse(typeof(TipoOlimpiada), datos["clase"].ToString().ToUpper());
            dia = (int)datos["dia"];
            numero = (int)datos["numero"];
            nombre = datos["nombre"].ToString().Trim();
            url = datos["url"].ToString().Trim();
            media = float.Parse(datos["media"].ToString().Trim());
            perfectos = (int)datos["perfectos"];
            ceros = (int)datos["ceros"];
            mediana = float.Parse(datos["mediana"].ToString());
            casos = datos["casos"].ToString().Trim();
            codigo = datos["codigo"].ToString().Trim();
        }

        /// <summary>
        /// Regresa la lista de problemas de la omi y dia
        /// </summary>
        /// <param name="omi">La omi de los problemas</param>
        /// <param name="tipoOlimpiada">El tipo olimpiada del que se requieren los datos</param>
        /// <param name="dia">El dia de los problemas</param>
        /// <returns>La lista de problemas</returns>
        /// <remarks>Siempre se regresara un arreglo con 6 elementos,
        /// de haber menos problemas, el resto de los elmentos tendrá null</remarks>
        public static List<Problema> obtenerProblemasDeOMI(string omi, TipoOlimpiada tipoOlimpiada, int dia)
        {
            List<Problema> problemas = new List<Problema>();
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            problemas.Add(null);
            problemas.Add(null);
            problemas.Add(null);
            problemas.Add(null);
            problemas.Add(null);
            problemas.Add(null);

            query.Append(" select * from problema where olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(omi));
            query.Append(" and dia = ");
            query.Append(dia);
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and numero <> 0 order by numero asc ");

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
        /// Regresa una lista con tres elementos y el metadata de los dias de la omi
        /// </summary>
        /// <param name="omi">La omi de los problemas</param>
        /// <param name="tipoOlimpiada">El tipo olimpiada del que se requieren los datos</param>
        /// <returns>La lista de problemas</returns>
        public static List<Problema> obetnerMetaDatadeOMI(string omi, TipoOlimpiada tipoOlimpiada)
        {
            List<Problema> problemas = new List<Problema>();
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from problema where olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(omi));
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and numero = 0 order by dia asc ");

            if (db.EjecutarQuery(query.ToString()).error)
                return null;

            DataTable table = db.getTable();
            foreach (DataRow r in table.Rows)
            {
                Problema p = new Problema();
                p.llenarDatos(r);

                problemas.Add(p);
            }

            return problemas;
        }

        /// <summary>
        /// Regresa cuántos problemas hay en el día de la omi seleccionada
        /// </summary>
        /// <param name="omi">La omi cuyos problemas se necesitan</param>
        /// <param name="tipoOlimpiada">El tipo de olimpiada</param>
        /// <param name="dia">El dia de los problemas</param>
        /// <returns>Un número entre 0 y 6 con los problemas de la OMI</returns>
        public static int obtenerCantidadDeProblemas(string omi, TipoOlimpiada tipoOlimpiada, int dia)
        {
            List<Problema> lista = obtenerProblemasDeOMI(omi, tipoOlimpiada, dia);
            int i = 0;
            for (i = 0; i < lista.Count; i++)
            {
                if (lista[i] == null)
                    break;
            }
            return i;
        }

        /// <summary>
        /// Obtiene el problema de la base de datos.
        /// De no existir, se regresa un objeto nuevo (sin actualizar la base)
        /// </summary>
        /// <param name="omi">La omi del problema</param>
        /// <param name="tipoOlimpiada">El tipo olimpiada del que se requieren los datos</param>
        /// <param name="dia">El día del problema</param>
        /// <param name="numero">El numero del problema</param>
        /// <returns>El objeto problema</returns>
        public static Problema obtenerProblema(string omi, TipoOlimpiada tipoOlimpiada, int dia, int numero)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from problema where olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(omi));
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
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
                p.tipoOlimpiada = tipoOlimpiada;
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
        /// Regresa todos los problemas del tipo de olimpiada
        /// </summary>
        /// <param name="tipoOlimpiada">El tipo de olimpiada</param>
        /// <returns>El diccionario de problemas, la llave es la olimpiada</returns>
        public static Dictionary<string, List<Problema>> obtenerProblemas(TipoOlimpiada tipoOlimpiada)
        {
            Dictionary<string, List<Problema>> problemas = new Dictionary<string, List<Problema>>();
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from problema where clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" order by olimpiada, dia asc, numero asc ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            string ultimaOMI = null;
            List<Problema> lista = null;
            foreach (DataRow r in table.Rows)
            {
                Problema p = new Problema();
                p.llenarDatos(r);

                if (ultimaOMI == null || ultimaOMI != p.olimpiada)
                {
                    lista = new List<Problema>();
                    problemas.Add(p.olimpiada, lista);
                    ultimaOMI = p.olimpiada;
                }

                lista.Add(p);
            }

            return problemas;
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
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(", ");
            query.Append(dia);
            query.Append(", ");
            query.Append(numero);
            query.Append(", '', '', 0.0, 0, 0, 0, '', '')");

            db.EjecutarQuery(query.ToString());

            query.Clear();

            query.Append(" update problema set nombre = ");
            query.Append(Utilities.Cadenas.comillas(nombre));
            query.Append(", url = ");
            query.Append(Utilities.Cadenas.comillas(url));
            query.Append(", casos = ");
            query.Append(Utilities.Cadenas.comillas(casos));
            query.Append(", codigo = ");
            query.Append(Utilities.Cadenas.comillas(codigo));
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
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and dia = ");
            query.Append(dia);
            query.Append(" and numero = ");
            query.Append(numero);

            db.EjecutarQuery(query.ToString());
        }
    }
}