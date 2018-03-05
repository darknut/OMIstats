using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using OMIstats.Models;

namespace OmegaUpPuller.WebRequest
{
    class Request
    {
        private static string OMEGAUP_API = "https://omegaup.com/api/contest/scoreboard?contest_alias={0}&token={1}";
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
            scoreboard = new Dictionary<string, Scoreboard>();
        }

        private Dictionary<string, Scoreboard> scoreboard;

        /// <summary>
        /// Regresa si funcionó o no la actualización
        /// </summary>
        public bool Call(OmegaUp pull, int intentos = 0)
        {
            string api = String.Format(OMEGAUP_API, pull.concurso, pull.token);

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

                var JSONObj = new JavaScriptSerializer().Deserialize<object>(content);
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

            return true;
        }
    }
}
