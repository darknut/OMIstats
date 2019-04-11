using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OMIstats.Ajax
{
    public class OverlayAjax
    {
        public OverlayPuntos puntosD1;
        public OverlayLugares lugaresD1;
        public OverlayPuntos puntosD2;
        public OverlayLugares lugaresD2;
        public List<int> problemas;

        public OverlayAjax()
        {
            puntosD1 = null;
            lugaresD1 = null;
            puntosD2 = null;
            lugaresD2 = null;
            problemas = null;
        }
    }
}