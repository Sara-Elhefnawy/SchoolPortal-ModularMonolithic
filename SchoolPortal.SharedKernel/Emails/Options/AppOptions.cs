namespace SchoolPortal.SharedKernel.Emails.Options;

// Separate from EmailOptions: this isn't an email concern,
//      it's "what's the public URL of this deployment"
// something any module building a user-facing link (not just email) would need.
// Application-layer handlers can depend on IOptions<AppOptions> without pulling
// in anything Web- or Infrastructure-specific, since Microsoft.Extensions.Options is just a
// configuration primitive, not a layering violation.
public class AppOptions
{
    public const string SectionName = "App";
 
    // e.g. https://schoolportal.example.com — no trailing slash
    public required string PublicBaseUrl { get; init; } 
}