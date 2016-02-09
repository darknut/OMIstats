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
        }

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
            USUARIO_INEXISTENTE,
            CAMPOS_USUARIO,
            FECHA_NACIMIENTO,
            CORREO,
            ESTADO,
            TIPO_ASISTENTE,
            GENERO,
            NIVEL_INSTITUCION,
            AñO_ESCUELA
        }

        // Este objeto debe de ser contenido por un objeto olimpiada,
        // por eso no cargamos un objeto olimpiada aqui

        public string usuario;
        public string nombreAsistente;
        public string fechaNacimiento;
        public string correo;
        public string genero;
        public string nombreEscuela;
        public bool escuelaPublica;
        public Institucion.NivelInstitucion nivelEscuela;
        public int añoEscuela;
        public string clave;
        public string estado;
        public TipoAsistente tipo;
        public TipoMedalla medalla;

        private bool eliminar;

        public MiembroDelegacion()
        {
            usuario = "";
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
            medalla = TipoMedalla.NULL;

            eliminar = false;
        }

        private void llenarDatos(DataRow row, bool incluirTablasAjenas)
        {
            if (incluirTablasAjenas)
            {
                usuario = row["usuario"].ToString().Trim();
                nombreAsistente = row["nombre"].ToString().Trim();
                fechaNacimiento = row["nacimiento"].ToString().Trim();
                genero = row["genero"].ToString().Trim();
                correo = row["correo"].ToString().Trim();
                nombreEscuela = row["nombreCorto"].ToString().Trim();
                escuelaPublica = (bool)row["publica"];
            }

            estado = row["estado"].ToString().Trim();
            clave = row["clave"].ToString().Trim();
            tipo = (TipoAsistente)Enum.Parse(typeof(TipoAsistente), row["tipo"].ToString().ToUpper());
            medalla = (TipoMedalla)Enum.Parse(typeof(TipoMedalla), row["medalla"].ToString().ToUpper());
            nivelEscuela = (Institucion.NivelInstitucion)row["nivel"];
            añoEscuela = (int)row["año"];
        }

        private TipoError obtenerCampos(string []datos)
        {
            if (datos.Length > 0)
                usuario = datos[0].Trim();
            if (datos.Length > 1)
                nombreAsistente = datos[1].Trim();
            if (datos.Length > 2)
                estado = datos[2].Trim();
            try
            {
                if (datos.Length > 3)
                    tipo = (TipoAsistente)Enum.Parse(typeof(TipoAsistente), datos[3].Trim().ToUpper());
            }
            catch (Exception)
            {
                return TipoError.TIPO_ASISTENTE;
            }
            if (datos.Length > 4)
                clave = datos[4].Trim();
            if (datos.Length > 5)
                fechaNacimiento = datos[5].Trim();
            try
            {
                if (datos.Length > 6)
                    genero = datos[6].Trim().ToCharArray()[0].ToString().ToUpper();
                if (genero != "M" && genero != "F")
                    return TipoError.GENERO;
            }
            catch (Exception)
            {
                return TipoError.GENERO;
            }
            if (datos.Length > 7)
                correo = datos[7].Trim();
            if (datos.Length > 8)
                nombreEscuela = datos[8].Trim();
            try
            {
                if (datos.Length > 9)
                    nivelEscuela = (Institucion.NivelInstitucion)Enum.Parse(typeof(Institucion.NivelInstitucion), datos[9].Trim().ToUpper());
            }
            catch (Exception)
            {
                return TipoError.NIVEL_INSTITUCION;
            }
            try
            {
                if (datos.Length > 10)
                    añoEscuela = Int32.Parse(datos[10]);
                if (añoEscuela < 1 || añoEscuela > 6)
                    return TipoError.AñO_ESCUELA;
            } catch (Exception)
            {
                return TipoError.AñO_ESCUELA;
            }
            if (datos.Length > 11)
                escuelaPublica = datos[11].Trim().Equals("publica", StringComparison.InvariantCultureIgnoreCase);
            if (datos.Length > 12)
                eliminar = datos[12].Trim().Equals("eliminar", StringComparison.InvariantCultureIgnoreCase);

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
        /// Regresa el año de la ultima OMI como competirdor para la persona mandada como parámetro
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
        /// <returns>Una lista con los asistentes de la OMI</returns>
        public static List<MiembroDelegacion> cargarAsistentesOMI(string omi)
        {
            List<MiembroDelegacion> lista = new List<MiembroDelegacion>();
            if (omi == null)
                return null;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select p.usuario, p.nombre, md.estado, md.tipo, md.clave,");
            query.Append(" p.nacimiento, p.genero, p.correo, i.nombreCorto, md.nivel, md.medalla,");
            query.Append(" md.año, i.publica from miembrodelegacion as md");
            query.Append(" inner join Persona as p on p.clave = md.persona ");
            query.Append(" inner join Institucion as i on i.clave = md.institucion");
            query.Append(" where md.olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(omi));
            query.Append(" order by md.clave ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                MiembroDelegacion md = new MiembroDelegacion();
                md.llenarDatos(r, incluirTablasAjenas:true);

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
        /// Guarda la linea mandada como parametro en la base de datos
        /// </summary>
        /// <param name="omi">La clave de la olimpiada</param>
        /// <param name="linea">Los datos tabulados por comas</param>
        /// <returns>Si hubo un error, lo devuelve</returns>
        public static TipoError guardarLineaAdmin(string omi, string linea)
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

                db.EjecutarQuery(query.ToString());
                table = db.getTable();

                // -TODO- Eliminar clave en tabla resultados

                return TipoError.OK;
            }

            // Verificamos que los datos mandatorios se hayan dado

            if (md.nombreAsistente.Length == 0 ||
                md.estado.Length == 0 ||
                md.tipo == TipoAsistente.NULL ||
                md.clave.Length == 0 ||
                md.genero.Length != 1 ||
                md.nombreEscuela.Length == 0 ||
                md.nivelEscuela == Institucion.NivelInstitucion.NULL ||
                md.añoEscuela == 0)
                return TipoError.FALTAN_CAMPOS;

            // Verificar que exista el usuario

            if (md.usuario.Length == 0)
            {
                // El usuario se desconoce, hay que buscarlo

                p = Persona.obtenerPersonaConNombre(md.nombreAsistente);

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

            Institucion i = Institucion.buscarInstitucionConNombre(md.nombreEscuela);

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
                query.Append(Utilities.Cadenas.comillas(md.clave));
                query.Append(", ");
                query.Append(Utilities.Cadenas.comillas(md.tipo.ToString().ToLower()));
                query.Append(", ");
                query.Append(p.clave);
                query.Append(", ");
                query.Append((int)md.medalla);
                query.Append(", ");
                query.Append((int)i.clave);
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
                md_current.llenarDatos(table.Rows[0], incluirTablasAjenas: false);

                if (md_current.clave != md.clave)
                {
                    // -TODO- Actualizar clave en tabla resultados
                }

                query.Clear();
                query.Append(" update miembrodelegacion set clave = ");
                query.Append(Utilities.Cadenas.comillas(md.clave));
                query.Append(", tipo = ");
                query.Append(Utilities.Cadenas.comillas(md.tipo.ToString().ToLower()));
                query.Append(", institucion = ");
                query.Append((int)i.clave);
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

                db.EjecutarQuery(query.ToString());
            }

            return TipoError.OK;
        }
    }
}