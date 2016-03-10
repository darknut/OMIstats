using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

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

        public const string CLAVE_DESCONOCIDA = "UNK";
        public const string CLAVE_FALTANTE = "???";

        public string omi;
        public Olimpiada.TipoOlimpiada tipoOlimpiada;
        public int usuario;
        public string estado;
        public string clave;
        public List<int> dia1;
        public int totalDia1;
        public List<int> dia2;
        public int totalDia2;
        public int total;
        public TipoMedalla medalla;
        public bool publico;
        public string ioi;

        public Persona persona;
        public Institucion escuela;
        public string nombreEstado;

        private bool eliminar;

        public Resultados()
        {
            omi = "";
            tipoOlimpiada = Olimpiada.TipoOlimpiada.NULL;

            usuario = 0;
            estado = "";
            clave = "";
            totalDia1 = 0;
            totalDia2 = 0;
            total = 0;
            medalla = TipoMedalla.NULL;
            publico = false;
            ioi = "";

            persona = null;
            escuela = null;

            dia1 = new List<int>();
            dia1.Add(0);
            dia1.Add(0);
            dia1.Add(0);
            dia1.Add(0);
            dia1.Add(0);
            dia1.Add(0);

            dia2 = new List<int>();
            dia2.Add(0);
            dia2.Add(0);
            dia2.Add(0);
            dia2.Add(0);
            dia2.Add(0);
            dia2.Add(0);
        }

        private void llenarDatos(DataRow row, bool cargarObjetos)
        {
            usuario = (int)row["concursante"];
            omi = row["olimpiada"].ToString().Trim();
            clave = row["clave"].ToString().Trim();
            estado = row["estado"].ToString().Trim();
            dia1[0] = (int)row["puntosD1P1"];
            dia1[1] = (int)row["puntosD1P2"];
            dia1[2] = (int)row["puntosD1P3"];
            dia1[3] = (int)row["puntosD1P4"];
            dia1[4] = (int)row["puntosD1P5"];
            dia1[5] = (int)row["puntosD1P6"];
            totalDia1 = (int)row["puntosD1"];
            dia2[0] = (int)row["puntosD2P1"];
            dia2[1] = (int)row["puntosD2P2"];
            dia2[2] = (int)row["puntosD2P3"];
            dia2[3] = (int)row["puntosD2P4"];
            dia2[4] = (int)row["puntosD2P5"];
            dia2[5] = (int)row["puntosD2P6"];
            totalDia2 = (int)row["puntosD2"];
            total = (int)row["puntos"];
            medalla = (TipoMedalla)Enum.Parse(typeof(TipoMedalla), row["medalla"].ToString().ToUpper());
            publico = (bool)row["publico"];
            ioi = row["ioi"].ToString().Trim();

            if (cargarObjetos)
            {
                persona = Persona.obtenerPersonaConClave(usuario);
                if (!clave.StartsWith(CLAVE_DESCONOCIDA))
                {
                    escuela = Institucion.obtenerInstitucionConNombreCorto(
                        MiembroDelegacion.obtenerMiembrosConClave(omi, tipoOlimpiada, clave)[0].nombreEscuela);
                }
                nombreEstado = Estado.obtenerEstadoConClave(estado).nombre;
            }
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
                        dia1[i] = Int32.Parse(datos[indice++]);
                }
                if (datos.Length > indice)
                    totalDia1 = Int32.Parse(datos[indice++]);
                for (int i = 0; i < problemasDia2; i++)
                {
                    if (datos.Length > indice)
                        dia2[i] = Int32.Parse(datos[indice++]);
                }
                if (datos.Length > indice)
                    totalDia2 = Int32.Parse(datos[indice++]);
                if (datos.Length > indice)
                    total = Int32.Parse(datos[indice++]);
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
                ioi = datos[indice++].Trim();
                if (ioi.Length > 0)
                {
                    ioi = ioi.Substring(0, 1);
                    if (ioi != "A" && ioi != "B")
                        return TipoError.EQUIPO_IOI_INCORRECTO;
                }
            }

            if (datos.Length > indice)
                eliminar = datos[indice].Trim().Equals("eliminar", StringComparison.InvariantCultureIgnoreCase);

            return TipoError.OK;
        }

        /// <summary>
        /// Regresa los resultados de la olimpiada mandada como parametro
        /// </summary>
        /// <param name="omi">La olimpiada en cuestión</param>
        /// <param name="tipoOlimpiada">El tipo de olimpiada</param>
        /// <param name="cargarObjetos">Si los objetos deben de llenarse</param>
        /// <returns>Una lista con los resultados</returns>
        public static List<Resultados> cargarResultados(string omi, Olimpiada.TipoOlimpiada tipoOlimpiada, bool cargarObjetos = false)
        {
            List<Resultados> lista = new List<Resultados>();
            if (omi == null)
                return null;

            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from resultados ");
            query.Append(" where olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(omi));
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" order by puntos desc, clave asc");

            db.EjecutarQuery(query.ToString());
            DataTable table = db.getTable();

            foreach (DataRow r in table.Rows)
            {
                Resultados res = new Resultados();
                res.tipoOlimpiada = tipoOlimpiada;
                res.llenarDatos(r, cargarObjetos);

                lista.Add(res);
            }

            return lista;
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
                s.Append(dia1[i]);
                s.Append(", ");
            }

            s.Append(totalDia1);
            s.Append(", ");

            for (int i = 0; i < problemasDia2; i++)
            {
                s.Append(dia2[i]);
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
        public static void eliminarResultado(string omi, Olimpiada.TipoOlimpiada tipoOlimpiada, string clave)
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
        public static bool cambiarClave(string omi, Olimpiada.TipoOlimpiada tipoOlimpiada, string claveOriginal, string claveNueva)
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

        /// <summary>
        /// Guarda la linea mandada como parametro en la base de datos
        /// </summary>
        /// <param name="omi">La clave de la olimpiada</param>
        /// <param name="tipoOlimpiada">El tipo de olimpiada a los que los datos pertenecen</param>
        /// <param name="problemasDia1">El número de problemas a desplegar el día 1</param>
        /// <param name="problemasDia2">El número de problemas a desplegar el día 2</param>
        /// <param name="linea">Los datos tabulados por comas</param>
        /// <returns>Si hubo un error, lo devuelve</returns>
        public static TipoError guardarLineaAdmin(string omi, Olimpiada.TipoOlimpiada tipoOlimpiada, int problemasDia1, int problemasDia2, string linea)
        {
            if (linea.Trim().Length == 0)
                return TipoError.OK;

            StringBuilder query = new StringBuilder();
            Utilities.Acceso db = new Utilities.Acceso();
            Resultados res = new Resultados();
            MiembroDelegacion m = null;

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

            // Revisamos si hay mas de un usuario con esa clave

            if (res.clave.StartsWith(CLAVE_DESCONOCIDA) || res.clave.StartsWith(CLAVE_FALTANTE))
            {
                Estado e = Estado.obtenerEstadoConClave(res.estado);
                if (e == null)
                    return TipoError.ESTADO_INEXISTENTE;
            }

            if (!res.clave.StartsWith(CLAVE_DESCONOCIDA))
            {
                List<MiembroDelegacion> lista = MiembroDelegacion.obtenerMiembrosConClave(omi, tipoOlimpiada, res.clave);
                if (lista.Count == 0)
                    return TipoError.CLAVE_INEXISTENTE;
                if (lista.Count != 1)
                    return TipoError.CLAVE_DUPLICADA;
                m = lista[0];
            }

            // Ya tenemos todos los datos necesarios, hacemos un insert con las claves

            query.Append("insert into Resultados values(");
            query.Append(Utilities.Cadenas.comillas(omi));
            query.Append(", ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(", 0, ");
            query.Append(Utilities.Cadenas.comillas(res.clave));
            query.Append(", '', 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,");
            query.Append((int)TipoMedalla.NADA);
            query.Append(", 0, '')");

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
            query.Append(Utilities.Cadenas.comillas(m == null ? res.estado : m.estado));
            query.Append(", puntosD1P1 = ");
            query.Append(res.dia1[0]);
            query.Append(", puntosD1P2 = ");
            query.Append(res.dia1[1]);
            query.Append(", puntosD1P3 = ");
            query.Append(res.dia1[2]);
            query.Append(", puntosD1P4 = ");
            query.Append(res.dia1[3]);
            query.Append(", puntosD1P5 = ");
            query.Append(res.dia1[4]);
            query.Append(", puntosD1P6 = ");
            query.Append(res.dia1[5]);
            query.Append(", puntosD1 = ");
            query.Append(res.totalDia1);
            query.Append(", puntosD2P1 = ");
            query.Append(res.dia2[0]);
            query.Append(", puntosD2P2 = ");
            query.Append(res.dia2[1]);
            query.Append(", puntosD2P3 = ");
            query.Append(res.dia2[2]);
            query.Append(", puntosD2P4 = ");
            query.Append(res.dia2[3]);
            query.Append(", puntosD2P5 = ");
            query.Append(res.dia2[4]);
            query.Append(", puntosD2P6 = ");
            query.Append(res.dia2[5]);
            query.Append(", puntosD2 = ");
            query.Append(res.totalDia2);
            query.Append(", puntos = ");
            query.Append(res.total);
            query.Append(", medalla = ");
            query.Append((int)res.medalla);
            query.Append(", ioi = ");
            query.Append(Utilities.Cadenas.comillas(res.ioi));
            query.Append(" where olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(omi));
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and clave = ");
            query.Append(Utilities.Cadenas.comillas(res.clave));

            db.EjecutarQuery(query.ToString());

            return TipoError.OK;
        }

        /// <summary>
        /// Determina si se deben de desplegar los resultados de los problemas individuales
        /// </summary>
        /// <param name="clave">La clave de la olimpiada</param>
        /// <param name="tipoOlimpiada">El tipo de olimpiada</param>
        /// <returns>Si se deben desplegar los resultados o no</returns>
        public static bool mostrarResultadosIndividuales(string clave, Olimpiada.TipoOlimpiada tipoOlimpiada)
        {
            StringBuilder query = new StringBuilder();
            Utilities.Acceso db = new Utilities.Acceso();

            query.Append(" select count(*) from resultados where olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(clave));
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and ((puntosD1P1 + puntosD1P2 + puntosD1P3 + puntosD1P4 + puntosD1P5 + puntosD1P6) <> puntosD1 ");
            query.Append(" or (puntosD2P1 + puntosD2P2 + puntosD2P3 + puntosD2P4 + puntosD2P5 + puntosD2P6) <> puntosD2)");

            db.EjecutarQuery(query.ToString());

            return ((int)db.getTable().Rows[0][0]) == 0;
        }

        /// <summary>
        /// Determina si se deben de desplegar los resultados de los dias individuales
        /// </summary>
        /// <param name="clave">La clave de la olimpiada</param>
        /// <param name="tipoOlimpiada">El tipo de olimpiada</param>
        /// <returns>Si se deben desplegar los resultados o no</returns>
        public static bool mostrarResultadosPorDia(string clave, Olimpiada.TipoOlimpiada tipoOlimpiada)
        {
            StringBuilder query = new StringBuilder();
            Utilities.Acceso db = new Utilities.Acceso();

            query.Append(" select count(*) from resultados where olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(clave));
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and (puntosD1 + puntosD2) <> puntos ");

            db.EjecutarQuery(query.ToString());

            return ((int)db.getTable().Rows[0][0]) == 0;
        }

        /// <summary>
        /// Regresa las participaciones en olimpiadas del usuario mandado como parametro
        /// </summary>
        /// <param name="persona">La persona de la que se quieren los datos</param>
        /// <param name="tipoOlimpiada">El tipo de olimpiada que se solicita</param>
        /// <returns>La lista de participaciones</returns>
        public static List<Resultados> obtenerParticipacionesComoCompetidorPara(int persona, Olimpiada.TipoOlimpiada tipoOlimpiada)
        {
            List<Resultados> lista = new List<Resultados>();
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from resultados ");
            query.Append(" where clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and concursante = ");
            query.Append(persona);
            query.Append(" order by olimpiada asc");

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
        public static List<Resultados> obtenerAlumnosDeInstitucion(int clave, Olimpiada.TipoOlimpiada tipoOlimpiada)
        {
            List<Resultados> lista = new List<Resultados>();
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select r.* from Resultados as r ");
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
                res.llenarDatos(r, cargarObjetos: true);

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
        public static List<Resultados> obtenerAlumnosDeEstado(string clave, Olimpiada.TipoOlimpiada tipoOlimpiada)
        {
            List<Resultados> lista = new List<Resultados>();
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" select * from Resultados ");
            query.Append(" where clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and estado = ");
            query.Append(Utilities.Cadenas.comillas(clave));
            query.Append(" and clave not like 'UNK%' ");
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
        public static Problema calcularNumeros(string olimpiada, Olimpiada.TipoOlimpiada tipoOlimpiada, int dia = 0, int numero = 0, int totalProblemas = 0)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();
            Problema p = new Problema();
            string columna = "puntos";
            DataTable table = null;
            int total, suma, mitad, perfecto = 100;

            if (dia > 0)
            {
                columna += "D" + dia;
                if (numero > 0)
                    columna += "P" + numero;
            }

            query.Append(" select count(");
            query.Append(columna);
            query.Append(") from Resultados where olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(olimpiada));
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));
            query.Append(" and ");
            query.Append(columna);

            db.EjecutarQuery(query.ToString() + " = 0");
            p.ceros = (int)db.getTable().Rows[0][0];

            if (totalProblemas > 0)
                perfecto *= totalProblemas;
            db.EjecutarQuery(query.ToString() + " = " + perfecto);
            p.perfectos = (int)db.getTable().Rows[0][0];

            query.Clear();
            query.Append(" select sum(");
            query.Append(columna);
            query.Append(") from Resultados where olimpiada = ");
            query.Append(Utilities.Cadenas.comillas(olimpiada));
            query.Append(" and clase = ");
            query.Append(Utilities.Cadenas.comillas(tipoOlimpiada.ToString().ToLower()));

            db.EjecutarQuery(query.ToString());
            suma = (int)db.getTable().Rows[0][0];

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
                p.media = suma * 1f / total;
                p.media = (float) Math.Round((Decimal)p.media, 2, MidpointRounding.AwayFromZero);
                mitad = total / 2;
                p.mediana = (int)table.Rows[mitad][0];

                if (total % 2 == 0)
                    p.mediana = (p.mediana + (int)table.Rows[mitad + 1][0]) / 2;
            }

            return p;
        }
    }
}