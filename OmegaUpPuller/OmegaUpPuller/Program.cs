using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using OMIstats.Utilities;
using OMIstats.Models;

namespace OmegaUpPuller
{
    class Program
    {
        private static int MAX_SLEEP = 60;

        private void setup()
        {
            Acceso.CADENA_CONEXION = ConfigurationManager.AppSettings["conexion"];
        }

        private bool hayOtroActivo()
        {
            List<OmegaUp> status = OmegaUp.obtenerInstrucciones(OmegaUp.Instruccion.STATUS);

            foreach (OmegaUp i in status)
            {
                if (i.status == OmegaUp.Status.OK)
                    return true;

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
            if (WebRequest.Request.Instance.Call(instruccion))
                instruccion.status = OmegaUp.Status.OK;
            else
                instruccion.status = OmegaUp.Status.ERROR;

            instruccion.guardar();
        }

        private void Run()
        {
            this.setup();

            if (this.hayOtroActivo())
                return;

            OmegaUp status = setStatus();
            Console.WriteLine("STARTING...");

            try
            {
                while (true)
                {
                    List<OmegaUp> instrucciones = OmegaUp.obtenerInstrucciones();
                    int sleep = MAX_SLEEP;
                    int polls = 0;

                    foreach (var instruccion in instrucciones)
                    {
                        switch (instruccion.instruccion)
                        {
                            case OmegaUp.Instruccion.KILL:
                                {
                                    Console.WriteLine("Instrucción KILL encontrada.\nEXITING...");
                                    instruccion.borrar();
                                    status.borrar();
                                    return;
                                }
                            case OmegaUp.Instruccion.POLL:
                                {
                                    Console.WriteLine("-------------------");
                                    Console.WriteLine("- Polling started -");
                                    Console.WriteLine("-------------------");
                                    polls++;
                                    this.poll(instruccion);
                                    sleep = sleep < instruccion.ping ? sleep : instruccion.ping;
                                    break;
                                }
                        }
                    }

                    if (polls == 0)
                    {
                        Console.WriteLine("Nothing to do, exiting...");
                        status.borrar();
                        return;
                    }

                    // Guardamos el status para actualizar el timestamp
                    status.guardar();

                    Console.WriteLine("Sleeping for " + sleep + " seconds.");
                    System.Threading.Thread.Sleep(sleep * 1000);
                }
            }
            catch (Exception)
            {
                status.status = OmegaUp.Status.ERROR;
                status.guardar();
            }
        }

        static void Main(string[] args)
        {
            new Program().Run();
        }
    }
}
