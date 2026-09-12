using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolPortal.Students.Domain;
using SchoolPortal.Students.Domain.Students;

namespace SchoolPortal.Students.Infrastructure.Configurations;

public sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.Property(p => p.Name).HasMaxLength(Constants.MaxNameLength).IsRequired();

        builder.Property(p => p.Email).HasMaxLength(Constants.MaxEmailLength).IsRequired();

        builder.Property(p => p.PhoneNumber).HasMaxLength(Constants.MaxPhoneNumberLength).IsRequired();

        builder.Property(x => x.DateOfBirth).HasColumnType("date").IsRequired();

        builder.Property(x => x.Gender).HasConversion<string>().HasMaxLength(20).IsRequired();

        builder.OwnsOne(x => x.Address, address =>
        {
            address.Property(a => a.State).HasColumnName("State").HasMaxLength(Constants.MaxStateLength).IsRequired();

            address.Property(a => a.City).HasColumnName("City").HasMaxLength(Constants.MaxCityLength).IsRequired();

            address.Property(a => a.Country).HasColumnName("Country").HasMaxLength(Constants.MaxCountryLength)
                .IsRequired();

            address.Property(a => a.ZipCode).HasColumnName("ZipCode").HasMaxLength(Constants.MaxZipCodeLength)
                .IsRequired();

            address.Property(a => a.BuildingNumber).HasColumnName("BuildingNumber").IsRequired();
        });

        builder.HasIndex(x => x.Email).IsUnique();
        
        builder.HasQueryFilter(u => !u.IsDeleted);
    }
}