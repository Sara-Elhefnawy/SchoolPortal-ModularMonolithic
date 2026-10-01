using Microsoft.Extensions.Configuration;
using SchoolPortal.SharedKernel.Persistence.Seeders;
using SchoolPortal.Students.Application.Directories;
using SchoolPortal.Students.Application.Persistence;
using SchoolPortal.Students.Domain.Addresses;
using SchoolPortal.Students.Domain.Enrollments;
using SchoolPortal.Students.Domain.enums;
using SchoolPortal.Students.Domain.Students;

namespace SchoolPortal.Students.Infrastructure.Persistence.Seeders;

internal sealed class StudentsSeeder(
    IStudentDirectory students,
    IEnrollmentDirectory enrollments,
    IStudentsUnitOfWork unitOfWork,
    TimeProvider clock,
    IConfiguration configuration) : IDataSeeder
{
    public int Order => 20;

    public async Task SeedAsync(CancellationToken ct)
    {
        // WHY: Email has a unique index, so a second run would throw without this guard.
        if ((await students.GetAllAsync(ct: ct)).Count > 0)
            return;

        var seededStudentEmail = configuration["Seed:Users:Student:Email"] ?? "student@schoolportal.test";

        var seededStudentName = configuration["Seed:Users:Student:Name"] ?? "Sara Ahmed";

        // WHY one list, not five copies: SeedConstants.StudentIds is [1..5], so this
        // list MUST have exactly five entries or the other modules' data points at nobody.
        var people = new (string Name, Gender Gender, DateOnly Birth, string City)[]
        {
            (seededStudentName, Gender.Female, new DateOnly(2009, 3, 14), "Cairo"),
            ("Omar Hassan", Gender.Male, new DateOnly(2008, 11, 2), "Alexandria"),
            ("Laila Mahmoud", Gender.Female, new DateOnly(2009, 7, 21), "Giza"),
            ("Youssef Ali", Gender.Male, new DateOnly(2008, 1, 30), "Mansoura"),
            ("Nour Ibrahim", Gender.Female, new DateOnly(2009, 9, 9), "Damanhur")
        };

        var studentList = people.Select((p, i) =>
        {
            var address = Address.Create(
                    buildingNumber: 10 + i, city: p.City, state: "Egypt", zipCode: $"1{i}000", country: "Egypt")
                .Unwrap($"address for {p.Name}");

            // The FIRST seeded Student uses the same email as
            // the development Student Identity account.
            var email = i == 0 ? seededStudentEmail : $"student{i + 1}@schoolportal.test";

            return Student.Create(p.Name, email, $"0100000000{i + 1}", address, p.Birth, p.Gender,
                new DateOnly(2024, 9, 1)).Unwrap(p.Name);
        }).ToList();

        foreach (var student in studentList)
            students.Add(student);

        await unitOfWork.SaveChangesAsync(ct);

        var enrolledOn = DateOnly.FromDateTime(clock.GetLocalNow().DateTime).AddDays(-28);

        foreach (var student in studentList)
        {
            foreach (var sectionId in SeedConstants.FullyEnrolledSectionIds)
                enrollments.Add(Enrollment.Create(student.Id, sectionId, enrolledOn).Unwrap("enrollment"));

            if (student.Id <= 3)
                enrollments.Add(Enrollment.Create(student.Id, 3, enrolledOn).Unwrap("enrollment"));
        }

        await unitOfWork.SaveChangesAsync(ct);
    }
}