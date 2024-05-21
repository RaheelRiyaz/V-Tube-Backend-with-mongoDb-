using FirstApp.Application.Abstractions.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RazorLight;

namespace FirstApp.Infrastrcuture.Services
{
    public class EmailTemplateRendererService : IEmailTemplateRendererAsyync
    {

        public async Task<string> EmailTemplate(string templateName, object model)
        {
            string template = string.Empty;

            try
            {
                var location = Assembly.GetExecutingAssembly().Location;

                var folderPath = Path.GetDirectoryName(location);

                var templatePath = Path.Combine(folderPath ?? "", "Templates");

                var razorEngine = new RazorLightEngineBuilder()
                    .UseFileSystemProject(templatePath)
                    .UseMemoryCachingProvider()
                    .EnableDebugMode()
                    .Build();

                template = await razorEngine.CompileRenderAsync(templateName, model);
            }
            catch (Exception ex)
            {

            }

            return template;
        }
    }
}
