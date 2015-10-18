using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace OMIstats.Models
{
    public class Estado
    {
        public string clave { get; set; }
        public string nombre { get; set; }
        public string sitio { get; set; }
        public Persona delegado { get; set; }

        // Las siguientes variables se necesitan porque
        // MVC no soporta modelos anidados
        public int claveDelegado { get; set; }
        public string usuarioDelegado { get; set; }
        public string nombreDelegado { get; set; }
        public string mailDelegado { get; set; }

        public Estado()
        {
            clave = "";
            nombre = "";
            sitio = "";
            delegado = null;

            claveDelegado = -1;
            usuarioDelegado = "";
            nombreDelegado = "";
        }

        public static List<Estado> obtenerEstados()
        {
            List<Estado> lista = new List<Estado>();
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from estado ");

            if (db.EjecutarQuery(query.ToString()).error)
                return lista;

            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                Estado e = new Estado();
                e.llenarDatos(r);

                lista.Add(e);
            }

            return lista;
        }

        private void llenarDatos(DataRow datos)
        {
            clave = datos["clave"].ToString();
            nombre = datos["nombre"].ToString();
            sitio = datos["sitio"].ToString();

            claveDelegado = (int) (datos["delegado"] == DBNull.Value ? 0 : datos["delegado"]);
            delegado = Persona.obtenerPersonaConClave(claveDelegado);

            if (delegado != null)
            {
                nombreDelegado = delegado.nombre;
                usuarioDelegado = delegado.usuario;
                mailDelegado = delegado.correo;
            }
        }
    }
}