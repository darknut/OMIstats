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
        private static int MAX_INTENTOS = 3;

        /// <summary>
        /// Regresa el diccionario si tuvo éxito o null si hubo algún error
        /// </summary>
        public static Dictionary<string, object> Call(OmegaUp pull, int intentos = 0)
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
                Log.add(Log.TipoLog.OMEGAUP, "Consultando el scoreboard en OmegaUp para " + pull.tipoOlimpiada.ToString() + " " + pull.olimpiada);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                string content = string.Empty;
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        content = sr.ReadToEnd();
                    }
                }

                Log.add(Log.TipoLog.OMEGAUP, "Se obtuvieron los resultados correctamente, se procede a guardarlos en la base de datos...");
                return resultados = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(content);
            }
            catch (Exception)
            {
                Log.add(Log.TipoLog.OMEGAUP, "Falló la llamada a: " + api);
                if (intentos >= MAX_INTENTOS)
                {
                    Log.add(Log.TipoLog.OMEGAUP, "Giving up...");
                    return null;
                }
                else
                {
                    Log.add(Log.TipoLog.OMEGAUP, "Intentando otra vez... ");
                    return Call(pull, intentos + 1);
                }
            }
        }
    }
}
