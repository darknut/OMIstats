using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace OMIstats.Models
{
    public class MiembroDelegacion
    {
        public enum TipoAsistente
        {
            NULL,
            COMPETIDOR,
            ASESOR,
            LIDER,
            DELEGADO,
            DELELIDER,
            COMI,
            COLO,
            INVITADO,
            ACOMPAÑANTE
        }

        public const string DELELIDER = "DELEGADO Y LIDER";
        public const string COLO = "COMITÉ LOCAL";

        public enum TipoError
        {
            OK,
            FALTAN_CAMPOS,
            USUARIO_INEXISTENTE,
            CAMPOS_USUARIO,
            FECHA_NACIMIENTO,
            CORREO,
            ESTADO,
            TIPO_ASISTENTE,
            GENERO,
            NIVEL_INSTITUCION,
            AñO_ESCUELA,
            CLAVE_DUPLICADA
        }

        private enum Campos
        {
            USUARIO = 0,
            NOMBRE,
            ESTADO,
            TIPO_ASISTENTE,
            CLAVE,
            FECHA_NACIMIENTO,
            GENERO,
            CORREO,
            CURP,
            NOMBRE_ESCUELA,
            NIVEL_ESCUELA,
            AÑO_ESCUELA,
            PUBLICA,
            ELIMINAR
        }

        public int claveUsuario;
        public int claveEscuela;
        public string usuario;
        public string olimpiada;
        public string nombreAsistente;
        public string fechaNacimiento;
        public string correo;
        public string CURP;
        public string genero;
        public string nombreEscuela;
        public bool escuelaPublica;
        public Institucion.NivelInstitucion nivelEscuela;
        public int añoEscuela;
        public string clave;
        public string estado;
        public TipoAsistente tipo;

        /// <summary>
        /// Solo presente cuando se llama a traves de 'obtenerMiembrosDelegacion'
        /// </summary>
        public Resultados.TipoMedalla medalla;
        /// <summary>
        /// Solo presente cuando se llama a traves de 'obtenerMiembrosDelegacion'
        /// </summary>
        public string fotoUsuario;

        private bool eliminar;

        public string nombreEstado;

        public MiembroDelegacion()
        {
            usuario = "";
            olimpiada = "";
            nombreAsistente = "";
            fechaNacimiento = "";
            correo = "";
            genero = "";
            nombreEscuela = "";
            escuelaPublica = false;
            nivelEscuela = Institucion.NivelInstitucion.NULL;
            añoEscuela = 0;
            clave = "";
            estado = "";
            tipo = TipoAsistente.NULL;
            medalla = Resultados.TipoMedalla.NULL;

            eliminar = false;
        }

        public string getTipoAsistenteString()
        {
            if (this.tipo == TipoAsistente.DELELIDER)
                return DELELIDER;
            if (this.tipo == TipoAsistente.COLO)
                return COLO;
            return this.tipo.ToString();
        }

        private void llenarDatos(DataRow row, bool incluirPersona = true, bool incluirEscuela = true)
        {
            if (incluirPersona)
            {
                usuario = row["usuario"].ToString().Trim();
                nombreAsistente = row["nombre"].ToString().Trim();
                fechaNacimiento = row["nacimiento"].ToString().Trim();
                genero = row["genero"].ToString().Trim();
                correo = row["correo"].ToString().Trim();
                CURP = row["CURP"].ToString().Trim();
            }

            if (incluirEscuela)
            {
                nombreEscuela = row["nombreCorto"].ToString().Trim();
                if (row["publica"] != DBNull.Value)
                    escuelaPublica = (bool)row["publica"];
            }

            claveUsuario = (int)row["persona"];
            claveEscuela = (int)row["institucion"];
            estado = row["estado"].ToString().Trim();
            olimpiada = row["olimpiada"].ToString().Trim();
            nombreEstado = Estado.obtenerEstadoConClave(estado).nombre;
            clave = row["clave"].ToString().Trim();
            tipo = (TipoAsistente)Enum.Parse(typeof(TipoAsistente), row["tipo"].ToString().ToUpper());
            nivelEscuela = (Institucion.NivelInstitucion)row["nivel"];
            añoEscuela = (int)row["año"];
        }

        private TipoError obtenerCampos(string []datos)
        {
            if (datos.Length > (int)Campos.USUARIO)
                usuario = datos[(int)Campos.USUARIO].Trim();
            if (datos.Length > (int)Campos.NOMBRE)
                nombreAsistente = datos[(int)Campos.NOMBRE].Trim();
            if (datos.Length > (int)Campos.ESTADO)
                estado = datos[(int)Campos.ESTADO].Trim();
            try
            {
                if (datos.Length > (int)Campos.TIPO_ASISTENTE)
                    tipo = (TipoAsistente)Enum.Parse(typeof(TipoAsistente), datos[(int)Campos.TIPO_ASISTENTE].Trim().ToUpper());
            }
            catch (Exception)
            {
                return TipoError.TIPO_ASISTENTE;
            }
            if (datos.Length > (int)Campos.CLAVE)
                clave = datos[(int)Campos.CLAVE].Trim();
            if (datos.Length > (int)Campos.FECHA_NACIMIENTO)
                fechaNacimiento = datos[(int)Campos.FECHA_NACIMIENTO].Trim();
            try
            {
                if (datos.Length > (int)Campos.GENERO)
                    genero = datos[(int)Campos.GENERO].Trim().ToCharArray()[0].ToString().ToUpper();
                if (genero != "M" && genero != "F")
                    return TipoError.GENERO;
            }
            catch (Exception)
            {
                return TipoError.GENERO;
            }
            if (datos.Length > (int)Campos.CORREO)
                correo = datos[(int)Campos.CORREO].Trim();
            if (datos.Length > (int)Campos.CURP)
                CURP = datos[(int)Campos.CURP].Trim();
            if (datos.Length > (int)Campos.NOMBRE_ESCUELA)
                nombreEscuela = datos[(int)Campos.NOMBRE_ESCUELA].Trim();
            try
            {
                if (datos.Length > (int)Campos.NIVEL_ESCUELA)
                {
                    if (datos[(int)Campos.NIVEL_ESCUELA].Trim().Length == 0)
                        nivelEscuela = Institucion.NivelInstitucion.NULL;
                    else
                        nivelEscuela = (Institucion.NivelInstitucion)Enum.Parse(typeof(Institucion.NivelInstitucion), datos[(int)Campos.NIVEL_ESCUELA].Trim().ToUpper());
                }
            }
            catch (Exception)
            {
                return TipoError.NIVEL_INSTITUCION;
            }
            try
            {
                if (datos.Length > (int)Campos.AÑO_ESCUELA)
                {
                    if (datos[(int)Campos.AÑO_ESCUELA].Trim().Length == 0)
                        añoEscuela = 0;
                    else
                        añoEscuela = Int32.Parse(datos[(int)Campos.AÑO_ESCUELA]);
                }
                if (añoEscuela < 0 || añoEscuela > 6)
                    return TipoError.AñO_ESCUELA;
            } catch (Exception)
            {
                return TipoError.AñO_ESCUELA;
            }
            if (datos.Length > (int)Campos.PUBLICA)
                escuelaPublica = datos[(int)Campos.PUBLICA].Trim().Equals("publica", StringComparison.InvariantCultureIgnoreCase);
            if (datos.Length > (int)Campos.ELIMINAR)
                eliminar = datos[(int)Campos.ELIMINAR].Trim().Equals("eliminar", StringComparison.InvariantCultureIgnoreCase);

            return TipoError.OK;
        }

        /// <summary>
        /// Regresa el año de la primera OMI para la persona mandada como parametro
        /// Si no se encuentra nada en la base de datos, se devuelve 0
        /// </summary>
        public static int primeraOMIPara(Persona p)
        {
            if (p == null)
                return 0;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select o.año from MiembroDelegacion as md ");
            query.Append(" inner join olimpiada as o on md.olimpiada = o.numero ");
            query.Append(" where md.persona = ");
            query.Append(p.clave);
            query.Append(" order by md.olimpiada asc ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            if (table.Rows.Count == 0)
                return 0;

            return Int32.Parse(table.Rows[0][0].ToString().Trim());
        }

        /// <summary>
        /// Regresa el año de la ultima OMI como competidor para la persona mandada como parámetro
        /// De no encontrarse ninguna, se devuelve 0
        /// </summary>
        public static int ultimaOMIComoCompetidorPara(Persona p)
        {
            if (p == null)
                return 0;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select o.año from MiembroDelegacion as md ");
            query.Append(" inner join olimpiada as o on md.olimpiada = o.numero ");
            query.Append(" where md.persona = ");
            query.Append(p.clave);
            query.Append(" and md.tipo = \'competidor\' ");
            query.Append(" order by md.olimpiada desc ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            if (table.Rows.Count == 0)
                return 0;

            return Int32.Parse(table.Rows[0][0].ToString().Trim());
        }

        /// <summary>
        /// Regresa todos los asistentes de la olimpiada mandada como parámetro
        /// </summary>
        /// <param name="omi">La omi de la que se necesitan los asistentes</param>
        /// <param name="tipoOlimpiada">El tipo de la olimpiada de la que se requieren asistentes</param>
        /// <returns>Una lista con los asistentes de la OMI</returns>
        public static List<MiembroDelegacion> cargarAsistentesOMI(string omi, TipoOlimpiada tipoOlimpiada)
        {
            List<MiembroDelegacion> lista = new List<MiembroDelegacion>();
            if (omi == null)
                return null;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select p.usuario, p.nombre, md.olimpiada, md.estado, md.tipo, md.clave,");
            query.Append(" p.nacimiento, p.genero, p.correo, p.CURP, i.nombreCorto, md.nivel,");
            query.Append(" md.año, i.publica, md.persona, md.institucion from miembrodelegacion as md");
            query.Append(" inner join Persona as p on p.clave = md.persona ");
            query.Append(" left outer join Institucion as i on i.clave = md.institucion");
            query.Append(" where md.olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(omi));
            query.Append(" and md.clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" order by md.clave ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                MiembroDelegacion md = new MiembroDelegacion();
                md.llenarDatos(r);

                lista.Add(md);
            }

            return lista;
        }

        /// <summary>
        /// Regresa los datos en este objeto como un string separado por comas
        /// para que los admins puedan ver los datos en una tabla
        /// </summary>
        /// <returns>Los datos separados por coma</returns>
        public string obtenerLineaAdmin()
        {
            StringBuilder s = new StringBuilder();

            s.Append(usuario);
            s.Append(", ");
            s.Append(nombreAsistente);
            s.Append(", ");
            s.Append(estado);
            s.Append(", ");
            s.Append(tipo.ToString().ToLower());
            s.Append(", ");
            s.Append(clave);
            s.Append(", ");
            s.Append(fechaNacimiento);
            s.Append(", ");
            s.Append(genero);
            s.Append(", ");
            s.Append(correo);
            s.Append(", ");
            s.Append(CURP);
            s.Append(", ");
            s.Append(nombreEscuela);
            s.Append(", ");
            s.Append(nivelEscuela.ToString().ToLower());
            s.Append(", ");
            s.Append(añoEscuela);
            s.Append(", ");
            s.Append(escuelaPublica ? "publica" : "privada");

            return s.ToString();
        }

        /// <summary>
        /// Guarda la linea mandada como parametro en la base de datos
        /// </summary>
        /// <param name="omi">La clave de la olimpiada</param>
        /// <param name="tipoOlimpiada">El tipo de olimpiada a los que los datos pertenecen</param>
        /// <param name="linea">Los datos tabulados por comas</param>
        /// <returns>Si hubo un error, lo devuelve</returns>
        public static TipoError guardarLineaAdmin(string omi, TipoOlimpiada tipoOlimpiada, string linea)
        {
            if (linea.Trim().Length == 0)
                return TipoError.OK;

            StringBuilder query = new StringBuilder();
            Utilities.Acceso db = new Utilities.Acceso();
            MiembroDelegacion md = new MiembroDelegacion();
            Persona p = null;
            DataTable table = null;

            string[] datos = linea.Split(',');

            // Casteamos los datos del string a variables

            TipoError err = md.obtenerCampos(datos);
            if (err != TipoError.OK)
                return err;

            // Borramos al usuario de la tabla

            if (md.eliminar)
            {
                p = Persona.obtenerPersonaDeUsuario(md.usuario);
                if (p == null)
                    return TipoError.OK;

                query.Append(" delete miembrodelegacion ");
                query.Append(" where olimpiada = ");
                query.Append(Utilities.Cadenas.comillas(omi));
                query.Append(" and persona = ");
                query.Append(p.clave);
                query.Append(" and estado = ");
                query.Append(Utilities.Cadenas.comillas(md.estado));
                query.Append(" and clase = ");
                query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));

                db.EjecutarQuery(query.ToString());
                Resultados.eliminarResultado(omi, tipoOlimpiada, md.clave);

                return TipoError.OK;
            }

            // Verificamos que los datos mandatorios se hayan dado

            if (md.nombreAsistente.Length == 0 ||
                md.estado.Length == 0 ||
                md.tipo == TipoAsistente.NULL ||
                md.clave.Length == 0 ||
                md.genero.Length != 1)
                return TipoError.FALTAN_CAMPOS;

            // Verificar que exista el usuario

            if (md.usuario.Length == 0)
            {
                // El usuario se desconoce, hay que buscarlo
                // Primero buscamos por CURP
                p = Persona.obtenerPersonaConCURP(md.CURP);

                if (p == null)
                {
                    // No se encontró con curp, aproximamos por nombre
                    p = Persona.obtenerPersonaConNombre(md.nombreAsistente);
                }

                // El usuario es nuevo, lo creamos

                if (p == null)
                {
                    p = new Persona();
                    p.nombre = md.nombreAsistente;

                    Utilities.Archivos.FotoInicial foto = Utilities.Archivos.FotoInicial.DOMI;
                    if (md.tipo == TipoAsistente.COMI || md.tipo == TipoAsistente.COLO)
                        foto = Utilities.Archivos.FotoInicial.OMI;
                    if (md.tipo == TipoAsistente.COMPETIDOR)
                        foto = Utilities.Archivos.FotoInicial.KAREL;

                    p.nuevoUsuario(foto);
                }
            }
            else
            {
                // El usuario está presente, lo sacamos de la base

                p = Persona.obtenerPersonaDeUsuario(md.usuario);

                // Si el usuario no existe, hay que lanzar un error

                if (p == null)
                    return TipoError.USUARIO_INEXISTENTE;
            }

            // Ya se tiene un usuario valido, guardamos sus datos

            p.nombre = md.nombreAsistente;

            try
            {
                if (md.fechaNacimiento.Length > 0)
                    p.nacimiento = Utilities.Fechas.stringToDate(md.fechaNacimiento);
            }
            catch (Exception)
            {
                return TipoError.FECHA_NACIMIENTO;
            }

            p.genero = md.genero;
            p.CURP = md.CURP;

            if (md.correo.Length > 0)
            {
                Regex regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$");
                if (!regex.IsMatch(md.correo))
                    return TipoError.CORREO;
                p.correo = md.correo;
            }

            if (!p.guardarDatos())
                return TipoError.CAMPOS_USUARIO;

            md.usuario = p.usuario;

            // Revisamos que exista la escuela

            Institucion i = null;

            if (md.nombreEscuela.Length > 0)
            {
                i = Institucion.buscarInstitucionConNombre(md.nombreEscuela);

                if (i == null)
                {
                    // La escuela es nueva, creamos una nueva.

                    i = new Institucion();
                    i.nombre = md.nombreEscuela;
                    i.nuevaInstitucion();
                }

                // Ya tenemos un objeto institución, actualizamos los datos

                switch (md.nivelEscuela)
                {
                    case Institucion.NivelInstitucion.PRIMARIA:
                        i.primaria = true;
                        break;
                    case Institucion.NivelInstitucion.SECUNDARIA:
                        i.secundaria = true;
                        break;
                    case Institucion.NivelInstitucion.PREPARATORIA:
                        i.preparatoria = true;
                        break;
                    case Institucion.NivelInstitucion.UNIVERSIDAD:
                        i.universidad = true;
                        break;
                }

                i.publica = md.escuelaPublica;

                i.guardar(generarPeticiones: false);
            }

            // Revisamos que el estado exista

            Estado e = Estado.obtenerEstadoConClave(md.estado);

            if (e == null)
                return TipoError.ESTADO;

            // Buscamos ahora si ya hay un miembro con estos datos

            query.Append(" select * from miembrodelegacion ");
            query.Append(" where olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(omi));
            query.Append(" and persona = ");
            query.Append(p.clave);
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            // Agregamos estado por casos como Martín que tienen dos roles en diferentes estados
            query.Append(" and estado = ");
            query.Append(Utilities.Cadenas.comillas(md.estado));

            db.EjecutarQuery(query.ToString());
            table = db.getTable();

            if (table.Rows.Count == 0)
            {
                // El usuario no existe, lo agregamos

                query.Clear();
                query.Append(" insert into miembrodelegacion values (");
                query.Append(Utilities.Cadenas.comillas(omi));
                query.Append(", ");
                query.Append(Utilities.Cadenas.comillas(md.estado));
                query.Append(", ");
                query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
                query.Append(", ");
                query.Append(Utilities.Cadenas.comillas(md.clave));
                query.Append(", ");
                query.Append(Utilities.Cadenas.comillas(md.tipo.ToString().ToLower()));
                query.Append(", ");
                query.Append(p.clave);
                query.Append(", ");
                query.Append(i == null ? "0" : i.clave.ToString());
                query.Append(", ");
                query.Append((int)md.nivelEscuela);
                query.Append(", ");
                query.Append(md.añoEscuela);
                query.Append(")");

                db.EjecutarQuery(query.ToString());
            }
            else
            {
                // El usuario existe, cargamos los datos y los actualizamos

                MiembroDelegacion md_current = new MiembroDelegacion();
                md_current.llenarDatos(table.Rows[0], incluirPersona: false, incluirEscuela: false);

                if (md_current.clave != md.clave)
                {
                    if (!Resultados.cambiarClave(omi, tipoOlimpiada, md_current.clave, md.clave))
                        return TipoError.CLAVE_DUPLICADA;
                }

                query.Clear();
                query.Append(" update miembrodelegacion set clave = ");
                query.Append(Utilities.Cadenas.comillas(md.clave));
                query.Append(", tipo = ");
                query.Append(Utilities.Cadenas.comillas(md.tipo.ToString().ToLower()));
                query.Append(", institucion = ");
                query.Append(i == null ? "0" : i.clave.ToString());
                query.Append(", nivel = ");
                query.Append((int)md.nivelEscuela);
                query.Append(", año = ");
                query.Append(md.añoEscuela);
                query.Append(" where olimpiada = ");
                query.Append(Utilities.Cadenas.comillas(omi));
                query.Append(" and persona = ");
                query.Append(p.clave);
                query.Append(" and estado = ");
                query.Append(Utilities.Cadenas.comillas(md.estado));
                query.Append(" and clase = ");
                query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));

                db.EjecutarQuery(query.ToString());
            }

            return TipoError.OK;
        }

        /// <summary>
        /// Regresa una lista de miembro de la delegación con la clave mandada como parametro
        /// </summary>
        /// <param name="omi">La olimpiada de la clave</param>
        /// <param name="tipoOlimpiada">El tipo de olimpiada</param>
        /// <param name="clave">La clave buscada</param>
        /// <returns>La lista de miembros con la clave buscada</returns>
        public static List<MiembroDelegacion> obtenerMiembrosConClave(string omi, TipoOlimpiada tipoOlimpiada, string clave)
        {
            List<MiembroDelegacion> lista = new List<MiembroDelegacion>();
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from miembrodelegacion ");
            query.Append(" where olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(omi));
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and clave = ");
            query.Append(Utilities.Cadenas.comillas(clave));
            query.Append(" order by clave ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                MiembroDelegacion md = new MiembroDelegacion();
                md.llenarDatos(r, incluirPersona: false, incluirEscuela: false);

                lista.Add(md);
            }

            return lista;
        }

        /// <summary>
        /// Regresa el número de estados participantes en la Olimpiada mandada como parámetro
        /// </summary>
        /// <param name="omi">La OMI deseada</param>
        /// <param name="tipoOlimpiada">El tipo de Olimpiada</param>
        /// <returns>Cuantos estados participaron</returns>
        public static int obtenerEstadosParticipantes(string omi, TipoOlimpiada tipoOlimpiada)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select count(distinct(Estado)) from MiembroDelegacion where olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(omi));
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));

            db.EjecutarQuery(query.ToString());

            return (int)db.getTable().Rows[0][0];
        }

        /// <summary>
        /// Regresa el número de participantes en la Olimpiada mandada como parámetro
        /// </summary>
        /// <param name="omi">La OMI deseada</param>
        /// <param name="tipoOlimpiada">El tipo de Olimpiada</param>
        /// <returns>Cuantos competidores participaron</returns>
        public static int obtenerParticipantes(string omi, TipoOlimpiada tipoOlimpiada)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select COUNT(*) from MiembroDelegacion where olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(omi));
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and tipo = ");
            query.Append(Utilities.Cadenas.comillas(TipoAsistente.COMPETIDOR.ToString().ToLower()));

            db.EjecutarQuery(query.ToString());
            return (int)db.getTable().Rows[0][0];
        }

        /// <summary>
        /// Regresa las participaciones del usuario mandado como parámetro que no son como competidor
        /// </summary>
        /// <param name="persona">La clave de la persona deseada</param>
        /// <param name="tipoOlimpiada">El tipo de olimpiada solicitado</param>
        /// <returns>La lista de participaciones</returns>
        public static List<MiembroDelegacion> obtenerParticipaciones(int persona, TipoOlimpiada tipoOlimpiada)
        {
            List<MiembroDelegacion> lista = new List<MiembroDelegacion>();

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select p.usuario, p.nombre, md.olimpiada, md.estado, md.tipo, md.clave, md.institucion, ");
            query.Append(" p.nacimiento, p.genero, p.correo, p.curp, i.nombreCorto, md.nivel,");
            query.Append(" md.año, i.publica, md.persona from miembrodelegacion as md");
            query.Append(" inner join Olimpiada as o on md.olimpiada = o.numero and md.clase = o.clase ");
            query.Append(" inner join Persona as p on p.clave = md.persona ");
            query.Append(" left outer join Institucion as i on i.clave = md.institucion");
            query.Append(" where p.clave = ");
            query.Append(persona);
            query.Append(" and md.clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and md.tipo <> ");
            query.Append(Utilities.Cadenas.comillas(TipoAsistente.COMPETIDOR.ToString().ToLower()));
            query.Append(" order by o.año asc ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                MiembroDelegacion md = new MiembroDelegacion();
                md.llenarDatos(r);

                lista.Add(md);
            }

            return lista;
        }

        /// <summary>
        /// Obtiene la lista de miembros de una delegacion
        /// </summary>
        /// <returns></returns>
        public static List<MiembroDelegacion> obtenerMiembrosDelegacion(string olimpiada, string estado, TipoOlimpiada tipoOlimpiada, TipoAsistente tipo = TipoAsistente.NULL)
        {
            List<MiembroDelegacion> lista = new List<MiembroDelegacion>();

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from MiembroDelegacion as md ");
            query.Append(" inner join Persona as p on p.clave = md.persona ");
            query.Append(" where md.olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(olimpiada));
            query.Append(" and md.clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and md.estado = ");
            query.Append(Utilities.Cadenas.comillas(estado));

            switch (tipo)
            {
                case TipoAsistente.COMPETIDOR:
                    {
                        query.Append(" and md.tipo = ");
                        query.Append(Utilities.Cadenas.comillas(TipoAsistente.COMPETIDOR.ToString().ToLower()));
                        break;
                    }
                case TipoAsistente.DELELIDER:
                case TipoAsistente.LIDER:
                case TipoAsistente.DELEGADO:
                    {
                        query.Append(" and (md.tipo = ");
                        query.Append(Utilities.Cadenas.comillas(TipoAsistente.LIDER.ToString().ToLower()));
                        query.Append(" or md.tipo = ");
                        query.Append(Utilities.Cadenas.comillas(TipoAsistente.DELEGADO.ToString().ToLower()));
                        query.Append(" or md.tipo = ");
                        query.Append(Utilities.Cadenas.comillas(TipoAsistente.DELELIDER.ToString().ToLower()));
                        query.Append(")");
                        break;
                    }
                case TipoAsistente.NULL:
                    {
                        query.Append(" and md.tipo <> ");
                        query.Append(Utilities.Cadenas.comillas(TipoAsistente.LIDER.ToString().ToLower()));
                        query.Append(" and md.tipo <> ");
                        query.Append(Utilities.Cadenas.comillas(TipoAsistente.DELEGADO.ToString().ToLower()));
                        query.Append(" and md.tipo <> ");
                        query.Append(Utilities.Cadenas.comillas(TipoAsistente.DELELIDER.ToString().ToLower()));
                        query.Append(" and md.tipo <> ");
                        query.Append(Utilities.Cadenas.comillas(TipoAsistente.COMPETIDOR.ToString().ToLower()));
                        break;
                    }
            }

            query.Append(" order by md.clave ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                MiembroDelegacion md = new MiembroDelegacion();
                md.llenarDatos(r, incluirEscuela: false);

                if (tipo == TipoAsistente.COMPETIDOR)
                    md.medalla = Resultados.cargarResultados(olimpiada, tipoOlimpiada, md.clave).medalla;

                md.fotoUsuario = r["foto"].ToString().Trim();

                lista.Add(md);
            }

            return lista;
        }
    }
}