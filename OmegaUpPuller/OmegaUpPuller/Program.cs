using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using OMIstats.Utilities;
using OMIstats.Models;
using System.Globalization;
using System.Threading;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace OmegaUpPuller
{
    class Program
    {
        public static bool HIDE;

        private bool hayOtroActivo()
        {
            List<OmegaUp> status = OmegaUp.obtenerInstrucciones(OmegaUp.Instruccion.STATUS);

            foreach (OmegaUp i in status)
            {
                if (i.status == OmegaUp.Status.OK)
                {
#if DEBUG
                    // Para facilitar depuración, en caso de que haya algo corriendo lo matamos
                    i.borrar();
                    return false;
#else
                    return true;
#endif
                }

                // Si hay algo vivo que no era status, lo borramos para que no haga ruido
                i.borrar();
            }

            return false;
        }

        private OmegaUp setStatus(OmegaUp instruccion = null, OmegaUp.Status status = OmegaUp.Status.OK)
        {
            if (instruccion == null)
            {
                instruccion = new OmegaUp();
                instruccion.instruccion = OmegaUp.Instruccion.STATUS;
                instruccion.status = status;
                instruccion.guardarNuevo();

                return OmegaUp.obtenerInstrucciones(OmegaUp.Instruccion.STATUS)[0];
            }

            instruccion.status = status;
            instruccion.guardar();
            return instruccion;
        }

        private void poll(OmegaUp instruccion)
        {
#if DEBUG
            if (WebRequest.ScoreboardManager.Instance.Update(instruccion, mock: true))
#else
            if (WebRequest.ScoreboardManager.Instance.Update(instruccion))
#endif
                instruccion.status = OmegaUp.Status.OK;
            else
                instruccion.status = OmegaUp.Status.ERROR;

            instruccion.guardar();
        }

        private void Run()
        {
            if (this.hayOtroActivo())
                return;

            OmegaUp status = setStatus();
            Log.add(Log.TipoLog.OMEGAUP, "STARTING...");

            try
            {
                while (true)
                {
                    List<OmegaUp> instrucciones = OmegaUp.obtenerInstrucciones();
                    int polls = 0;
                    bool wasHidden = HIDE;
                    HIDE = false;

                    foreach (var instruccion in instrucciones)
                    {
                        switch (instruccion.instruccion)
                        {
                            case OmegaUp.Instruccion.HIDE:
                                {
                                    Log.add(Log.TipoLog.OMEGAUP, "Instrucción HIDE encontrada, no se actualizará scoreboard general");
                                    HIDE = true;
                                    break;
                                }
                            case OmegaUp.Instruccion.KILL:
                                {
                                    Log.add(Log.TipoLog.OMEGAUP, "Instrucción KILL encontrada, saliendo...");
                                    instruccion.borrar();
                                    status.borrar();
                                    return;
                                }
                            case OmegaUp.Instruccion.POLL:
                                {
                                    Log.add(Log.TipoLog.OMEGAUP, "-------------------");
                                    Log.add(Log.TipoLog.OMEGAUP, "- Polling started -");
                                    Log.add(Log.TipoLog.OMEGAUP, "-------------------");
                                    polls++;
                                    this.poll(instruccion);
#if !DEBUG
                                    Log.add(Log.TipoLog.OMEGAUP, "Sleeping for " + instruccion.ping + " seconds.");
                                    System.Threading.Thread.Sleep(instruccion.ping * 1000);
#endif
                                    break;
                                }
                        }
                    }

                    if (polls == 0)
                    {
                        Log.add(Log.TipoLog.OMEGAUP, "Nothing to do, exiting...");
                        status.borrar();
                        return;
                    }

                    if (wasHidden && !HIDE)
                        WebRequest.ScoreboardManager.Instance.Unhide();

                    // Guardamos el status para actualizar el timestamp
                    status.guardar();
                }
            }
            catch (Exception e)
            {
                Log.add(Log.TipoLog.OMEGAUP, "Excepción en el ciclo normal de ejecución del Puller:");
                Log.add(Log.TipoLog.OMEGAUP, e.ToString());
                status.status = OmegaUp.Status.ERROR;
                status.guardar();
            }
        }

        private static bool AlwaysGoodCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true;
        }

        static void Main(string[] args)
        {
#if DEBUG
            Log.ToConsole = true;
#endif
            CultureInfo culture = new CultureInfo("es-MX");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            Acceso.CADENA_CONEXION = ConfigurationManager.AppSettings["conexion"];
            OmegaUp.startTimestampsForPolls();

            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(AlwaysGoodCertificate);
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            new Program().Run();
        }
    }
}
