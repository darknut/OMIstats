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

        private void Run()
        {
            this.setup();

            if (this.hayOtroActivo())
                return;

            OmegaUp status = setStatus();
        }

        static void Main(string[] args)
        {
            new Program().Run();
        }
    }
}
