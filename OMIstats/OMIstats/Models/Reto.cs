using System;
using System.Collections.Generic;
#if OMISTATS
using System.ComponentModel.DataAnnotations;
#endif
using System.Linq;
using System.Web;
using OMIstats.Utilities;
using System.Text;
using System.Data;

namespace OMIstats.Models
{
    public class Reto
    {
        public int clave { get; set; }
        public string olimpiada { get; set; }
#if OMISTATS
        [MaxLength(500, ErrorMessage = "El tamaño máximo es de 500 caracteres")]
#endif
        public string pregunta { get; set; }
        public int orden { get; set; }
        public bool activo { get; set; }
        public bool cerrado { get; set; }
#if OMISTATS
        [MaxLength(200, ErrorMessage = "El tamaño máximo es de 200 caracteres")]
#endif
        public string respuesta { get; set; }

        public RetoPersona retoPersona { get; set; }

        public Reto()
        {
            clave = 0;
            olimpiada = "";
            pregunta = null;
            orden = 0;
            activo = false;
            cerrado = false;
            respuesta = null;

            retoPersona = null;
        }

        public Reto(string olimpiada) : base()
        {
            this.olimpiada = olimpiada;
        }

        private void llenarDatos(DataRow r)
        {
            clave = DataRowParser.ToInt(r["clave"]);
            olimpiada = DataRowParser.ToString(r["olimpiada"]);
            pregunta = DataRowParser.ToString(r["pregunta"]);
            orden = DataRowParser.ToInt(r["orden"]);
            activo = DataRowParser.ToBool(r["activo"]);
            cerrado = DataRowParser.ToBool(r["cerrado"]);
            respuesta = DataRowParser.ToString(r["respuesta"]);
        }

        /// <summary>
        /// Regresa el reto especificado por la clave mandada
        /// </summary>
        public static Reto obtenerRetoConClave(int reto)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from Reto where clave = ");
            query.Append(reto);

            db.EjecutarQuery(query.ToString());

            DataTable table = db.getTable();

            if (table.Rows.Count == 0)
                return null;

            Reto r = new Reto();
            r.llenarDatos(table.Rows[0]);

            return r;
        }

        private void nuevo()
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" insert into Reto values (");
            query.Append(Cadenas.comillas(olimpiada));
            query.Append(" ,");
            query.Append(Cadenas.comillas(pregunta));
            query.Append(" ,");
            query.Append(orden);
            query.Append(",0,0,");
            query.Append(Cadenas.comillas(respuesta));
            query.Append(")");

            db.EjecutarQuery(query.ToString());
        }

        private void actualizar()
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" update Reto set pregunta = ");
            query.Append(Cadenas.comillas(pregunta));
            query.Append(" , respuesta = ");
            query.Append(Cadenas.comillas(respuesta));
            query.Append(" , orden = ");
            query.Append(orden);
            query.Append(" where clave = ");
            query.Append(clave);

            db.EjecutarQuery(query.ToString());
        }

        /// <summary>
        /// Guarda un reto, se encarga de agregar uno nuevo o actualizar dependiendo de la clave
        /// </summary>
        public void guardar()
        {
            if (clave == 0)
                nuevo();
            else
                actualizar();
        }

        /// <summary>
        /// Obtiene una lista de todos los retos de la olimpiada ordenados por el campo orden
        /// </summary>
        /// <param name="persona">Si el paámetro persona es proporcionado, se llena el objeto
        /// RetoPersona con los valores del envío más reciente</param>
        public static List<Reto> obtenerRetosDeOlimpiada(string omi, int persona = -1)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from reto where olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" order by orden asc");

            db.EjecutarQuery(query.ToString());

            List<Reto> retos = new List<Reto>();
            DataTable table = db.getTable();
            foreach (DataRow r in table.Rows)
            {
                Reto reto = new Reto();
                reto.llenarDatos(r);
                if (persona != -1)
                {
                    reto.retoPersona = RetoPersona.obtenerMasReciente(omi, persona, reto.clave);
                }
                retos.Add(reto);
            }

            return retos;
        }

        /// <summary>
        /// Borra el registro actual de la base de datos
        /// </summary>
        public void borrar()
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" delete Reto where clave = ");
            query.Append(clave);

            db.EjecutarQuery(query.ToString());
        }

        /// <summary>
        /// Regresa si el reto está en este momento activo
        /// </summary>
        public static bool isRetoActivo(string omi)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select top 1 activo from reto where olimpiada = ");
            query.Append(Cadenas.comillas(omi));

            db.EjecutarQuery(query.ToString());

            DataTable table = db.getTable();
            if (table.Rows.Count == 0)
                return false;

            return DataRowParser.ToBool(table.Rows[0][0]);
        }

        /// <summary>
        /// Regresa si el reto está en este momento cerrado
        /// </summary>
        public static bool isRetoCerrado(string omi)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select top 1 cerrado from reto where olimpiada = ");
            query.Append(Cadenas.comillas(omi));

            db.EjecutarQuery(query.ToString());

            DataTable table = db.getTable();
            if (table.Rows.Count == 0)
                return false;

            return DataRowParser.ToBool(table.Rows[0][0]);
        }

        /// <summary>
        /// Activa o desactiva el Reto
        /// </summary>
        public static void switchActivo(string omi)
        {
            bool value = !isRetoActivo(omi);

            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" update reto set activo = ");
            query.Append(Cadenas.boolToInt(value));
            query.Append(" where olimpiada = ");
            query.Append(Cadenas.comillas(omi));

            db.EjecutarQuery(query.ToString());
        }

        /// <summary>
        /// Termina el reto o lo vuelve a abrir
        /// </summary>
        public static void switchCerrado(string omi)
        {
            bool value = !isRetoCerrado(omi);

            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" update reto set cerrado = ");
            query.Append(Cadenas.boolToInt(value));
            query.Append(" where olimpiada = ");
            query.Append(Cadenas.comillas(omi));

            db.EjecutarQuery(query.ToString());
        }

        /// <summary>
        /// Regresa el primer RetoPersona que no se haya evaluado
        /// </summary>
        public static Reto obtenerPrimerRetoPorEvaluar(string omi)
        {
            RetoPersona rp = RetoPersona.obtenerPrimerRetoPorEvaluar(omi);
            if (rp == null)
                return null;

            Reto r = obtenerRetoConClave(rp.reto);
            r.retoPersona = rp;
            return r;
        }
    }
}