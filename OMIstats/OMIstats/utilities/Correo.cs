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
            mail.From = new MailAddress("omistatstest@outlook.com", "OMI stats test");
            mail.Subject = asunto;
            mail.Body = mensaje;
            mail.IsBodyHtml = true;

            SmtpClient smtpMail = new SmtpClient();

            smtpMail.Host = "smtp.live.com";
            smtpMail.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpMail.Credentials = new NetworkCredential("omistatstest@outlook.com", "");
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
            string template = Archivos.leerArchivoHTML(Archivos.ArchivosHTML.PASSWORD);
            string server = direccionServer();
            return mandarCorreo(correo, "Solicitud de cambio de contraseña",
                String.Format(template, server, clave, guid));
        }
    }
}