using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using OMIstats.Utilities;
using System.ComponentModel.DataAnnotations;

namespace OMIstats.Models
{
    public class SedeOnline
    {
        public int clave { get; set; }
        public string estado { get; set; }
        public string omi { get; set; }
        public TipoOlimpiada tipoOlimpiada { get; set; }

        [Required(ErrorMessage = "Escribe el nombre")]
        [RegularExpression(@"^[a-zA-Z0-9 ñÑáéíóúÁÉÍÓÚäëïöü#\.'-]*$", ErrorMessage = "Escribiste caracteres inválidos")]
        [MaxLength(200, ErrorMessage = "El tamaño máximo es 200 caracteres")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "Escribe el nombre")]
        [RegularExpression(@"^[a-zA-Z ñÑáéíóúÁÉÍÓÚäëïöü\.'-]*$", ErrorMessage = "Escribiste caracteres inválidos en el nombre")]
        [MaxLength(200, ErrorMessage = "El tamaño máximo es 200 caracteres")]
        public string supervisor { get; set; }

        [Required(ErrorMessage = "Escribe el teléfono")]
        [RegularExpression(@"^[0-9\.]+$", ErrorMessage = "Escribe un teléfono válido, no incluyas guiones, espacios o paréntesis")]
        [MaxLength(12, ErrorMessage = "El tamaño máximo es de 12 caracteres, no incluyas guiones, espacios o paréntesis")]
        public string telefono { get; set; }

        [Required(ErrorMessage = "Escribe el correo electrónico")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$", ErrorMessage = "Escribe un correo electrónico válido")]
        [MaxLength(50, ErrorMessage = "El tamaño máximo es de 50 caracteres")]
        public string correo { get; set; }

        [RegularExpression(@"^[a-zA-Z ñÑáéíóúÁÉÍÓÚäëïöü\.'-]*$", ErrorMessage = "Escribiste caracteres inválidos en el nombre")]
        [MaxLength(200, ErrorMessage = "El tamaño máximo es 200 caracteres")]
        public string supervisor2 { get; set; }

        [RegularExpression(@"^[0-9\.]+$", ErrorMessage = "Escribe un teléfono válido, no incluyas guiones, espacios o paréntesis")]
        [MaxLength(12, ErrorMessage = "El tamaño máximo es de 12 caracteres, no incluyas guiones, espacios o paréntesis")]
        public string telefono2 { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$", ErrorMessage = "Escribe un correo electrónico válido")]
        [MaxLength(50, ErrorMessage = "El tamaño máximo es de 50 caracteres")]
        public string correo2 { get; set; }

        [RegularExpression(@"^[a-zA-Z ñÑáéíóúÁÉÍÓÚäëïöü\.'-]*$", ErrorMessage = "Escribiste caracteres inválidos en el nombre")]
        [MaxLength(200, ErrorMessage = "El tamaño máximo es 200 caracteres")]
        public string supervisor3 { get; set; }

        [RegularExpression(@"^[0-9\.]+$", ErrorMessage = "Escribe un teléfono válido, no incluyas guiones, espacios o paréntesis")]
        [MaxLength(12, ErrorMessage = "El tamaño máximo es de 12 caracteres, no incluyas guiones, espacios o paréntesis")]
        public string telefono3 { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$", ErrorMessage = "Escribe un correo electrónico válido")]
        [MaxLength(50, ErrorMessage = "El tamaño máximo es de 50 caracteres")]
        public string correo3 { get; set; }

        public SedeOnline()
        {
            clave = 0;
            estado = "";
            omi = "";
            tipoOlimpiada = TipoOlimpiada.NULL;
            nombre = "";
            supervisor = "";
            telefono = "";
            correo = "";
            supervisor2 = "";
            telefono2 = "";
            correo2 = "";
            supervisor3 = "";
            telefono3 = "";
            correo3 = "";
        }

        public void llenarDatos(DataRow r)
        {
            clave = DataRowParser.ToInt(r["clave"]);
            nombre = DataRowParser.ToString(r["nombre"]);
            omi = DataRowParser.ToString(r["olimpiada"]);
            tipoOlimpiada = DataRowParser.ToTipoOlimpiada(r["clase"]);
            estado = DataRowParser.ToString(r["estado"]);
            supervisor = DataRowParser.ToString(r["supervisor"]);
            telefono = DataRowParser.ToString(r["telefono"]);
            correo = DataRowParser.ToString(r["correo"]);
            supervisor2 = DataRowParser.ToString(r["supervisor2"]);
            telefono2 = DataRowParser.ToString(r["telefono2"]);
            correo2 = DataRowParser.ToString(r["correo2"]);
            supervisor3 = DataRowParser.ToString(r["supervisor3"]);
            telefono3 = DataRowParser.ToString(r["telefono3"]);
            correo3 = DataRowParser.ToString(r["correo3"]);
        }

        private void nuevo()
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" insert into SedeOnline values( ");
            query.Append(Cadenas.comillas(estado));
            query.Append(" ,");
            query.Append(Cadenas.comillas(omi));
            query.Append(" ,");
            query.Append(Cadenas.comillas(nombre));
            query.Append(" ,");
            query.Append(Cadenas.comillas(supervisor));
            query.Append(" ,");
            query.Append(Cadenas.comillas(telefono));
            query.Append(" ,");
            query.Append(Cadenas.comillas(correo));
            query.Append(" ,");
            query.Append(Cadenas.comillas(supervisor2));
            query.Append(" ,");
            query.Append(Cadenas.comillas(telefono2));
            query.Append(" ,");
            query.Append(Cadenas.comillas(correo2));
            query.Append(" ,");
            query.Append(Cadenas.comillas(supervisor3));
            query.Append(" ,");
            query.Append(Cadenas.comillas(telefono3));
            query.Append(" ,");
            query.Append(Cadenas.comillas(correo3));
            query.Append(" ,");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(")");

            db.EjecutarQuery(query.ToString());
        }

        private void update()
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" update SedeOnline set estado = ");
            query.Append(Cadenas.comillas(estado));
            query.Append(" , olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" , clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString()));
            query.Append(" , nombre = ");
            query.Append(Cadenas.comillas(nombre));
            query.Append(" , supervisor = ");
            query.Append(Cadenas.comillas(supervisor));
            query.Append(" , telefono = ");
            query.Append(Cadenas.comillas(telefono));
            query.Append(" , correo = ");
            query.Append(Cadenas.comillas(correo));
            query.Append(" , supervisor2 = ");
            query.Append(Cadenas.comillas(supervisor2));
            query.Append(" , telefono2 = ");
            query.Append(Cadenas.comillas(telefono2));
            query.Append(" , correo2 = ");
            query.Append(Cadenas.comillas(correo2));
            query.Append(" , supervisor3 = ");
            query.Append(Cadenas.comillas(supervisor3));
            query.Append(" , telefono3 = ");
            query.Append(Cadenas.comillas(telefono3));
            query.Append(" , correo3 = ");
            query.Append(Cadenas.comillas(correo3));
            query.Append(" where clave = ");
            query.Append(clave);

            db.EjecutarQuery(query.ToString());
        }

        public void guardar()
        {
            tipoOlimpiada = obtenerTipo(tipoOlimpiada);

            if (clave == 0)
                nuevo();
            else
                update();

            // Una vez guardada la sede, tratamos de crear usuarios para los supervisores
            List<MiembroDelegacion> miembros = MiembroDelegacion.obtenerMiembrosDelegacion(omi, estado, TipoOlimpiada.NULL, esParaRegistro: true);
            tryGeneraUsuarioParaSupervisor(supervisor, correo, telefono, miembros);
            if (!String.IsNullOrEmpty(supervisor2))
                tryGeneraUsuarioParaSupervisor(supervisor2, correo2, telefono2, miembros);
            if (!String.IsNullOrEmpty(supervisor3))
                tryGeneraUsuarioParaSupervisor(supervisor3, correo3, telefono3, miembros);
        }

        private void tryGeneraUsuarioParaSupervisor(string nombre, string correo, string telefono, List<MiembroDelegacion> miembros)
        {
            Persona p = Persona.obtenerPersonaConNombre(nombre);
            if (p == null)
                p = Persona.obtenerPersonaConCorreo(correo);
            // Si no hay persona con ese nombre o correo, creamos una cuenta
            if (p == null)
            {
                p = new Persona();
                p.nombre = nombre;
                p.breakNombre();
                p.correo = correo;
                p.celular = telefono;
                p.nuevoUsuario(Archivos.FotoInicial.DOMI);
                p.guardarDatos(lugarGuardado: Persona.LugarGuardado.REGISTRO);
            }

            // Si la persona todavía no es parte de la delegación, la agregamos
            if (!miembros.Any(miembro => miembro.claveUsuario == p.clave))
            {
                MiembroDelegacion md = new MiembroDelegacion();
                md.claveUsuario = p.clave;
                md.estado = estado;
                md.tipo = MiembroDelegacion.TipoAsistente.SUPERVISOR;
                md.tipoOlimpiada = this.tipoOlimpiada;
                md.olimpiada = omi;
                md.nuevo();
            }
        }

        private static TipoOlimpiada obtenerTipo(TipoOlimpiada tipo)
        {
            if (tipo == TipoOlimpiada.OMIP || tipo == TipoOlimpiada.OMIS)
                return TipoOlimpiada.OMI;
            if (tipo == TipoOlimpiada.OMISO)
                return TipoOlimpiada.OMIPO;
            return tipo;
        }

        public static List<SedeOnline> obtenerSedes(string omi, string estado, TipoOlimpiada tipoOlimpiada)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();
            List<SedeOnline> list = new List<SedeOnline>();

            tipoOlimpiada = obtenerTipo(tipoOlimpiada);

            query.Append(" select * from SedeOnline where olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString()));
            if (!String.IsNullOrEmpty(estado))
            {
                query.Append(" and estado = ");
                query.Append(Cadenas.comillas(estado));
            }
            query.Append(" order by estado asc ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                SedeOnline so = new SedeOnline();
                so.llenarDatos(r);
                list.Add(so);
            }

            return list;
        }

        public static SedeOnline obtenerSedeConClave(int clave)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from SedeOnline where clave = ");
            query.Append(clave);

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();
            SedeOnline so = null;

            if (table.Rows.Count > 0)
            {
                so = new SedeOnline();
                so.llenarDatos(table.Rows[0]);
            }

            return so;
        }

        public void borrar()
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" delete SedeOnline where clave = ");
            query.Append(clave);

            db.EjecutarQuery(query.ToString());
        }
    }
}
