﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace OMIstats.Models
{   
    /// <summary>
    /// Clase para comunicarse con la base de datos de la OMI
    /// </summary>
    public class Usuario
    {
        public long Id { get; set; }

        public string Email { get; set; }

        public string Foto { get; set; }

        public string Nombre { get; set; }

        public string CURP { get; set; }

        public Usuario()
        {
            Id = 0;
            Email = "";
            Foto = "";
            Nombre = "";
            CURP = "";
        }

        /// <summary>
        /// Llena los datos de un usuario de la fila mandada como parametro
        /// </summary>
        /// <param name="datos">La fila con el origen de los datos</param>
        private void llenarDatos(DataRow datos)
        {
            Id = (long)datos["Id"];
            Email = datos["Email"].ToString().Trim();
            Foto = datos["Foto"].ToString().Trim();
            Nombre = datos["Nombre"].ToString().Trim();

            try
            {
                // El CURP aun no está en la base de la OMI
                CURP = datos["CURP"].ToString().Trim();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Loggea al usuario mandado como parámetro, como si la página de la OMI lo hubiera hecho
        /// </summary>
        /// <param name="userId">El usuario que se quiere loggear</param>
        /// <returns>El guid que se guardó en la base de datos</returns>
        public static string MockUserLoggedIn(int userId)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();
            string guid = Guid.NewGuid().ToString();

            query.Append(" update [Usuarios.Usuarios] set GUID = ");
            query.Append(Utilities.Cadenas.comillas(guid));
            query.Append(" where Id = ");
            query.Append(userId);

            db.EjecutarQuery(query.ToString(), Utilities.Acceso.BaseDeDatos.OMI);
            return guid;
        }

        /// <summary>
        /// Borra el GUID del usuario en la base de datos
        /// </summary>
        public void borrarGUID()
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();

            query.Append(" update [Usuarios.Usuarios] set GUID = ''");
            query.Append(" where Id = ");
            query.Append(Id);

            db.EjecutarQuery(query.ToString(), Utilities.Acceso.BaseDeDatos.OMI);
        }

        /// <summary>
        /// Regresa el usuario en la base de datos con el GUID mandado como parámetro
        /// </summary>
        /// <param name="guid">El guid buscado</param>
        /// <returns>Un objeto Usuario con los datos en la base de datos</returns>
        public static Usuario obtenerUsuarioConGUID(string guid)
        {
            Utilities.Acceso db = new Utilities.Acceso();
            StringBuilder query = new StringBuilder();
            Usuario usuario = new Usuario();

            query.Append(" select * from [Usuarios.Usuarios] where GUID = ");
            query.Append(Utilities.Cadenas.comillas(guid));

            db.EjecutarQuery(query.ToString(), Utilities.Acceso.BaseDeDatos.OMI);

            DataTable table = db.getTable();

            if (table.Rows.Count == 0)
                return null;

            usuario.llenarDatos(table.Rows[0]);

            return usuario;
        }
    }
}