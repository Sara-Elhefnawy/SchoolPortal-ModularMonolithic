using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SchoolPortal.Email.Domain;
using SchoolPortal.SharedKernel.Emails.Contracts;

namespace SchoolPortal.Email.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddEmailModule(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<EmailOptions>(config.GetSection(EmailOptions.SectionName));
        services.AddScoped<IEmailSender, GmailSmtpEmailSender>();
        
        return services;
    }
}