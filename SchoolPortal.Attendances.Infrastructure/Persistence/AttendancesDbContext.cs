using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.Attendances.Domain.Attendances;
using SchoolPortal.Attendances.Domain.Sessions;
using SchoolPortal.Attendances.Domain.SessionOccurrences;
using SchoolPortal.SharedKernel.Persistence;

namespace SchoolPortal.Attendances.Infrastructure.Persistence;

public class AttendancesDbContext(
    DbContextOptions<AttendancesDbContext> options, 
    IPublisher publisher)
    : ModuleDbContext(options, publisher)
{
    public const string Schema = "attendance";
    
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<SessionOccurrence> SessionOccurrences => Set<SessionOccurrence>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AttendancesDbContext).Assembly);
    }
}