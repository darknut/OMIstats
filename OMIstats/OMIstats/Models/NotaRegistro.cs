using OMIstats.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace OMIstats.Models
{
    public class NotaRegistro
    {
        public string olimpiada;
        public TipoOlimpiada tipoOlimpiada;
        public string estado;
        public int claveUsuario;
        public string nota;

        private NotaRegistro(string olimpiada, TipoOlimpiada tipo, string estado, int clave)
        {
            this.olimpiada = olimpiada;
            this.tipoOlimpiada = tipo;
            this.estado = estado;
            this.claveUsuario = clave;
            nota = null;
        }

        private void llenarDatos(DataRow row)
        {
            nota = DataRowParser.ToString(row["nota"]);
        }

        public static NotaRegistro obtenerNotaPara(string olimpiada, TipoOlimpiada tipo, string estado, int persona)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select nota from NotaRegistro ");
            query.Append(" where olimpiada = ");
            query.Append(Cadenas.comillas(olimpiada));
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipo.ToString().ToLower()));
            query.Append(" and estado = ");
            query.Append(Cadenas.comillas(estado));
            query.Append(" and persona = ");
            query.Append(persona);

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            NotaRegistro nr = new NotaRegistro(olimpiada, tipo, estado, persona);

            if (table.Rows.Count == 0)
                return nr;

            nr.llenarDatos(table.Rows[0]);

            return nr;
        }

        public void guardar()
        {
            if (nota == null || nota.Trim().Length == 0)
            {
                borrar();
                return;
            }

            if (nota.Length > 200)
                nota = nota.Substring(0, 200);

            NotaRegistro current = NotaRegistro.obtenerNotaPara(olimpiada, tipoOlimpiada, estado, claveUsuario);
            if (current.nota == null)
            {
                nuevo();
            }
            else
            {
                update();
            }
        }

        private void nuevo()
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" insert into NotaRegistro values(");
            query.Append(Cadenas.comillas(olimpiada));
            query.Append(" ,");
            query.Append(Cadenas.comillas(estado));
            query.Append(" ,");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" ,");
            query.Append(claveUsuario);
            query.Append(" ,");
            query.Append(Cadenas.comillas(nota));
            query.Append(")");

            db.EjecutarQuery(query.ToString());
        }

        private void update()
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" update NotaRegistro set nota = ");
            query.Append(Cadenas.comillas(nota));
            query.Append(" where olimpiada = ");
            query.Append(Cadenas.comillas(olimpiada));
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and estado = ");
            query.Append(Cadenas.comillas(estado));
            query.Append(" and persona = ");
            query.Append(claveUsuario);

            db.EjecutarQuery(query.ToString());
        }

        private void borrar()
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" delete NotaRegistro ");
            query.Append(" where olimpiada = ");
            query.Append(Cadenas.comillas(olimpiada));
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and estado = ");
            query.Append(Cadenas.comillas(estado));
            query.Append(" and persona = ");
            query.Append(claveUsuario);

            db.EjecutarQuery(query.ToString(), esperaError: true);
        }
    }
}