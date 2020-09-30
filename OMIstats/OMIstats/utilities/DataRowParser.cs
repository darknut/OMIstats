using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OMIstats.Models;

namespace OMIstats.Utilities
{
    public class DataRowParser
    {
        public static int ToInt(object value)
        {
            if (value == DBNull.Value)
                return 0;
            return (int)value;
        }

        public static string ToString(object value)
        {
            return value.ToString().Trim();
        }

        public static float? ToFloat(object value)
        {
            if (value is DBNull)
                return null;
            return float.Parse(value.ToString());
        }

        public static float ToStrictFloat(object value)
        {
            if (value is DBNull)
                return 0;
            return float.Parse(value.ToString());
        }

        public static DateTime ToLongDateTime(object value)
        {
            return DateTime.Parse(value.ToString().Trim());
        }

        public static DateTime ToDateTime(object value)
        {
            return Fechas.stringToDate(value.ToString().Trim());
        }

        public static bool ToBool(object value)
        {
            if (value == DBNull.Value)
                return false;
            return (bool)value;
        }

        public static short ToShort(object value)
        {
            return (short)value;
        }

        public static long ToLong(object value)
        {
            return long.Parse(value.ToString());
        }

        public static MiembroDelegacion.TipoAsistente ToTipoAsistente(object value)
        {
            return (MiembroDelegacion.TipoAsistente)Enum.Parse(typeof(MiembroDelegacion.TipoAsistente), value.ToString().ToUpper());
        }

        public static TipoOlimpiada ToTipoOlimpiada(object value)
        {
            return (TipoOlimpiada)Enum.Parse(typeof(TipoOlimpiada), value.ToString().ToUpper());
        }

        public static Resultados.TipoMedalla ToTipoMedalla(object value)
        {
            return (Resultados.TipoMedalla)Enum.Parse(typeof(Resultados.TipoMedalla), value.ToString().ToUpper());
        }

        public static Log.TipoLog ToTipoLog(object value)
        {
            return (Log.TipoLog)Enum.Parse(typeof(Log.TipoLog), value.ToString().ToUpper());
        }

        public static Medallero.TipoMedallero ToTipoMedallero(object value)
        {
            return (Medallero.TipoMedallero)Enum.Parse(typeof(Medallero.TipoMedallero), value.ToString().ToUpper());
        }

        public static OmegaUp.Instruccion ToInstruccion(object value)
        {
            return (OmegaUp.Instruccion)Enum.Parse(typeof(OmegaUp.Instruccion), value.ToString().ToUpper());
        }

        public static OmegaUp.Status ToStatus(object value)
        {
            return (OmegaUp.Status)Enum.Parse(typeof(OmegaUp.Status), value.ToString().ToUpper());
        }

#if OMISTATS
        public static Persona.TipoPermisos ToTipoPermisos(object value)
        {
            return (Persona.TipoPermisos)Enum.Parse(typeof(Persona.TipoPermisos), value.ToString().ToUpper());
        }

        public static Peticion.TipoPeticion ToTipoPeticion(object value)
        {
            return (Peticion.TipoPeticion)Enum.Parse(typeof(Peticion.TipoPeticion), value.ToString().ToUpper());
        }

        public static Institucion.NivelInstitucion ToNivelInstitucion(object value)
        {
            return (Institucion.NivelInstitucion)Enum.Parse(typeof(Institucion.NivelInstitucion), value.ToString().ToUpper());
        }
#endif
    }
}