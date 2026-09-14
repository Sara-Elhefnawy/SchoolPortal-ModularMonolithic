using Microsoft.EntityFrameworkCore;
using SchoolPortal.Attendances.Domain.Attendances;
using SchoolPortal.Attendances.Domain.Sessions;
using SchoolPortal.Attendances.Domain.SessionOccurrences;

namespace SchoolPortal.Attendances.Infrastructure.Persistence;

public class AttendancesDbContext(DbContextOptions<AttendancesDbContext> options) : DbContext(options)
{
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<SessionOccurrence> SessionOccurrences => Set<SessionOccurrence>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("attendance");
        
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AttendancesDbContext).Assembly);
    }
}