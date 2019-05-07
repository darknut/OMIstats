using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OMIstats.Ajax
{
    public class OverlayPuntos
    {
        public List<int> timestamp;
        public List<float?> puntosP1;
        public List<float?> puntosP2;
        public List<float?> puntosP3;
        public List<float?> puntosP4;
        public List<float?> puntosP5;
        public List<float?> puntosP6;
        public List<float?> puntos;

        internal List<float?>[] problemas;

        public OverlayPuntos()
        {
            timestamp = new List<int>();
            puntosP1 = new List<float?>();
            puntosP2 = new List<float?>();
            puntosP3 = new List<float?>();
            puntosP4 = new List<float?>();
            puntosP5 = new List<float?>();
            puntosP6 = new List<float?>();
            puntos = new List<float?>();

            problemas = new List<float?>[6];
            problemas[0] = puntosP1;
            problemas[1] = puntosP2;
            problemas[2] = puntosP3;
            problemas[3] = puntosP4;
            problemas[4] = puntosP5;
            problemas[5] = puntosP6;
        }
    }
}