using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace OMIstats.Utilities
{
    public class Correo
    {
        /// <summary>
        /// Manda un correo electronico
        /// </summary>
        /// <returns>Si el correo se mandó satisfactoriamente o no</returns>
        public static bool mandarCorreo(string destinatario, string asunto, string mensaje)
        {
            MailMessage mail = new MailMessage();

            mail.To.Add(destinatario);
            mail.From = new MailAddress("omistatstest@outlook.com", "OMI stats test");
            mail.Subject = asunto;
            mail.Body = mensaje;
            mail.IsBodyHtml = false; // -TODO- Poner el mesaje en un formato bonito

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
    }
}