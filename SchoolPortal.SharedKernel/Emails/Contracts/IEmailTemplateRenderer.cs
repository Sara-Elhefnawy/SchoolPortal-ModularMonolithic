namespace SchoolPortal.SharedKernel.Emails;

// Infrastructure depends ONLY on this interface. It has no idea a Razor view
// engine exists behind it - that's the whole point. Whoever implements this
// is free to render however they want.
public interface IEmailTemplateRenderer
{
    Task<string> RenderAsync<TModel>(string templateName, TModel model);
}