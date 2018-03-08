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

        private string getClaveScoreBoard(OmegaUp pull)
        {
            return pull.tipoOlimpiada.ToString() + "_" + pull.olimpiada;
        }

        /// <summary>
        /// Regresa true si se tuvo éxito o falso si no
        /// </summary>
        public bool Update(OmegaUp pull)
        {
            Dictionary<string, object> resultados = Request.Call(pull);

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
                Console.WriteLine("Falló algo cuando se parseaba el json de OmegaUp: " + e.StackTrace);
                return false;
            }

            // Finalmente, ordenamos y guardamos en la base de datos
            try
            {
                scoreboard.ordena();
            }
            catch (Exception e)
            {
                Console.WriteLine("Falló algo cuando se guardaba en la base de datos: " + e.StackTrace);
                return false;
            }

            return true;
        }
    }
}
