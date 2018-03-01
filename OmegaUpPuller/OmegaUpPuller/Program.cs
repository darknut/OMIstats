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
            return OmegaUp.obtenerInstrucciones(OmegaUp.Instruccion.STATUS).Count > 0;
        }

        private OmegaUp setStatus(OmegaUp instruccion = null, OmegaUp.Status status = OmegaUp.Status.ALIVE)
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

        /// <summary>
        /// Regresa verdadero si el poll fue exitoso
        /// </summary>
        private bool poll(OmegaUp instruccion)
        {
            return true;
        }

        private void Run()
        {
            this.setup();

            if (this.hayOtroActivo())
                return;

            OmegaUp status = setStatus();
            Console.WriteLine("STARTING...");

            while (true)
            {
                List<OmegaUp> instrucciones = OmegaUp.obtenerInstrucciones();
                int sleep = MAX_SLEEP;
                int polls = 0;
                string errors = "";

                foreach (var instruccion in instrucciones)
                {
                    switch (instruccion.instruccion)
                    {
                        case OmegaUp.Instruccion.KILL:
                            {
                                Console.WriteLine("EXITING...");
                                instruccion.borrar();
                                status.borrar();
                                return;
                            }
                        case OmegaUp.Instruccion.POLL:
                            {
                                Console.WriteLine("-----------------");
                                Console.WriteLine("-Polling started-");
                                Console.WriteLine("-----------------");
                                polls++;
                                bool success = this.poll(instruccion);
                                if (!success)
                                    errors += instruccion.clave + ",";
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

                status.errors = errors;
                if (errors.Length > 0)
                    status.status = OmegaUp.Status.ERROR;
                else
                    status.status = OmegaUp.Status.ALIVE;

                status.guardar();

                Console.WriteLine("Sleeping for " + sleep + " seconds.");
                System.Threading.Thread.Sleep(sleep * 1000);
            }
        }

        static void Main(string[] args)
        {
            new Program().Run();
        }
    }
}
