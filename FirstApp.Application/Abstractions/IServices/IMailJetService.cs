using FirstApp.Application.MailSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.Abstractions.IServices
{
    public interface IMailJetService
    {
        Task<bool> SendEmailAsync(MailSetting model);
    }
}
