using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace OMIstats.Models
{
    public class CachedResult
    {
        public int lugar;
        public string clave;
        public List<float?> puntos;
        public float? totalDia;
        public float? total;
        public string medalla;

        public void llenarDatos(Resultados r, int dia, int problemas)
        {
            puntos = new List<float?>();

            lugar = r.lugar;
            clave = r.clave;
            if (dia == 1)
            {
                for (int i = 0; i < problemas; i++)
                    puntos.Add(r.dia1[i]);
                totalDia = r.totalDia1;
            }
            else
            {
                for (int i = 0; i < problemas; i++)
                    puntos.Add(r.dia2[i]);
                totalDia = r.totalDia2;
            }
            total = r.total;
            medalla = r.medalla.ToString();
        }
    }

    public class ScoreboardAjax
    {
        public enum Status
        {
            UPDATED,
            NOT_CHANGED,
            FINISHED,
            ERROR
        }

        public List<CachedResult> resultados;
        public int secondsSinceUpdate;
        public int timeToFinish;
        public string status;
        public string ticks;
        public bool retry;

        public ScoreboardAjax()
        {
            this.resultados = null;
            this.secondsSinceUpdate = 0;
            this.timeToFinish = 0;
            this.ticks = "0";
            this.retry = false;
        }
    }
}