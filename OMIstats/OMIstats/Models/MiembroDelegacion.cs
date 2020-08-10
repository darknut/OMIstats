using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using OMIstats.Utilities;

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
            ACOMPAÑANTE,
            SUBLIDER
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
            CLAVE_DUPLICADA,
            ESCUELA
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
        public string genero;
        public string nombreEscuela;
        public bool escuelaPublica;
#if OMISTATS
        public Institucion.NivelInstitucion nivelEscuela;
#endif
        public int añoEscuela;
        public string clave;
        public string estado;
        public TipoAsistente tipo;
        public TipoOlimpiada tipoOlimpiada;

        /// <summary>
        /// Solo presente cuando se llama a traves de 'obtenerMiembrosDelegacion'
        /// </summary>
        public Resultados resultados;
        /// <summary>
        /// Solo presente cuando se llama a traves de 'obtenerMiembrosDelegacion'
        /// </summary>
        public string fotoUsuario;
#if OMISTATS
        private bool eliminar;
#endif
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
#if OMISTATS
            nivelEscuela = Institucion.NivelInstitucion.NULL;
            eliminar = false;
#endif
            añoEscuela = 0;
            clave = "";
            estado = "";
            tipo = TipoAsistente.NULL;
            resultados = null;
        }

        public string getTipoAsistenteString()
        {
            return getTipoAsistenteString(tipo);
        }

        public static string getTipoAsistenteString(TipoAsistente tipo)
        {
            if (tipo == TipoAsistente.DELELIDER)
                return DELELIDER;
            if (tipo == TipoAsistente.COLO)
                return COLO;
            return tipo.ToString();
        }

        private void llenarDatos(DataRow row, bool incluirPersona = true, bool incluirEscuela = true)
        {
            if (incluirPersona)
            {
                usuario = row["usuario"].ToString().Trim();
                nombreAsistente = row["nombre"].ToString().Trim() + " " + row["apellidoP"].ToString().Trim() + " " + row["apellidoM"].ToString().Trim();
                fechaNacimiento = row["nacimiento"].ToString().Trim();
                genero = row["genero"].ToString().Trim();
                correo = row["correo"].ToString().Trim();
            }

            if (incluirEscuela)
            {
                nombreEscuela = row["nombreCorto"].ToString().Trim();
                if (row["publica"] != DBNull.Value)
                    escuelaPublica = (bool)row["publica"];
            }

            claveUsuario = (int)row["persona"];
            estado = row["estado"].ToString().Trim();
            olimpiada = row["olimpiada"].ToString().Trim();
            clave = row["clave"].ToString().Trim();
            tipo = EnumParser.ToTipoAsistente(row["tipo"].ToString().ToUpper());
            tipoOlimpiada = EnumParser.ToTipoOlimpiada(row["clase"].ToString().ToUpper());
#if OMISTATS
            try
            {
                claveEscuela = (int)row["institucion"];
                nivelEscuela = (Institucion.NivelInstitucion)row["nivel"];
                añoEscuela = (int)row["año"];
            } catch(Exception) { }
            nombreEstado = Estado.obtenerEstadoConClave(estado).nombre;
#endif
        }

#if OMISTATS
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
                if (tipo == TipoAsistente.NULL)
                    throw new Exception();
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
            }
            catch (Exception)
            {
            }
            if (datos.Length > (int)Campos.CORREO)
                correo = datos[(int)Campos.CORREO].Trim();
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

            Acceso db = new Acceso();
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

            Acceso db = new Acceso();
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

            query.Append(" select p.usuario, p.nombre, p.apellidoP, p.apellidoM, md.olimpiada, md.estado, md.tipo, md.clave, md.clase, ");
            query.Append(" p.nacimiento, p.genero, p.correo, i.nombreCorto, md.nivel,");
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
        /// Borra la instancia de miembro delegación de la base de datos
        /// </summary>
        /// <param name="byClave">Si true, borra utilizando la clave, si false, utilizando persona & estado</param>
        public void borrarMiembroDelegacion(bool byClave = true)
        {
            StringBuilder query = new StringBuilder();
            Acceso db = new Acceso();

            query.Append(" delete miembrodelegacion ");
            query.Append(" where olimpiada = ");
            query.Append(Cadenas.comillas(this.olimpiada));
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(this.tipoOlimpiada.ToString().ToLower()));
            if (byClave)
            {
                query.Append(" and clave = ");
                query.Append(Cadenas.comillas(this.clave));
            }
            else
            {
                query.Append(" and persona = ");
                query.Append(this.claveUsuario);
                query.Append(" and estado = ");
                query.Append(Cadenas.comillas(this.estado));
            }

            db.EjecutarQuery(query.ToString());
            Resultados.eliminarResultado(this.olimpiada, this.tipoOlimpiada, this.clave);
        }

        /// <summary>
        /// Guarda la linea mandada como parametro en la base de datos
        /// </summary>
        /// <param name="omi">La clave de la olimpiada</param>
        /// <param name="tipoOlimpiada">El tipo de olimpiada a los que los datos pertenecen</param>
        /// <param name="linea">Los datos tabulados por comas</param>
        /// <returns>Si hubo un error, lo devuelve casteado a int, si no, devuelve la clave de usuario</returns>
        public static int guardarLineaAdmin(string omi, TipoOlimpiada tipoOlimpiada, string linea)
        {
            if (linea.Trim().Length == 0)
                return (int) TipoError.OK;

            StringBuilder query = new StringBuilder();
            Acceso db = new Acceso();
            MiembroDelegacion md = new MiembroDelegacion();
            Persona p = null;
            DataTable table = null;

            string[] datos = linea.Split(',');

            // Casteamos los datos del string a variables
            TipoError err = md.obtenerCampos(datos);
            if (err != TipoError.OK)
                return (int) err;

            // Borramos al usuario de la tabla
            if (md.eliminar)
            {
                p = Persona.obtenerPersonaDeUsuario(md.usuario);
                if (p == null)
                    return (int) TipoError.OK;

                md.olimpiada = omi;
                md.tipoOlimpiada = tipoOlimpiada;
                md.claveUsuario = p.clave;
                md.borrarMiembroDelegacion(byClave: false);

                return (int) TipoError.OK;
            }

            // La clave solo es requerida para competidores, si no es competidor, la generamos
            if (md.clave.Length == 0 && md.tipo != TipoAsistente.COMPETIDOR)
                md.clave = md.estado + "-" + md.tipo.ToString()[0];

            // Verificamos que los datos mandatorios se hayan dado
            if ((md.nombreAsistente.Length == 0 && md.usuario.Length == 0) ||
                md.estado.Length == 0 ||
                md.tipo == TipoAsistente.NULL ||
                md.clave.Length == 0)
                return (int) TipoError.FALTAN_CAMPOS;

            // Verificar que exista el usuario
            if (md.usuario.Length == 0)
            {
                // El usuario se desconoce, hay que buscarlo
                // Buscamos por nombre
                p = Persona.obtenerPersonaConNombre(md.nombreAsistente);

                // El usuario es nuevo, lo creamos
                if (p == null)
                {
                    p = new Persona();
                    p.nombre = md.nombreAsistente;

                    if (md.genero != "M" && md.genero != "F")
                        return (int) TipoError.GENERO;

                    Archivos.FotoInicial foto = Archivos.FotoInicial.DOMI;
                    if (md.tipo == TipoAsistente.COMI || md.tipo == TipoAsistente.COLO)
                        foto = Archivos.FotoInicial.OMI;
                    if (md.tipo == TipoAsistente.COMPETIDOR)
                        foto = Archivos.FotoInicial.KAREL;

                    p.nuevoUsuario(foto);
                }
            }
            else
            {
                p = Persona.obtenerPersonaDeUsuario(md.usuario);

                // Si el usuario no existe, hay que lanzar un error
                if (p == null)
                    return (int) TipoError.USUARIO_INEXISTENTE;
            }

            // Ya se tiene un usuario valido, guardamos sus datos
            if (md.nombreAsistente.Length > 0)
            {
                p.nombre = md.nombreAsistente;
                p.breakNombre();
            }

            try
            {
                if (md.fechaNacimiento.Length > 0)
                    p.nacimiento = Fechas.stringToDate(md.fechaNacimiento);
            }
            catch (Exception)
            {
                return (int) TipoError.FECHA_NACIMIENTO;
            }

            if (md.genero == "M" || md.genero == "F")
                p.genero = md.genero;

            if (md.correo.Length > 0)
            {
                if (!Cadenas.esCorreo(md.correo))
                    return (int) TipoError.CORREO;
                p.correo = md.correo;
            }

            if (!p.guardarDatos())
                return (int) TipoError.CAMPOS_USUARIO;

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
                return (int) TipoError.ESTADO;

            // Buscamos ahora si ya hay un miembro con estos datos
            query.Append(" select * from miembrodelegacion ");
            query.Append(" where olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" and persona = ");
            query.Append(p.clave);
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            // Agregamos estado por casos como Martín que tienen dos roles en diferentes estados
            query.Append(" and estado = ");
            query.Append(Cadenas.comillas(md.estado));

            db.EjecutarQuery(query.ToString());
            table = db.getTable();

            if (table.Rows.Count == 0)
            {
                // El usuario no existe, lo agregamos
                query.Clear();
                query.Append(" insert into miembrodelegacion values (");
                query.Append(Cadenas.comillas(omi));
                query.Append(", ");
                query.Append(Cadenas.comillas(md.estado));
                query.Append(", ");
                query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
                query.Append(", ");
                query.Append(Cadenas.comillas(md.clave));
                query.Append(", ");
                query.Append(Cadenas.comillas(md.tipo.ToString().ToLower()));
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
                        return (int) TipoError.CLAVE_DUPLICADA;
                }

                query.Clear();
                query.Append(" update miembrodelegacion set clave = ");
                query.Append(Cadenas.comillas(md.clave));
                query.Append(", tipo = ");
                query.Append(Cadenas.comillas(md.tipo.ToString().ToLower()));
                query.Append(", institucion = ");
                query.Append(i == null ? "0" : i.clave.ToString());
                query.Append(", nivel = ");
                query.Append((int)md.nivelEscuela);
                query.Append(", año = ");
                query.Append(md.añoEscuela);
                query.Append(" where olimpiada = ");
                query.Append(Cadenas.comillas(omi));
                query.Append(" and persona = ");
                query.Append(p.clave);
                query.Append(" and estado = ");
                query.Append(Cadenas.comillas(md.estado));
                query.Append(" and clase = ");
                query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));

                db.EjecutarQuery(query.ToString());
            }

            return p.clave;
        }

        public void nuevo()
        {
            // Primero se revisa si hay clave
            if (String.IsNullOrEmpty(clave))
            {
                // Si no hay, se asigna la siguiente disponible
                clave = MiembroDelegacion.obtenerPrimerClaveDisponible(olimpiada, tipoOlimpiada, estado, tipo);
            }
            else
            {
                // Si sí hay, se revisa por colisiones
                List<MiembroDelegacion> md = MiembroDelegacion.obtenerMiembrosConClave(olimpiada, tipoOlimpiada, clave);
                if (md.Count > 0)
                {
                    // Si hay colisión, se asigna una nueva clave al competidor viejo
                    md[0].clave = MiembroDelegacion.obtenerPrimerClaveDisponible(olimpiada, tipoOlimpiada, estado, tipo);
                    md[0].guardarDatos(clave, tipoOlimpiada, ignoreCollisions: true);
                }
            }

            StringBuilder query = new StringBuilder();
            Acceso db = new Acceso();

            query.Append(" insert into miembrodelegacion (olimpiada, estado, clase, clave, tipo, persona) values(");
            query.Append(Cadenas.comillas(olimpiada));
            query.Append(",");
            query.Append(Cadenas.comillas(estado));
            query.Append(",");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(",");
            query.Append(Cadenas.comillas(clave));
            query.Append(",");
            query.Append(Cadenas.comillas(tipo.ToString().ToLower()));
            query.Append(",");
            query.Append(claveUsuario);
            query.Append(")");

            db.EjecutarQuery(query.ToString());
        }

        public void guardarDatos(string claveOriginal, TipoOlimpiada tipoOriginal, bool ignoreCollisions = false)
        {
            if (String.IsNullOrEmpty(clave))
            {
                List<MiembroDelegacion> md = MiembroDelegacion.obtenerMiembrosConClave(olimpiada, tipoOriginal, claveOriginal);
                if (md.Count > 0 && md[0].tipo == tipo && md[0].tipoOlimpiada == tipoOlimpiada && md[0].estado == estado)
                    clave = claveOriginal;
                else
                    clave = MiembroDelegacion.obtenerPrimerClaveDisponible(olimpiada, tipoOlimpiada, estado, tipo);
            }
            else
            {
                if (!ignoreCollisions)
                {
                    List<MiembroDelegacion> md = MiembroDelegacion.obtenerMiembrosConClave(olimpiada, tipoOlimpiada, clave);
                    // Revisamos si hay colisiones de clave
                    if (md.Count > 0 && md[0].claveUsuario != claveUsuario)
                    {
                        // Se hace cambalache de claves en caso de que este registrado en la misma competicion
                        // y sea el mismo tipo de asistente
                        MiembroDelegacion original = MiembroDelegacion.obtenerMiembrosConClave(olimpiada, tipoOriginal, claveOriginal)[0];
                        if (md[0].tipo == original.tipo && md[0].tipoOlimpiada == original.tipoOlimpiada && md[0].estado == original.estado)
                            md[0].clave = claveOriginal;
                        else
                            md[0].clave = MiembroDelegacion.obtenerPrimerClaveDisponible(olimpiada, tipoOlimpiada, estado, tipo);
                        md[0].guardarDatos(clave, tipoOlimpiada, ignoreCollisions: true);
                    }
                }
            }

            StringBuilder query = new StringBuilder();
            Acceso db = new Acceso();

            query.Append(" update miembrodelegacion set clave = ");
            query.Append(Cadenas.comillas(clave));
            query.Append(", tipo = ");
            query.Append(Cadenas.comillas(tipo.ToString().ToLower()));
            query.Append(", clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(", estado = ");
            query.Append(Cadenas.comillas(estado));
            /*query.Append(", institucion = ");
            query.Append(i == null ? "0" : i.clave.ToString());
            query.Append(", nivel = ");
            query.Append((int)md.nivelEscuela);
            query.Append(", año = ");
            query.Append(md.añoEscuela);*/
            query.Append(" where olimpiada = ");
            query.Append(Cadenas.comillas(olimpiada));
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipoOriginal.ToString().ToLower()));
            query.Append(" and clave = ");
            query.Append(Cadenas.comillas(claveOriginal));
            query.Append(" and persona = ");
            query.Append(claveUsuario);

            db.EjecutarQuery(query.ToString());
        }
#endif

        /// <summary>
        /// Regresa una lista de miembro de la delegación con la clave mandada como parametro
        /// </summary>
        /// <param name="omi">La olimpiada de la clave</param>
        /// <param name="tipoOlimpiada">El tipo de olimpiada</param>
        /// <param name="clave">La clave buscada</param>
        /// <param name="aproximarClave">Si la clave solo debe de ser aproximada</param>
        /// <returns>La lista de miembros con la clave buscada</returns>
        public static List<MiembroDelegacion> obtenerMiembrosConClave(string omi, TipoOlimpiada tipoOlimpiada, string clave, bool aproximarClave = false, TipoAsistente tipoAsistente = TipoAsistente.NULL)
        {
            List<MiembroDelegacion> lista = new List<MiembroDelegacion>();
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from miembrodelegacion ");
            query.Append(" where olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            if (tipoOlimpiada != TipoOlimpiada.NULL)
            {
                query.Append(" and clase = ");
                query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            }
            if (aproximarClave)
            {
                query.Append(" and clave like ");
                query.Append(Cadenas.comillas("%" + clave + "%"));
            }
            else
            {
                query.Append(" and clave = ");
                query.Append(Cadenas.comillas(clave));
            }
            if (tipoAsistente != TipoAsistente.NULL)
            {
                query.Append(" and tipo = ");
                query.Append(Cadenas.comillas(tipoAsistente.ToString().ToLower()));
            }
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
        /// Regresa el número de participantes en la Olimpiada mandada como parámetro
        /// </summary>
        /// <param name="omi">La OMI deseada</param>
        /// <param name="tipoOlimpiada">El tipo de Olimpiada</param>
        /// <returns>Cuantos competidores participaron</returns>
        public static int obtenerParticipantes(string omi, TipoOlimpiada tipoOlimpiada)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select COUNT(*) from MiembroDelegacion where olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and tipo = ");
            query.Append(Cadenas.comillas(TipoAsistente.COMPETIDOR.ToString().ToLower()));

            db.EjecutarQuery(query.ToString());
            return (int)db.getTable().Rows[0][0];
        }

        /// <summary>
        /// Obtiene el objeto MiembroDeletacion más reciente para la persona mandada como parámetro
        /// </summary>
        /// <param name="persona">La persona en cuestión</param>
        /// <returns>El objeto deseado</returns>
        private static MiembroDelegacion obtenerParticipacionMasReciente(int persona)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();
            MiembroDelegacion md = new MiembroDelegacion();

            query.Append(" select * from MiembroDelegacion where persona = ");
            query.Append(persona);
            query.Append(" order by olimpiada desc ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();
            md.llenarDatos(table.Rows[0], incluirPersona: false, incluirEscuela: false);

            return md;
        }

        /// <summary>
        /// Regresa las participaciones del usuario mandado como parámetro que no son como competidor
        /// </summary>
        /// <param name="persona">La clave de la persona deseada</param>
        /// <param name="tipoOlimpiada">El tipo de olimpiada solicitado</param>
        /// <returns>La lista de participaciones</returns>
        public static List<MiembroDelegacion> obtenerParticipaciones(int persona)
        {  // -TODO- Cuando agregue IOI, hay que revisitar este método
            List<MiembroDelegacion> lista = new List<MiembroDelegacion>();

            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select p.usuario, p.nombre, p.apellidoP, p.apellidoM, md.olimpiada, md.estado, md.tipo, md.clave, md.clase, md.institucion, ");
            query.Append(" p.nacimiento, p.genero, p.correo, i.nombreCorto, md.nivel,");
            query.Append(" md.año, i.publica, md.persona from miembrodelegacion as md");
            query.Append(" inner join Olimpiada as o on md.olimpiada = o.numero and md.clase = o.clase ");
            query.Append(" inner join Persona as p on p.clave = md.persona ");
            query.Append(" left outer join Institucion as i on i.clave = md.institucion");
            query.Append(" where p.clave = ");
            query.Append(persona);
            query.Append(" and md.tipo <> ");
            query.Append(Cadenas.comillas(TipoAsistente.COMPETIDOR.ToString().ToLower()));
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

            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from MiembroDelegacion as md ");
            query.Append(" inner join Persona as p on p.clave = md.persona ");
            query.Append(" where md.olimpiada = ");
            query.Append(Cadenas.comillas(olimpiada));
            if (tipo == TipoAsistente.COMPETIDOR)
            {
                query.Append(" and md.clase = ");
                query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            }
            if (estado != null)
            {
                query.Append(" and md.estado = ");
                query.Append(Cadenas.comillas(estado));
            }

            switch (tipo)
            {
                case TipoAsistente.COMPETIDOR:
                    {
                        query.Append(" and md.tipo = ");
                        query.Append(Cadenas.comillas(TipoAsistente.COMPETIDOR.ToString().ToLower()));
                        break;
                    }
                case TipoAsistente.DELELIDER:
                case TipoAsistente.LIDER:
                case TipoAsistente.DELEGADO:
                case TipoAsistente.SUBLIDER:
                    {
                        query.Append(" and (md.tipo = ");
                        query.Append(Cadenas.comillas(TipoAsistente.LIDER.ToString().ToLower()));
                        query.Append(" or md.tipo = ");
                        query.Append(Cadenas.comillas(TipoAsistente.DELEGADO.ToString().ToLower()));
                        query.Append(" or md.tipo = ");
                        query.Append(Cadenas.comillas(TipoAsistente.DELELIDER.ToString().ToLower()));
                        query.Append(" or md.tipo = ");
                        query.Append(Cadenas.comillas(TipoAsistente.SUBLIDER.ToString().ToLower()));
                        query.Append(")");
                        break;
                    }
                case TipoAsistente.INVITADO:
                    {
                        query.Append(" and md.tipo <> ");
                        query.Append(Cadenas.comillas(TipoAsistente.LIDER.ToString().ToLower()));
                        query.Append(" and md.tipo <> ");
                        query.Append(Cadenas.comillas(TipoAsistente.DELEGADO.ToString().ToLower()));
                        query.Append(" and md.tipo <> ");
                        query.Append(Cadenas.comillas(TipoAsistente.DELELIDER.ToString().ToLower()));
                        query.Append(" and md.tipo <> ");
                        query.Append(Cadenas.comillas(TipoAsistente.SUBLIDER.ToString().ToLower()));
                        query.Append(" and md.tipo <> ");
                        query.Append(Cadenas.comillas(TipoAsistente.COMPETIDOR.ToString().ToLower()));
                        break;
                    }
            }

            if (tipo == TipoAsistente.NULL)
            {
                query.Append(" order by md.estado, md.clase desc, md.clave ");
            }
            else
            {
                query.Append(" order by md.clave ");
            }

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                MiembroDelegacion md = new MiembroDelegacion();
                md.llenarDatos(r, incluirEscuela: false);

                if (tipo == TipoAsistente.COMPETIDOR)
                    md.resultados = Resultados.cargarResultados(olimpiada, tipoOlimpiada, md.clave);

                md.fotoUsuario = r["foto"].ToString().Trim();

                lista.Add(md);
            }

            return lista;
        }
#if OMISTATS
        public static string generarDiplomas(string omi, string X, string baseURL, string[] stringsAsistentes)
        {
            StringBuilder lineas = new StringBuilder();
            StringBuilder query = new StringBuilder();
            Acceso db = new Acceso();

            query.Append(" select p.clave as persona, p.nombre, p.apellidoP, p.apellidoM, p.genero, md.clave, md.clase, md.tipo, md.estado from miembrodelegacion as md ");
            query.Append(" inner join Persona as p on p.clave = md.persona ");
            query.Append(" where md.olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" order by persona ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();
            int lastUsuario = 0;

            foreach (DataRow r in table.Rows)
            {
                int claveUsuario = (int)r["persona"];
                string nombre = r["nombre"].ToString().Trim() + " " + r["apellidoP"].ToString().Trim() + " " + r["apellidoM"].ToString().Trim();
                string clave = r["clave"].ToString().Trim();
                string estado = r["estado"].ToString().Trim();
                string genero = r["genero"].ToString().Trim();
                TipoOlimpiada clase = EnumParser.ToTipoOlimpiada(r["clase"].ToString().ToUpper());
                TipoAsistente tipo = EnumParser.ToTipoAsistente(r["tipo"].ToString().ToUpper());

                if (lastUsuario == claveUsuario)
                    continue;
                lastUsuario = claveUsuario;

                lineas.Append(estado);
                lineas.Append("\\");
                if (tipo == TipoAsistente.COMPETIDOR)
                {
                    if (clase == TipoOlimpiada.OMIP)
                        lineas.Append("P-");
                    if (clase == TipoOlimpiada.OMIS)
                        lineas.Append("S-");
                }
                lineas.Append(clave);
                lineas.Append(".pdf,");
                lineas.Append(nombre);
                lineas.Append(",");
                lineas.Append(X);
                lineas.Append(",");

                string asistente = stringsAsistentes[((int)tipo) - 1];
                if (asistente.Trim().Length == 0)
                    asistente = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(tipo.ToString().ToLower());
                string[] generos = asistente.Split('/');
                if (generos.Length > 1)
                {
                    if (genero == "M")
                        asistente = generos[0];
                    else
                        asistente = generos[1];
                }

                lineas.Append(asistente);

                switch (tipo)
                {
                    case TipoAsistente.COMPETIDOR:
                    case TipoAsistente.ASESOR:
                    case TipoAsistente.LIDER:
                    case TipoAsistente.DELEGADO:
                    case TipoAsistente.DELELIDER:
                    case TipoAsistente.SUBLIDER:
                        {
                            if (estado == "MDF")
                            {
                                lineas.Append(" de la Ciudad de México");
                            }
                            else if (estado == "MEX")
                            {
                                lineas.Append(" del Estado de México");
                            }
                            else
                            {
                                lineas.Append(" del Estado de ");
                                lineas.Append(Estado.obtenerEstadoConClave(estado).nombre);
                            }
                            break;
                        }
                }

                lineas.Append(",");
                lineas.Append(baseURL);
                lineas.Append("/Profile/");
                lineas.Append(clase.ToString());
                lineas.Append("/");
                lineas.Append(omi);
                lineas.Append("/");
                lineas.Append(clave);
                lineas.Append(",");
                lineas.Append(clase.ToString());
                lineas.Append("\n");
            }

            return lineas.ToString();
        }

        public static List<OMIstats.Ajax.BuscarPersonas> buscarParaRegistro(string omi, TipoOlimpiada tipo, string estado, string input)
        {
            List<OMIstats.Ajax.BuscarPersonas> personas = new List<OMIstats.Ajax.BuscarPersonas>();

            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            if (String.IsNullOrEmpty(input))
            {
                Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, TipoOlimpiada.OMI);

                query.Append(" select p.clave, MAX(o.año) as reciente from MiembroDelegacion as md ");
                query.Append(" inner join Persona as p on p.clave = md.persona ");
                query.Append(" inner join Olimpiada as o on md.olimpiada = o.numero and md.clase = o.clase ");
                query.Append(" where md.estado = ");
                query.Append(Cadenas.comillas(estado));
                query.Append(" and o.año > ");
                query.Append(o.año - 3);
                query.Append(" and md.tipo ");
                if (tipo == TipoOlimpiada.NULL)
                    query.Append(" <> '");
                else
                    query.Append(" = '");
                query.Append(TipoAsistente.COMPETIDOR.ToString().ToLower());
                query.Append("' group by p.clave ");
                query.Append(" order by reciente desc ");

                db.EjecutarQuery(query.ToString());
                DataTable table = db.getTable();

                foreach (DataRow r in table.Rows)
                {
                    int año = int.Parse(r[1].ToString().Trim());
                    int clavePersona = (int)r[0];

                    // Filtra a los que ya están registrados este año
                    if (año == o.año)
                        continue;

                    if (tipo != TipoOlimpiada.NULL)
                    {
                        // Revisamos la última participación del competidor en particular
                        MiembroDelegacion md = MiembroDelegacion.obtenerParticipacionMasReciente(clavePersona);

                        // Esto será true si está registrado como otro tipo de asistente este año
                        if (md.olimpiada == omi)
                            continue;

                        // Descartamos a los que ya participaron en categorías mas altas
                        if (tipo == TipoOlimpiada.OMIS && md.tipoOlimpiada == TipoOlimpiada.OMI ||
                            tipo == TipoOlimpiada.OMIP && md.tipoOlimpiada != TipoOlimpiada.OMIP)
                                continue;

                        // Descartamos a los que ya se gruaduaron de la escuela en su nivel
                        Institucion.NivelInstitucion nivel = Institucion.calculaNuevoNivel(md.nivelEscuela, md.añoEscuela, (int)o.año - año);
                        if (nivel == Institucion.NivelInstitucion.UNIVERSIDAD ||
                            tipo == TipoOlimpiada.OMIS && nivel == Institucion.NivelInstitucion.PREPARATORIA ||
                            tipo == TipoOlimpiada.OMIP && nivel != Institucion.NivelInstitucion.PRIMARIA)
                            continue;
                    }

                    Persona p = Persona.obtenerPersonaConClave(clavePersona, completo:true, incluirDatosPrivados:true);
                    personas.Add(new OMIstats.Ajax.BuscarPersonas(p));
                    if (personas.Count == 10)
                        break;
                }
            }
            else
            {
                query.Append(" select top 11 * from Persona where search like ");
                query.Append(Cadenas.comillas("%" + input + "%"));
                query.Append(" and clave not in ( select persona from MiembroDelegacion where olimpiada = ");
                query.Append(Cadenas.comillas(omi));
                query.Append(" and estado = ");
                query.Append(Cadenas.comillas(estado));
                query.Append(")");

                db.EjecutarQuery(query.ToString());
                DataTable table = db.getTable();

                if (table != null)
                    foreach (DataRow r in table.Rows)
                    {
                        Persona p = new Persona();
                        p.llenarDatos(r, completo: false);
                        personas.Add(new OMIstats.Ajax.BuscarPersonas(p));
                    }
            }

            return personas;
        }
#endif
        public static string obtenerPrimerClaveDisponible(string omi, TipoOlimpiada tipo, string estado, TipoAsistente tipoAsistente = TipoAsistente.COMPETIDOR)
        {
            var e = Estado.obtenerEstadoConClave(estado);
            var prefijo = e.ISO;
            string subfijo = null;
            if (tipoAsistente == TipoAsistente.COLO || tipoAsistente == TipoAsistente.COMI)
            {
                prefijo = tipoAsistente.ToString();
                subfijo = "";
            }

            var miembros = MiembroDelegacion.obtenerMiembrosConClave(omi, tipo, prefijo, aproximarClave: true, tipoAsistente: tipoAsistente)
                .Select((miembro) => miembro.clave);

            if (tipoAsistente == TipoAsistente.COMPETIDOR)
            {
                Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, tipo);
                int maxUsers = o.getMaxParticipantesDeEstado(estado);
                for (int i = 1; i <= maxUsers; i++)
                {
                    var testClave = prefijo + "-" + i;
                    if (!miembros.Contains(testClave))
                        return testClave;
                }
            }
            else
            {
                bool siempreNumero = true;
                if (tipoAsistente == TipoAsistente.DELEGADO ||
                    tipoAsistente == TipoAsistente.DELELIDER ||
                    tipoAsistente == TipoAsistente.SUBLIDER ||
                    tipoAsistente == TipoAsistente.LIDER)
                    siempreNumero = false;
                if (subfijo == null)
                {
                    subfijo = tipoAsistente.ToString()[0] + "";
                }
                int i = 1;
                while (true)
                {
                    var testClave = prefijo + "-" + subfijo + (i > 1 || siempreNumero ? i.ToString() : "");
                    if (!miembros.Contains(testClave))
                        return testClave;
                    i++;
                }
            }

            return "";
        }
    }
}