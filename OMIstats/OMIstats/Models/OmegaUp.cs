using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace OMIstats.Models
{
    /// <summary>
    /// Clase para conectarse con OmegaUp
    /// </summary>
    public class OmegaUp
    {
        public enum Instruccion
        {
            NULL,
            POLL,
            KILL,
            STATUS
        }

        public enum Status
        {
            NULL,
            ALIVE,
            ERROR
        }

        public int clave { get; set; }

        public Instruccion instruccion { get; set; }

        public string olimpiada { get; set; }

        public TipoOlimpiada tipoOlimpiada { get; set; }

        public short dia { get; set; }

        public int ping { get; set; }

        public string concurso { get; set; }

        public string token { get; set; }

        public string prefijo { get; set; }

        public Status status
        {
            get
            {
                return (Status)Enum.Parse(typeof(Status), concurso);
            }

            set
            {
                concurso = value.ToString();
            }
        }

        public DateTime timestamp
        {
            get
            {
                return DateTime.Parse(token);
            }

            set
            {
                token = value.ToString();
            }
        }

        public OmegaUp()
        {
            clave = 0;
            instruccion = Instruccion.NULL;
            olimpiada = "";
            tipoOlimpiada = TipoOlimpiada.NULL;
            dia = 0;
            ping = 0;
            concurso = "";
            token = "";
            prefijo = "";
        }

        private void llenarDatos(DataRow r)
        {
            clave = (int)r["clave"];
            instruccion = (Instruccion)Enum.Parse(typeof(Instruccion), r["tipo"].ToString().ToUpper());
            olimpiada = r["olimpiada"].ToString().Trim();
            tipoOlimpiada = (TipoOlimpiada)Enum.Parse(typeof(TipoOlimpiada), r["clase"].ToString().ToUpper());
            dia = (short)r["dia"];
            ping = (int)r["ping"];
            concurso = r["concurso"].ToString().Trim();
            token = r["token"].ToString().Trim();
            prefijo = r["prefijo"].ToString().Trim();
        }

        public static List<OmegaUp> obtenerInstrucciones(Instruccion i = Instruccion.NULL)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();
            List<OmegaUp> lista = new List<OmegaUp>();

            query.Append(" select * from omegaup ");
            if (i != Instruccion.NULL)
            {
                query.Append(" where tipo = ");
                query.Append(Utilities.Cadenas.comillas(i.ToString().ToLower()));
            }
            query.Append(" order by tipo asc ");

            db.EjecutarQuery(query.ToString());

            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                OmegaUp o = new OmegaUp();
                o.llenarDatos(r);

                lista.Add(o);
            }

            return lista;
        }

        public void guardar()
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            if (instruccion == Instruccion.STATUS)
                this.timestamp = DateTime.UtcNow;

            query.Append(" update omegaup set tipo = ");
            query.Append(Utilities.Cadenas.comillas(instruccion.ToString().ToLower()));
            query.Append(", olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(olimpiada));
            query.Append(", clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(", ping = ");
            query.Append(ping);
            query.Append(", concurso = ");
            query.Append(Utilities.Cadenas.comillas(concurso));
            query.Append(", token = ");
            query.Append(Utilities.Cadenas.comillas(token));
            query.Append(", prefijo = ");
            query.Append(Utilities.Cadenas.comillas(prefijo));
            query.Append(", dia = ");
            query.Append(dia);
            query.Append(" where clave = ");
            query.Append(clave);

            db.EjecutarQuery(query.ToString());
        }

        public void guardarNuevo()
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            if (instruccion == Instruccion.STATUS)
                this.timestamp = DateTime.UtcNow;

            query.Append(" insert into OmegaUp (tipo, olimpiada, clase, ping, concurso, token, prefijo, dia) values (");
            query.Append(Utilities.Cadenas.comillas(instruccion.ToString().ToLower()));
            query.Append(",");
            query.Append(Utilities.Cadenas.comillas(olimpiada));
            query.Append(",");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(",");
            query.Append(ping);
            query.Append(",");
            query.Append(Utilities.Cadenas.comillas(concurso));
            query.Append(",");
            query.Append(Utilities.Cadenas.comillas(token));
            query.Append(",");
            query.Append(Utilities.Cadenas.comillas(prefijo));
            query.Append(",");
            query.Append(dia);
            query.Append(")");

            db.EjecutarQuery(query.ToString());
        }
    }
}