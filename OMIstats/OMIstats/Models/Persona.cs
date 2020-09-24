using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using OMIstats.Utilities;

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

        public enum LugarGuardado
        {
            PROFILE,
            REGISTRO
        }

        public int clave { get; set; }

        [Required(ErrorMessage = "Escribe el nombre")]
        [RegularExpression(@"^[a-zA-Z ñÑáéíóúÁÉÍÓÚäëïöü\.'-]*$", ErrorMessage = "Escribiste caracteres inválidos en el nombre")]
        [MaxLength(60, ErrorMessage = "El tamaño máximo es 60 caracteres")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "Escribe el apellido paterno")]
        [RegularExpression(@"^[a-zA-Z ñÑáéíóúÁÉÍÓÚäëïöü\.'-]*$", ErrorMessage = "Escribiste caracteres inválidos en el apellido paterno")]
        [MaxLength(60, ErrorMessage = "El tamaño máximo es 60 caracteres")]
        public string apellidoPaterno { get; set; }

        [RegularExpression(@"^[a-zA-Z ñÑáéíóúÁÉÍÓÚäëïöü\.'-]*$", ErrorMessage = "Escribiste caracteres inválidos en el apellido materno")]
        [MaxLength(60, ErrorMessage = "El tamaño máximo es 60 caracteres")]
        public string apellidoMaterno { get; set; }

        public DateTime nacimiento { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9\.]+$", ErrorMessage = "Escribe un nombre de usuario válido")]
        [MaxLength(50, ErrorMessage = "El tamaño máximo es de 50 caracteres")]
        public string facebook { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Escribe un nombre de usuario válido")]
        [MaxLength(15, ErrorMessage = "El tamaño máximo es de 15 caracteres")]
        public string twitter { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9_.-]+(?::[a-zA-Z0-9_.-]+)?$", ErrorMessage = "Escribe un nombre de usuario válido")]
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

        [RegularExpression(@"^[0-9\.]+$", ErrorMessage = "Escribe un teléfono válido, no incluyas guiones, espacios o paréntesis")]
        [MaxLength(12, ErrorMessage = "El tamaño máximo es de 12 caracteres, no incluyas guiones, espacios o paréntesis")]
        public string celular { get; set; }

        [RegularExpression(@"^[0-9\.]+$", ErrorMessage = "Escribe un teléfono válido, no incluyas guiones, espacios o paréntesis")]
        [MaxLength(12, ErrorMessage = "El tamaño máximo es de 12 caracteres, no incluyas guiones, espacios o paréntesis")]
        public string telefono { get; set; }

        [MaxLength(100, ErrorMessage = "El tamaño máximo es de 100 caracteres. Abrevia la dirección lo más posible")]
        public string direccion { get; set; }

        [MaxLength(60, ErrorMessage = "El tamaño máximo es de 60 caracteres")]
        public string emergencia { get; set; }

        [MaxLength(20, ErrorMessage = "El tamaño máximo es de 20 caracteres")]
        public string parentesco { get; set; }

        [RegularExpression(@"^[0-9\.]+$", ErrorMessage = "Escribe un teléfono válido, no incluyas guiones, espacios o paréntesis")]
        [MaxLength(12, ErrorMessage = "El tamaño máximo es de 12 caracteres, no incluyas guiones, espacios o paréntesis")]
        public string telEmergencia { get; set; }

        [MaxLength(60, ErrorMessage = "El tamaño máximo es de 60 caracteres")]
        public string medicina { get; set; }

        [MaxLength(60, ErrorMessage = "El tamaño máximo es de 60 caracteres")]
        public string alergias { get; set; }

        public string usuario { get; set; }

        public TipoPermisos permisos { get; set; }

        public string genero { get; set; }

        public string foto { get; set; }

        public int ioiID { get; set; }

        public bool omips;

        public string nombreCompleto
        {
            get
            {
                return (nombre + " " + apellidoPaterno + " " + apellidoMaterno).Trim();
            }
        }

        public Persona(int clave): this()
        {
            this.clave = clave;
        }

        public Persona()
        {
            clave = 0;
            nombre = "";
            apellidoPaterno = "";
            apellidoMaterno = "";
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
            omegaup = "";
            codeforces = "";
            topcoder = "";
            celular = "";
            telefono = "";
            direccion = "";
            emergencia = "";
            parentesco = "";
            telEmergencia = "";
            medicina = "";
            alergias = "";
            omips = false;
        }

        /// <summary>
        /// Llena los datos de una persona de la fila mandada como parametro
        /// </summary>
        /// <param name="datos">La fila con el origen de los datos</param>
        /// <param name="completo">Si es true, saca todos los datos de la fila, de ser false, solo nombre, usuario y clave</param>
        /// <param name="completo">Si es true, incluye datos privados como telefono y direccion </param>
        public void llenarDatos(DataRow datos, bool completo = true, bool incluirDatosPrivados = false)
        {
            clave = (int) datos["clave"];
            nombre = datos["nombre"].ToString().Trim();
            apellidoPaterno = datos["apellidoP"].ToString().Trim();
            apellidoMaterno = datos["apellidoM"].ToString().Trim();
            usuario = datos["usuario"].ToString().Trim();
            omips = (bool) datos["omips"];

            if (completo)
            {
                nacimiento = Fechas.stringToDate(datos["nacimiento"].ToString().Trim());
                facebook = datos["facebook"].ToString().Trim();
                twitter = datos["twitter"].ToString().Trim();
                sitio = datos["sitio"].ToString().Trim();
                correo = datos["correo"].ToString().Trim();
                permisos = EnumParser.ToTipoPermisos(datos["permisos"].ToString());
                genero = datos["genero"].ToString();
                foto = datos["foto"].ToString().Trim();
                ioiID = (int)datos["ioiID"];
                omegaup = datos["omegaup"].ToString().Trim();
                topcoder = datos["topcoder"].ToString().Trim();
                codeforces = datos["codeforces"].ToString().Trim();

                if (incluirDatosPrivados)
                {
                    celular = datos["celular"].ToString().Trim();
                    telefono = datos["telefono"].ToString().Trim();
                    direccion = datos["direccion"].ToString().Trim();
                    emergencia = datos["emergencia"].ToString().Trim();
                    parentesco = datos["parentesco"].ToString().Trim();
                    telEmergencia = datos["telemergencia"].ToString().Trim();
                    medicina = datos["medicina"].ToString().Trim();
                    alergias = datos["alergias"].ToString().Trim();
                }
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

            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append("select * from persona where usuario = ");
            query.Append(Cadenas.comillas(usuario.ToLower()));

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

            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append("select * from persona where search = ");
            query.Append(Cadenas.comillas(Cadenas.quitaEspeciales(nombre)));
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
        public static Persona obtenerPersonaConClave(int clave, bool completo = true, bool incluirDatosPrivados = false)
        {
            Persona p = new Persona();
            p.clave = clave;
            if (p.recargarDatos(completo, incluirDatosPrivados))
                return p;
            else
                return null;
        }

        /// <summary>
        /// Lee de nuevo los datos del usuario de la base de datos y los actualiza en el objeto
        /// </summary>
        /// <returns>Si se recargaron satisfactoriamente los datos</returns>
        public bool recargarDatos(bool completo = true, bool incluirDatosPrivados = false)
        {
            if (clave == 0)
                return false;

            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append("select * from persona where clave = ");
            query.Append(clave);

            if (db.EjecutarQuery(query.ToString()).error)
                return false;

            DataTable table = db.getTable();
            if (table.Rows.Count != 1)
                return false;

            llenarDatos(table.Rows[0], completo, incluirDatosPrivados);

            return true;
        }

        /// <summary>
        /// Guarda los datos en la base de datos
        /// </summary>
        /// <param name="generarPeticiones">Si nombre y foto deben de guardarse
        /// directamente en la base de datos o si se deben generar peticiones</param>
        /// <param name="currentValues">El valor actual de los datos</param>
        /// <param name="lugarGuardado">El lugar de donde viene el guardado de datos</param>
        /// <returns>Si el update se ejecutó correctamente</returns>
        public bool guardarDatos(bool generarPeticiones = false, Persona currentValues = null, LugarGuardado lugarGuardado = LugarGuardado.PROFILE)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" update persona set ");

            if (generarPeticiones && nombreCompleto != currentValues.nombreCompleto)
            {
                if (nombre != currentValues.nombre)
                {
                    Peticion pet = new Peticion();
                    pet.tipo = Peticion.TipoPeticion.USUARIO;
                    pet.subtipo = Peticion.TipoPeticion.NOMBRE;
                    pet.usuario = this;
                    pet.datos1 = nombre;
                    pet.guardarPeticion();
                }

                if (apellidoPaterno != currentValues.apellidoPaterno)
                {
                    Peticion pet = new Peticion();
                    pet.tipo = Peticion.TipoPeticion.USUARIO;
                    pet.subtipo = Peticion.TipoPeticion.APELLIDOPATERNO;
                    pet.usuario = this;
                    pet.datos1 = apellidoPaterno;
                    pet.guardarPeticion();
                }

                if (apellidoMaterno != currentValues.apellidoMaterno)
                {
                    Peticion pet = new Peticion();
                    pet.tipo = Peticion.TipoPeticion.USUARIO;
                    pet.subtipo = Peticion.TipoPeticion.APELLIDOMATERNO;
                    pet.usuario = this;
                    pet.datos1 = apellidoMaterno;
                    pet.guardarPeticion();
                }
            }
            else
            {
                query.Append(" nombre = ");
                query.Append(Cadenas.comillas(nombre));
                query.Append(",");

                query.Append(" apellidoP = ");
                query.Append(Cadenas.comillas(apellidoPaterno));
                query.Append(",");

                query.Append(" apellidoM = ");
                query.Append(Cadenas.comillas(apellidoMaterno));
                query.Append(",");

                query.Append(" search = ");
                query.Append(Cadenas.comillas(Cadenas.quitaEspeciales(nombre + " " + apellidoPaterno + " " + apellidoMaterno)));
                query.Append(",");
            }

            if (lugarGuardado == LugarGuardado.PROFILE)
            {
                query.Append(" facebook = ");
                query.Append(Cadenas.comillas(facebook));
                query.Append(",");

                query.Append(" twitter = ");
                query.Append(Cadenas.comillas(twitter));
                query.Append(",");

                query.Append(" sitio = ");
                query.Append(Cadenas.comillas(sitio));
                query.Append(",");

                query.Append(" usuario = ");
                query.Append(Cadenas.comillas(usuario));
                query.Append(",");

                query.Append(" permisos = ");
                query.Append((int)permisos);
                query.Append(",");

                query.Append(" codeforces = ");
                query.Append(Cadenas.comillas(codeforces));
                query.Append(",");

                query.Append(" topcoder = ");
                query.Append(Cadenas.comillas(topcoder));
                query.Append(",");

                query.Append(" ioiID = ");
                query.Append(ioiID);
                query.Append(",");
            }

            query.Append(" genero = ");
            query.Append(Cadenas.comillas(genero));
            query.Append(",");

            query.Append(" omegaup = ");
            query.Append(Cadenas.comillas(omegaup));
            query.Append(",");

            query.Append(" omips = ");
            query.Append(omips ? 1 : 0);
            query.Append(",");

            if (lugarGuardado == LugarGuardado.REGISTRO)
            {
                query.Append(" celular = ");
                query.Append(Cadenas.comillas(celular));
                query.Append(",");

                query.Append(" telefono = ");
                query.Append(Cadenas.comillas(telefono));
                query.Append(",");

                query.Append(" direccion = ");
                query.Append(Cadenas.comillas(direccion));
                query.Append(",");

                query.Append(" emergencia = ");
                query.Append(Cadenas.comillas(emergencia));
                query.Append(",");

                query.Append(" parentesco = ");
                query.Append(Cadenas.comillas(parentesco));
                query.Append(",");

                query.Append(" telemergencia = ");
                query.Append(Cadenas.comillas(telEmergencia));
                query.Append(",");

                query.Append(" medicina = ");
                query.Append(Cadenas.comillas(medicina));
                query.Append(",");

                query.Append(" alergias = ");
                query.Append(Cadenas.comillas(alergias));
                query.Append(",");
            }

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
                    query.Append(Cadenas.comillas(foto));
                    query.Append(",");
                }
            }

            query.Append(" correo = ");
            query.Append(Cadenas.comillas(correo));
            query.Append(",");

            query.Append(" nacimiento = ");
            query.Append(Cadenas.comillas(Fechas.dateToString(nacimiento)));

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
        public void nuevoUsuario(Archivos.FotoInicial fotoInicial = Archivos.FotoInicial.KAREL)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" declare @inserted table(clave int); ");
            query.Append(" insert into persona (nombre, facebook, twitter, sitio, usuario, permisos, codeforces,");
            query.Append(" topcoder, ioiID, celular, telefono, direccion, emergencia, parentesco, telemergencia,");
            query.Append(" medicina, alergias, omips) output inserted.clave into @inserted values( ");
            query.Append(Cadenas.comillas(nombre));
            query.Append(" ,'', '', '', '', 0, '', '', 0, '', '', '', '', '', '', '', '', 0); select clave from @inserted ");

            if (db.EjecutarQuery(query.ToString()).error)
                return;

            DataTable table = db.getTable();
            if (table.Rows.Count != 1)
                return;
            clave = (int)table.Rows[0][0];
            usuario = "_" + clave.ToString();
            foto = Archivos.obtenerFotoInicial(fotoInicial);

            guardarDatos();
        }

        /// <summary>
        /// Borra de la base de datos todos los competidores sin ningún rol en la OMI
        /// </summary>
        public static void borrarZombies()
        {
            Acceso db = new Acceso();
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
            // Primero intentamos aproximar el nombre
            Persona p = Persona.obtenerPersonaConNombre(usuario.Nombre, ignorarUsuarios: true);
            if (p != null)
                return p;

            // También buscamos por correo
            p = Persona.obtenerPersonaConCorreo(usuario.Email, ignorarUsuarios: true);

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

            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append("select * from persona where correo = ");
            query.Append(Cadenas.comillas(correo));
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

            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append("select * from persona where search like ");
            query.Append(Cadenas.comillas("%" + Cadenas.quitaEspeciales(nombre) + "%"));
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
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select distinct(estado) from MiembroDelegacion where persona = ");
            query.Append(this.clave);
            if (tipoOlimpiada != TipoOlimpiada.NULL)
            {
                query.Append(" and clase = ");
                query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
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
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select distinct(tipo) from MiembroDelegacion where persona = ");
            query.Append(this.clave);
            query.Append(" and tipo != ");
            query.Append(Cadenas.comillas(MiembroDelegacion.TipoAsistente.COMPETIDOR.ToString().ToLower()));
            if (tipoOlimpiada != TipoOlimpiada.NULL)
            {
                query.Append(" and clase = ");
                query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            }

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            tipos = new List<string>();
            foreach (DataRow r in table.Rows)
            {
                MiembroDelegacion.TipoAsistente tipo = EnumParser.ToTipoAsistente(r[0].ToString().Trim().ToUpper());
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
            Acceso db = new Acceso();
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

        /// <summary>
        /// Trata de poner el nombre mandado como parámetro en nombre y apellidos
        /// </summary>
        /// <param name="nombre">El nombre a analizar, de no venir se usa el que está en el objeto</param>
        /// <param name="ignorarApellidos">Si se van a sobreescribir los apellidos en el objeto</param>
        public void breakNombre(string nombre = null, bool ignorarApellidos = false)
        {
            if (ignorarApellidos)
                apellidoMaterno = apellidoPaterno = "";
            if (nombre == null)
            {
                if (this.apellidoPaterno.Length > 0)
                    return;
                nombre = this.nombre;
            }

            string[] nombres = nombre.Split(' ');
            if (nombres.Count() == 1)
            {
                this.nombre = nombre;
                this.apellidoPaterno = "";
                this.apellidoMaterno = "";
            }
            else if (nombres.Count() == 2)
            {
                this.nombre = nombres[0];
                this.apellidoPaterno = nombres[1];
                this.apellidoMaterno = "";
            }
            else
            {
                int i = nombres.Count() - 1;
                if (nombres[i - 1].Length < 4)
                {
                    this.apellidoMaterno = nombres[i - 1] + " " + nombres[i];
                    i -= 2;
                }
                else
                {
                    this.apellidoMaterno = nombres[i];
                    i--;
                }

                if (i == 0)
                {
                    this.nombre = nombres[0];
                    this.apellidoPaterno = this.apellidoMaterno;
                    this.apellidoMaterno = "";
                }
                else if (i == 1)
                {
                    this.nombre = nombres[0];
                    this.apellidoPaterno = nombres[1];
                }
                else
                {
                    if (nombres[i - 1].Length < 4)
                    {
                        this.apellidoPaterno = nombres[i - 1] + " " + nombres[i];
                        i -= 2;
                    }
                    else
                    {
                        this.apellidoPaterno = nombres[i];
                        i--;
                    }

                    this.nombre = "";
                    for (int j = 0; j <= i; j++)
                    {
                        this.nombre += nombres[j] + " ";
                    }
                    this.nombre = this.nombre.TrimEnd();
                }

            }
        }

        /// <summary>
        /// Regresa los datos en este objeto como un string separado por comas
        /// con los datos privados del usuario
        /// </summary>
        /// <returns>Los datos separados por coma</returns>
        public string obtenerLineaAdmin()
        {
            StringBuilder s = new StringBuilder();

            s.Append(", ");
            s.Append(celular);
            s.Append(", ");
            s.Append(telefono);
            s.Append(", ");
            s.Append(Cadenas.comillas(direccion, "\""));
            s.Append(", ");
            s.Append(omegaup);
            s.Append(", ");
            s.Append(emergencia);
            s.Append(", ");
            s.Append(parentesco);
            s.Append(", ");
            s.Append(telEmergencia);
            s.Append(", ");
            s.Append(medicina);
            s.Append(", ");
            s.Append(alergias);

            return s.ToString();
        }
    }
}