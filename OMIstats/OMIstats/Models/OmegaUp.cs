using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using OMIstats.Utilities;

namespace OMIstats.Models
{
    /// <summary>
    /// Clase para conectarse con OmegaUp
    /// </summary>
    public class OmegaUp
    {
        private static string SCOREBOARD_DIRECTORY_STRING = "scoreboardDirectory";
        private static string SCOREBOARD_EXE_STRING = "scoreboardExe";
        private static int SLEEP_TIME = 2000;
        private static int STANDARD_SECONDS = 18000;

        public enum Instruccion
        {
            NULL,
            POLL,
            KILL,
            STATUS,
            HIDE
        }

        public enum Status
        {
            NULL,
            OK,
            ERROR,
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

        public Status status { get; set; }

        public DateTime timestamp { get; set; }

        public int secondsToFinish { get; set; }

        public static bool RunnerStarted = false;

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
            status = Status.NULL;
            timestamp = DateTime.UtcNow;
            secondsToFinish = 0;
        }

        public static void StartScoreboard()
        {
            // Primero matamos todos los status que estén en un estado de error
            List<OmegaUp> status = OmegaUp.obtenerInstrucciones(Instruccion.STATUS);

            foreach (OmegaUp s in status)
            {
                if (s.status != Status.OK)
                    s.borrar();
                else
                    // Si encontramos un OK, no iniciamos nada pues ya hay un proceso corriendo
                    return;
            }

            // Obtenemos la dirección del exe
            string directorio = ConfigurationManager.AppSettings.Get(SCOREBOARD_DIRECTORY_STRING);
            string exe = ConfigurationManager.AppSettings.Get(SCOREBOARD_EXE_STRING);

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.WorkingDirectory = directorio;
            psi.FileName = exe;
            Process.Start(psi);

            // Esperamos un par de segundos a que el proceso empiece y darle tiempo a la página
            // a que lo registre
            System.Threading.Thread.Sleep(SLEEP_TIME);
        }

        private void llenarDatos(DataRow r)
        {
            clave = (int)r["clave"];
            instruccion = EnumParser.ToInstruccion(r["tipo"].ToString().ToUpper());
            olimpiada = r["olimpiada"].ToString().Trim();
            tipoOlimpiada = EnumParser.ToTipoOlimpiada(r["clase"].ToString().ToUpper());
            dia = (short)r["dia"];
            ping = (int)r["ping"];
            concurso = r["concurso"].ToString().Trim();
            token = r["token"].ToString().Trim();
            prefijo = r["prefijo"].ToString().Trim();
            status = EnumParser.ToStatus(r["status"].ToString().ToUpper());
            secondsToFinish = (int)r["secondsToFinish"];
            try
            {
                timestamp = new DateTime(long.Parse(r["timestamp"].ToString()));
            }
            catch (Exception)
            {
            }
        }

        public static List<OmegaUp> obtenerInstrucciones(Instruccion i = Instruccion.NULL)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();
            List<OmegaUp> lista = new List<OmegaUp>();

            query.Append(" select * from omegaup ");
            if (i != Instruccion.NULL)
            {
                query.Append(" where tipo = ");
                query.Append(Cadenas.comillas(i.ToString().ToLower()));
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
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            if (status == Status.OK)
                this.timestamp = DateTime.UtcNow;

            query.Append(" update omegaup set tipo = ");
            query.Append(Cadenas.comillas(instruccion.ToString().ToLower()));
            query.Append(", olimpiada = ");
            query.Append(Cadenas.comillas(olimpiada));
            query.Append(", clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(", ping = ");
            query.Append(ping);
            query.Append(", concurso = ");
            query.Append(Cadenas.comillas(concurso));
            query.Append(", token = ");
            query.Append(Cadenas.comillas(token));
            query.Append(", prefijo = ");
            query.Append(Cadenas.comillas(prefijo));
            query.Append(", dia = ");
            query.Append(dia);
            query.Append(", status = ");
            query.Append(Cadenas.comillas(status.ToString().ToLower()));
            query.Append(", timestamp = ");
            query.Append(Cadenas.comillas(timestamp.Ticks.ToString()));
            query.Append(", secondsToFinish = ");
            query.Append(secondsToFinish);
            query.Append(" where clave = ");
            query.Append(clave);

            db.EjecutarQuery(query.ToString());
        }

        public void guardarNuevo()
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            if (status == Status.OK)
                this.timestamp = DateTime.UtcNow;
            if (instruccion == Instruccion.POLL)
                secondsToFinish = STANDARD_SECONDS;

            query.Append(" insert into OmegaUp (tipo, olimpiada, clase, ping, concurso, ");
            query.Append(" token, prefijo, dia, status, timestamp, secondsToFinish) values (");
            query.Append(Cadenas.comillas(instruccion.ToString().ToLower()));
            query.Append(",");
            query.Append(Cadenas.comillas(olimpiada));
            query.Append(",");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(",");
            query.Append(ping);
            query.Append(",");
            query.Append(Cadenas.comillas(concurso));
            query.Append(",");
            query.Append(Cadenas.comillas(token));
            query.Append(",");
            query.Append(Cadenas.comillas(prefijo));
            query.Append(",");
            query.Append(dia);
            query.Append(",");
            query.Append(Cadenas.comillas(status.ToString().ToLower()));
            query.Append(",");
            query.Append(Cadenas.comillas(timestamp.Ticks.ToString()));
            query.Append(",");
            query.Append(secondsToFinish);
            query.Append(")");

            db.EjecutarQuery(query.ToString());
        }

        public void borrar()
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append("delete OmegaUp where clave = ");
            query.Append(clave);

            db.EjecutarQuery(query.ToString());
        }

        public static void borrarTodo()
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append("delete OmegaUp");

            db.EjecutarQuery(query.ToString());
        }

        public static void borrarConClave(int clave)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append("delete OmegaUp where clave = ");
            query.Append(clave);

            db.EjecutarQuery(query.ToString());
        }

        public static OmegaUp obtenerConClave(int clave)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append("select * from OmegaUp where clave = ");
            query.Append(clave);

            db.EjecutarQuery(query.ToString());

            DataTable table = db.getTable();

            if (table.Rows.Count == 0)
                return null;

            OmegaUp o = new OmegaUp();
            o.llenarDatos(table.Rows[0]);

            return o;
        }

        public static OmegaUp obtenerParaOMI(string olimpiada, TipoOlimpiada tipoOlimpiada)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from OmegaUp where tipo = ");
            query.Append(Cadenas.comillas(Instruccion.POLL.ToString().ToLower()));
            query.Append(" and olimpiada = ");
            query.Append(Cadenas.comillas(olimpiada));
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));

            db.EjecutarQuery(query.ToString());

            DataTable table = db.getTable();

            if (table.Rows.Count == 0)
                return null;

            OmegaUp o = new OmegaUp();
            o.llenarDatos(table.Rows[0]);

            return o;
        }

        public static void startTimestampsForPolls()
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" update OmegaUp set timestamp = ");
            query.Append(Cadenas.comillas(DateTime.UtcNow.Ticks.ToString()));
            query.Append(" where tipo = ");
            query.Append(Cadenas.comillas(Instruccion.POLL.ToString()));

            db.EjecutarQuery(query.ToString());
        }

        public void setSecondsToFinish(long seconds)
        {
            DateTime finishTime = Fechas.fromUnixTime(seconds);
            DateTime now = DateTime.UtcNow;

            if (now >= finishTime)
            {
                this.secondsToFinish = 0;
            }
            else
            {
                TimeSpan diff = finishTime - now;
                this.secondsToFinish = (int) Math.Round(diff.TotalSeconds);
            }
        }

        public int getRemainingContestTime()
        {
            int delta = (int)Math.Round((DateTime.UtcNow - this.timestamp).TotalSeconds);
            int faltante = this.secondsToFinish - delta;

            if (faltante <= 0)
            {
                faltante = 0;
                this.secondsToFinish = 0;
            }

            return faltante;
        }
    }
}