using Razor.Templating.Core;
using SchoolPortal.SharedKernel.Emails.Contracts;

namespace SchoolPortal.Web.EmailTemplates;

public class RazorEmailTemplateRenderer : IEmailTemplateRenderer
{
    // templateName is now a full view path (e.g. "/Views/Shared/ResetPassword.cshtml"),
    // not just a bare file name - so we pass it straight through instead of
    // building the path ourselves. Keeping path construction out of this class
    // means the interface stays a dumb "render this, with this model" contract;
    // Infrastructure decides what "this" refers to.
    public Task<string> RenderAsync<TModel>(string templateName, TModel model) =>
        RazorTemplateEngine.RenderAsync(templateName, model);
}