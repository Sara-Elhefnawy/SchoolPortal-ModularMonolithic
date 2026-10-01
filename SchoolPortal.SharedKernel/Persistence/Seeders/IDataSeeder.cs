namespace SchoolPortal.SharedKernel.Persistence.Seeders;

public interface IDataSeeder
{
    // Lower runs first. Identity 10, Grades 20, Students 30, Attendances 40.
    int Order { get; }
    
    Task SeedAsync(CancellationToken ct);
}