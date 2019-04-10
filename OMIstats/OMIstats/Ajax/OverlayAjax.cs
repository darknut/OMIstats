using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OMIstats.Ajax
{
    public class OverlayAjax
    {
        public int timestamp;
        public List<float?> dia1;
        public float? totalDia1;
        public List<float?> dia2;
        public float? totalDia2;
        public float? total;

        public OverlayAjax()
        {
            timestamp = 0;
            dia1 = null;
            totalDia1 = null;
            dia2 = null;
            totalDia2 = null;
            total = null;
        }
    }
}