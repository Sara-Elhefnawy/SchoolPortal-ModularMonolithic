using SchoolPortal.SharedKernel.Results.Errors;

namespace SchoolPortal.Email.Domain;

public static class EmailErrors
{
    public static Error EmailNotAuthenticated =
        Error.Unauthorized("EmailErrors.EmailNotAuthenticated", "Email is not authenticated.");
    
    public static Error EmailSendFailed =
        Error.Unauthorized("EmailErrors.EmailSendFailed", "Failed to send Email.");
}