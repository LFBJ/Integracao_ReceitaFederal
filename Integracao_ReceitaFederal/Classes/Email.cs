using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;


namespace Integracao_ReceitaFederal
{
    class Email
    {
        private string Login { get; set; }
        private string Password { get; set; }
        private string SMTP { get; set; }
        private int Port { get; set; }
        private Boolean EnableSsl { get; set; }
        private string Para { get; set; }


        public Email()
        {
            Login = ConfigurationManager.AppSettings.Get("Login");
            Password = ConfigurationManager.AppSettings.Get("Password");
            SMTP = ConfigurationManager.AppSettings.Get("SMTP");
            Port = Convert.ToInt32(ConfigurationManager.AppSettings.Get("Port"));
            EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("EnableSsl"));
            Para = ConfigurationManager.AppSettings.Get("Para");
        }
        public void EnviaEmail(string titulo, string corpo)
        {
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress(Login);
            mail.To.Add(Para);
            mail.Subject = titulo;
            mail.Body = corpo;
            //mail.Attachments.Add(new Attachment(@"C:\teste.txt"));
            using (var smtp = new SmtpClient(SMTP))
            {
                smtp.EnableSsl = EnableSsl;
                smtp.Port = Port;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(Login, Password);
                smtp.Send(mail);
            }
        }
    }
}
