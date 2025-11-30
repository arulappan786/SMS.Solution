using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using SMS.Application.Services.Common;

namespace SMS.Infrastructure.Services.Common
{
    public class EmailTemplatesLoader : IEmailTemplatesLoader
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        public EmailTemplatesLoader(IWebHostEnvironment env, IConfiguration config)
        {
            _env = env;
            _config = config;
        }

        public string LoadEmailTemplate(string templateName)
        {
            string templatePath = Path.Combine(_env.ContentRootPath, "EmailTemplates", templateName);
            
            if(!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Email template '{templateName}' not found at path: {templatePath}");
            }

            string htmlBody = File.ReadAllText(templatePath);

            return htmlBody;
        }
    }
}
