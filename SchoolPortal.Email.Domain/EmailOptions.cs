namespace SchoolPortal.Email.Domain;

public class EmailOptions
{
    public const string SectionName = "Email";
    
    public string SenderAddress { get; init; } = null!;
    public string AppPassword { get; init; } = null!;
    public string SenderDisplayName { get; init; } = "SchoolPortal";
    // Gmail's fixed SMTP endpoint for STARTTLS. 
    public string Host { get; init; } = "smtp.gmail.com";    
    public int Port { get; init; } = 587;
}