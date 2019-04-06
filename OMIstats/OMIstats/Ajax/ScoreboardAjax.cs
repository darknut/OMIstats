using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OMIstats.Ajax
{
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