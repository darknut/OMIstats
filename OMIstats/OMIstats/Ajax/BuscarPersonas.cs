using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OMIstats.Models;

namespace OMIstats.Ajax
{
    public class BuscarPersonas
    {
        public string nombre;
        public string apellidoP;
        public string apellidoM;
        public int clave;
        public string genero;
        public string nacimiento;
        public string correo;
        public string celular;
        public string telefono;
        public string direccion;
        public string omegaup;
        public string telEmergencia;
        public string emergencia;
        public string parentesco;
        public string medicina;
        public string alergias;
        public bool tieneFoto;

        // Datos de la escuela
        public string nombreEscuela;
        public int claveEscuela;
        public string nivelEscuela;
        public int anioEscuela;

        public BuscarPersonas(Persona persona, MiembroDelegacion md)
        {
            persona.breakNombre();
            nombre = persona.nombre;
            apellidoP = persona.apellidoPaterno;
            apellidoM = persona.apellidoMaterno;
            genero = persona.genero;
            nacimiento = Utilities.Fechas.dateToString(persona.nacimiento);
            correo = persona.correo;
            celular = persona.celular;
            telefono = persona.telefono;
            direccion = persona.direccion;
            omegaup = persona.omegaup;
            emergencia = persona.emergencia;
            telEmergencia = persona.telEmergencia;
            parentesco = persona.parentesco;
            clave = persona.clave;
            medicina = persona.medicina;
            alergias = persona.alergias;
            tieneFoto = persona.esFotoPersonalizada();

            if (md != null)
            {
                nombreEscuela = md.nombreEscuela;
                claveEscuela = md.claveEscuela;
                nivelEscuela = md.nivelEscuela.ToString();
                anioEscuela = md.añoEscuela;
            }
        }
    }
}