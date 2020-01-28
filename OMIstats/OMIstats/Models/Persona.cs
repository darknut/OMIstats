using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace OMIstats.Models
{
    public class Persona
    {
        public const int TamañoUsuarioMaximo = 20;
        public const int UsuarioNulo = -1;
        public const int PrimerClave = 1000;

        public enum TipoPermisos
        {
            NORMAL,
            ADMIN,
            COMI,
            DELEGADO
        }

        public int clave { get; set; }

        [Required(ErrorMessage = "Escribe tu nombre")]
        [RegularExpression(@"^[a-zA-Z ñÑáéíóúÁÉÍÓÚäëïöü\.'-]*$", ErrorMessage = "Escribiste caracteres inválidos en tu nombre")]
        [MaxLength(60, ErrorMessage = "El tamaño máximo es 60 caracteres")]
        public string nombre { get; set; }

        public DateTime nacimiento { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9\.]+$", ErrorMessage = "Escribe un nombre de usuario válido")]
        [MaxLength(50, ErrorMessage = "El tamaño máximo es de 50 caracteres")]
        public string facebook { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Escribe un nombre de usuario válido")]
        [MaxLength(15, ErrorMessage = "El tamaño máximo es de 15 caracteres")]
        public string twitter { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9\.]+$", ErrorMessage = "Escribe un nombre de usuario válido")]
        [MaxLength(50, ErrorMessage = "El tamaño máximo es de 50 caracteres")]
        public string omegaup { get; set; }

        [RegularExpression(@"^(https?:\/\/)?((([\w-]+)\.){1,})([\/\w\.-]+)(\?[\/\w\.-=%&]*)?$", ErrorMessage = "Escribe una URL válida")]
        [MaxLength(100, ErrorMessage = "El tamaño máximo es de 100 caracteres")]
        public string sitio { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$", ErrorMessage = "Escribe un correo electrónico válido")]
        [MaxLength(50, ErrorMessage = "El tamaño máximo es de 50 caracteres")]
        public string correo { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9\.]+$", ErrorMessage = "Escribe un nombre de usuario válido")]
        [MaxLength(50, ErrorMessage = "El tamaño máximo es de 50 caracteres")]
        public string codeforces { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9\.]+$", ErrorMessage = "Escribe un nombre de usuario válido")]
        [MaxLength(50, ErrorMessage = "El tamaño máximo es de 50 caracteres")]
        public string topcoder { get; set; }

        public string usuario { get; set; }

        public TipoPermisos permisos { get; set; }

        public string genero { get; set; }

        public string foto { get; set; }

        public int ioiID { get; set; }

        public string CURP { get; set; }

        public Persona(int clave): this()
        {
            this.clave = clave;
        }

        public Persona()
        {
            clave = 0;
            nombre = "";
            nacimiento = new DateTime(1900, 1, 1);
            facebook = "";
            twitter = "";
            sitio = "";
            correo = "";
            usuario = "";
            permisos = TipoPermisos.NORMAL;
            genero = "M";
            foto = "";
            ioiID = 0;
            CURP = "";
            omegaup = "";
            codeforces = "";
            topcoder = "";
        }

        /// <summary>
        /// Llena los datos de una persona de la fila mandada como parametro
        /// </summary>
        /// <param name="datos">La fila con el origen de los datos</param>
        /// <param name="completo">Si es true, saca todos los datos de la fila, de ser false, solo nombre, usuario y clave</param>
        private void llenarDatos(DataRow datos, bool completo = true)
        {
            clave = (int) datos["clave"];
            nombre = datos["nombre"].ToString().Trim();
            usuario = datos["usuario"].ToString().Trim();

            if (completo)
            {
                nacimiento = Utilities.Fechas.stringToDate(datos["nacimiento"].ToString().Trim());
                facebook = datos["facebook"].ToString().Trim();
                twitter = datos["twitter"].ToString().Trim();
                sitio = datos["sitio"].ToString().Trim();
                correo = datos["correo"].ToString().Trim();
                permisos = (TipoPermisos)Enum.Parse(typeof(TipoPermisos), datos["permisos"].ToString());
                genero = datos["genero"].ToString();
                foto = datos["foto"].ToString().Trim();
                ioiID = (int) datos["ioiID"];
                CURP = datos["CURP"].ToString().Trim();
                omegaup = datos["omegaup"].ToString().Trim();
                topcoder = datos["topcoder"].ToString().Trim();
                codeforces = datos["codeforces"].ToString().Trim();
            }
        }

        /// <summary>
        /// Revisa si hay un usuario loggeado
        /// </summary>
        public static bool isLoggedIn(Object obj)
        {
            if (obj == null)
                return false;

            Persona p = (Persona)obj;

            return (p.clave > 0);
        }

        /// <summary>
        /// Regresa el objeto persona asociado con el nombre de usuario mandado como parámetro
        /// </summary>
        public static Persona obtenerPersonaDeUsuario(string usuario)
        {
            if (String.IsNullOrEmpty(usuario))
                return null;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append("select * from persona where usuario = ");
            query.Append(Utilities.Cadenas.comillas(usuario.ToLower()));

            if (db.EjecutarQuery(query.ToString()).error)
                return null;

            DataTable table = db.getTable();
            if (table.Rows.Count != 1)
                return null;

            Persona p = new Persona();
            p.llenarDatos(table.Rows[0]);

            return p;
        }

        /// <summary>
        /// Regresa el objeto persona asociado con el nombre mandado como parámetro
        /// <param name="nombre">El nombre a aproximar</param>
        /// <param name="ignorarUsuarios">verdadero si queremos ignorar personas que ya hayan
        /// sido asignados un usuario</param>
        /// </summary>
        public static Persona obtenerPersonaConNombre(string nombre, bool ignorarUsuarios = false)
        {
            if (String.IsNullOrEmpty(nombre))
                return null;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append("select * from persona where search = ");
            query.Append(Utilities.Cadenas.comillas(Utilities.Cadenas.quitaEspeciales(nombre)));
            if (ignorarUsuarios)
                query.Append(" and LEFT(usuario, 1) = '_' ");

            if (db.EjecutarQuery(query.ToString()).error)
                return null;

            DataTable table = db.getTable();
            if (table.Rows.Count == 0)
                return null;

            Persona p = new Persona();
            p.llenarDatos(table.Rows[0]);

            return p;
        }

        /// <summary>
        /// Regresa la persona asociada con la clave mandada como parametro
        /// </summary>
        public static Persona obtenerPersonaConClave(int clave)
        {
            Persona p = new Persona();
            p.clave = clave;
            if (p.recargarDatos())
                return p;
            else
                return null;
        }

        /// <summary>
        /// Lee de nuevo los datos del usuario de la base de datos y los actualiza en el objeto
        /// </summary>
        /// <returns>Si se recargaron satisfactoriamente los datos</returns>
        public bool recargarDatos()
        {
            if (clave == 0)
                return false;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append("select * from persona where clave = ");
            query.Append(clave);

            if (db.EjecutarQuery(query.ToString()).error)
                return false;

            DataTable table = db.getTable();
            if (table.Rows.Count != 1)
                return false;

            llenarDatos(table.Rows[0]);

            return true;
        }

        /// <summary>
        /// Guarda los datos en la base de datos
        /// </summary>
        /// <param name="generarPeticiones">Si nombre y foto deben de guardarse
        /// directamente en la base de datos o si se deben generar peticiones</param>
        /// <returns>Si el update se ejecutó correctamente</returns>
        public bool guardarDatos(bool generarPeticiones = false)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" update persona set ");

            if (!String.IsNullOrEmpty(nombre))
            {
                if (generarPeticiones)
                {
                    Peticion pet = new Peticion();
                    pet.tipo = Peticion.TipoPeticion.USUARIO;
                    pet.subtipo = Peticion.TipoPeticion.NOMBRE;
                    pet.usuario = this;
                    pet.datos1 = nombre;
                    pet.guardarPeticion();
                }
                else
                {
                    query.Append(" nombre = ");
                    query.Append(Utilities.Cadenas.comillas(nombre));
                    query.Append(",");

                    query.Append(" search = ");
                    query.Append(Utilities.Cadenas.comillas(Utilities.Cadenas.quitaEspeciales(nombre)));
                    query.Append(",");
                }
            }

            query.Append(" facebook = ");
            query.Append(Utilities.Cadenas.comillas(facebook));
            query.Append(",");

            query.Append(" twitter = ");
            query.Append(Utilities.Cadenas.comillas(twitter));
            query.Append(",");

            query.Append(" sitio = ");
            query.Append(Utilities.Cadenas.comillas(sitio));
            query.Append(",");

            query.Append(" usuario = ");
            query.Append(Utilities.Cadenas.comillas(usuario));
            query.Append(",");

            query.Append(" permisos = ");
            query.Append((int) permisos);
            query.Append(",");

            query.Append(" genero = ");
            query.Append(Utilities.Cadenas.comillas(genero));
            query.Append(",");

            query.Append(" omegaup = ");
            query.Append(Utilities.Cadenas.comillas(omegaup));
            query.Append(",");

            query.Append(" codeforces = ");
            query.Append(Utilities.Cadenas.comillas(codeforces));
            query.Append(",");

            query.Append(" topcoder = ");
            query.Append(Utilities.Cadenas.comillas(topcoder));
            query.Append(",");

            query.Append(" CURP = ");
            query.Append(Utilities.Cadenas.comillas(CURP.Trim().ToUpper()));
            query.Append(",");

            if (!String.IsNullOrEmpty(foto))
            {
                if (generarPeticiones)
                {
                    Peticion pet = new Peticion();
                    pet.tipo = Peticion.TipoPeticion.USUARIO;
                    pet.subtipo = Peticion.TipoPeticion.FOTO;
                    pet.usuario = this;
                    pet.datos1 = foto;
                    pet.guardarPeticion();
                }
                else
                {
                    query.Append(" foto = ");
                    query.Append(Utilities.Cadenas.comillas(foto));
                    query.Append(",");
                }
            }

            query.Append(" correo = ");
            query.Append(Utilities.Cadenas.comillas(correo));
            query.Append(",");

            query.Append(" nacimiento = ");
            query.Append(Utilities.Cadenas.comillas(Utilities.Fechas.dateToString(nacimiento)));
            query.Append(",");

            query.Append(" ioiID = ");
            query.Append(ioiID);

            query.Append(" where clave = ");
            query.Append(clave);

            return !db.EjecutarQuery(query.ToString()).error;
        }

        /// <summary>
        /// Regresa si el usuario tiene peticiones que él mismo puede ver
        /// </summary>
        /// <returns></returns>
        public bool tienePeticiones()
        {
            List<Peticion> peticiones = Peticion.obtenerPeticionesDeUsuario(this);
            return peticiones.Count > 0;
        }

        public bool esAdmin()
        {
            return this.permisos == TipoPermisos.ADMIN;
        }

        public bool esSuperUsuario()
        {
            return this.permisos == TipoPermisos.ADMIN ||
                this.permisos == TipoPermisos.COMI;
        }

        /// <summary>
        /// Crea un nuevo usuario con los datos en el objeto
        /// </summary>
        public void nuevoUsuario(Utilities.Archivos.FotoInicial fotoInicial = Utilities.Archivos.FotoInicial.KAREL)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" declare @inserted table(clave int); ");
            query.Append(" insert into persona (nombre) output inserted.clave into @inserted values( ");
            query.Append(Utilities.Cadenas.comillas(nombre));
            query.Append("); select clave from @inserted ");

            if (db.EjecutarQuery(query.ToString()).error)
                return;

            DataTable table = db.getTable();
            if (table.Rows.Count != 1)
                return;
            clave = (int)table.Rows[0][0];
            usuario = "_" + clave.ToString();
            foto = Utilities.Archivos.obtenerFotoInicial(fotoInicial);

            guardarDatos();
        }

        /// <summary>
        /// Borra de la base de datos todos los competidores sin ningún rol en la OMI
        /// </summary>
        public static void borrarZombies()
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" delete persona where clave in ( ");
            query.Append(" select clave from Persona where clave ");
            query.Append(" not in (select distinct(Persona) from MiembroDelegacion) and clave not in ");
            query.Append(" (select delegado from Estado where delegado is not null))");

            db.EjecutarQuery(query.ToString());
        }

        /// <summary>
        /// A partir de la persona obtenida de la base de datos de la OMI, hacemos la mejor aproximacion
        /// para obtener el objeto deseado
        /// </summary>
        /// <param name="usuario">El usuario obtenido de la base de la OMI</param>
        /// <returns>La persona correspondiente en nuestra base</returns>
        public static Persona obtenerPersonaDeUsuario(Usuario usuario)
        {
            // Primero revisamos los CURP
            Persona p = Persona.obtenerPersonaConCURP(usuario.CURP, ignorarUsuarios: true);
            if (p != null)
                return p;

            // Ahora intentamos aproximar el nombre
            p = Persona.obtenerPersonaConNombre(usuario.Nombre, ignorarUsuarios: true);
            if (p != null)
                return p;

            // Como último recurso, buscamos por correo
            p = Persona.obtenerPersonaConCorreo(usuario.Email, ignorarUsuarios: true);

            return p;
        }

        /// <summary>
        /// Busca una persona con el curp mandado como parámetro
        /// </summary>
        /// <param name="CURP">El curp de la persona deseada</param>
        /// <param name="ignorarUsuarios">verdadero cuando no queremos recibir resultados de
        /// personas que ya tienen asignado un usuario</param>
        /// <returns></returns>
        public static Persona obtenerPersonaConCURP(string CURP, bool ignorarUsuarios = false)
        {
            if (CURP == null)
                return null;

            CURP = CURP.Trim().ToUpper();

            if (CURP.Length == 0)
                return null;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append("select * from persona where CURP = ");
            query.Append(Utilities.Cadenas.comillas(CURP));
            if (ignorarUsuarios)
                query.Append(" and LEFT(usuario, 1) = '_' ");

            if (db.EjecutarQuery(query.ToString()).error)
                return null;

            DataTable table = db.getTable();
            if (table.Rows.Count != 1)
                return null;

            Persona p = new Persona();
            p.llenarDatos(table.Rows[0]);

            return p;
        }

        /// <summary>
        /// Busca una persona con el correo mandado como parámetro
        /// </summary>
        /// <param name="correo">El correode la persona deseada</param>
        /// <param name="ignorarUsuarios">verdadero cuando no queremos recibir resultados de
        /// personas que ya tienen asignado un usuario</param>
        /// <returns></returns>
        public static Persona obtenerPersonaConCorreo(string correo, bool ignorarUsuarios = false)
        {
            if (correo == null)
                return null;

            correo = correo.Trim().ToLower();

            if (correo.Length == 0)
                return null;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append("select * from persona where correo = ");
            query.Append(Utilities.Cadenas.comillas(correo));
            if (ignorarUsuarios)
                query.Append(" and LEFT(usuario, 1) = '_' ");

            if (db.EjecutarQuery(query.ToString()).error)
                return null;

            DataTable table = db.getTable();
            if (table.Rows.Count == 0)
                return null;

            Persona p = new Persona();
            p.llenarDatos(table.Rows[0]);

            return p;
        }

        /// <summary>
        /// Busca personas con el nombre mandado como parámetro
        /// </summary>
        /// <param name="nombre">El nombre a buscar</param>
        /// <returns>La lista de resultados</returns>
        public static List<SearchResult> buscar(string nombre)
        {
            List<SearchResult> resultados = new List<SearchResult>();
            if (String.IsNullOrEmpty(nombre))
                return resultados;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append("select * from persona where search like ");
            query.Append(Utilities.Cadenas.comillas("%" + Utilities.Cadenas.quitaEspeciales(nombre) + "%"));
            query.Append(" order by search asc");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();
            if (table.Rows.Count == 0)
                return resultados;

            foreach (DataRow r in table.Rows)
            {
                Persona p = new Persona();
                p.llenarDatos(r, completo: false);

                SearchResult sr = new SearchResult(p);
                resultados.Add(sr);
            }

            return resultados;
        }

        /// <summary>
        /// Calcula los estados de los cuales hay participantes para la persona
        /// </summary>
        /// <param name="tipoOlimpiada">El tipo de olimpiada</param>
        public List<string> consultarEstados(TipoOlimpiada tipoOlimpiada = TipoOlimpiada.NULL)
        {
            List<string> estados = new List<string>();
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select distinct(estado) from MiembroDelegacion where persona = ");
            query.Append(this.clave);
            if (tipoOlimpiada != TipoOlimpiada.NULL)
            {
                query.Append(" and clase = ");
                query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            }

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            estados = new List<string>();
            foreach (DataRow r in table.Rows)
            {
                string estado = r[0].ToString().Trim();
                estados.Add(estado);
            }
            return estados;
        }

        /// <summary>
        /// Calcula las participaciones diferente a competidor que tiene la persona
        /// </summary>
        /// <param name="tipoOlimpiada">El tipo de olimpiada</param>
        public List<string> consultarParticipaciones(TipoOlimpiada tipoOlimpiada = TipoOlimpiada.NULL)
        {
            List<string> tipos = new List<string>();
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select distinct(tipo) from MiembroDelegacion where persona = ");
            query.Append(this.clave);
            query.Append(" and tipo != ");
            query.Append(Utilities.Cadenas.comillas(MiembroDelegacion.TipoAsistente.COMPETIDOR.ToString().ToLower()));
            if (tipoOlimpiada != TipoOlimpiada.NULL)
            {
                query.Append(" and clase = ");
                query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            }

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            tipos = new List<string>();
            foreach (DataRow r in table.Rows)
            {
                MiembroDelegacion.TipoAsistente tipo = (MiembroDelegacion.TipoAsistente) Enum.Parse(typeof(MiembroDelegacion.TipoAsistente), r[0].ToString().Trim().ToUpper());
                if (tipo == MiembroDelegacion.TipoAsistente.DELELIDER)
                {
                    if (!tipos.Contains(MiembroDelegacion.TipoAsistente.LIDER.ToString()))
                        tipos.Add(MiembroDelegacion.TipoAsistente.LIDER.ToString());
                    if (!tipos.Contains(MiembroDelegacion.TipoAsistente.DELEGADO.ToString()))
                        tipos.Add(MiembroDelegacion.TipoAsistente.DELEGADO.ToString());
                }
                else
                {
                    string t = MiembroDelegacion.getTipoAsistenteString(tipo);
                    if (!tipos.Contains(t))
                        tipos.Add(t);
                }
            }
            return tipos;
        }

        /// <summary>
        /// Regresa como un arreglo de strings las claves para las que este delegado puede registrar
        /// competidores
        /// </summary>
        /// <returns>La lista de claves de estado</returns>
        public List<Estado> obtenerEstadosDeDelegado()
        {
            if (this.permisos != TipoPermisos.DELEGADO)
                return null;
            List<Estado> estados = new List<Estado>();
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select distinct(estado) from MiembroDelegacion where persona =");
            query.Append(this.clave);

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                estados.Add(Estado.obtenerEstadoConClave(r[0].ToString().Trim()));
            }

            return estados;
        }
    }
}