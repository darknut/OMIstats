using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using OMIstats.Ajax;

namespace OMIstats.Models
{
    public class Resultados
    {
        public enum TipoMedalla
        {
            NULL,
            ORO_1,
            ORO_2,
            ORO_3,
            ORO,
            PLATA,
            BRONCE,
            NADA,
        }

        public enum TipoError
        {
            OK,
            FALTAN_CAMPOS,
            CLAVE_INEXISTENTE,
            CLAVE_DUPLICADA,
            ESPERABA_NUMEROS,
            MEDALLA_DESCONOCIDA,
            EQUIPO_IOI_INCORRECTO,
            ESTADO_INEXISTENTE
        }

        // Para usarse cuando se están inventando entradas para que los calculos por estado tengan sentido
        public const string CLAVE_DESCONOCIDA = "UNK";
        // Para usarse cuando se tienen los datos de los usuarios, pero la clave es desconocida
        public const string CLAVE_FALTANTE = "???";
        // Para usarse cuando se tiene la clave y los puntos, pero no el nombre del usuario
        public const string NOMBRE_FALTANTE = "XXX";
        public const string NULL_POINTS = "-";

        public int lugar;
        public string omi;
        public TipoOlimpiada tipoOlimpiada;
        public int usuario;
        public string estado;
        public string clave;
        public List<float?> dia1;
        public float? totalDia1;
        public List<float?> dia2;
        public float? totalDia2;
        public float? total;
        public TipoMedalla medalla;
        public bool publico;
        public string ioi;

#if OMISTATS
        public Persona persona;
        public Institucion escuela;
        public Institucion.NivelInstitucion nivelInstitucion;
#endif
        public int añoEscolar;
        public string nombreEstado;

        private bool eliminar;

        public Resultados()
        {
            omi = "";
            tipoOlimpiada = TipoOlimpiada.NULL;

            lugar = 0;
            usuario = 0;
            estado = "";
            clave = "";
            totalDia1 = 0;
            totalDia2 = 0;
            total = 0;
            medalla = TipoMedalla.NULL;
            publico = false;
            ioi = "";

#if OMISTATS
            persona = null;
            escuela = null;
            nivelInstitucion = Institucion.NivelInstitucion.NULL;
#endif
            añoEscolar = 0;

            dia1 = new List<float?>();
            dia1.Add(0);
            dia1.Add(0);
            dia1.Add(0);
            dia1.Add(0);
            dia1.Add(0);
            dia1.Add(0);

            dia2 = new List<float?>();
            dia2.Add(0);
            dia2.Add(0);
            dia2.Add(0);
            dia2.Add(0);
            dia2.Add(0);
            dia2.Add(0);
        }

        private void llenarDatos(DataRow row, bool cargarObjetos, bool miembroDelegacionIncluido = false)
        {
            lugar = (int)row["lugar"];
            usuario = (int)row["concursante"];
            omi = row["olimpiada"].ToString().Trim();
            clave = row["clave"].ToString().Trim();
            estado = row["estado"].ToString().Trim();
            for (int i = 0; i < 6; i++)
                if (row["puntosD1P" + (i + 1)] == DBNull.Value)
                    dia1[i] = null;
                else
                    dia1[i] = float.Parse(row["puntosD1P" + (i + 1)].ToString());
            totalDia1 = float.Parse(row["puntosD1"].ToString());
            for (int i = 0; i < 6; i++)
                if (row["puntosD2P" + (i + 1)] == DBNull.Value)
                    dia2[i] = null;
                else
                    dia2[i] = float.Parse(row["puntosD2P" + (i + 1)].ToString());
            totalDia2 = float.Parse(row["puntosD2"].ToString());
            total = float.Parse(row["puntos"].ToString());
            medalla = (TipoMedalla)Enum.Parse(typeof(TipoMedalla), row["medalla"].ToString());
            publico = (bool)row["publico"];
            ioi = row["ioi"].ToString().Trim();

#if OMISTATS
            if (cargarObjetos)
            {
                persona = Persona.obtenerPersonaConClave(usuario);
                if (!(clave.StartsWith(CLAVE_DESCONOCIDA) || clave.StartsWith(NOMBRE_FALTANTE)))
                {
                    if (miembroDelegacionIncluido)
                    {
                        nivelInstitucion = (Institucion.NivelInstitucion)row["nivel"];
                        añoEscolar = (int)row["año"];
                    }
                    else
                    {
                        MiembroDelegacion md = MiembroDelegacion.obtenerMiembrosConClave(omi, tipoOlimpiada, clave)[0];
                        escuela = Institucion.obtenerInstitucionConClave(md.claveEscuela);
                        nivelInstitucion = md.nivelEscuela;
                        añoEscolar = md.añoEscuela;
                    }
                }
                nombreEstado = Estado.obtenerEstadoConClave(estado).nombre;
            }
#endif
        }

        private TipoError obtenerCampos(string[] datos, int problemasDia1, int problemasDia2)
        {
            int indice = 0;
            if (datos.Length > indice)
                clave = datos[indice++].Trim();
            if (datos.Length > indice)
                estado = datos[indice++].Trim();
            try
            {
                for(int i = 0; i < problemasDia1; i++)
                {
                    if (datos.Length > indice)
                    {
                        if (datos[indice].ToString().Trim() == "-")
                            dia1[i] = null;
                        else
                            dia1[i] = float.Parse(datos[indice]);
                    }
                    indice++;
                }
                if (datos.Length > indice)
                    totalDia1 = float.Parse(datos[indice++]);
                for (int i = 0; i < problemasDia2; i++)
                {
                    if (datos.Length > indice)
                    {
                        if (datos[indice].ToString().Trim() == "-")
                            dia2[i] = null;
                        else
                            dia2[i] = float.Parse(datos[indice]);
                        indice++;
                    }
                }
                if (datos.Length > indice)
                    totalDia2 = float.Parse(datos[indice++]);
                if (datos.Length > indice)
                    total = float.Parse(datos[indice++]);
            }
            catch (Exception)
            {
                return TipoError.ESPERABA_NUMEROS;
            }

            try
            {
                medalla = TipoMedalla.NADA;
                if (datos.Length > indice)
                {
                    if (datos[indice].Trim().Length == 0)
                        indice++;
                    else
                        medalla = (TipoMedalla)Enum.Parse(typeof(TipoMedalla), datos[indice++].Trim().ToUpper());
                }
            }
            catch (Exception)
            {
                return TipoError.MEDALLA_DESCONOCIDA;
            }

            if (datos.Length > indice)
            {
                ioi = datos[indice].Trim();
                if (ioi.Length > 0)
                {
                    ioi = ioi.Substring(0, 1);
                    if (ioi != "A" && ioi != "B")
                    {
                        ioi = "";
                        // Este campo también puede contener el lugar del competidor cuando está en vivo el scoreboard
                        try
                        {
                            lugar = int.Parse(datos[indice].Trim());
                        }
                        catch (Exception)
                        {
                            return TipoError.EQUIPO_IOI_INCORRECTO;
                        }
                    }
                }
                indice++;
            }

            if (datos.Length > indice)
                eliminar = datos[indice].Trim().Equals("eliminar", StringComparison.InvariantCultureIgnoreCase);

            return TipoError.OK;
        }

        private static List<Resultados> cargarResultados(string omi, TipoOlimpiada tipoOlimpiada, bool cargarObjetos, bool cargarCache, int dia, int problemas, out List<CachedResult> cached)
        {
            List<Resultados> lista = new List<Resultados>();
            List<CachedResult> temp = null;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from resultados ");
            query.Append(" where clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            if (omi != null)
            {
                query.Append(" and olimpiada = ");
                query.Append(Utilities.Cadenas.comillas(omi));
            }
            query.Append(" order by puntos desc, clave asc");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            if (cargarCache)
                temp = new List<CachedResult>();

            foreach (DataRow r in table.Rows)
            {
                Resultados res = new Resultados();
                res.tipoOlimpiada = tipoOlimpiada;
                res.llenarDatos(r, cargarObjetos);

                lista.Add(res);

                if (cargarCache)
                {
                    CachedResult cr = new CachedResult();
                    cr.llenarDatos(res, dia, problemas);
                    temp.Add(cr);
                }
            }

            cached = temp;

            return lista;
        }

        /// <summary>
        /// Regresa los resultados de la olimpiada mandada como parametro
        /// </summary>
        /// <param name="omi">La olimpiada en cuestión</param>
        /// <param name="tipoOlimpiada">El tipo de olimpiada</param>
        /// <param name="cargarObjetos">Si los objetos deben de llenarse</param>
        /// <returns>Una lista con los resultados</returns>
        public static List<Resultados> cargarResultados(string omi, TipoOlimpiada tipoOlimpiada, bool cargarObjetos = false)
        {
            List<CachedResult> cache;
            return cargarResultados(omi, tipoOlimpiada, cargarObjetos, false, 0, 0, out cache);
        }

        /// <summary>
        /// Regresa los resultados de la olimpiada mandada como parametro así como
        /// los resultados para mandar por ajax
        /// </summary>
        /// <param name="omi">La olimpiada en cuestión</param>
        /// <param name="tipoOlimpiada">El tipo de olimpiada</param>
        /// <param name="dia">El día del examen</param>
        /// <param name="problemas">Cuántos problemas hay en el día</param>
        /// <param name="cached">La lista por referencia donde se guardarán los
        /// resultados ajax</param>
        /// <returns>La lista con los resultados</returns>
        public static List<Resultados> cargarResultados(string omi, TipoOlimpiada tipoOlimpiada, int dia, int problemas, out List<CachedResult> cached)
        {
            return cargarResultados(omi, tipoOlimpiada, true, true, dia, problemas, out cached);
        }

        /// <summary>
        /// Regresa los resultados de la persona mandada como parametro en la olimpiada particular
        /// </summary>
        /// <param name="omi">La olimpiada en cuestión</param>
        /// <param name="tipoOlimpiada">El tipo de olimpiada</param>
        /// <param name="clave">La clave de la persona</param>
        /// <returns>Los resultados pedidos</returns>
        public static Resultados cargarResultados(string omi, TipoOlimpiada tipoOlimpiada, string clave)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from resultados ");
            query.Append(" where clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(omi));
            query.Append(" and clave = ");
            query.Append(Utilities.Cadenas.comillas(clave));

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            Resultados res = new Resultados();
            res.tipoOlimpiada = tipoOlimpiada;
            if (table.Rows.Count > 0)
                res.llenarDatos(table.Rows[0], cargarObjetos: false);

            return res;
        }

        /// <summary>
        /// Obtiene un string separado con comas, con todos los datos en el objeto
        /// </summary>
        /// <param name="problemasDia1">El número de problemas en el día 1</param>
        /// <param name="problemasDia1">El número de problemas en el día 2</param>
        /// <returns>El string para la tabla</returns>
        public string obtenerLineaAdmin(int problemasDia1, int problemasDia2)
        {
            StringBuilder s = new StringBuilder();

            s.Append(clave);
            s.Append(", ");
            s.Append(estado);
            s.Append(", ");

            for (int i = 0; i < problemasDia1; i++)
            {
                s.Append(dia1[i] == null ? NULL_POINTS : dia1[i].ToString());
                s.Append(", ");
            }

            s.Append(totalDia1);
            s.Append(", ");

            for (int i = 0; i < problemasDia2; i++)
            {
                s.Append(dia2[i] == null ? NULL_POINTS : dia2[i].ToString());
                s.Append(", ");
            }

            s.Append(totalDia2);
            s.Append(", ");
            s.Append(total);
            s.Append(", ");
            s.Append(medalla.ToString().ToLower());
            s.Append(", ");
            s.Append(ioi);

            return s.ToString();
        }

        /// <summary>
        /// Elimina los resultados con las claves mandadas como parametro
        /// </summary>
        /// <param name="omi">La omi de los resultados</param>
        /// <param name="tipoOlimpiada">El tipo de olimpiada</param>
        /// <param name="clave">La clave a borrar</param>
        public static void eliminarResultado(string omi, TipoOlimpiada tipoOlimpiada, string clave)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" delete Resultados where olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(omi));
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and clave = ");
            query.Append(Utilities.Cadenas.comillas(clave));

            db.EjecutarQuery(query.ToString());
        }

        /// <summary>
        /// Actualiza la clave del resultado de claveOriginal a claveNueva
        /// </summary>
        /// <param name="omi">La omi del resultado</param>
        /// <param name="tipoOlimpiada">El tipo de olimpiada</param>
        /// <param name="claveOriginal">La clave original</param>
        /// <param name="claveNueva">La nueva clave</param>
        /// <rereturns>Si se cambio exitosamente la clave</rereturns>
        public static bool cambiarClave(string omi, TipoOlimpiada tipoOlimpiada, string claveOriginal, string claveNueva)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" update Resultados set clave = ");
            query.Append(Utilities.Cadenas.comillas(claveNueva));
            query.Append(" where olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(omi));
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and clave = ");
            query.Append(Utilities.Cadenas.comillas(claveOriginal));

            if(db.EjecutarQuery(query.ToString()).error)
                return false;
            return true;
        }

#if OMISTATS
        /// <summary>
        /// Guarda la linea mandada como parametro en la base de datos
        /// </summary>
        /// <param name="omi">La clave de la olimpiada</param>
        /// <param name="tipoOlimpiada">El tipo de olimpiada a los que los datos pertenecen</param>
        /// <param name="problemasDia1">El número de problemas a desplegar el día 1</param>
        /// <param name="problemasDia2">El número de problemas a desplegar el día 2</param>
        /// <param name="linea">Los datos tabulados por comas</param>
        /// <returns>Si hubo un error, lo devuelve</returns>
        public static TipoError guardarLineaAdmin(string omi, TipoOlimpiada tipoOlimpiada, int problemasDia1, int problemasDia2, string linea)
        {
            if (linea.Trim().Length == 0)
                return TipoError.OK;

            Resultados res = new Resultados();
            string[] datos = linea.Split(',');

            // Casteamos los datos del string a variables

            TipoError err = res.obtenerCampos(datos, problemasDia1, problemasDia2);
            if (err != TipoError.OK)
                return err;

            // Si se pidio que se borre, se borra y olvidamos el resto del codigo

            if (res.eliminar)
            {
                eliminarResultado(omi, tipoOlimpiada, res.clave);
                return TipoError.OK;
            }

            // Ya se validaron los campos, ahora se guarda el objeto en la base

            res.omi = omi;
            res.tipoOlimpiada = tipoOlimpiada;

            return res.guardar();
        }
#endif

        /// <summary>
        /// Guarda los datos del objeto en la base de datos
        /// <param name="detalles">Si las tablas de detalles también deben de guardarse</param>
        /// <param name="timestamp">Si detalles es true, el timestamp es requerido</param>
        /// <param name="timestamp">Si detalles es true, el dia es requerido</param>
        /// </summary>
        public TipoError guardar(bool detalles = false, int timestamp = 0, int dia = 0)
        {
            StringBuilder query = new StringBuilder();
            Utilities.Acceso db = new Utilities.Acceso();
            MiembroDelegacion m = null;

            // Revisamos si hay mas de un usuario con esa clave

            if (this.clave.StartsWith(CLAVE_DESCONOCIDA) || this.clave.StartsWith(CLAVE_FALTANTE) || this.clave.StartsWith(NOMBRE_FALTANTE))
            {
#if OMISTATS
                Estado e = Estado.obtenerEstadoConClave(this.estado);
                if (e == null)
                    return TipoError.ESTADO_INEXISTENTE;
#endif
            }

            if (!(this.clave.StartsWith(CLAVE_DESCONOCIDA) || this.clave.StartsWith(NOMBRE_FALTANTE)))
            {
                List<MiembroDelegacion> lista = MiembroDelegacion.obtenerMiembrosConClave(this.omi, this.tipoOlimpiada, this.clave);
                if (lista.Count == 0)
                    return TipoError.CLAVE_INEXISTENTE;
                if (lista.Count != 1)
                    return TipoError.CLAVE_DUPLICADA;
                m = lista[0];
            }

            // Ya tenemos todos los datos necesarios, hacemos un insert con las claves

            query.Append("insert into Resultados values(");
            query.Append(Utilities.Cadenas.comillas(this.omi));
            query.Append(", ");
            query.Append(Utilities.Cadenas.comillas(this.tipoOlimpiada.ToString().ToLower()));
            query.Append(", 0, ");
            query.Append(Utilities.Cadenas.comillas(this.clave));
            query.Append(", '', 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,");
            query.Append((int)TipoMedalla.NADA);
            query.Append(", 0, '', 0)");

            db.EjecutarQuery(query.ToString());
            query.Clear();

            // Ahora actualizamos con los datos que tenemos en el objeto

            query.Append(" update Resultados set ");
            if (m != null)
            {
                query.Append(" concursante = ");
                query.Append(m.claveUsuario);
                query.Append(", ");
            }
            query.Append(" estado = ");
            query.Append(Utilities.Cadenas.comillas(m == null ? this.estado : m.estado));
            query.Append(", puntosD1P1 = ");
            query.Append(this.dia1[0] == null ? "null" : this.dia1[0].ToString());
            query.Append(", puntosD1P2 = ");
            query.Append(this.dia1[1] == null ? "null" : this.dia1[1].ToString());
            query.Append(", puntosD1P3 = ");
            query.Append(this.dia1[2] == null ? "null" : this.dia1[2].ToString());
            query.Append(", puntosD1P4 = ");
            query.Append(this.dia1[3] == null ? "null" : this.dia1[3].ToString());
            query.Append(", puntosD1P5 = ");
            query.Append(this.dia1[4] == null ? "null" : this.dia1[4].ToString());
            query.Append(", puntosD1P6 = ");
            query.Append(this.dia1[5] == null ? "null" : this.dia1[5].ToString());
            query.Append(", puntosD1 = ");
            query.Append(this.totalDia1 == null ? "null" : this.totalDia1.ToString());
            query.Append(", puntosD2P1 = ");
            query.Append(this.dia2[0] == null ? "null" : this.dia2[0].ToString());
            query.Append(", puntosD2P2 = ");
            query.Append(this.dia2[1] == null ? "null" : this.dia2[1].ToString());
            query.Append(", puntosD2P3 = ");
            query.Append(this.dia2[2] == null ? "null" : this.dia2[2].ToString());
            query.Append(", puntosD2P4 = ");
            query.Append(this.dia2[3] == null ? "null" : this.dia2[3].ToString());
            query.Append(", puntosD2P5 = ");
            query.Append(this.dia2[4] == null ? "null" : this.dia2[4].ToString());
            query.Append(", puntosD2P6 = ");
            query.Append(this.dia2[5] == null ? "null" : this.dia2[5].ToString());
            query.Append(", puntosD2 = ");
            query.Append(this.totalDia2 == null ? "null" : this.totalDia2.ToString());
            query.Append(", puntos = ");
            query.Append(this.total == null ? "null" : this.total.ToString());
            query.Append(", medalla = ");
            query.Append((int)this.medalla);
            query.Append(", ioi = ");
            query.Append(Utilities.Cadenas.comillas(this.ioi));
            if (this.lugar > 0)
            {
                query.Append(", lugar = ");
                query.Append(this.lugar);
            }
            query.Append(" where olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(this.omi));
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(this.tipoOlimpiada.ToString().ToLower()));
            query.Append(" and clave = ");
            query.Append(Utilities.Cadenas.comillas(this.clave));

            db.EjecutarQuery(query.ToString());

            if (detalles)
            {
                DetallePuntos detallePuntos = new DetallePuntos(omi, tipoOlimpiada, clave, timestamp, dia, dia == 1 ? dia1 : dia2);
                detallePuntos.guardar();
                DetalleLugar detalleLugar = new DetalleLugar(omi, tipoOlimpiada, clave, timestamp, dia, medalla, lugar);
                detalleLugar.guardar();
            }

            return TipoError.OK;
        }

        /// <summary>
        /// Determina si se deben de desplegar los resultados de los problemas individuales
        /// </summary>
        /// <param name="clave">La clave de la olimpiada</param>
        /// <param name="tipoOlimpiada">El tipo de olimpiada</param>
        /// <returns>Si se deben desplegar los resultados o no</returns>
        public static bool mostrarResultadosIndividuales(string clave, TipoOlimpiada tipoOlimpiada)
        {
            StringBuilder query = new StringBuilder();
            Utilities.Acceso db = new Utilities.Acceso();

            query.Append(" select count(*) from resultados where olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(clave));
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and (round(puntosD1P1 + puntosD1P2 + puntosD1P3 + puntosD1P4 + puntosD1P5 + puntosD1P6,0) <> round(puntosD1,0) ");
            query.Append(" or round(puntosD2P1 + puntosD2P2 + puntosD2P3 + puntosD2P4 + puntosD2P5 + puntosD2P6,0) <> round(puntosD2,0))");

            db.EjecutarQuery(query.ToString());

            return ((int)db.getTable().Rows[0][0]) == 0;
        }

        /// <summary>
        /// Determina si se deben de desplegar los resultados de los dias individuales
        /// </summary>
        /// <param name="clave">La clave de la olimpiada</param>
        /// <param name="tipoOlimpiada">El tipo de olimpiada</param>
        /// <returns>Si se deben desplegar los resultados o no</returns>
        public static bool mostrarResultadosPorDia(string clave, TipoOlimpiada tipoOlimpiada)
        {
            StringBuilder query = new StringBuilder();
            Utilities.Acceso db = new Utilities.Acceso();

            query.Append(" select count(*) from resultados where olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(clave));
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and round(puntosD1 + puntosD2, 0) <> round(puntos, 0) ");

            db.EjecutarQuery(query.ToString());

            return ((int)db.getTable().Rows[0][0]) == 0;
        }

        /// <summary>
        /// Regresa las participaciones en olimpiadas del usuario mandado como parametro
        /// </summary>
        /// <param name="persona">La persona de la que se quieren los datos</param>
        /// <param name="tipoOlimpiada">El tipo de olimpiada que se solicita</param>
        /// <returns>La lista de participaciones</returns>
        public static Dictionary<TipoOlimpiada, List<Resultados>> obtenerParticipacionesComoCompetidorPara(int persona, TipoOlimpiada tipoOlimpiada)
        {
            Dictionary<TipoOlimpiada, List<Resultados>> participaciones = new Dictionary<TipoOlimpiada, List<Resultados>>();

            participaciones.Add(tipoOlimpiada, obtenerParticipaciones(persona, tipoOlimpiada));
            if (tipoOlimpiada == TipoOlimpiada.OMI)
            {
                participaciones.Add(TipoOlimpiada.OMIP, obtenerParticipaciones(persona, TipoOlimpiada.OMIP));
                participaciones.Add(TipoOlimpiada.OMIS, obtenerParticipaciones(persona, TipoOlimpiada.OMIS));
            }

            return participaciones;
        }

        private static List<Resultados> obtenerParticipaciones(int persona, TipoOlimpiada tipoOlimpiada)
        {
            List<Resultados> lista = new List<Resultados>();
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select r.* from resultados  as r ");
            query.Append(" inner join Olimpiada as o on o.numero = r.olimpiada and o.clase = r.clase ");
            query.Append(" where r.clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and r.concursante = ");
            query.Append(persona);
            query.Append(" order by o.año asc");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                Resultados res = new Resultados();
                res.tipoOlimpiada = tipoOlimpiada;
                res.llenarDatos(r, cargarObjetos: true);

                lista.Add(res);
            }

            return lista;
        }

        /// <summary>
        /// Regresa los alumnos de la institucion mandada como parametro
        /// </summary>
        /// <param name="clave">La clave de la institución deseada</param>
        /// <param name="tipoOlimpiada">El tipo de olimpiada solicitado</param>
        /// <returns>La lista de alumnos</returns>
        public static List<Resultados> obtenerAlumnosDeInstitucion(int clave, TipoOlimpiada tipoOlimpiada)
        {
            List<Resultados> lista = new List<Resultados>();
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select r.*, md.institucion, md.nivel, md.año from Resultados as r ");
            query.Append(" inner join MiembroDelegacion as md on r.clave = md.clave ");
            query.Append(" and md.olimpiada = r.olimpiada ");
            query.Append(" and md.clase = r.clase ");
            query.Append(" where r.clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and md.institucion = ");
            query.Append(clave);
            query.Append(" order by r.medalla, r.concursante, md.olimpiada desc");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                Resultados res = new Resultados();
                res.tipoOlimpiada = tipoOlimpiada;
                res.llenarDatos(r, cargarObjetos: true, miembroDelegacionIncluido: true);

                lista.Add(res);
            }

            return lista;
        }

        /// <summary>
        /// Regresa los alumnos del estado mandado como parametro
        /// </summary>
        /// <param name="clave">La clave del estado deseado</param>
        /// <param name="tipoOlimpiada">El tipo de olimpiada solicitado</param>
        /// <returns>La lista de alumnos</returns>
        public static List<Resultados> obtenerAlumnosDeEstado(string clave, TipoOlimpiada tipoOlimpiada)
        {
            List<Resultados> lista = new List<Resultados>();
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from Resultados ");
            query.Append(" where clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and estado = ");
            query.Append(Utilities.Cadenas.comillas(clave));
            query.Append(" and clave not like '");
            query.Append(CLAVE_DESCONOCIDA);
            query.Append("%' ");
            query.Append(" order by medalla, concursante, olimpiada desc");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                Resultados res = new Resultados();
                res.tipoOlimpiada = tipoOlimpiada;
                res.llenarDatos(r, cargarObjetos: true);

                lista.Add(res);
            }

            return lista;
        }

        /// <summary>
        /// Calcula los números de una olimpiada terminada para el problema de instancia
        /// NO se guarda en la base de datos
        /// </summary>
        /// <param name="olimpiada">La olimpiada de la que se solicitan los datos</param>
        /// <param name="tipoOlimpiada">El tipo de olimpiada que se solicita</param>
        /// <param name="dia">El dia del problema</param>
        /// <param name="numero">El numero del problema</param>
        public static Problema calcularNumeros(string olimpiada, TipoOlimpiada tipoOlimpiada, int dia = 0, int numero = 0, int totalProblemas = 0)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();
            Problema p = new Problema();
            string columna = "puntos";
            DataTable table = null;
            int total, mitad, perfecto = 100;
            float suma;

            if (dia > 0)
            {
                columna += "D" + dia;
                if (numero > 0)
                    columna += "P" + numero;
            }

            query.Append(" select count(*) from Resultados where olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(olimpiada));
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and (");
            query.Append(columna);

            db.EjecutarQuery(query.ToString() + " = 0 or " + columna + " is null)");
            p.ceros = (int)db.getTable().Rows[0][0];

            if (totalProblemas > 0)
                perfecto *= totalProblemas;
            db.EjecutarQuery(query.ToString() + " = " + perfecto + ")");
            p.perfectos = (int)db.getTable().Rows[0][0];

            query.Clear();
            query.Append(" select sum(");
            query.Append(columna);
            query.Append(") from Resultados where olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(olimpiada));
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));

            db.EjecutarQuery(query.ToString());
            string totalStr = db.getTable().Rows[0][0].ToString();
            if (totalStr == "")
                totalStr = "0";
            suma = float.Parse(totalStr);

            query.Clear();
            query.Append(" select ");
            query.Append(columna);
            query.Append(" from Resultados where olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(olimpiada));
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" order by ");
            query.Append(columna);
            query.Append(" desc ");

            db.EjecutarQuery(query.ToString());
            table = db.getTable();
            total = table.Rows.Count;

            if (total > 0)
            {
                p.media = suma / total;
                p.media = (float) Math.Round((Decimal)p.media, 2, MidpointRounding.AwayFromZero);
                mitad = total / 2;
                p.mediana = table.Rows[mitad][0] is DBNull ? 0 : float.Parse(table.Rows[mitad][0].ToString());

                if (total % 2 == 0)
                    p.mediana = (p.mediana + (table.Rows[mitad + 1][0] is DBNull ? 0 : float.Parse(table.Rows[mitad + 1][0].ToString()))) / 2;
            }

            p.dia = dia;
            p.numero = numero;
            p.olimpiada = olimpiada;
            p.tipoOlimpiada = tipoOlimpiada;

            return p;
        }

        /// <summary>
        /// Guarda el lugar de la linea en la base de datos
        /// </summary>
        public void guardarLugar()
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" update Resultados set lugar = ");
            query.Append(lugar);
            query.Append(" where olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(omi));
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and clave = ");
            query.Append(Utilities.Cadenas.comillas(clave));

            db.EjecutarQuery(query.ToString());
        }

        /// <summary>
        /// Regresa el número de estados participantes en la Olimpiada mandada como parámetro
        /// </summary>
        /// <param name="omi">La OMI deseada</param>
        /// <param name="tipoOlimpiada">El tipo de Olimpiada</param>
        /// <returns>Cuantos estados participaron</returns>
        public static int obtenerEstadosParticipantes(string omi, TipoOlimpiada tipoOlimpiada)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select count(distinct(Estado)) from Resultados where olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(omi));
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));

            db.EjecutarQuery(query.ToString());

            return (int)db.getTable().Rows[0][0];
        }

        private static int countMejores(string columna, float? puntos, string omi, string tipo)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select COUNT(*) from Resultados where ");
            query.Append(columna);
            query.Append(" > ");
            query.Append(puntos);
            query.Append(" and olimpiada = ");
            query.Append(omi);
            query.Append(" and clase = ");
            query.Append(tipo);

            db.EjecutarQuery(query.ToString());
            return (int)db.getTable().Rows[0][0] + 1;
        }

        /// <summary>
        /// Regresa una lista con el lugar del competidor mandado como parametro para cada problema y cada dia
        /// </summary>
        public static List<int> cargarMejores(string omi, TipoOlimpiada tipo, string clave, int problemasDia1, int problemasDia2)
        {
            List<int> mejores = new List<int>();
            Resultados res = Resultados.cargarResultados(omi, tipo, clave);
            string omiString = Utilities.Cadenas.comillas(omi);
            string tipoString = Utilities.Cadenas.comillas(tipo.ToString().ToLower());

            for (int i = 1; i <= problemasDia1; i++)
            {
                if (res.dia1[i - 1] == null || res.dia1[i - 1] == 0.0)
                {
                    mejores.Add(0);
                    continue;
                }
                mejores.Add(countMejores("puntosD1P" + i, res.dia1[i - 1], omiString, tipoString));
            }

            if (problemasDia2 > 0)
            {
                mejores.Add(countMejores("puntosD1", res.totalDia1, omiString, tipoString));

                for (int i = 1; i <= problemasDia2; i++)
                {
                    if (res.dia2[i - 1] == null || res.dia2[i - 1] == 0.0)
                    {
                        mejores.Add(0);
                        continue;
                    }
                    mejores.Add(countMejores("puntosD2P" + i, res.dia2[i - 1], omiString, tipoString));
                }

                mejores.Add(countMejores("puntosD2", res.totalDia2, omiString, tipoString));
            }

            return mejores;
        }

        public static string generarDiplomas(string omi, string X, string baseURL)
        {
            StringBuilder lineas = new StringBuilder();
            StringBuilder query = new StringBuilder();
            Utilities.Acceso db = new Utilities.Acceso();

            query.Append(" select p.clave as persona, p.nombre, r.clave,r.clase, r.medalla, r.estado from Resultados as r ");
            query.Append(" inner join Persona as p on p.clave = r.concursante ");
            query.Append(" where r.olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(omi));
            query.Append(" and medalla <> 7 ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                int claveUsuario = (int)r["persona"];
                string nombre = r["nombre"].ToString().Trim();
                string clave = r["clave"].ToString().Trim();
                string estado = r["estado"].ToString().Trim();
                TipoOlimpiada clase = (TipoOlimpiada)Enum.Parse(typeof(TipoOlimpiada), r["clase"].ToString().ToUpper());
                TipoMedalla medalla = (TipoMedalla)Enum.Parse(typeof(TipoMedalla), r["medalla"].ToString().ToUpper());

                lineas.Append(estado);
                lineas.Append("\\");
                lineas.Append(clave);
                lineas.Append("-medalla.pdf,");
                lineas.Append(nombre);
                lineas.Append(",");
                lineas.Append(X);
                lineas.Append(",");
                lineas.Append("Medalla de ");

                if (medalla == TipoMedalla.BRONCE)
                    lineas.Append("Bronce");
                else if (medalla == TipoMedalla.PLATA)
                    lineas.Append("Plata");
                else
                    lineas.Append("Oro");

                lineas.Append(",");
                lineas.Append(baseURL);
                lineas.Append("/Profile/");
                lineas.Append(clase.ToString());
                lineas.Append("/");
                lineas.Append(omi);
                lineas.Append("/");
                lineas.Append(clave);
                lineas.Append(",");
                lineas.Append(clase.ToString());
                lineas.Append("\n");
            }

            return lineas.ToString();
        }

        public static string generarDiplomasEspeciales(string omi, string baseURL)
        {
            StringBuilder lineas = new StringBuilder();
            StringBuilder query = new StringBuilder();
            Utilities.Acceso db = new Utilities.Acceso();

            // El primer diploma especial es el medallista más joven
            var resultados = Resultados.cargarResultados(omi, TipoOlimpiada.OMI, cargarObjetos: true);
            Resultados joven = resultados[0];

            foreach (Resultados resultado in resultados)
            {
                if (resultado.medalla != TipoMedalla.NADA &&
                    resultado.persona.nacimiento > joven.persona.nacimiento)
                {
                    joven = resultado;
                }
            }

            lineas.Append(joven.estado);
            lineas.Append("\\");
            lineas.Append(joven.clave);
            lineas.Append("-joven.pdf,");
            lineas.Append(joven.persona.nombre);
            lineas.Append(",POR HABER SIDO,El Medallista Más Joven,");
            lineas.Append(baseURL);
            lineas.Append("/Profile/");
            lineas.Append(joven.tipoOlimpiada.ToString());
            lineas.Append("/");
            lineas.Append(omi);
            lineas.Append("/");
            lineas.Append(joven.clave);
            lineas.Append(",");
            lineas.Append(joven.tipoOlimpiada.ToString());
            lineas.Append("\n");

            // Los siguientes diplomas, son los diplomas de estados

            Medallero medalleroGral = null;
            List<Medallero> medallero = Medallero.obtenerTablaEstados(TipoOlimpiada.OMI, omi, out medalleroGral);

            for (int i = 0; i < 3; i++)
            {
                Estado estado = Estado.obtenerEstadoConClave(medallero[i].clave);
                lineas.Append(estado.clave);
                lineas.Append("\\");
                lineas.Append(estado.nombre);
                lineas.Append(".pdf,");
                switch (estado.clave)
                {
                    case "MDF":
                        lineas.Append("La ");
                        break;
                    case "MEX":
                        lineas.Append("El ");
                        break;
                    default:
                        lineas.Append("El Estado de ");
                        break;
                }
                lineas.Append(estado.nombre);
                lineas.Append(",");
                lineas.Append("POR HABER OBTENIDO");
                lineas.Append(",");
                switch (i)
                {
                    case 0:
                        lineas.Append("Primer");
                        break;
                    case 1:
                        lineas.Append("Segundo");
                        break;
                    case 2:
                        lineas.Append("Tercer");
                        break;
                }
                lineas.Append(" Lugar a Nivel Estados,,OMI");
                lineas.Append("\n");
            }

            return lineas.ToString();
        }
    }
}