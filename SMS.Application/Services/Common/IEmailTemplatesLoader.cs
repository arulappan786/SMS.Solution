namespace SMS.Application.Services.Common
{
    public interface IEmailTemplatesLoader
    {
        string LoadEmailTemplate(string templateName);
    }
}
