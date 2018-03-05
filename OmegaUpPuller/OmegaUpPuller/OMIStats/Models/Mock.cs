using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMIstats.Models
{
    public class Persona
    {
        public int clave;
        public string nombre;
        public DateTime nacimiento;
        public string genero;
        public string CURP;
        public string correo;
        public string usuario;

        public static Persona obtenerPersonaConClave(int clave)
        {
            throw new InvalidOperationException("obtenerPersonaConClave no permitido en este contexto");
        }

        public static Persona obtenerPersonaDeUsuario(string usuario)
        {
            throw new InvalidOperationException("obtenerPersonaDeUsuario no permitido en este contexto");
        }

        public static Persona obtenerPersonaConCURP(string usuario)
        {
            throw new InvalidOperationException("obtenerPersonaConCURP no permitido en este contexto");
        }

        public static Persona obtenerPersonaConNombre(string usuario)
        {
            throw new InvalidOperationException("obtenerPersonaConNombre no permitido en este contexto");
        }

        public void nuevoUsuario(Utilities.Archivos.FotoInicial foto)
        {
            throw new InvalidOperationException("nuevoUsuario no permitido en este contexto");
        }

        public bool guardarDatos()
        {
            throw new InvalidOperationException("guardarDatos no permitido en este contexto");
        }
    }

    public class Institucion
    {
        public string nombre;
        public bool primaria;
        public bool secundaria;
        public bool preparatoria;
        public bool universidad;
        public bool publica;
        public int clave;

        public enum NivelInstitucion
        {
            NULL,
            PRIMARIA,
            SECUNDARIA,
            PREPARATORIA,
            UNIVERSIDAD
        }

        public static Institucion obtenerInstitucionConClave(int clave)
        {
            throw new InvalidOperationException("obtenerInstitucionConClave no permitido en este contexto");
        }

        public static Institucion buscarInstitucionConNombre(string nombre)
        {
            throw new InvalidOperationException("buscarInstitucionConNombre no permitido en este contexto");
        }

        public void nuevaInstitucion()
        {
            throw new InvalidOperationException("nuevaInstitucion no permitido en este contexto");
        }

        public void guardar(bool generarPeticiones)
        {
            throw new InvalidOperationException("guardar no permitido en este contexto");
        }
    }

    public class Estado
    {
        public string nombre;

        public static Estado obtenerEstadoConClave(string clave)
        {
            return new Estado();
        }
    }

    public class Problema
    {
        public int ceros;
        public int perfectos;
        public float media;
        public float mediana;
        public int dia;
        public int numero;
        public string olimpiada;
        public TipoOlimpiada tipoOlimpiada;
    }
}
