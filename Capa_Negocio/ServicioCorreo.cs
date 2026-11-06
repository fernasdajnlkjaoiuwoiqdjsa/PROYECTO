using System.Net;
using System.Net.Mail;

namespace Capa_Negocio
{
    // Servicio de correo: es la clase que manda el recordatorio por email cuando el médico le
    // avisa a un paciente que faltó a su cita (el formulario de enviar recordatorio la llama).
    // La hice static porque no guarda ningún estado: solo recibe los datos, arma el mensaje y lo
    // envía, así que no hace falta crear un objeto cada vez. Usa el SMTP de Gmail con la cuenta
    // de la clínica.
    public static class ServicioCorreo
    {
        // Cuenta de Gmail desde donde salen todos los correos (la de la clínica).
        private const string Remitente = "saludvital283@gmail.com";  // verifica que sea tu dirección completa
        // Contraseña de aplicación de Google (no es la contraseña normal de la cuenta): Gmail no
        // deja iniciar sesión con la normal desde un programa, así que genera esta de 16
        // caracteres. Viene con espacios, por eso más abajo se los quito con Replace.
        private const string ClaveApp = "vfye qbmb gflv fylp";        // la contraseña de aplicación

        // Envía el correo de recordatorio al paciente. Recibe el correo de destino, el nombre del
        // médico (para firmar) y el mensaje que escribió el médico en el formulario. Arma el
        // correo con asunto fijo, le agrega al final la firma con el nombre del médico y lo manda
        // por el servidor de Gmail. Si algo falla (sin internet, clave mala, correo inválido) la
        // excepción sube hasta el formulario, que se la muestra al usuario.
        public static void EnviarRecordatorio(string destino, string nombreMedico, string mensaje)
        {
            // using libera el mensaje y la conexión al terminar, aunque falle el envío
            using (MailMessage mail = new MailMessage())
            {
                // Remitente con nombre visible, para que al paciente le salga "SaludVital" y no solo el correo
                mail.From = new MailAddress(Remitente, "SaludVital - Sistema de Gestión");
                mail.To.Add(new MailAddress(destino));
                mail.Subject = "Recordatorio de cita - SaludVital";
                mail.Body = mensaje + "\n\nAtentamente,\n" + nombreMedico + "\nSaludVital";
                mail.IsBodyHtml = false;   // texto plano, sin formato HTML

                // Gmail usa el puerto 587 con SSL/TLS
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;   // envía por red, no guarda en una carpeta
                    smtp.UseDefaultCredentials = false;                 // uso mis credenciales, no las de Windows
                    smtp.Credentials = new NetworkCredential(Remitente, ClaveApp.Replace(" ", ""));
                    smtp.Send(mail);
                }
            }
        }
    }
}