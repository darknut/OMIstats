using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OMIstats.Ajax
{
    public class OverlayPuntos
    {
        public List<string> timestamp;
        public List<float?> puntosD1;
        public List<float?> puntosD2;
        public List<float?> puntosD3;
        public List<float?> puntosD4;
        public List<float?> puntosD5;
        public List<float?> puntosD6;
        public List<float?> puntos;

        internal List<float?>[] problemas;

        public OverlayPuntos()
        {
            timestamp = new List<string>();
            puntosD1 = new List<float?>();
            puntosD2 = new List<float?>();
            puntosD3 = new List<float?>();
            puntosD4 = new List<float?>();
            puntosD5 = new List<float?>();
            puntosD6 = new List<float?>();
            puntos = new List<float?>();

            problemas = new List<float?>[6];
            problemas[0] = puntosD1;
            problemas[1] = puntosD2;
            problemas[2] = puntosD3;
            problemas[3] = puntosD4;
            problemas[4] = puntosD5;
            problemas[5] = puntosD6;
        }
    }
}