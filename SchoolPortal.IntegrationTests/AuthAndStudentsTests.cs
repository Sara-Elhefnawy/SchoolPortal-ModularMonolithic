using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using SchoolPortal.Web;
using Xunit;

namespace SchoolPortal.IntegrationTests;

// IClassFixture = boot the app ONCE for all tests in this class. Booting runs
// startup and the seeders, so doing it per test would be slow.
public class AuthAndStudentsTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private const string CsrfTokenUrl = "/api/antiforgery/token";
    private const string CsrfHeader   = "X-CSRF-TOKEN";
    private const string LoginUrl     = "/api/account/login";
    private const string StudentsUrl  = "/api/students";

    // TEST 1: login works, and the cookie it sets really authenticates us.
    [Fact]
    public async Task Login_with_seeded_admin_sets_a_cookie_that_unlocks_protected_endpoints()
    {
        var client = CreateClient();

        var login = await LoginAsAdminAsync(client);
        Assert.True(login.IsSuccessStatusCode,
            $"Login failed with {(int)login.StatusCode}: {await login.Content.ReadAsStringAsync()}");

        // WHY a second request: a 200 from login only proves the server was
        // happy. Calling a protected endpoint proves the cookie was stored by
        // the client AND accepted by the server. That is what "login works" means.
        var protectedCall = await client.GetAsync(StudentsUrl);
        Assert.Equal(HttpStatusCode.OK, protectedCall.StatusCode);
    }

    // TEST 2: an authenticated admin can create a student (real DB write).
    [Fact]
    public async Task Admin_can_create_a_student()
    {
        var client = CreateClient();
        var login = await LoginAsAdminAsync(client);
        Assert.True(login.IsSuccessStatusCode, "Precondition failed: admin login.");

        // WHY unique per run: a local Postgres keeps data between runs, and a
        // fixed email would hit a "duplicate" rule the second time.
        var unique = Guid.NewGuid().ToString("N")[..8];

        var response = await PostJsonAsync(client, StudentsUrl, new
        {
            // CHANGE: match these names to your CreateStudent request body,
            // and add any required fields (address, etc.).
            firstName   = "Test",
            lastName    = $"Student{unique}",
            email       = $"student.{unique}@example.com",
            dateOfBirth = "2010-01-15"
        });

        // WHY print the body: "expected 201, got 400" is useless. The body
        // usually names the exact validation rule that failed.
        Assert.True(response.IsSuccessStatusCode,
            $"Create student failed with {(int)response.StatusCode}: {await response.Content.ReadAsStringAsync()}");

        // If the endpoint returns a Location header, read the row back to
        // prove it was really saved. If not, this part is skipped.
        if (response.Headers.Location is not null)
        {
            var fetched = await client.GetAsync(response.Headers.Location);
            Assert.Equal(HttpStatusCode.OK, fetched.StatusCode);
        }
    }

    // ----------------------------- helpers -----------------------------

    private HttpClient CreateClient() =>
        factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            // WHY https: if you later set the auth cookie to SecurePolicy.Always,
            // a cookie set over http would never be sent back. The in-memory
            // server ignores the scheme, so https costs nothing.
            BaseAddress = new Uri("https://localhost"),

            // WHY off: we want to SEE a 302/401 as it is, not silently follow
            // a redirect to the login page and get a confusing 200.
            AllowAutoRedirect = false,
            // Cookies are handled by default, so the login cookie sticks to
            // this client between calls.
        });

    private static async Task<HttpResponseMessage> LoginAsAdminAsync(HttpClient client)
    {
        // WHY env vars: the test needs the same admin the app seeds, and
        // keeping the password out of the code means nothing is committed.
        var email = Environment.GetEnvironmentVariable("TEST_ADMIN_EMAIL");
        var password = Environment.GetEnvironmentVariable("TEST_ADMIN_PASSWORD");

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException(
                "Set TEST_ADMIN_EMAIL and TEST_ADMIN_PASSWORD (the same admin your seeder creates).");

        // CHANGE if your login body uses different field names.
        return await PostJsonAsync(client, LoginUrl, new { email, password });
    }

    // Sends a POST with a FRESH CSRF token every time.
    // WHY fresh: your antiforgery token is tied to the current user, so a
    // token fetched before login is invalid after login. Never reuse one.
    private static async Task<HttpResponseMessage> PostJsonAsync(HttpClient client, string url, object body)
    {
        var token = await GetCsrfTokenAsync(client);

        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(body)
        };
        request.Headers.Add(CsrfHeader, token);

        return await client.SendAsync(request);
    }

    private static async Task<string> GetCsrfTokenAsync(HttpClient client)
    {
        var response = await client.GetAsync(CsrfTokenUrl);
        response.EnsureSuccessStatusCode();

        var raw = await response.Content.ReadAsStringAsync();

        // Handles both {"token":"..."} and a bare token string.
        try
        {
            using var doc = JsonDocument.Parse(raw);
            if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                doc.RootElement.TryGetProperty("token", out var t))
                return t.GetString()!;
        }
        catch (JsonException) {  }

        return raw.Trim('"');
    }
}