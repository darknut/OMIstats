using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace OMIstats.Utilities
{
    public class Acceso
    {
        public static string CADENA_CONEXION;
        public static string CADENA_CONEXION_OMI;
        private SqlConnection conexion = new SqlConnection();
        private SqlDataAdapter adapter = new SqlDataAdapter();
        private DataSet dataset = new DataSet();

        public class Estatus
        {
            public bool error = false;
            public string descripcion = "";
        }

        public enum BaseDeDatos
        {
            OMIStats,
            OMI
        }

        /// <summary>
        /// Regresa la tabla obtenida despues de ejecutar un query
        /// </summary>
        /// <returns></returns>
        public DataTable getTable()
        {
            if (dataset == null || dataset.Tables.Count == 0)
                return null;
            return dataset.Tables[0];
        }

        private Estatus Conectar(BaseDeDatos db)
        {
            Estatus resultado = new Estatus();
            try
            {
                conexion.ConnectionString = db == BaseDeDatos.OMIStats ? CADENA_CONEXION : CADENA_CONEXION_OMI;
                conexion.Open();
            }
            catch(Exception e)
            {
                resultado.error = true;
                resultado.descripcion = e.Message;
            }

            return resultado;
        }

        private Estatus Desconectar()
        {
            Estatus resultado = new Estatus();
            try
            {
                if (conexion.State == ConnectionState.Open)
                    conexion.Close();
            }
            catch (Exception e)
            {
                resultado.error = true;
                resultado.descripcion = e.Message;
            }

            return resultado;
        }

        /// <summary>
        /// Ejecuta un query.
        /// Si el query es un select, llamar a getTable devolvera la tabla consultada
        /// </summary>
        /// <param name="query"></param>
        public Estatus EjecutarQuery(string query, BaseDeDatos db = BaseDeDatos.OMIStats, bool expectErrors = false)
        {
            Estatus resultado = Conectar(db);
            if (resultado.error)
                return resultado;

            query = query.Trim();

            try
            {
                if (query.StartsWith("update"))
                {
                    adapter.UpdateCommand = new SqlCommand(query, conexion);
                    adapter.UpdateCommand.ExecuteNonQuery();
                }
                else if (query.StartsWith("delete"))
                {
                    adapter.DeleteCommand = new SqlCommand(query, conexion);
                    adapter.DeleteCommand.ExecuteNonQuery();
                }
                else if (query.StartsWith("insert"))
                {
                    adapter.InsertCommand = new SqlCommand(query, conexion);
                    adapter.InsertCommand.ExecuteNonQuery();
                }
                else if (query.StartsWith("select") || query.StartsWith("declare"))
                {
                    dataset.Clear();
                    dataset.Tables.Clear();
                    adapter.SelectCommand = new SqlCommand(query, conexion);
                    adapter.Fill(dataset);
                }
                else
                {
                    resultado.error = true;
                    resultado.descripcion = "Comando inválido";
                    return resultado;
                }
            }
            catch (Exception e)
            {
                if (!expectErrors)
                    Models.Log.add(Models.Log.TipoLog.DATABASE, e.ToString());
                resultado.error = true;
                resultado.descripcion = e.Message;
                try
                {
                    Desconectar();
                }
                catch (Exception)
                {
                }
                return resultado;
            }

            resultado = Desconectar();
            return resultado;
        }

    }
}