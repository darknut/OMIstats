﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using OMIstats.Utilities;

namespace OMIstats.Models
{
    public class Log
    {
        private const int MAX_LOG_LEN = 200;
        private const int DEFAULT_LOG_COUNT = 50;

        public int clave { get; set; }

        public DateTime timestamp { get; set; }

        public string log { get; set; }

        public TipoLog tipo { get; set; }

        public static bool ToConsole = false;

        public Log() { }

        private Log(TipoLog tipo, string log)
        {
            this.tipo = tipo;
            this.log = log;
            this.timestamp = DateTime.Now;
        }

        public enum TipoLog
        {
            NULL,
            OMEGAUP,
            USUARIO,
            ADMIN,
            SCOREBOARD,
            FACEBOOK,
            REGISTRO,
            EXCEPTIONS,
            DATABASE,
            RETO
        }

        private void llenarDatos(DataRow r)
        {
            clave = DataRowParser.ToInt(r["clave"]);
            log = DataRowParser.ToString(r["log"]);
            tipo = DataRowParser.ToTipoLog(r["tipo"]);
            timestamp = DataRowParser.ToLongDateTime(r["timestamp"]);
        }

        public static void add(TipoLog tipo, string log)
        {
            try
            {
                if (ToConsole)
                {
                    Console.WriteLine(log);
                    return;
                }

                if (String.IsNullOrEmpty(log))
                    return;

                for (int i = 0; i < log.Length; i += MAX_LOG_LEN)
                {
                    string str;

                    if ((i + MAX_LOG_LEN) >= log.Length)
                        str = log.Substring(i);
                    else
                        str = log.Substring(i, MAX_LOG_LEN);

                    new Log(tipo, str).guardar();
                }
            }
            catch (Exception)
            {
                // No queremos que al guardar los logs se genere una excepción
            }
        }

        public static List<Log> get(int count = DEFAULT_LOG_COUNT, TipoLog tipo = TipoLog.NULL)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            if (count == 0)
                count = DEFAULT_LOG_COUNT;

            query.Append(" select top ");
            query.Append(count);
            query.Append(" * from Log ");

            if (tipo != TipoLog.NULL)
            {
                query.Append(" where tipo = ");
                query.Append(Cadenas.comillas(tipo.ToString().ToLower()));
            }

            query.Append(" order by clave desc ");

            db.EjecutarQuery(query.ToString());

            DataTable table = db.getTable();
            List<Log> lista = new List<Log>();

            foreach (DataRow r in table.Rows)
            {
                Log l = new Log();
                l.llenarDatos(r);

                lista.Insert(0, l);
            }

            return lista;
        }

        private void guardar()
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" insert into Log values(");
            query.Append(Cadenas.comillas(timestamp.ToString()));
            query.Append(", ");
            query.Append(Cadenas.comillas(tipo.ToString().ToLower()));
            query.Append(", ");
            query.Append(Cadenas.comillas(log));
            query.Append(")");

            db.EjecutarQuery(query.ToString());
        }

        public static void clear()
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" delete log ");

            db.EjecutarQuery(query.ToString());
        }
    }
}