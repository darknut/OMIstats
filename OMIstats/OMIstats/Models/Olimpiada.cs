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
        public const string TEMP_CLAVE = "TMP";

        [Required(ErrorMessage = "Campo requerido")]
        [MaxLength(3, ErrorMessage = "El tamaño máximo es 3 caracteres")]
        public string numero { get; set; }

        public TipoOlimpiada tipoOlimpiada { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [MaxLength(30, ErrorMessage = "El tamaño máximo es 30 caracteres")]
        public string ciudad { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [MaxLength(3, ErrorMessage = "El tamaño máximo es 3 caracteres")]
        public string claveEstado { get; set; }

        public string pais { get; set; }

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

        [MaxLength(100, ErrorMessage = "El tamaño máximo es 100 caracteres")]
        public string reporte { get; set; }

        public int estados { get; set; }

        public bool datosPublicos { get; set; }

        [MaxLength(3, ErrorMessage = "El tamaño máximo es 3 caracteres")]
        public string relacion { get; set; }

        public int participantes { get; set; }

        [MaxLength(100, ErrorMessage = "El tamaño máximo es 100 caracteres")]
        public string nombreEscuela { get; set; }

        public int claveEscuela { get; set; }

        public string nombreEscuelaCompleto { get; set; }

        public string escuelaURL { get; set; }

        public string friendlyDate { get; set; }

        public string logo { get; set; }

        private List<MiembroDelegacion> asistentes;
        private List<Resultados> resultados;

        public enum TipoOlimpiada
        {
            NULL,
            OMI,
            IOI
        }

        public Olimpiada()
        {
            numero = "";
            tipoOlimpiada = TipoOlimpiada.NULL;
            ciudad = "";
            pais = "";
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
            relacion = "";
            reporte = "";

            asistentes = null;
        }

        private void llenarDatos(DataRow datos)
        {
            numero = datos["numero"].ToString().Trim();
            tipoOlimpiada = (TipoOlimpiada)Enum.Parse(typeof(TipoOlimpiada), datos["clase"].ToString().ToUpper());
            ciudad = datos["ciudad"].ToString().Trim();
            pais = datos["pais"].ToString().Trim();
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
            relacion = datos["relacion"].ToString().Trim();
            reporte = datos["reporte"].ToString().Trim();

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

        /// <summary>
        /// Regresa todas las olimpiadas del tipo mandado como parametro
        /// </summary>
        /// <param name="tipoOlimpiada">El tipo de olimpiada que se busca</param>
        /// <returns></returns>
        public static List<Olimpiada> obtenerOlimpiadas(TipoOlimpiada tipoOlimpiada)
        {
            List<Olimpiada> lista = new List<Olimpiada>();
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from olimpiada where clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" order by año desc");

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
        /// <param name="tipoOlimpiada">El tipo de olimpiada que se busca</param>
        /// <returns>El objeto olimpiada</returns>
        public static Olimpiada obtenerOlimpiadaConClave(string clave, TipoOlimpiada tipoOlimpiada)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from olimpiada where numero = ");
            query.Append(Utilities.Cadenas.comillas(clave));
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));

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
            query.Append(", clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(", año = ");
            query.Append(año);
            query.Append(", estado = ");
            query.Append(Utilities.Cadenas.comillas(claveEstado));
            query.Append(", pais = ");
            query.Append(Utilities.Cadenas.comillas(pais));
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
            query.Append(", relacion = ");
            query.Append(Utilities.Cadenas.comillas(relacion));
            query.Append(", reporte = ");
            query.Append(Utilities.Cadenas.comillas(reporte));
            query.Append(" where numero = ");
            query.Append(Utilities.Cadenas.comillas(clave));

            return !db.EjecutarQuery(query.ToString()).error;
        }

        /// <summary>
        /// Crea una nueva OMI en el sitio completamente vacia con clave TMP
        /// </summary>
        /// <param name="tipoOlimpiada">El tipo de olimpiada que se quiere crear</param>
        public static void nuevaOMI(TipoOlimpiada tipoOlimpiada)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" insert into olimpiada values (");
            query.Append(Utilities.Cadenas.comillas(TEMP_CLAVE));
            query.Append(", ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(",'', 'MEX', 'México' , '0'");
            query.Append(",'', '', 0, 0, '', '', '', 0, 0, 0, 0, '') ");

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
                asistentes = MiembroDelegacion.cargarAsistentesOMI(numero, tipoOlimpiada);

            StringBuilder tabla = new StringBuilder();

            foreach (MiembroDelegacion asistente in asistentes)
            {
                tabla.Append(asistente.obtenerLineaAdmin());
                tabla.Append("\n");
            }

            return tabla.ToString();
        }

        /// <summary>
        /// Regresa la tabla de puntos en formato tabulado, con el número de problemas
        /// mandado como parámetro
        /// </summary>
        /// <returns>La tabla con los resultados</returns>
        public string obtenerResultadosAdmin()
        {
            if (resultados == null)
                resultados = Resultados.cargarResultados(numero, tipoOlimpiada);

            StringBuilder tabla = new StringBuilder();
            int problemasDia1 = Problema.obtenerCantidadDeProblemas(numero, tipoOlimpiada, 1);
            int problemasDia2 = Problema.obtenerCantidadDeProblemas(numero, tipoOlimpiada, 2);

            foreach (Resultados resultado in resultados)
            {
                tabla.Append(resultado.obtenerLineaAdmin(problemasDia1, problemasDia2));
                tabla.Append("\n");
            }

            return tabla.ToString();
        }

        /// <summary>
        /// Guarda en la base de datos la tabla de asistentes
        /// </summary>
        /// <param name="tabla">La nueva tabla de asistentes, un asistente por renglon
        /// y tabulada con comas</param>
        /// <returns>Los registros que ocasionaron error</returns>
        public string guardarTablaAsistentes(string tabla)
        {
            StringBuilder errores = new StringBuilder();
            string[] lineas;

            lineas = tabla.Split('\n');
            foreach (string linea in lineas)
            {
                MiembroDelegacion.TipoError error = MiembroDelegacion.guardarLineaAdmin(numero, tipoOlimpiada, linea.Trim());
                if (error != MiembroDelegacion.TipoError.OK)
                {
                    errores.Append(linea.Trim());
                    errores.Append(": ");
                    errores.Append(error.ToString());
                    errores.Append("\n");
                }
            }

            return errores.ToString();
        }

        /// <summary>
        /// Guarda en la base de datos la tabla de resultados
        /// </summary>
        /// <param name="tabla">La nueva tabla de resultados, un competidor por renglon
        /// y tabulada con comas</param>
        /// <returns>Los registros que ocasionaron error</returns>
        public string guardarTablaResultados(string tabla)
        {
            StringBuilder errores = new StringBuilder();
            string[] lineas;

            lineas = tabla.Split('\n');
            int problemasDia1 = Problema.obtenerCantidadDeProblemas(numero, tipoOlimpiada, 1);
            int problemasDia2 = Problema.obtenerCantidadDeProblemas(numero, tipoOlimpiada, 2);

            foreach (string linea in lineas)
            {
                Resultados.TipoError error = Resultados.guardarLineaAdmin(numero, tipoOlimpiada, problemasDia1, problemasDia2, linea.Trim());
                if (error != Resultados.TipoError.OK)
                {
                    errores.Append(linea.Trim());
                    errores.Append(": ");
                    errores.Append(error.ToString());
                    errores.Append("\n");
                }
            }

            return errores.ToString();
        }

        /// <summary>
        /// Calcula los campos calculados de olimpiadas y problemas
        /// </summary>
        public void calcularNumeros()
        {
            estados = MiembroDelegacion.obtenerEstadosParticipantes(numero, tipoOlimpiada);
            participantes = MiembroDelegacion.obtenerParticipantes(numero, tipoOlimpiada);
            mediana = Resultados.obtenerPrimerBronce(numero, tipoOlimpiada);
            int suma = Resultados.obtenerPuntosTotales(numero, tipoOlimpiada);

            if (participantes > 0)
                media = suma * 1f / participantes;

            guardarDatos();

            // -TODO- Agregar calcualr problemas
        }
    }
}
