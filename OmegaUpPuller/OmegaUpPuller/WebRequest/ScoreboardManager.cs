using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMIstats.Models;
using System.Collections;

namespace OmegaUpPuller.WebRequest
{
    public class ScoreboardManager
    {
        private static string PROBLEMAS_STRING = "problems";
        private static string RANKING_STRING = "ranking";
        private static string USERNAME_STRING = "username";
        private static string POINTS_STRING = "points";
        private static string RUNS_STRING = "runs";
        private static string FINISH_TIME = "finish_time";

        private static ScoreboardManager _instance = null;

        public static ScoreboardManager Instance
        {
            get
            {
                if (ScoreboardManager._instance == null)
                    ScoreboardManager._instance = new ScoreboardManager();
                return ScoreboardManager._instance;
            }
        }

        private ScoreboardManager()
        {
            scoreboards = new Dictionary<string, Scoreboard>();
        }

        private Dictionary<string, Scoreboard> scoreboards;
        private Dictionary<string, Dictionary<string, object>> mockScoreboards;

        private string getClaveScoreBoard(OmegaUp pull)
        {
            return pull.tipoOlimpiada.ToString() + "_" + pull.olimpiada;
        }

        /// <summary>
        /// Función para ayudar a testear OmegaUp
        /// </summary>
        private void cambiaValoresMock(OmegaUp pull, Dictionary<string, object> mockScoreboard, bool inicializa = false)
        {
            ArrayList problemas = (ArrayList)mockScoreboard[PROBLEMAS_STRING];
            ArrayList ranking = (ArrayList)mockScoreboard[RANKING_STRING];
            Random r = new Random();

            foreach (Dictionary<string, object> persona in ranking)
            {
                string usuario = (string)persona[USERNAME_STRING];

                if (usuario.IndexOf(pull.prefijo) != 0)
                    continue;

                usuario = usuario.Substring(pull.prefijo.Length);

                ArrayList resultadosUsuario = (ArrayList)persona[PROBLEMAS_STRING];

                foreach (Dictionary<string, object> problema in resultadosUsuario)
                {
                    if (inicializa)
                    {
                        problema[RUNS_STRING] = 0;
                        problema[POINTS_STRING] = 0;
                    }
                    else
                    {
                        if (r.Next(10) < 2)
                        {
                            int newPoints = r.Next(101);
                            if ((int)problema[POINTS_STRING] < newPoints)
                                problema[POINTS_STRING] = newPoints;
                            else
                            {
                                if (r.Next(10) < 4)
                                    problema[POINTS_STRING] = newPoints;
                            }
                            problema[RUNS_STRING] = (int)problema[RUNS_STRING] + 1;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Regresa true si se tuvo éxito o falso si no
        /// </summary>
        public bool Update(OmegaUp pull, bool mock = false)
        {
            Dictionary<string, object> resultados = null;

            if (mock)
            {
                if (mockScoreboards == null)
                    mockScoreboards = new Dictionary<string, Dictionary<string, object>>();
                Dictionary<string, object> mockScoreboard = null;
                mockScoreboards.TryGetValue(pull.tipoOlimpiada.ToString(), out mockScoreboard);

                if (mockScoreboard == null)
                {
                    Console.WriteLine("Estamos haciendo mock... presiona ENTER para comenzar");
                    Console.ReadLine();

                    mockScoreboard = Request.Call(pull);
                    cambiaValoresMock(pull, mockScoreboard, true);
                    mockScoreboards.Add(pull.tipoOlimpiada.ToString(), mockScoreboard);
                }
                else
                {
                    cambiaValoresMock(pull, mockScoreboard);
                }

                resultados = mockScoreboard;
            }
            else
            {
                resultados = Request.Call(pull);
            }

            // Si recibimos null, hubo un error llamando a OmegaUp
            if (resultados == null)
                return false;

            // Luego parseamos los datos del json
            Scoreboard scoreboard;
            try
            {
                // Sacamos el número de problemas del response de OmegaUp
                ArrayList problemas = (ArrayList)resultados[PROBLEMAS_STRING];

                if (!scoreboards.TryGetValue(getClaveScoreBoard(pull), out scoreboard))
                {
                    scoreboard = new Scoreboard(pull.olimpiada, pull.tipoOlimpiada, pull.dia, problemas.Count);
                    scoreboards.Add(getClaveScoreBoard(pull), scoreboard);
                }

                ArrayList ranking = (ArrayList)resultados[RANKING_STRING];

                Log.add(Log.TipoLog.OMEGAUP, "Guardando los resultados en la base");
                foreach (Dictionary<string, object> persona in ranking)
                {
                    string usuario = (string)persona[USERNAME_STRING];

                    if (usuario.IndexOf(pull.prefijo) != 0)
                        continue;

                    usuario = usuario.Substring(pull.prefijo.Length);

                    decimal?[] puntos = new decimal?[problemas.Count];
                    ArrayList resultadosUsuario = (ArrayList)persona[PROBLEMAS_STRING];
                    int i = 0;

                    foreach (Dictionary<string, object> problema in resultadosUsuario)
                    {
                        if (problema[POINTS_STRING] is int)
                            puntos[i] = (int)problema[POINTS_STRING];
                        else
                            puntos[i] = (decimal)problema[POINTS_STRING];

                        if (puntos[i] == 0)
                        {
                            int tries = (int)problema[RUNS_STRING];
                            if (tries == 0)
                                puntos[i] = null;
                        }

                        i++;
                    }

                    scoreboard.actualiza(usuario, puntos);
                }
            }
            catch (Exception e)
            {
                Log.add(Log.TipoLog.OMEGAUP, "Falló algo cuando se parseaba el json de OmegaUp: ");
                Log.add(Log.TipoLog.OMEGAUP, e.ToString());
                return false;
            }

            // Finalmente, ordenamos y guardamos en la base de datos
            try
            {
                Log.add(Log.TipoLog.OMEGAUP, "Ordenando los resultados");
                scoreboard.ordena();
            }
            catch (Exception e)
            {
                Log.add(Log.TipoLog.OMEGAUP, "Falló algo cuando se guardaba en la base de datos: ");
                Log.add(Log.TipoLog.OMEGAUP, e.ToString());
                return false;
            }

            if (mock)
            {
                Console.WriteLine("Mete los segundos que queden de examen (5 horas = 18000; 0 para terminar)");
                try
                {
                    int seconds = Convert.ToInt32(Console.ReadLine());
                    if (seconds < 0)
                        seconds = 0;
                    pull.setSecondsToFinish(OMIstats.Utilities.Fechas.secondsSinceUnixTime() + seconds);
                }
                catch (Exception)
                {
                }
            }
            else
            {
                pull.setSecondsToFinish((int)resultados[FINISH_TIME]);
            }

            Log.add(Log.TipoLog.OMEGAUP, "Scoreboard actualizado con éxito");
            return true;
        }
    }
}
