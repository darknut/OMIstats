﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;

namespace OMIstats.Models
{
    public class Medallero
    {
        public enum TipoMedallero
        {
            NULL,
            PERSONA,
            ESTADO,
            INSTITUCION,
            ASESOR
        }

        public Olimpiada.TipoOlimpiada tipoOlimpiada { get; set; }

        public TipoMedallero tipoMedallero { get; set; }

        public string clave { get; set; }

        public int oros { get; set; }

        public int platas { get; set; }

        public int bronces { get; set; }

        public int otros { get; set; }

        public Medallero()
        {
            tipoOlimpiada = Olimpiada.TipoOlimpiada.NULL;
            tipoMedallero = TipoMedallero.NULL;
            clave = "";
            oros = 0;
            platas = 0;
            bronces = 0;
            otros = 0;
        }

        private void llenarDatos(DataRow datos)
        {
            tipoOlimpiada = (Olimpiada.TipoOlimpiada)Enum.Parse(typeof(Olimpiada.TipoOlimpiada), datos["clase"].ToString().ToUpper());
            tipoMedallero = (TipoMedallero)Enum.Parse(typeof(TipoMedallero), datos["tipo"].ToString().ToUpper());
            clave = datos["clave"].ToString().Trim();
            oros = (int)datos["oro"];
            platas = (int)datos["plata"];
            bronces = (int)datos["bronce"];
            otros = (int)datos["otros"];
        }

        /// <summary>
        /// Obtiene el medallero de la base de datos
        /// </summary>
        /// <param name="tipoOlimpiada">El tipo de la olimpiada de la que se requiere el medallero</param>
        /// <param name="tipoMedallero">Si es estado, persona, institucion o asesor</param>
        /// <param name="clave">La clave del estado/persona/institucion/asesor</param>
        /// <returns>Un objeto medallero con los datos deseados</returns>
        public static Medallero obtenerMedallas(Olimpiada.TipoOlimpiada tipoOlimpiada, TipoMedallero tipoMedallero, string clave)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from medallero where clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and tipo = ");
            query.Append((int)tipoMedallero);
            query.Append(" and clave = ");
            query.Append(Utilities.Cadenas.comillas(clave));

            if (db.EjecutarQuery(query.ToString()).error)
                return null;

            DataTable table = db.getTable();

            Medallero m = new Medallero();
            m.tipoMedallero = tipoMedallero;
            m.tipoOlimpiada = tipoOlimpiada;
            m.clave = clave;

            if (table.Rows.Count > 0)
                m.llenarDatos(table.Rows[0]);

            return m;
        }

        /// <summary>
        /// Guarda los datos en el objeto en la base de datos
        /// </summary>
        /// <returns>Regresa si se guardo o no</returns>
        public bool guardarDatos()
        {
            if (tipoMedallero == TipoMedallero.NULL || tipoOlimpiada == Olimpiada.TipoOlimpiada.NULL || clave == "")
                return false;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" insert into medallero values( ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(", ");
            query.Append((int)tipoMedallero);
            query.Append(", ");
            query.Append(Utilities.Cadenas.comillas(clave));
            query.Append(", ");
            query.Append(oros);
            query.Append(", ");
            query.Append(platas);
            query.Append(", ");
            query.Append(bronces);
            query.Append(", ");
            query.Append(otros);
            query.Append(")");

            return !db.EjecutarQuery(query.ToString()).error;
        }

        /// <summary>
        /// Usa las variables en el objeto para calcular las medallas basadas en lo que hay en la base de datos
        /// </summary>
        /// </param name="tipoOlimpiada">El tipo de olimpiada para el que se requieren los tipos</param>
        public static void calcularMedallas(Olimpiada.TipoOlimpiada tipoOlimpiada)
        {
            if (tipoOlimpiada == Olimpiada.TipoOlimpiada.NULL)
                return;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" delete medallero where clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));

            //Primero borramos todo lo que está en la base de datos
            db.EjecutarQuery(query.ToString());

            //Obtenermos todos los resultados
            List<Resultados> resultados = Resultados.cargarResultados(null, tipoOlimpiada, cargarObjetos: true, incluirDesconocidos: false);

            //Diccionarios para los diferentes tipos de medalleros
            Dictionary<int, Medallero> personas = new Dictionary<int,Medallero>();
            Dictionary<int, Medallero> instituciones = new Dictionary<int,Medallero>();
            Dictionary<string, Medallero> estados = new Dictionary<string,Medallero>();

            // Recorremos todos los resultados agregando contadores
            foreach(Resultados resultado in resultados)
            {
                Medallero persona, institucion, estado;

                if (!personas.TryGetValue(resultado.usuario, out persona))
                {
                    persona = new Medallero();
                    persona.clave = resultado.usuario.ToString();
                    persona.tipoOlimpiada = tipoOlimpiada;
                    persona.tipoMedallero = TipoMedallero.PERSONA;
                    personas.Add(resultado.usuario, persona);
                }

                if (resultado.escuela != null)
                {
                    if (!instituciones.TryGetValue(resultado.escuela.clave, out institucion))
                    {
                        institucion = new Medallero();
                        institucion.clave = resultado.escuela.clave.ToString();
                        institucion.tipoOlimpiada = tipoOlimpiada;
                        institucion.tipoMedallero = TipoMedallero.INSTITUCION;
                        instituciones.Add(resultado.escuela.clave, institucion);
                    }
                }
                else
                {
                    // Agregamos un dummy para evitar if's abajo
                    institucion = new Medallero();
                }

                if (!estados.TryGetValue(resultado.estado, out estado))
                {
                    estado = new Medallero();
                    estado.clave = resultado.estado;
                    estado.tipoOlimpiada = tipoOlimpiada;
                    estado.tipoMedallero = TipoMedallero.ESTADO;
                    estados.Add(resultado.estado, estado);
                }

                switch (resultado.medalla)
                {
                    case Resultados.TipoMedalla.ORO_3:
                    case Resultados.TipoMedalla.ORO_2:
                    case Resultados.TipoMedalla.ORO_1:
                    case Resultados.TipoMedalla.ORO:
                        persona.oros++;
                        estado.oros++;
                        institucion.oros++;
                        break;
                    case Resultados.TipoMedalla.PLATA:
                        persona.platas++;
                        estado.platas++;
                        institucion.platas++;
                        break;
                    case Resultados.TipoMedalla.BRONCE:
                        persona.bronces++;
                        estado.bronces++;
                        institucion.bronces++;
                        break;
                    default:
                        persona.otros++;
                        estado.otros++;
                        institucion.otros++;
                        break;
                }
            }

            // Guardamos los contadores en la base de datos
            foreach (Medallero persona in personas.Values)
                if (persona.clave != "0")
                    persona.guardarDatos();

            foreach (Medallero institucion in instituciones.Values)
                if (institucion.clave != "0")
                    institucion.guardarDatos();

            foreach (Medallero estado in estados.Values)
                estado.guardarDatos();
        }
    }
}