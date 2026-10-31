using System.Net;
using System.Net.Mail;

namespace Capa_Negocio
{
    public static class ServicioCorreo
    {
        private const string Remitente = "saludvital283@gmail.com";  // verifica que sea tu dirección completa
        private const string ClaveApp = "vfye qbmb gflv fylp";        // la contraseña de aplicación

        public static void EnviarRecordatorio(string destino, string nombreMedico, string mensaje)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(Remitente, "SaludVital - Sistema de Gestión");
                mail.To.Add(new MailAddress(destino));
                mail.Subject = "Recordatorio de cita - SaludVital";
                mail.Body = mensaje + "\n\nAtentamente,\n" + nombreMedico + "\nSaludVital";
                mail.IsBodyHtml = false;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(Remitente, ClaveApp.Replace(" ", ""));
                    smtp.Send(mail);
                }
            }
        }
    }
}