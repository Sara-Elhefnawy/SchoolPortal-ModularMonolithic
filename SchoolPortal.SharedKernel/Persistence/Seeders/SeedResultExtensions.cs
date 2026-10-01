using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.SharedKernel.Persistence.Seeders;

public static class SeedResultExtensions
{
    public static T Unwrap<T>(this Result<T> result, string what) =>
        result.IsFailure ? throw new InvalidOperationException($"Seeding {what} failed: {result.Error}") : result.Value;

    public static void EnsureSuccess(this Result result, string what)
    {
        if (result.IsFailure)
            throw new InvalidOperationException($"Seeding {what} failed: {result.Error}");
    }
}