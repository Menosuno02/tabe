using System.Net.Mail;
using System.Net;
using Azure.Security.KeyVault.Secrets;
using TabeNuget;

namespace ProyectoASPNET.Helpers
{
    public class HelperMails
    {
        private IConfiguration configuration;
        private string pass;

        public HelperMails(IConfiguration configuration, KeysModel keys)
        {
            this.configuration = configuration;
            this.pass = keys.TabeMailPass;
        }

        private async Task<MailMessage> ConfigureMailMessage
            (string para, string asunto, string mensaje)
        {
            MailMessage mail = new MailMessage();
            string user = this.configuration.GetValue<string>
                ("MailSettings:Credentials:User");
            mail.From = new MailAddress(user);
            mail.To.Add(para);
            mail.Subject = asunto;
            mail.Body = mensaje;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.Normal;
            return mail;
        }

        private async Task<SmtpClient> ConfigureSmtpClient()
        {
            string user = this.configuration.GetValue<string>
                ("MailSettings:Credentials:User");
            string password = pass;
            string hostName = this.configuration.GetValue<string>
                ("MailSettings:ServerSmtp:Host");
            int port = this.configuration.GetValue<int>
                ("MailSettings:ServerSmtp:Port");
            bool enableSsl = this.configuration.GetValue<bool>
                ("MailSettings:ServerSmtp:EnableSsl");
            bool defaultCredentials = this.configuration.GetValue<bool>
                ("MailSettings:ServerSmtp:DefaultCredentials");
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = hostName;
            smtpClient.Port = port;
            smtpClient.EnableSsl = enableSsl;
            smtpClient.UseDefaultCredentials = defaultCredentials;

            NetworkCredential credentials = new NetworkCredential(user, password);
            smtpClient.Credentials = credentials;
            return smtpClient;
        }

        public async Task SendMailAsync
            (string para, string asunto, string mensaje)
        {
            MailMessage mail = await ConfigureMailMessage(para, asunto, mensaje);
            SmtpClient smtpClient = await ConfigureSmtpClient();
            await smtpClient.SendMailAsync(mail);
        }
    }
}
