using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OMIstats.Models;

namespace OMIstats.Utilities
{
    public class EnumParser
    {
        public static MiembroDelegacion.TipoAsistente ToTipoAsistente(string value)
        {
            return (MiembroDelegacion.TipoAsistente)Enum.Parse(typeof(MiembroDelegacion.TipoAsistente), value);
        }

        public static TipoOlimpiada ToTipoOlimpiada(string value)
        {
            return (TipoOlimpiada)Enum.Parse(typeof(TipoOlimpiada), value);
        }

        public static Resultados.TipoMedalla ToTipoMedalla(string value)
        {
            return (Resultados.TipoMedalla)Enum.Parse(typeof(Resultados.TipoMedalla), value);
        }

        public static Log.TipoLog ToTipoLog(string value)
        {
            return (Log.TipoLog)Enum.Parse(typeof(Log.TipoLog), value);
        }

        public static Medallero.TipoMedallero ToTipoMedallero(string value)
        {
            return (Medallero.TipoMedallero)Enum.Parse(typeof(Medallero.TipoMedallero), value);
        }

        public static OmegaUp.Instruccion ToInstruccion(string value)
        {
            return (OmegaUp.Instruccion)Enum.Parse(typeof(OmegaUp.Instruccion), value);
        }

        public static OmegaUp.Status ToStatus(string value)
        {
            return (OmegaUp.Status)Enum.Parse(typeof(OmegaUp.Status), value);
        }

#if OMISTATS
        public static Persona.TipoPermisos ToTipoPermisos(string value)
        {
            return (Persona.TipoPermisos)Enum.Parse(typeof(Persona.TipoPermisos), value);
        }

        public static Peticion.TipoPeticion ToTipoPeticion(string value)
        {
            return (Peticion.TipoPeticion)Enum.Parse(typeof(Peticion.TipoPeticion), value);
        }

        public static Institucion.NivelInstitucion ToNivelInstitucion(string value)
        {
            return (Institucion.NivelInstitucion)Enum.Parse(typeof(Institucion.NivelInstitucion), value);
        }
#endif
    }
}