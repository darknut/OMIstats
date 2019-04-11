using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OMIstats.Ajax
{
    public class OverlayLugares
    {
        public List<int> timestamp;
        public List<int> lugar;
        public List<int> medalla;

        public OverlayLugares()
        {
            timestamp = new List<int>();
            lugar = new List<int>();
            medalla = new List<int>();
        }
    }
}