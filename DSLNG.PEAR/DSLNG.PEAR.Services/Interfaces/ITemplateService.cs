

using DSLNG.PEAR.Services.Requests.Template;
using DSLNG.PEAR.Services.Responses.Template;
namespace DSLNG.PEAR.Services.Interfaces
{
    public interface ITemplateService
    {
        CreateTemplateResponse CreateTemplate(CreateTemplateRequest request);
        GetTemplatesResponse GetTemplates(GetTemplatesRequest request);
        GetTemplateResponse GetTemplate(GetTemplateRequest request);
        UpdateTemplateResponse UpdateTemplate(UpdateTemplateRequest request);
        DeleteTemplateResponse DeleteTemplate(DeleteTemplateRequest request);
    }
}
