using SchoolPortal.SharedKernel.Events.Identity.Auth;
using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.Students.Application.Directories;
using SchoolPortal.Students.Application.Persistence;
using SchoolPortal.Students.Domain.Addresses;
using SchoolPortal.Students.Domain.enums;
using SchoolPortal.Students.Domain.Students;

namespace SchoolPortal.Students.Application.EventHandlers.Identity;

public sealed class UserSignedUpHandler(IStudentDirectory directory, IStudentsUnitOfWork uow)
    : INotificationHandler<UserSignedUp>
{
    public async Task Handle(UserSignedUp notification, CancellationToken ct)
    {
        if (!Enum.TryParse<Gender>(notification.Gender, true, out var gender))
            throw new InvalidOperationException($"Invalid gender value: {notification.Gender}");

        var addressResult = Address.Create(notification.City, notification.State, notification.ZipCode,
            notification.Country, notification.BuildingNumber);

        if (addressResult.IsFailure)
            throw new InvalidOperationException(addressResult.Error.Message);

        var studentResult = Student.Create(notification.Name, notification.Email, notification.PhoneNumber,
            addressResult.Value, notification.DateOfBirth, gender, notification.AdmissionDate);

        if (studentResult.IsFailure)
            throw new InvalidOperationException(studentResult.Error.Message);

        directory.Add(studentResult.Value);

        await uow.SaveChangesAsync(ct);
    }
}