﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace OMIstats.Models
{
    public class Olimpiada
    {
        [Required(ErrorMessage = "Campo requerido")]
        [MaxLength(6, ErrorMessage = "El tamaño máximo es 6 caracteres")]
        public string numero { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [MaxLength(30, ErrorMessage = "El tamaño máximo es 30 caracteres")]
        public string ciudad { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [MaxLength(3, ErrorMessage = "El tamaño máximo es 3 caracteres")]
        public string estado { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public float año { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public DateTime inicio { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public DateTime fin { get; set; }

        public float media { get; set; }

        public int mediana { get; set; }

        [MaxLength(100, ErrorMessage = "El tamaño máximo es 100 caracteres")]
        public string video { get; set; }

        [MaxLength(100, ErrorMessage = "El tamaño máximo es 100 caracteres")]
        public string poster { get; set; }

        public int estados { get; set; }

        public int participantes { get; set; }

        [MaxLength(100, ErrorMessage = "El tamaño máximo es 100 caracteres")]
        public string escuela { get; set; }

        private void llenarDatos(DataRow datos)
        {
            numero = datos["numero"].ToString().Trim();
            ciudad = datos["ciudad"].ToString().Trim();
            estado = datos["estado"].ToString().Trim();
            año = float.Parse(datos["año"].ToString().Trim());
            inicio = Utilities.Fechas.stringToDate(datos["inicio"].ToString().Trim());
            fin = Utilities.Fechas.stringToDate(datos["fin"].ToString().Trim());
            media = float.Parse(datos["media"].ToString().Trim());
            mediana = (int)datos["mediana"];
            video = datos["video"].ToString().Trim();
            poster = datos["poster"].ToString().Trim();
            estados = (int)datos["estados"];
            participantes = (int)datos["participantes"];
            // -TODO- Cargar escuela?
        }

        public static List<Olimpiada> obtenerOlimpiadas()
        {
            List<Olimpiada> lista = new List<Olimpiada>();
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from olimpiada ");

            if (db.EjecutarQuery(query.ToString()).error)
                return lista;

            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                Olimpiada o = new Olimpiada();
                o.llenarDatos(r);

                lista.Add(o);
            }

            return lista;
        }
    }
}