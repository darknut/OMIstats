using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using OMIstats.Models;
using System.Collections;

namespace OmegaUpPuller.WebRequest
{
    class Request
    {
        private static string OMEGAUP_API = "https://omegaup.com/api/contest/scoreboard?contest_alias={0}&token={1}";
        private static string PROBLEMAS_STRING = "problems";
        private static string RANKING_STRING = "ranking";
        private static string USERNAME_STRING = "username";
        private static string POINTS_STRING = "points";
        private static int MAX_INTENTOS = 3;

        private static Request _instance = null;

        public static Request Instance
        {
            get {
                if (Request._instance == null)
                    Request._instance = new Request();
                return Request._instance;
            }
        }

        private Request()
        {
            scoreboards = new Dictionary<string, Scoreboard>();
        }

        private Dictionary<string, Scoreboard> scoreboards;

        private string getClaveScoreBoard(OmegaUp pull)
        {
            return pull.tipoOlimpiada.ToString() + "_" + pull.olimpiada;
        }

        /// <summary>
        /// Regresa si funcionó o no la actualización
        /// </summary>
        public bool Call(OmegaUp pull, int intentos = 0)
        {
            // Primero hacemos el request a OmegaUp
            string api = String.Format(OMEGAUP_API, pull.concurso, pull.token);
            Dictionary<string, object> resultados;

            HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(api);
            request.Method = "GET";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            try
            {
                Console.WriteLine("Consultando el scoreboard en OmegaUp para " + pull.tipoOlimpiada.ToString() + " " + pull.olimpiada);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                string content = string.Empty;
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        content = sr.ReadToEnd();
                    }
                }

                resultados = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(content);
                Console.WriteLine("Se obtuvieron los resultados correctamente.\nSe procede a guardarlos en la base de datos...");
            }
            catch (Exception)
            {
                if (intentos >= MAX_INTENTOS)
                {
                    Console.WriteLine("Falló la llamada a: " + api + "\nGiving up...");
                    return false;
                }
                else
                {
                    Console.WriteLine("Falló la llamada a: " + api + "\nIntentando otra vez... ");
                    return Call(pull, intentos + 1);
                }
            }

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

                    decimal[] puntos = new decimal[problemas.Count];
                    ArrayList resultadosUsuario = (ArrayList)persona[PROBLEMAS_STRING];
                    int i = 0;

                    foreach (Dictionary<string, object> problema in resultadosUsuario)
                    {
                        if (problema[POINTS_STRING] is int)
                            puntos[i++] = (int)problema[POINTS_STRING];
                        else
                            puntos[i++] = (decimal)problema[POINTS_STRING];
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
                scoreboard.guarda();
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
