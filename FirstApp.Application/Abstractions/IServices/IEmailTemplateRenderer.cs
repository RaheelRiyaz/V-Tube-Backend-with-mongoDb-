using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.Abstractions.IServices
{
    public interface IEmailTemplateRendererAsyync
    {
        Task<string> EmailTemplate(string templateName, object model);
    }
}
