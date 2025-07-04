﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using OMIstats.Ajax;
using OMIstats.Utilities;

namespace OMIstats.Models
{
    public class Resultados : IComparable<Resultados>
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
            DESCALIFICADO,
            CLASIFICADO,
            EMPATE,
            MENCION,
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
        public bool invitado;
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
#if OMISTATS
        private Resultados(MiembroDelegacion competidor, int contestantCount) : this()
        {
            usuario = competidor.claveUsuario;
            omi = competidor.olimpiada;
            clave = competidor.clave;
            estado = competidor.estado;
            publico = true;
            nivelInstitucion = competidor.nivelEscuela;
            añoEscolar = competidor.añoEscuela;
            lugar = contestantCount;

            persona = Persona.obtenerPersonaConClave(competidor.claveUsuario);
            nombreEstado = Estado.obtenerEstadoConClave(estado).nombre;
        }
#endif
        public int CompareTo(Resultados obj)
        {
            return lugar - obj.lugar;
        }

        private void llenarDatos(DataRow row, bool cargarObjetos, bool miembroDelegacionIncluido = false)
        {
            lugar =  DataRowParser.ToInt(row["lugar"]);
            usuario = DataRowParser.ToInt(row["concursante"]);
            omi = DataRowParser.ToString(row["olimpiada"]);
            tipoOlimpiada = DataRowParser.ToTipoOlimpiada(row["clase"]);
            clave = DataRowParser.ToString(row["clave"]);
            estado = DataRowParser.ToString(row["estado"]);
            for (int i = 0; i < 6; i++)
                dia1[i] = DataRowParser.ToFloat(row["puntosD1P" + (i + 1)]);
            totalDia1 = DataRowParser.ToFloat(row["puntosD1"]);
            for (int i = 0; i < 6; i++)
                dia2[i] = DataRowParser.ToFloat(row["puntosD2P" + (i + 1)]);
            totalDia2 = DataRowParser.ToFloat(row["puntosD2"]);
            total = DataRowParser.ToFloat(row["puntos"]);
            medalla = DataRowParser.ToTipoMedalla(row["medalla"]);
            publico = DataRowParser.ToBool(row["publico"]);
            ioi = DataRowParser.ToString(row["ioi"]);

#if OMISTATS
            if (cargarObjetos)
            {
                persona = Persona.obtenerPersonaConClave(usuario);
                if (!(clave.StartsWith(CLAVE_DESCONOCIDA) || clave.StartsWith(NOMBRE_FALTANTE)))
                {
                    if (miembroDelegacionIncluido)
                    {
                        nivelInstitucion = DataRowParser.ToNivelInstitucion(row["nivel"]);
                        añoEscolar = DataRowParser.ToInt(row["año"]);
                    }
                    else
                    {
                        List<MiembroDelegacion> mds = MiembroDelegacion.obtenerMiembrosConClave(omi, tipoOlimpiada, clave);
                        if (mds.Count > 0)
                        {
                            MiembroDelegacion md = mds[0];
                            escuela = Institucion.obtenerInstitucionConClave(md.claveEscuela);
                            nivelInstitucion = md.nivelEscuela;
                            añoEscolar = md.añoEscuela;
                        }
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
                        medalla = DataRowParser.ToTipoMedalla(datos[indice++].Trim().ToUpper());
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
                    if (ioi != "A" && ioi != "B" && ioi != "Y")
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

        private static List<Resultados> cargarResultados(string omi, TipoOlimpiada tipoOlimpiada, bool cargarObjetos, bool cargarCache, bool porLugar, int dia, int problemas, out List<CachedResult> cached)
        {
            List<Resultados> lista = new List<Resultados>();
            List<CachedResult> temp = null;

            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from resultados ");
            query.Append(" where clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            if (omi != null)
            {
                query.Append(" and olimpiada = ");
                query.Append(Cadenas.comillas(omi));
            }
            query.Append(" order by ");
            if (porLugar)
                query.Append(" lugar asc, ");
            query.Append(" puntos desc, clave asc");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            if (cargarCache)
                temp = new List<CachedResult>();

            foreach (DataRow r in table.Rows)
            {
                Resultados res = new Resultados();
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
        public static List<Resultados> cargarResultados(string omi, TipoOlimpiada tipoOlimpiada, bool cargarObjetos = false, bool porLugar = false)
        {
            List<CachedResult> cache;
            return cargarResultados(omi, tipoOlimpiada, cargarObjetos, /*cargarCache*/ false, porLugar, 0, 0, out cache);
        }
#if OMISTATS

        private static List<Resultados> cargarResultadosVacios(string clave, TipoOlimpiada tipo)
        {
            List<Resultados> lista = new List<Resultados>();
            List<MiembroDelegacion> competidores = MiembroDelegacion.obtenerMiembrosDelegacion(clave, null, tipo, MiembroDelegacion.TipoAsistente.COMPETIDOR);
            int count = competidores.Count;

            foreach (MiembroDelegacion competidor in competidores)
                lista.Add(new Resultados(competidor, count));

            return lista;
        }

        public static List<Resultados> cargarResultadosSecretos(string clave, TipoOlimpiada tipo, int dia)
        {
            List<Resultados> lista = null;

            if (dia == 1)
                lista = cargarResultadosVacios(clave, tipo);
            else
                lista = cargarResultados(clave, tipo, cargarObjetos: true);

            // Una vez con la lista normal, la modificamos con los datos de los detalles
            int timestamp = DetallePuntos.obtenerTimestampMasReciente(clave, tipo, dia);
            Dictionary<string, DetallePuntos> puntos = DetallePuntos.obtenerPuntosConTimestamp(clave, tipo, dia, timestamp);
            Dictionary<string, DetalleLugar> lugares = DetalleLugar.obtenerLugaresConTimestamp(clave, tipo, dia, timestamp);

            // Ya que tenemos esto, los unimos
            for (int i = 0; i < lista.Count; i++)
            {
                if (puntos.ContainsKey(lista[i].clave))
                {
                    List<float?> problemas = null;
                    if (dia == 1)
                        problemas = lista[i].dia1;
                    else
                        problemas = lista[i].dia2;
                    DetallePuntos dp = puntos[lista[i].clave];
                    for (int j = 0; j < 6; j++)
                        problemas[j] = dp.puntosProblemas[j];
                    if (dia == 1)
                        lista[i].totalDia1 = dp.puntosDia;
                    else
                        lista[i].totalDia2 = dp.puntosDia;
                }
                if (lugares.ContainsKey(lista[i].clave))
                {
                    DetalleLugar dl = lugares[lista[i].clave];
                    lista[i].total = lista[i].totalDia1 + lista[i].totalDia2;
                    lista[i].lugar = dl.lugar;
                    lista[i].medalla = dl.medalla;
                }
            }

            lista.Sort();
            return lista;
        }
#endif
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
            return cargarResultados(omi, tipoOlimpiada, true, true, true, dia, problemas, out cached);
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
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from resultados ");
            query.Append(" where clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" and clave = ");
            query.Append(Cadenas.comillas(clave));

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            Resultados res = new Resultados();
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
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" delete Resultados where olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and clave = ");
            query.Append(Cadenas.comillas(clave));

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
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" update Resultados set clave = ");
            query.Append(Cadenas.comillas(claveNueva));
            query.Append(" where olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and clave = ");
            query.Append(Cadenas.comillas(claveOriginal));

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
        /// <param name="dia">Si detalles es true, el dia es requerido</param>
        /// <param name="soloDetalles">Si detalles es true y soloDetalles tambien, se salta guardar en tabla resultados y solo guarda los detalles</param>
        /// </summary>
        public TipoError guardar(bool detalles = false, int timestamp = 0, int dia = 0, bool soloDetalles = false, bool expectErrors = false)
        {
            StringBuilder query = new StringBuilder();
            Acceso db = new Acceso();
            MiembroDelegacion m = null;

            if (detalles)
            {
                DetallePuntos detallePuntos = new DetallePuntos(omi, tipoOlimpiada, clave, timestamp, dia, dia == 1 ? dia1 : dia2);
                detallePuntos.guardar(expectErrors);
                DetalleLugar detalleLugar = new DetalleLugar(omi, tipoOlimpiada, clave, timestamp, dia, medalla, lugar);
                detalleLugar.guardar();

                if (soloDetalles)
                    return TipoError.OK;
            }

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
            query.Append(Cadenas.comillas(this.omi));
            query.Append(", ");
            query.Append(Cadenas.comillas(this.tipoOlimpiada.ToString().ToLower()));
            query.Append(", 0, ");
            query.Append(Cadenas.comillas(this.clave));
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
            query.Append(Cadenas.comillas(m == null ? this.estado : m.estado));
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
            query.Append(Cadenas.comillas(this.ioi));
            if (this.lugar > 0)
            {
                query.Append(", lugar = ");
                query.Append(this.lugar);
            }
            query.Append(" where olimpiada = ");
            query.Append(Cadenas.comillas(this.omi));
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(this.tipoOlimpiada.ToString().ToLower()));
            query.Append(" and clave = ");
            query.Append(Cadenas.comillas(this.clave));

            db.EjecutarQuery(query.ToString());

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
            Acceso db = new Acceso();

            query.Append(" select count(*) from resultados where olimpiada = ");
            query.Append(Cadenas.comillas(clave));
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
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
            Acceso db = new Acceso();

            query.Append(" select count(*) from resultados where olimpiada = ");
            query.Append(Cadenas.comillas(clave));
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and round(puntosD1 + puntosD2, 0) <> round(puntos, 0) ");

            db.EjecutarQuery(query.ToString());

            return ((int)db.getTable().Rows[0][0]) == 0;
        }
#if OMISTATS
        /// <summary>
        /// Regresa las participaciones en olimpiadas del usuario mandado como parametro
        /// </summary>
        /// <param name="persona">La persona de la que se quieren los datos</param>
        /// <param name="tipoOlimpiada">El tipo de olimpiada que se solicita</param>
        /// <returns>La lista de participaciones</returns>
        public static Dictionary<TipoOlimpiada, List<Resultados>> obtenerParticipacionesComoCompetidorPara(int persona, TipoOlimpiada tipoOlimpiada)
        {
            Dictionary<TipoOlimpiada, List<Resultados>> participaciones = new Dictionary<TipoOlimpiada, List<Resultados>>();

            participaciones.Add(TipoOlimpiada.OMI, obtenerParticipaciones(persona, TipoOlimpiada.OMI));
            participaciones.Add(TipoOlimpiada.OMIP, obtenerParticipaciones(persona, TipoOlimpiada.OMIP));
            participaciones.Add(TipoOlimpiada.OMIS, obtenerParticipaciones(persona, TipoOlimpiada.OMIS));
            participaciones.Add(TipoOlimpiada.OMIA, obtenerParticipaciones(persona, TipoOlimpiada.OMIA));

            return participaciones;
        }

        private static List<Resultados> obtenerParticipaciones(int persona, TipoOlimpiada tipoOlimpiada)
        {
            List<Resultados> lista = new List<Resultados>();
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select r.* from resultados  as r ");
            query.Append(" inner join Olimpiada as o on o.numero = r.olimpiada and o.clase = r.clase ");
            query.Append(" where (r.clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            if (tipoOlimpiada == TipoOlimpiada.OMIS || tipoOlimpiada == TipoOlimpiada.OMIP)
            {
                TipoOlimpiada pequeña = Olimpiada.getOlimpiadaPequeña(tipoOlimpiada);
                query.Append(" or (r.clase = ");
                query.Append(Cadenas.comillas(pequeña.ToString().ToLower()));
                query.Append(" and r.medalla <> ");
                query.Append((int)TipoMedalla.CLASIFICADO);
                query.Append(")");
            }
            query.Append(") and r.concursante = ");
            query.Append(persona);
            query.Append(" order by o.año asc");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                Resultados res = new Resultados();
                res.llenarDatos(r, cargarObjetos: true);

                lista.Add(res);
            }

            return lista;
        }
#endif
        /// <summary>
        /// Regresa los alumnos de la institucion mandada como parametro
        /// </summary>
        /// <param name="clave">La clave de la institución deseada</param>
        /// <param name="tipoOlimpiada">El tipo de olimpiada solicitado</param>
        /// <returns>La lista de alumnos</returns>
        public static List<Resultados> obtenerAlumnosDeInstitucion(int clave, TipoOlimpiada tipoOlimpiada)
        {
            List<Resultados> lista = new List<Resultados>();
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select r.*, md.institucion, md.nivel, md.año from Resultados as r ");
            query.Append(" inner join MiembroDelegacion as md on r.clave = md.clave ");
            query.Append(" and md.olimpiada = r.olimpiada ");
            query.Append(" and md.clase = r.clase ");
            query.Append(" where r.clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and md.institucion = ");
            query.Append(clave);
            query.Append(" order by r.medalla, r.concursante, md.olimpiada desc");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                Resultados res = new Resultados();
                res.llenarDatos(r, cargarObjetos: true, miembroDelegacionIncluido: true);

                lista.Add(res);
            }

            return ordenaMenciones(lista);
        }

        private static List<Resultados> ordenaMenciones(List<Resultados> lista)
        {
            var menciones = lista.FindAll((Resultados r) => r.medalla == TipoMedalla.MENCION);
            if (menciones.Count() == 0 || menciones.Count() == lista.Count())
                return lista;
            var sinMedalla = lista.FindAll((Resultados r) => ((int)r.medalla) >= ((int)TipoMedalla.NADA) && r.medalla != TipoMedalla.MENCION);
            if (sinMedalla.Count() == 0)
                return lista;
            var medallas = lista.FindAll((Resultados r) => ((int)r.medalla) < ((int)TipoMedalla.NADA));

            return medallas.Concat(menciones).Concat(sinMedalla).ToList();
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
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from Resultados ");
            query.Append(" where clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and estado = ");
            query.Append(Cadenas.comillas(clave));
            query.Append(" and clave not like '");
            query.Append(CLAVE_DESCONOCIDA);
            query.Append("%' ");
            query.Append(" order by medalla, concursante, olimpiada desc");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                Resultados res = new Resultados();
                res.llenarDatos(r, cargarObjetos: true);

                lista.Add(res);
            }

            return ordenaMenciones(lista);
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
            Acceso db = new Acceso();
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

            query.Append(" select count(*) from Resultados as r ");
            query.Append(" inner join Estado as e on r.estado = e.clave where olimpiada = ");
            query.Append(Cadenas.comillas(olimpiada));
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and e.ext = 0 ");
            query.Append(" and (");
            query.Append(columna);

            db.EjecutarQuery(query.ToString() + " = 0 or " + columna + " is null) and medalla <> " + (int)TipoMedalla.DESCALIFICADO);
            p.ceros = (int)db.getTable().Rows[0][0];

            if (totalProblemas > 0)
                perfecto *= totalProblemas;
            db.EjecutarQuery(query.ToString() + " = " + perfecto + ") and medalla <> " + (int)TipoMedalla.DESCALIFICADO);
            p.perfectos = (int)db.getTable().Rows[0][0];

            query.Clear();
            query.Append(" select sum(");
            query.Append(columna);
            query.Append(") from Resultados as r ");
            query.Append(" inner join Estado as e on r.estado = e.clave where olimpiada = ");
            query.Append(Cadenas.comillas(olimpiada));
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and medalla <> ");
            query.Append((int)TipoMedalla.DESCALIFICADO);
            query.Append(" and e.ext = 0 ");

            db.EjecutarQuery(query.ToString());
            suma = DataRowParser.ToStrictFloat(db.getTable().Rows[0][0]);

            query.Clear();
            query.Append(" select ");
            query.Append(columna);
            query.Append(" from Resultados as r ");
            query.Append(" inner join Estado as e on r.estado = e.clave where olimpiada = ");
            query.Append(Cadenas.comillas(olimpiada));
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and medalla <> ");
            query.Append((int)TipoMedalla.DESCALIFICADO);
            query.Append(" and e.ext = 0 ");
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
                p.mediana = DataRowParser.ToStrictFloat(table.Rows[mitad][0]);

                if (total % 2 == 0)
                    p.mediana = (p.mediana + DataRowParser.ToStrictFloat(table.Rows[mitad - 1][0])) / 2;
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
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" update Resultados set lugar = ");
            query.Append(lugar);
            query.Append(" where olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and clave = ");
            query.Append(Cadenas.comillas(clave));

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
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select count(distinct(Estado)) from Resultados as r ");
            query.Append(" inner join Estado as e on r.estado = e.clave where olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" and clase = ");
            query.Append(Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and e.ext = 0 ");

            db.EjecutarQuery(query.ToString());

            return (int)db.getTable().Rows[0][0];
        }

        private static int countMejores(string columna, float? puntos, string omi, string tipo)
        {
            Acceso db = new Acceso();
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
        /// Regresa si ya hay resultados grabados en la base de datos para la omi
        /// </summary>
        public static bool hayResultadosParaOMI(string omi, TipoOlimpiada tipoOlimpiada)
        {
            Acceso db = new Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select COUNT(*) from Resultados where ");
            query.Append(" olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            if (tipoOlimpiada != TipoOlimpiada.OMIPO && tipoOlimpiada != TipoOlimpiada.OMISO)
            {
                query.Append(" and clase <> ");
                query.Append(Cadenas.comillas(TipoOlimpiada.OMIPO.ToString().ToLower()));
                query.Append(" and clase <> ");
                query.Append(Cadenas.comillas(TipoOlimpiada.OMISO.ToString().ToLower()));
            }

            db.EjecutarQuery(query.ToString());
            return (int)db.getTable().Rows[0][0] > 0;
        }

        /// <summary>
        /// Regresa una lista con el lugar del competidor mandado como parametro para cada problema y cada dia
        /// </summary>
        public static List<int> cargarMejores(string omi, TipoOlimpiada tipo, string clave, int problemasDia1, int problemasDia2)
        {
            List<int> mejores = new List<int>();
            Resultados res = Resultados.cargarResultados(omi, tipo, clave);
            string omiString = Cadenas.comillas(omi);
            string tipoString = Cadenas.comillas(tipo.ToString().ToLower());

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
#if OMISTATS
        public static string generarDiplomas(string omi, string X, string baseURL, string Z, bool isNaked = false)
        {
            StringBuilder lineas = new StringBuilder();
            StringBuilder query = new StringBuilder();
            Acceso db = new Acceso();

            query.Append(" select p.clave as persona, p.nombre, p.apellidoP, p.apellidoM, r.clave,r.clase, r.medalla, r.estado, md.tipo from Resultados as r ");
            query.Append(" inner join Persona as p on p.clave = r.concursante ");
            query.Append(" inner join MiembroDelegacion as md on md.clave = r.clave and md.olimpiada = r.olimpiada and r.clase = md.clase ");
            query.Append(" where r.olimpiada = ");
            query.Append(Cadenas.comillas(omi));
            query.Append(" and (medalla < 7 or medalla = ");
            query.Append((int)TipoMedalla.MENCION);
            query.Append(")");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                int claveUsuario = DataRowParser.ToInt(r["persona"]);
                string nombre = DataRowParser.ToString(r["nombre"]) + " " +
                                DataRowParser.ToString(r["apellidoP"]) + " " +
                                DataRowParser.ToString(r["apellidoM"]);
                string clave = DataRowParser.ToString(r["clave"]);
                string estado = DataRowParser.ToString(r["estado"]);
                TipoOlimpiada clase = DataRowParser.ToTipoOlimpiada(r["clase"]);
                TipoMedalla medalla = DataRowParser.ToTipoMedalla(r["medalla"]);
                MiembroDelegacion.TipoAsistente asistente = DataRowParser.ToTipoAsistente(r["tipo"]);

                if (Olimpiada.esOMIPOS(clase))
                    if (medalla != TipoMedalla.MENCION)
                        continue;

                if (isNaked)
                {
                    lineas.Append(medalla.ToString());
                }
                else
                {
                    lineas.Append(estado);
                }
                lineas.Append("\\");
                if (clase == TipoOlimpiada.OMIS || clase == TipoOlimpiada.OMISO)
                    lineas.Append("S-");
                lineas.Append(clave);
                if (medalla == TipoMedalla.MENCION)
                    lineas.Append("-mencion.pdf,");
                else
                    lineas.Append("-medalla.pdf,");
                lineas.Append(nombre);
                lineas.Append(",");

                Estado e = Estado.obtenerEstadoConClave(estado);
                string medallaStr;
                if (medalla == TipoMedalla.ORO_1 || medalla == TipoMedalla.ORO_2 || medalla == TipoMedalla.ORO_3)
                    medallaStr = TipoMedalla.ORO.ToString();
                else if (medalla == TipoMedalla.MENCION)
                    medallaStr = "MENCIÓN HONORÍFICA";
                else
                    medallaStr = medalla.ToString();
                string prefijoM = "";
                if ((e.extranjero || asistente == MiembroDelegacion.TipoAsistente.DELEB) && medalla != TipoMedalla.MENCION)
                    prefijoM = "Puntaje para ";

                lineas.Append(Cadenas.reemplazaValoresDiploma(X, medallaStr, e.nombre, e.clave, clase.ToString(), null, prefijoM));
                lineas.Append(",");
                lineas.Append(Cadenas.reemplazaValoresDiploma(Z, medallaStr, e.nombre, e.clave, clase.ToString(), null, prefijoM));
                lineas.Append(",");

                lineas.Append(clase.ToString());
                lineas.Append(",");
                lineas.Append(medalla == TipoMedalla.MENCION ? medalla.ToString().ToLower() : medallaStr.ToLower());
                lineas.Append(",");
                lineas.Append(baseURL);
                lineas.Append("/Profile/");
                lineas.Append(clase.ToString());
                lineas.Append("/");
                lineas.Append(omi);
                lineas.Append("/");
                lineas.Append(clave);

                lineas.Append("\n");
            }

            return lineas.ToString();
        }
        public static string generarDiplomasEspeciales(string omi, string baseURL)
        {
            StringBuilder lineas = new StringBuilder();
            StringBuilder query = new StringBuilder();
            Acceso db = new Acceso();

            // El primer diploma especial es el medallista más joven
            var resultados = Resultados.cargarResultados(omi, TipoOlimpiada.OMI, cargarObjetos: true);
            if (resultados.Count == 0)
                return "";
            Resultados joven = resultados[0];

            foreach (Resultados resultado in resultados)
            {
                if (resultado.medalla != TipoMedalla.NADA &&
                    resultado.medalla != TipoMedalla.MENCION &&
                    resultado.persona.nacimiento > joven.persona.nacimiento)
                {
                    joven = resultado;
                }
            }
            Estado e = Estado.obtenerEstadoConClave(joven.estado);

            lineas.Append(joven.estado);
            lineas.Append("\\");
            lineas.Append(joven.clave);
            lineas.Append("-joven.pdf,");
            lineas.Append(joven.persona.nombreCompleto);
            lineas.Append(",por haber sido,EL MEDALLISTA MÁS JOVEN a,");
            lineas.Append(joven.tipoOlimpiada.ToString());
            lineas.Append(",reconocimiento,");

            lineas.Append(baseURL);
            lineas.Append("/Profile/");
            lineas.Append(joven.tipoOlimpiada.ToString());
            lineas.Append("/");
            lineas.Append(omi);
            lineas.Append("/");
            lineas.Append(joven.clave);

            lineas.Append("\n");

            // Los siguientes diplomas, son los diplomas de estados

            Medallero medalleroGral = null;
            List<Medallero> medallero = Medallero.obtenerTablaEstados(TipoOlimpiada.OMI, omi, out medalleroGral);
            int lugar = 0;

            for (int i = 0; lugar < 3 && i < medallero.Count; i++)
            {
                Estado estado = Estado.obtenerEstadoConClave(medallero[i].clave);
                if (estado.extranjero)
                    continue;
                lineas.Append(estado.clave);
                lineas.Append("\\");
                lineas.Append(estado.nombre);
                lineas.Append(".pdf,");
                lineas.Append(estado.nombre.ToUpperInvariant());
                lineas.Append(",Por ser ");
                switch (++lugar)
                {
                    case 1:
                        lineas.Append("PRIMER");
                        break;
                    case 2:
                        lineas.Append("SEGUNDO");
                        break;
                    case 3:
                        lineas.Append("TERCER");
                        break;
                }
                lineas.Append(" LUGAR, a Nivel Estados ");
                lineas.Append(TableManager.getPreEstado(estado.clave));
                lineas.Append(",OMI,");
                switch (lugar)
                {
                    case 1:
                        lineas.Append("oro");
                        break;
                    case 2:
                        lineas.Append("plata");
                        break;
                    case 3:
                        lineas.Append("bronce");
                        break;
                }
                lineas.Append("\n");
            }

            return lineas.ToString();
        }

        public static List<Resultados> obtenerLugaresParaOMIPS(Olimpiada omi)
        {
            List<Olimpiada> omis = Olimpiada.obtenerOlimpiadas(TipoOlimpiada.OMI);
            List<Resultados> lista = new List<Resultados>();
            Olimpiada omi2 = null, omi3 = null;
            foreach (var o in omis)
            {
                if (o.año == omi.año - 1)
                    omi2 = o;
                if (o.año == omi.año - 2)
                    omi3 = o;
                if (omi2 != null && omi3 != null)
                    break;
            }

            StringBuilder query = new StringBuilder();
            Acceso db = new Acceso();

            query.Append(" select * from Resultados where ( ");
            query.Append(" olimpiada = ");
            query.Append(Cadenas.comillas(omi.numero));
            query.Append(" or olimpiada = ");
            query.Append(Cadenas.comillas(omi2.numero));
            query.Append(" or olimpiada = ");
            query.Append(Cadenas.comillas(omi3.numero));
            query.Append(" ) and (clase = ");
            query.Append(Cadenas.comillas(TipoOlimpiada.OMIP.ToString().ToLower()));
            query.Append(" or clase = ");
            query.Append(Cadenas.comillas(TipoOlimpiada.OMIPO.ToString().ToLower()));
            query.Append(" or clase = ");
            query.Append(Cadenas.comillas(TipoOlimpiada.OMIS.ToString().ToLower()));
            query.Append(" or clase = ");
            query.Append(Cadenas.comillas(TipoOlimpiada.OMISO.ToString().ToLower()));
            query.Append(" ) order by estado, clase, olimpiada, concursante ");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                Resultados res = new Resultados();
                res.llenarDatos(r, cargarObjetos: false);

                lista.Add(res);
            }
            return lista;
        }
#endif
    }
}