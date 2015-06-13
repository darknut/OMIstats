using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace OMIstats.Utilities
{
    public class Correo
    {
        public static string CORREO;
        public static string PASSWORD;

        public const string TITULO_CORREO_PASSWORD = "Solicitud de cambio de contraseña";
        public const string TITULO_CORREO_BIENVENIDO = "Bienvenido a OMI stats";

        private static string direccionServer()
        {
            HttpRequest request = HttpContext.Current.Request;
            string urlBase = string.Format("{0}://{1}{2}",
                request.Url.Scheme,
                request.Url.Authority,
                (new System.Web.Mvc.UrlHelper(request.RequestContext)).Content("~"));
            return urlBase;
        }

        /// <summary>
        /// Manda un correo electronico
        /// </summary>
        /// <returns>Si el correo se mandó satisfactoriamente o no</returns>
        private static bool mandarCorreo(string destinatario, string asunto, string mensaje)
        {
            MailMessage mail = new MailMessage();

            mail.To.Add(destinatario);
            mail.From = new MailAddress(CORREO, "OMI stats");
            mail.Subject = asunto;
            mail.Body = mensaje;
            mail.IsBodyHtml = true;

            SmtpClient smtpMail = new SmtpClient();

            smtpMail.Host = "smtp.live.com";
            smtpMail.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpMail.Credentials = new NetworkCredential(CORREO, PASSWORD);
            smtpMail.EnableSsl = true;
            smtpMail.Port = 587;
            smtpMail.Timeout = 10000;
            try
            {
                smtpMail.Send(mail);
            }
            catch (Exception)
            {
                smtpMail.Port = 25;
                try
                {
                    smtpMail.Send(mail);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Manda un correo para recuperar contraseña
        /// </summary>
        public static bool enviarPeticionPassword(int clave, string guid, string correo)
        {
            return enviarCorreoPeticion(clave, guid, correo, Archivos.ArchivosHTML.PASSWORD, TITULO_CORREO_PASSWORD);
        }

        /// <summary>
        /// Manda un correo de bienvenida al usuario
        /// </summary>
        public static bool enviarPeticionBienvenido(int clave, string guid, string correo)
        {
            return enviarCorreoPeticion(clave, guid, correo, Archivos.ArchivosHTML.BIENVENIDO, TITULO_CORREO_BIENVENIDO);
        }

        private static bool enviarCorreoPeticion(int clave, string guid, string correo, Archivos.ArchivosHTML archivo, string titulo)
        {
            string template = Archivos.leerArchivoHTML(archivo);
            string server = direccionServer();
            return mandarCorreo(correo, titulo, String.Format(template, server, clave, guid));
        }
    }
}