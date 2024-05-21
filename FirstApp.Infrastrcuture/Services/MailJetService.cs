
using FirstApp.Application.Abstractions.IServices;
using FirstApp.Application.MailSettings;
using Mailjet.Client;
using Mailjet.Client.TransactionalEmails;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Infrastrcuture.Services
{
    public class MailJetService : IMailJetService
    {
        private readonly MailJet options;

        public MailJetService(IOptions<MailJet> options)
        {
            this.options = options.Value;
        }

        public async Task<bool> SendEmailAsync(MailSetting model)
        {
            MailjetClient mailjetClient = new MailjetClient(options.ApiKey, options.SecretKey);
            var email = new TransactionalEmailBuilder()
                .WithFrom(new SendContact(options.FromEmail)).
                WithSubject(model.Subject).
                WithTo(new SendContact(model.To.FirstOrDefault())).
                WithHtmlPart(model.Body).
                Build();

            await mailjetClient.SendTransactionalEmailAsync(email);

            return true;
        }
    }
}
