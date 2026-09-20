using MediatR;
using Microsoft.Extensions.Logging;
using SchoolPortal.Notifications.Application.Directories;
using SchoolPortal.Notifications.Domain.Notifications;
using SchoolPortal.SharedKernel.Domain.Events.Students;

namespace SchoolPortal.Notifications.Application.EventHandlers.Students;

// web endpoint calls mediator.Send(UpdateStudentPhoneCommand) → that goes to
// UpdateStudentPhoneCommandHandler in Students (does the work, saves, which triggers
// publisher.Publish inside the UnitOfWork) → MediatR then separately finds every
// INotificationHandler<StudentPhoneNumberUpdated> across the whole solution and calls
// each one → right now there's exactly one, and it lives in Notifications because
// Notifications is the module that cares about reacting to phone changes, not Students.

// INotificationHandler<T> is how MediatR knows "this class wants to react when a
// StudentPhoneNumberUpdated event is published." MediatR finds this class automatically
// via assembly scanning at startup
//      nothing else needs to reference this class by name anywhere.
public sealed class StudentPhoneNumberUpdatedHandler(
    INotificationDirectory directory,
    ILogger<StudentPhoneNumberUpdatedHandler> logger) : INotificationHandler<StudentPhoneNumberUpdated>
{
    public async Task Handle(StudentPhoneNumberUpdated notification, CancellationToken ct)
    {
        // logger = how YOU, as the dev, see this fire right now — check your console/
        // log output when you test this, before any DB or email/SMS work exists at all.
        // This is the fastest way to confirm the pipeline is actually running.
        logger.LogInformation(
            "Phone number updated for student {StudentId}", notification.StudentId);

        var record = Notification.Create(
            recipientId: notification.StudentId,
            message: $"Your phone number was updated to {notification.NewPhone}.");

        directory.Add(record);
        await directory.SaveChangesAsync(ct);

        // Email/SMS calls will go HERE, after the row is saved — see the note below
        // for exactly where those implementations will live.
    }
}