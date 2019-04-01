using System;
using System.Collections.Generic;
#if OMISTATS
using System.ComponentModel.DataAnnotations;
#endif
using System.Linq;
using System.Text;
using System.Web;

namespace OMIstats.Models
{
    public partial class Olimpiada
    {
#if OMISTATS
        [Required(ErrorMessage = "Campo requerido")]
        [MaxLength(3, ErrorMessage = "El tamaño máximo es 3 caracteres")]
#endif
        public string numero { get; set; }

        public TipoOlimpiada tipoOlimpiada { get; set; }

        public int estados { get; set; }
        public int participantes { get; set; }
        public int problemasDia1 { get; set; }
        public int problemasDia2 { get; set; }

        public bool noMedallistasConocidos { get; set; }
        public bool puntosDesconocidos { get; set; }

        /// <summary>
        /// Calcula los campos calculados de olimpiadas y problemas
        /// </summary>
        public void calcularNumeros()
        {
            Problema prob;

            estados = Resultados.obtenerEstadosParticipantes(numero, tipoOlimpiada);
            participantes = MiembroDelegacion.obtenerParticipantes(numero, tipoOlimpiada);

            problemasDia1 = Problema.obtenerCantidadDeProblemas(numero, tipoOlimpiada, 1);
            problemasDia2 = Problema.obtenerCantidadDeProblemas(numero, tipoOlimpiada, 2);

            // Calculamos las estadisticas por dia y por competencia y las guardamos en la base
            prob = Resultados.calcularNumeros(numero, tipoOlimpiada, dia: 1, totalProblemas: problemasDia1);
            prob.guardar(guardarTodo: false);

            prob = Resultados.calcularNumeros(numero, tipoOlimpiada, dia: 2, totalProblemas: problemasDia2);
            prob.guardar(guardarTodo: false);

            prob = Models.Resultados.calcularNumeros(numero, tipoOlimpiada, totalProblemas: problemasDia1 + problemasDia2);
            prob.guardar(guardarTodo: false);

            List<Problema> lista = Problema.obtenerProblemasDeOMI(numero, tipoOlimpiada, 1);
            foreach (Problema p in lista)
                if (p != null)
                {
                    Problema pp = Resultados.calcularNumeros(numero, tipoOlimpiada, p.dia, p.numero);
                    p.media = pp.media;
                    p.mediana = pp.mediana;
                    p.perfectos = pp.perfectos;
                    p.ceros = pp.ceros;
                    p.guardar(guardarTodo: false);
                }

            lista = Problema.obtenerProblemasDeOMI(numero, tipoOlimpiada, 2);
            foreach (Problema p in lista)
                if (p != null)
                {
                    Problema pp = Resultados.calcularNumeros(numero, tipoOlimpiada, p.dia, p.numero);
                    p.media = pp.media;
                    p.mediana = pp.mediana;
                    p.perfectos = pp.perfectos;
                    p.ceros = pp.ceros;
                    p.guardar(guardarTodo: false);
                }
        }

        public static void guardaProblemas(string olimpiada, TipoOlimpiada tipoOlimpiada, int problemas, int dia)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" update olimpiada set datospublicos = 1 ");
            if (dia == 1)
                query.Append(", problemasDia1 = ");
            else
                query.Append(", problemasDia2 = ");
            query.Append(problemas);
            query.Append(", mostrarResultadosPorDia = ");
            query.Append(dia == 1 ? "0" : "1");
            query.Append(", mostrarResultadosPorProblema = 1 ");
            query.Append(", mostrarResultadosTotales = 1 ");
            query.Append(" where numero = ");
            query.Append(Utilities.Cadenas.comillas(olimpiada));
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));

            db.EjecutarQuery(query.ToString());
        }
    }
}