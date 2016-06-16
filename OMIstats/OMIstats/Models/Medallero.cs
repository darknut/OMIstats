using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;

namespace OMIstats.Models
{
    public class Medallero
    {
        public enum TipoMedallero
        {
            NULL,
            PERSONA,
            ESTADO,
            INSTITUCION,
            ASESOR
        }

        public Olimpiada.TipoOlimpiada tipoOlimpiada { get; set; }

        public TipoMedallero tipoMedallero { get; set; }

        public string clave { get; set; }

        public int oros { get; set; }

        public int platas { get; set; }

        public int bronces { get; set; }

        public int otros { get; set; }

        public string orosTooltip { get; set; }

        public string platasTooltip { get; set; }

        public string broncesTooltip { get; set; }

        public string otrosTooltip { get; set; }

        public Medallero()
        {
            tipoOlimpiada = Olimpiada.TipoOlimpiada.NULL;
            tipoMedallero = TipoMedallero.NULL;
            clave = "";
            oros = 0;
            platas = 0;
            bronces = 0;
            otros = 0;
        }

        private void llenarDatos(DataRow datos)
        {
            tipoOlimpiada = (Olimpiada.TipoOlimpiada)Enum.Parse(typeof(Olimpiada.TipoOlimpiada), datos["clase"].ToString().ToUpper());
            tipoMedallero = (TipoMedallero)Enum.Parse(typeof(TipoMedallero), datos["tipo"].ToString().ToUpper());
            clave = datos["clave"].ToString().Trim();
            oros = (int)datos["oros"];
            platas = (int)datos["platas"];
            bronces = (int)datos["bronces"];
            otros = (int)datos["otros"];
        }

        /// <summary>
        /// Obtiene el medallero de la base de datos
        /// </summary>
        /// <param name="tipoOlimpiada">El tipo de la olimpiada de la que se requiere el medallero</param>
        /// <param name="tipoMedallero">Si es estado, persona, institucion o asesor</param>
        /// <param name="clave">La clave del estado/persona/institucion/asesor</param>
        /// <returns>Un objeto medallero con los datos deseados</returns>
        public Medallero obtenerMedallas(Olimpiada.TipoOlimpiada tipoOlimpiada, TipoMedallero tipoMedallero, string clave)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from medallero where clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and tipo = ");
            query.Append((int)tipoMedallero);
            query.Append(" and clave = ");
            query.Append(Utilities.Cadenas.comillas(clave));

            if (db.EjecutarQuery(query.ToString()).error)
                return null;

            DataTable table = db.getTable();

            Medallero m = new Medallero();
            m.llenarDatos(table.Rows[0]);

            return m;
        }

        /// <summary>
        /// Guarda los datos en el objeto en la base de datos
        /// </summary>
        /// <returns>Regresa si se guardo o no</returns>
        public bool guardarDatos()
        {
            if (tipoMedallero == TipoMedallero.NULL || tipoOlimpiada == Olimpiada.TipoOlimpiada.NULL || clave == "")
                return false;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" update medallero set oros = ");
            query.Append(oros);
            query.Append(", platas = ");
            query.Append(platas);
            query.Append(", bronces = ");
            query.Append(bronces);
            query.Append(", otros = ");
            query.Append(otros);
            query.Append(" where clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and tipo = ");
            query.Append((int)tipoMedallero);
            query.Append(" and clave = ");
            query.Append(Utilities.Cadenas.comillas(clave));

            return !db.EjecutarQuery(query.ToString()).error;
        }

        /// <summary>
        /// Usa las variables en el objeto para calcular las medallas basadas en lo que hay en la base de datos
        /// </summary>
        /// <returns>Regresa si se calcularon o no</returns>
        public bool calcularMedallas()
        {
            if (tipoMedallero == TipoMedallero.NULL || tipoOlimpiada == Olimpiada.TipoOlimpiada.NULL || clave == "")
                return false;

            switch (tipoMedallero)
            {
                case TipoMedallero.PERSONA:
                    {
                        Utilities.Acceso db = new Utilities.Acceso();
                        StringBuilder query = new StringBuilder();

                        query.Append(" select medalla, olimpiada from resultados where concursante = ");
                        query.Append(Utilities.Cadenas.comillas(clave));
                        query.Append(" and clase = ");
                        query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
                        query.Append(" order by medalla desc ");

                        if (db.EjecutarQuery(query.ToString()).error)
                            return false;

                        DataTable table = db.getTable();

                        foreach (DataRow r in table.Rows)
                        {
                            Resultados.TipoMedalla medalla = (Resultados.TipoMedalla)Enum.Parse(typeof(Resultados.TipoMedalla), r["medalla"].ToString().ToUpper());
                            string olimpiada = r["olimpiada"].ToString().Trim();

                            switch (medalla)
                            {
                                case Resultados.TipoMedalla.ORO_3:
                                    oros++;
                                    orosTooltip += olimpiada + "ª(III) ";
                                    break;
                                case Resultados.TipoMedalla.ORO_2:
                                    oros++;
                                    orosTooltip += olimpiada + "ª(II) ";
                                    break;
                                case Resultados.TipoMedalla.ORO_1:
                                    oros++;
                                    orosTooltip += olimpiada + "ª(I) ";
                                    break;
                                case Resultados.TipoMedalla.ORO:
                                    oros++;
                                    orosTooltip += olimpiada + "ª ";
                                    break;
                                case Resultados.TipoMedalla.PLATA:
                                    platas++;
                                    platasTooltip += olimpiada + "ª ";
                                    break;
                                case Resultados.TipoMedalla.BRONCE:
                                    bronces++;
                                    broncesTooltip += olimpiada + "ª ";
                                    break;
                                case default:
                                    otros++;
                                    otrosTooltip += olimpiada + "ª ";
                                    break;
                            }
                        }

                        break;
                    }
            }

            return true;
        }
    }
}