using Template.Application.Common.Interfaces.Services;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Template.Infra.ExternalServices.SendEmails
{
    internal class SendGrid : ISendGrid
    {
        private readonly string _api_Key;
        private readonly string[] _emailDocumento;
        public SendGrid(string apiKey, string[] emailDocumento)
        {
            _api_Key = apiKey;
            _emailDocumento = emailDocumento;
        }

        public async Task EnviaEmailSubidaDocumento()
        {
            try
            {
                var client = new SendGridClient(_api_Key);
                var from = new EmailAddress("admin@admin.com", "Aviso de Documento Pendente");
                var subject = "Existe um documento pendente";
                var to = new EmailAddress(_emailDocumento[0], "");
                var plainTextContent = "Um novo documento foi iniciado na Plataforme Verifique!";
                var htmlContent = "<strong>Um novo documento foi iniciado na Plataforme Verifique!</strong>";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = await client.SendEmailAsync(msg);
            }
            catch (Exception)
            {
            }
        }
    }
}