using System;
using System.Collections.Generic;
#if OMISTATS
using System.ComponentModel.DataAnnotations;
using OMIstats.Ajax;
#endif
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace OMIstats.Models
{
    public class Olimpiada
    {
#if OMISTATS
        [Required(ErrorMessage = "Campo requerido")]
        [MaxLength(3, ErrorMessage = "El tamaño máximo es 3 caracteres")]
#endif
        public string numero { get; set; }

        public TipoOlimpiada tipoOlimpiada { get; set; }

        public int estados { get; set; }
        public int participantes { get; set; }
        public int problemasDia1 { get; set; }
        public int problemasDia2 { get; set; }

        public bool noMedallistasConocidos { get; set; }
        public bool puntosDesconocidos { get; set; }

#if OMISTATS
        public const string TEMP_CLAVE = "TMP";
        private const int PUNTOS_MINIMOS_CONOCIDOS = 100;
        private const int OMIS_SIN_OMIPS = 20;

        private const string APPLICATION_OMI = "OlimpiadasOMI";
        private const string APPLICATION_OMIS = "OlimpiadasOMIS";
        private const string APPLICATION_OMIP = "OlimpiadasOMIP";

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

        [MaxLength(100, ErrorMessage = "El tamaño máximo es 100 caracteres")]
        public string video { get; set; }

        [MaxLength(100, ErrorMessage = "El tamaño máximo es 100 caracteres")]
        public string poster { get; set; }

        [MaxLength(100, ErrorMessage = "El tamaño máximo es 100 caracteres")]
        public string reporte { get; set; }

        public bool datosPublicos { get; set; }

        [MaxLength(3, ErrorMessage = "El tamaño máximo es 3 caracteres")]
        public string relacion { get; set; }

        [MaxLength(100, ErrorMessage = "El tamaño máximo es 100 caracteres")]
        public string nombreEscuela { get; set; }

        public int claveEscuela { get; set; }

        public string nombreEscuelaCompleto { get; set; }

        public string escuelaURL { get; set; }

        public string friendlyDate { get; set; }

        public string logo { get; set; }

        public bool mostrarResultadosPorDia { get; set; }

        public bool mostrarResultadosPorProblema { get; set; }

        public bool mostrarResultadosTotales { get; set; }

        public bool alsoOmips { get; set; }

        public string omisActualNumber { get; set; }

        public bool liveResults { get; set; }

        public bool puntosDetallados { get; set; }

        public bool registroActivo { get; set; }

        public bool diplomasOnline { get; set; }

        public float media
        {
            get
            {
                if (datosGenerales != null)
                    return datosGenerales.media;
                return 0;
            }
        }

        public float mediana
        {
            get
            {
                if (datosGenerales != null)
                    return datosGenerales.mediana;
                return 0;
            }
        }

        private Problema datosGenerales;

        public List<CachedResult> cachedResults = null;
        public List<Resultados> resultados = null;
        private DateTime lastUpdate;

        /// <summary>
        /// Calcula los campos que aparecen el el footer de la tabla de resultados
        /// Calcula media, mediana, perfectos, ceros de cada problema, día y general
        /// así como número de estados y número de competidores
        /// </summary>
        public void calcularNumeros()
        {
            Problema prob;

            estados = Resultados.obtenerEstadosParticipantes(numero, tipoOlimpiada);
            participantes = MiembroDelegacion.obtenerParticipantes(numero, tipoOlimpiada);

            int[] problemasDia = new int[3];

            // Calculamos las estadisticas por dia y por competencia y las guardamos en la base
            for (int i = 1; i <= 2; i++)
            {
                problemasDia[i] = Problema.obtenerCantidadDeProblemas(numero, tipoOlimpiada, i);

                prob = Resultados.calcularNumeros(numero, tipoOlimpiada, dia: i, totalProblemas: problemasDia[i]);
                prob.guardar(guardarTodo: false);

                List<Problema> lista = Problema.obtenerProblemasDeOMI(numero, tipoOlimpiada, i);
                foreach (Problema p in lista)
                    if (p != null)
                    {
                        Problema pp = Resultados.calcularNumeros(numero, tipoOlimpiada, p.dia, p.numero);
                        p.media = pp.media;
                        p.mediana = pp.mediana;
                        p.perfectos = pp.perfectos;
                        p.ceros = pp.ceros;
                        p.guardar(guardarTodo: false);
                    }
            }

            problemasDia1 = problemasDia[1];
            problemasDia2 = problemasDia[2];
            prob = Models.Resultados.calcularNumeros(numero, tipoOlimpiada, totalProblemas: problemasDia1 + problemasDia2);
            prob.guardar(guardarTodo: false);

            // Guardar en la base
            this.guardarDatos();
        }
#endif

        public static void guardaProblemas(string olimpiada, TipoOlimpiada tipoOlimpiada, int problemas, int dia)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" update olimpiada set datospublicos = 1 ");
            if (dia == 1)
                query.Append(", problemasDia1 = ");
            else
                query.Append(", problemasDia2 = ");
            query.Append(problemas);
            query.Append(", mostrarResultadosPorDia = ");
            query.Append(dia == 1 ? "0" : "1");
            query.Append(", mostrarResultadosPorProblema = 1 ");
            query.Append(", mostrarResultadosTotales = 1 ");
            query.Append(" where numero = ");
            query.Append(Utilities.Cadenas.comillas(olimpiada));
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));

            db.EjecutarQuery(query.ToString());
        }

#if OMISTATS
        private void cacheResults(int dia)
        {
            resultados = Resultados.cargarResultados(this.numero, this.tipoOlimpiada, dia,
                dia == 1 ? this.problemasDia1 : this.problemasDia2, out cachedResults);
        }

        public OmegaUp calculateCachedResults()
        {
            try
            {
                OmegaUp poll = OmegaUp.obtenerParaOMI(this.numero, this.tipoOlimpiada);
                if (poll == null)
                {
                    this.liveResults = false;
                    return poll;
                }

                if (this.cachedResults == null || resultados == null)
                {
                    cacheResults(poll.dia);
                    this.lastUpdate = poll.timestamp;
                    return poll;
                }

                if (poll.timestamp.CompareTo(lastUpdate) <= 0)
                    return poll;

                this.lastUpdate = poll.timestamp;
                cacheResults(poll.dia);

                return poll;
            }
            catch (Exception e)
            {
                Log.add(Log.TipoLog.SCOREBOARD, "Excepción tratando de cachear resultados para " + this.tipoOlimpiada + " " + this.numero);
                Log.add(Log.TipoLog.SCOREBOARD, e.ToString());

                return null;
            }
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
            problemasDia1 = 0;
            problemasDia2 = 0;
            mostrarResultadosPorDia = false;
            mostrarResultadosPorProblema = false;
            mostrarResultadosTotales = false;
            puntosDesconocidos = false;
            alsoOmips = false;
            noMedallistasConocidos = false;
            omisActualNumber = "";
            registroActivo = false;
            puntosDetallados = false;
            diplomasOnline = false;

            liveResults = false;
        }

        public static string obtenerApplicationString(TipoOlimpiada tipoOlimpiada)
        {
            switch (tipoOlimpiada)
            {
                case TipoOlimpiada.OMI:
                    return APPLICATION_OMI;
                case TipoOlimpiada.OMIP:
                    return APPLICATION_OMIP;
                case TipoOlimpiada.OMIS:
                    return APPLICATION_OMIS;
            }

            return "";
        }

        private static Dictionary<string, Olimpiada> cargarOlimpiadas(TipoOlimpiada tipoOlimpiada)
        {
            Dictionary<string, Olimpiada> lista = new Dictionary<string, Olimpiada>();
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            TipoOlimpiada tipoQuery = tipoOlimpiada;

            query.Append(" select * from olimpiada ");
            query.Append(" where clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoQuery.ToString().ToLower()));
            query.Append(" order by año desc");

            db.EjecutarQuery(query.ToString());

            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                Olimpiada o = new Olimpiada();
                o.llenarDatos(r);

                lista.Add(o.numero, o);
            }

            return lista;
        }

        /// <summary>
        /// Dado que las olimpiadas son pocas y a que se hacen muchas llamadas a la base para obtener estos objetos
        /// los cargamos una vez por aplicacion y los dejamos ahi
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, Olimpiada> getOlimpiadas(TipoOlimpiada tipoOlimpiada)
        {
            Dictionary<string, Olimpiada> olimpiadas;
            string application = obtenerApplicationString(tipoOlimpiada);

            olimpiadas = (Dictionary<string, Olimpiada>)HttpContext.Current.Application[application];

            if (olimpiadas == null)
            {
                olimpiadas = cargarOlimpiadas(tipoOlimpiada);
                HttpContext.Current.Application[application] = olimpiadas;

                // Cargamos las instrucciones en caso de que haya un scoreboard activo
                List<OmegaUp> polls = OmegaUp.obtenerInstrucciones(OmegaUp.Instruccion.POLL);

                foreach (OmegaUp p in polls)
                {
                    if (p.tipoOlimpiada == tipoOlimpiada)
                    {
                        Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(p.olimpiada, p.tipoOlimpiada);
                        o.liveResults = true;
                    }
                }
            }

            return olimpiadas;
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
            video = datos["video"].ToString().Trim();
            poster = datos["poster"].ToString().Trim();
            estados = (int)datos["estados"];
            participantes = (int)datos["participantes"];
            datosPublicos = (bool)datos["datospublicos"];
            relacion = datos["relacion"].ToString().Trim();
            reporte = datos["reporte"].ToString().Trim();
            problemasDia1 = (int)datos["problemasDia1"];
            problemasDia2 = (int)datos["problemasDia2"];
            mostrarResultadosPorDia = (bool)datos["mostrarResultadosPorDia"];
            mostrarResultadosPorProblema = (bool)datos["mostrarResultadosPorProblema"];
            mostrarResultadosTotales = (bool)datos["mostrarResultadosTotales"];
            puntosDesconocidos = (bool)datos["puntosDesconocidos"];
            alsoOmips = (bool)datos["alsoOmips"];
            noMedallistasConocidos = (bool)datos["noMedallistasConocidos"];
            puntosDetallados = (bool)datos["puntosDetallados"];
            registroActivo = (bool)datos["registroActivo"];
            diplomasOnline = (bool)datos["diplomasOnline"];

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
                logo = Utilities.Archivos.OMI_LOGO;

            if (numero != TEMP_CLAVE &&
                (tipoOlimpiada == TipoOlimpiada.OMIP ||
                tipoOlimpiada == TipoOlimpiada.OMIS))
                omisActualNumber = (Int32.Parse(numero) - OMIS_SIN_OMIPS).ToString();

            datosGenerales = Problema.obtenerProblema(numero, tipoOlimpiada, 0, 0);
        }

        /// <summary>
        /// Regresa todas las olimpiadas del tipo mandado como parametro
        /// </summary>
        /// <param name="tipoOlimpiada">El tipo de olimpiada que se busca</param>
        /// <returns></returns>
        public static List<Olimpiada> obtenerOlimpiadas(TipoOlimpiada tipoOlimpiada)
        {
            return getOlimpiadas(tipoOlimpiada).Values.ToList();
        }

        /// <summary>
        /// Regresa el objeto olimpiada relacionado con la clave mandada como parametro
        /// </summary>
        /// <param name="clave">La clave de la olimpiada</param>
        /// <param name="tipoOlimpiada">El tipo de olimpiada que se busca</param>
        /// <returns>El objeto olimpiada</returns>
        public static Olimpiada obtenerOlimpiadaConClave(string clave, TipoOlimpiada tipoOlimpiada)
        {
            Dictionary<string, Olimpiada> olimpiadas = getOlimpiadas(tipoOlimpiada);

            Olimpiada o = null;

            if (clave == null)
                return null;

            olimpiadas.TryGetValue(clave.Trim(), out o);

            return o;
        }

        /// <summary>
        /// Borra la referencia en la aplicación para poder recargar olimpiadas
        /// </summary>
        public static void resetOMIs(TipoOlimpiada tipoOlimpiada)
        {
            HttpContext.Current.Application[obtenerApplicationString(tipoOlimpiada)] = null;
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
            query.Append(", problemasDia1 = ");
            query.Append(problemasDia1);
            query.Append(", problemasDia2 = ");
            query.Append(problemasDia2);
            query.Append(", mostrarResultadosPorDia = ");
            query.Append(mostrarResultadosPorDia ? 1 : 0);
            query.Append(", mostrarResultadosPorProblema = ");
            query.Append(mostrarResultadosPorProblema ? 1 : 0);
            query.Append(", mostrarResultadosTotales = ");
            query.Append(mostrarResultadosTotales ? 1 : 0);
            query.Append(", puntosDesconocidos = ");
            query.Append(puntosDesconocidos ? 1 : 0);
            query.Append(", alsoOmips = ");
            query.Append(alsoOmips ? 1 : 0);
            query.Append(", noMedallistasConocidos = ");
            query.Append(noMedallistasConocidos ? 1 : 0);
            query.Append(", puntosDetallados = ");
            query.Append(puntosDetallados ? 1 : 0);
            query.Append(", registroActivo = ");
            query.Append(registroActivo ? 1 : 0);
            query.Append(", diplomasOnline = ");
            query.Append(diplomasOnline ? 1 : 0);
            query.Append(" where numero = ");
            query.Append(Utilities.Cadenas.comillas(clave));
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));

            if (db.EjecutarQuery(query.ToString()).error)
                return false;

            // Si esta omi es tambien OMIPS, creamos tambien los objetos
            if (tipoOlimpiada == TipoOlimpiada.OMI && this.alsoOmips)
            {
                this.actualizaOMIPS(TipoOlimpiada.OMIP, clave);
                this.actualizaOMIPS(TipoOlimpiada.OMIS, clave);
            }

            // Borramos la referencia en la aplicacion para que el siguiente query recargue las olimpiadas
            resetOMIs(this.tipoOlimpiada);

            return true;
        }

        private void actualizaOMIPS(TipoOlimpiada tipoOlimpiada, string clave)
        {
            Olimpiada omi = obtenerOlimpiadaConClave(this.numero, tipoOlimpiada);

            if (omi == null)
            {
                nuevaOMI(tipoOlimpiada);
                omi = obtenerOlimpiadaConClave(TEMP_CLAVE, tipoOlimpiada);
                clave = TEMP_CLAVE;
            }

            omi.numero = this.numero;
            omi.año = this.año;
            omi.claveEstado = this.claveEstado;
            omi.ciudad = this.ciudad;
            omi.inicio = this.inicio;
            omi.fin = this.fin;
            omi.datosPublicos = this.datosPublicos;
            omi.puntosDesconocidos = this.puntosDesconocidos;
            omi.alsoOmips = this.alsoOmips;
            omi.claveEscuela = this.claveEscuela;
            omi.relacion = this.relacion;
            omi.video = this.video;
            omi.reporte = this.reporte;
            omi.logo = this.logo;
            omi.poster = this.poster;
            omi.puntosDetallados = this.puntosDetallados;
            omi.registroActivo = this.registroActivo;
            omi.diplomasOnline = this.diplomasOnline;

            omi.guardarDatos(clave);
        }

        /// <summary>
        /// Crea una nueva OMI en el sitio completamente vacia con clave TMP
        /// </summary>
        /// <param name="tipoOlimpiada">El tipo de olimpiada que se quiere crear</param>
        public static void nuevaOMI(TipoOlimpiada tipoOlimpiada)
        {
            // Borramos la referencia en la aplicacion para que el siguiente query recargue las olimpiadas
            resetOMIs(tipoOlimpiada);

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" insert into olimpiada values (");
            query.Append(Utilities.Cadenas.comillas(TEMP_CLAVE));
            query.Append(", ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(",'', 'MEX', 'México' , '0'");
            query.Append(",'', '', '', '', '', 0, 0, 0, 0, '', 0, 0, 0, 0, 1, 0, 0, 1, 1, 0, 0) ");

            db.EjecutarQuery(query.ToString());
        }

        /// <summary>
        /// Regresa la tabla de asistentes en un formato tabulado con comas
        /// para la edición manual para admins
        /// </summary>
        /// <returns>La tabla tabulada con comas</returns>
        public string obtenerTablaAsistentes()
        {
            List<MiembroDelegacion> asistentes = MiembroDelegacion.cargarAsistentesOMI(numero, tipoOlimpiada);

            StringBuilder tabla = new StringBuilder();

            foreach (MiembroDelegacion asistente in asistentes)
            {
                tabla.Append(asistente.obtenerLineaAdmin());
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
                int result = MiembroDelegacion.guardarLineaAdmin(numero, tipoOlimpiada, linea.Trim());
                MiembroDelegacion.TipoError error = result >= Persona.PrimerClave ?
                    MiembroDelegacion.TipoError.OK : (MiembroDelegacion.TipoError)result;
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
        /// Regresa la tabla de puntos en formato tabulado, con el número de problemas
        /// mandado como parámetro
        /// </summary>
        /// <returns>La tabla con los resultados</returns>
        public string obtenerResultadosAdmin()
        {
            List<Resultados> resultados = Resultados.cargarResultados(numero, tipoOlimpiada);

            StringBuilder tabla = new StringBuilder();

            foreach (Resultados resultado in resultados)
            {
                tabla.Append(resultado.obtenerLineaAdmin(problemasDia1, problemasDia2));
                tabla.Append("\n");
            }

            return tabla.ToString();
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
            precalcularValores();
            // No se llama a calcularNumeros porque
            // olimpiadas donde no se tienen los datos se
            // romperían, hay que llamar calcular números aparte desde el UI.

            return errores.ToString();
        }

        /// <summary>
        /// Guarda valores en la base de datos que estan directamente relacionados
        /// con los resultados y que no pueden escribirse a mano
        /// Calcula las banderas en el objeto olimpiada, el número de problemas por día,
        /// genera el metadata (vacio) de los problemas, asigna lugar a los competidores
        /// y calcula las medallas de todas las personas y escuelas
        /// </summary>
        private void precalcularValores()
        {
            // Calculamos si hay resultados para mostrar por problema y lo guardamos en la base
            problemasDia1 = Problema.obtenerCantidadDeProblemas(numero, tipoOlimpiada, 1);
            problemasDia2 = Problema.obtenerCantidadDeProblemas(numero, tipoOlimpiada, 2);
            mostrarResultadosPorDia = Resultados.mostrarResultadosPorDia(numero, tipoOlimpiada);
            if (mostrarResultadosPorDia)
                mostrarResultadosPorProblema = Resultados.mostrarResultadosIndividuales(numero, tipoOlimpiada);
            else
                mostrarResultadosPorProblema = false;

            // En caso de que solo haya un dia o solo hayamos metido un dia
            if (problemasDia2 == 0)
                mostrarResultadosPorDia = false;

            // Se guardan los bosquejos del metadata de la omi y los dias
            // Los cargamos de la base de datos en caso de ya existir.
            Problema p = Problema.obtenerProblema(this.numero, this.tipoOlimpiada, 0, 0);
            p.guardar(guardarTodo: false);
            p = Problema.obtenerProblema(this.numero, this.tipoOlimpiada, 1, 0);
            p.guardar(guardarTodo: false);
            p = Problema.obtenerProblema(this.numero, this.tipoOlimpiada, 2, 0);
            p.guardar(guardarTodo: false);

            // Calculamos el lugar de cada competidor y lo guardamos en la base
            List<Resultados> resultados = Resultados.cargarResultados(numero, tipoOlimpiada, cargarObjetos: false);
            int competidores = 0;
            int lugar = 0;
            float? puntosMaximos = resultados.Count > 0 ? resultados[0].total : 0;
            bool unkEnTabla = false;

            for (int i = 0; i < resultados.Count; i++)
            {
                resultados[i].lugar = 0;
                if (resultados[i].clave.StartsWith(Resultados.CLAVE_DESCONOCIDA))
                    unkEnTabla = true;
                else
                    if (!(unkEnTabla && resultados[i].medalla == Resultados.TipoMedalla.NADA))
                    {
                        competidores++;
                        if (i == 0 || resultados[i - 1].total != resultados[i].total)
                            lugar = competidores;
                        // Si el competidor tiene 0 puntos (y sabemos cuántos competidores hay),
                        // le asignamos el último lugar posible al competidor, de lo contrario,
                        // le asignamos el siguiente lugar disponible
                        if (resultados[i].total == 0 && participantes > 0)
                            resultados[i].lugar = participantes;
                        else
                            resultados[i].lugar = lugar;
                    }
                resultados[i].guardarLugar();
            }

            // Si el primer lugar tiene menos de 100 puntos, entonces no tenemos los puntos
            mostrarResultadosTotales = puntosMaximos > PUNTOS_MINIMOS_CONOCIDOS;

            // Calculamos el medallero y lo guardamos en la base
            Medallero.calcularMedallas(tipoOlimpiada, numero);

            // Guardamos los datos en la base
            guardarDatos();
        }

        /// <summary>
        /// Copia todos los datos precalculados del objeto old a este objeto
        /// </summary>
        /// <param name="old"></param>
        public void copiarDatosPrecalculados(Olimpiada old)
        {
            this.logo = old.logo;
            this.mostrarResultadosPorDia = old.mostrarResultadosPorDia;
            this.mostrarResultadosPorProblema = old.mostrarResultadosPorProblema;
            this.mostrarResultadosTotales = old.mostrarResultadosTotales;
            this.problemasDia1 = old.problemasDia1;
            this.problemasDia2 = old.problemasDia2;
        }

        /// <summary>
        /// Para ser llamado durante un concurso en vivo, para verificar si los
        /// objetos olimpiada deben de ser recargados de la base de datos
        /// </summary>
        /// <param name="dia">El día del concurso</param>
        /// <returns>True si el cliente debe de recargar</returns>
        public bool shouldReload(int dia)
        {
            // Primero vemos si el objeto ya tiene los datos correctos
            if (dia == 1)
            {
                if (problemasDia1 > 0)
                    return true;
            }
            else
            {
                if (problemasDia2 > 0)
                    return true;
            }

            // Revisamos directamente en la base si el runner y actulizó los problemas
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select problemasDia");
            query.Append(dia);
            query.Append(" from olimpiada where numero = ");
            query.Append(Utilities.Cadenas.comillas(this.numero));
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(this.tipoOlimpiada.ToString().ToLower()));

            db.EjecutarQuery(query.ToString());

            // Si el campo problemasDiaX fue actualizado, reseteamos las OMI
            // y le pedimos al cliente que recargue
            int problemas = (int)db.getTable().Rows[0][0];
            if (problemas > 0)
            {
                if (dia == 1)
                {
                    problemasDia1 = problemas;
                }
                else
                {
                    problemasDia2 = problemas;
                    mostrarResultadosPorDia = true;
                }
                mostrarResultadosPorProblema = true;
                mostrarResultadosTotales = true;
                return true;
            }

            return false;
        }

        public static Olimpiada obtenerMasReciente(bool yaEmpezada = true)
        {
            List<Olimpiada> omis = obtenerOlimpiadas(TipoOlimpiada.OMI);

            if (yaEmpezada)
                for (int i = 0; i < omis.Count; i++)
                    if (omis[i].inicio <= DateTime.Now)
                        return omis[i];

            return omis[0];
        }
#endif
    }
}
