using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace OMIstats.Models
{
    public class Olimpiada
    {
        [Required(ErrorMessage = "Campo requerido")]
        [MaxLength(3, ErrorMessage = "El tamaño máximo es 3 caracteres")]
        public string numero { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [MaxLength(30, ErrorMessage = "El tamaño máximo es 30 caracteres")]
        public string ciudad { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [MaxLength(3, ErrorMessage = "El tamaño máximo es 3 caracteres")]
        public string claveEstado { get; set; }

        public string nombreEstado { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public float año { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public DateTime inicio { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public DateTime fin { get; set; }

        public float media { get; set; }

        public int mediana { get; set; }

        [MaxLength(100, ErrorMessage = "El tamaño máximo es 100 caracteres")]
        public string video { get; set; }

        [MaxLength(100, ErrorMessage = "El tamaño máximo es 100 caracteres")]
        public string poster { get; set; }

        public int estados { get; set; }

        public bool datosPublicos { get; set; }

        public int participantes { get; set; }

        [MaxLength(100, ErrorMessage = "El tamaño máximo es 100 caracteres")]
        public string nombreEscuela { get; set; }

        public int claveEscuela { get; set; }

        public string nombreEscuelaCompleto { get; set; }

        public string escuelaURL { get; set; }

        public string friendlyDate { get; set; }

        public string logo { get; set; }

        public List<MiembroDelegacion> asistentes;

        public Olimpiada()
        {
            numero = "";
            ciudad = "";
            año = 0;
            inicio = new DateTime(1990, 1, 1);
            fin = new DateTime(1990, 1, 1);
            media = 0;
            mediana = 0;
            video = "";
            poster = "";
            estados = 0;
            participantes = 0;
            claveEstado = "";
            nombreEstado = "";
            claveEscuela = 0;
            nombreEscuela = "";
            friendlyDate = "";
            logo = "";

            asistentes = null;
        }

        private void llenarDatos(DataRow datos)
        {
            numero = datos["numero"].ToString().Trim();
            ciudad = datos["ciudad"].ToString().Trim();
            año = float.Parse(datos["año"].ToString().Trim());
            inicio = Utilities.Fechas.stringToDate(datos["inicio"].ToString().Trim());
            fin = Utilities.Fechas.stringToDate(datos["fin"].ToString().Trim());
            media = float.Parse(datos["media"].ToString().Trim());
            mediana = (int)datos["mediana"];
            video = datos["video"].ToString().Trim();
            poster = datos["poster"].ToString().Trim();
            estados = (int)datos["estados"];
            participantes = (int)datos["participantes"];
            datosPublicos = (bool)datos["datospublicos"];

            claveEstado = datos["estado"].ToString().Trim();
            Estado estado = Estado.obtenerEstadoConClave(claveEstado);
            if (estado != null)
                nombreEstado = estado.nombre;

            claveEscuela = (int)datos["escuela"];
            Institucion institucion = Institucion.obtenerInstitucionConClave(claveEscuela);
            if (institucion != null)
            {
                nombreEscuela = institucion.nombreCorto;
                escuelaURL = institucion.nombreURL;
                nombreEscuelaCompleto = institucion.nombre;
            }

            if (inicio.Year > 1990)
            {
                if (inicio.Month == fin.Month)
                    friendlyDate = "Del " + inicio.Day +
                                    " al " + Utilities.Fechas.friendlyString(fin);
                else
                    friendlyDate = "Del " + Utilities.Fechas.friendlyString(inicio) +
                                   " al " + Utilities.Fechas.friendlyString(fin);
            }

            if (Utilities.Archivos.existeArchivo(Utilities.Archivos.FolderImagenes.OLIMPIADAS,
                numero + ".png"))
                logo = numero + ".png";
            else
                logo = "omi.png";
        }

        public static List<Olimpiada> obtenerOlimpiadas()
        {
            List<Olimpiada> lista = new List<Olimpiada>();
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from olimpiada order by año desc");

            if (db.EjecutarQuery(query.ToString()).error)
                return lista;

            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                Olimpiada o = new Olimpiada();
                o.llenarDatos(r);

                lista.Add(o);
            }

            return lista;
        }

        /// <summary>
        /// Regresa el objeto olimpiada relacionado con la clave mandada como parametro
        /// </summary>
        /// <param name="clave">La clave de la olimpiada</param>
        /// <returns>El objeto olimpiada</returns>
        public static Olimpiada obtenerOlimpiadaConClave(string clave)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from olimpiada where numero = ");
            query.Append(Utilities.Cadenas.comillas(clave));

            if (db.EjecutarQuery(query.ToString()).error)
                return null;

            DataTable table = db.getTable();
            if (table.Rows.Count != 1)
                return null;

            Olimpiada o = new Olimpiada();
            o.llenarDatos(table.Rows[0]);

            return o;
        }

        /// <summary>
        /// Guarda los datos del objeto en la base de datos
        /// </summary>
        /// <param name="clave">La nueva clave para el objeto</param>
        /// <returns>Si se guardó satisfactoriamente el objeto</returns>
        /// <remarks>Crea un nuevo objeto instutucion si la institucion
        /// referenciada no existe</remarks>
        public bool guardarDatos(string clave = null)
        {
            if (clave == null)
                clave = numero;

            if (claveEscuela == 0)
            {
                if (!String.IsNullOrEmpty(nombreEscuela))
                {
                    Institucion i = Institucion.obtenerInstitucionConNombreCorto(nombreEscuela);
                    if (i == null)
                    {
                        i = new Institucion();
                        i.nombre = nombreEscuela;
                        if (!i.nuevaInstitucion())
                            return false;
                    }
                    claveEscuela = i.clave;
                }
            }

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" update olimpiada set numero = ");
            query.Append(Utilities.Cadenas.comillas(numero));
            query.Append(", año = ");
            query.Append(año);
            query.Append(", estado = ");
            query.Append(Utilities.Cadenas.comillas(claveEstado));
            query.Append(", ciudad = ");
            query.Append(Utilities.Cadenas.comillas(ciudad));
            query.Append(", inicio = ");
            query.Append(Utilities.Cadenas.comillas(Utilities.Fechas.dateToString(inicio)));
            query.Append(", fin = ");
            query.Append(Utilities.Cadenas.comillas(Utilities.Fechas.dateToString(fin)));
            query.Append(", escuela = ");
            query.Append(claveEscuela);
            query.Append(", video = ");
            query.Append(Utilities.Cadenas.comillas(video));
            query.Append(", poster = ");
            query.Append(Utilities.Cadenas.comillas(poster));
            query.Append(", media = ");
            query.Append(media);
            query.Append(", mediana = ");
            query.Append(mediana);
            query.Append(", estados = ");
            query.Append(estados);
            query.Append(", participantes = ");
            query.Append(participantes);
            query.Append(", datospublicos = ");
            query.Append(datosPublicos ? 1 : 0);
            query.Append(" where numero = ");
            query.Append(Utilities.Cadenas.comillas(clave));

            return !db.EjecutarQuery(query.ToString()).error;
        }

        /// <summary>
        /// Crea una nueva OMI en el sitio completamente vacia con clave TMP
        /// </summary>
        public static void nuevaOMI()
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" insert into olimpiada values ('TMP', '', 'MEX', '0',");
            query.Append(" '', '', 0, 0, '', '', 0, 0, 0) ");

            db.EjecutarQuery(query.ToString());
        }

        /// <summary>
        /// Regresa la tabla de asistentes en un formato tabulado con comas
        /// para la edición manual para admins
        /// </summary>
        /// <returns>La tabla tabulada con comas</returns>
        public string obtenerTablaAsistentes()
        {
            if (asistentes == null)
                asistentes = MiembroDelegacion.cargarAsistentesOMI(numero);

            StringBuilder tabla = new StringBuilder();

            foreach (MiembroDelegacion asistente in asistentes)
            {
                tabla.Append(asistente.obtenerLineaAdmin());
                tabla.Append("\n");
            }

            return tabla.ToString();
        }
    }
}