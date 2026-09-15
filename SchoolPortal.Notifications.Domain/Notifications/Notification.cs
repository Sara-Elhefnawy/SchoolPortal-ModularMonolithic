namespace SchoolPortal.Notifications.Domain.Notifications;

// This is the "row in a table" that lets you and users actually SEE that something
// happened — separate from the transient event that triggered it. The event
// (StudentPhoneNumberUpdated) exists only in memory for a split second while MediatR
// publishes it; this entity is what survives afterward, in the database.
public sealed class Notification
{
    public int Id { get; private set; }

    // Who this notification is "about" — kept generic (not tied to Students
    // specifically) since Grades/Attendance events will create notifications too,
    // and Notifications.Domain isn't allowed to know Student/Grade/Attendance exist.
    public int RecipientId { get; private set; }

    // The human-readable text — built by the handler, not stored as raw event data,
    // so what's saved is already "Your phone number was updated to 010..." not just
    // a StudentId and a phone string.
    public string Message { get; private set; } = string.Empty;

    public DateTime CreatedAtUtc { get; private set; }

    // Lets you build a "mark as read" feature later without changing the schema.
    public bool IsRead { get; private set; }

    // Private constructor + static factory, matching the pattern you're already
    // using on Student/Attendance — forces creation through Create() so you can't
    // end up with a Notification in a half-valid state.
    private Notification() { }

    public static Notification Create(int recipientId, string message)
    {
        return new Notification
        {
            RecipientId = recipientId,
            Message = message,
            CreatedAtUtc = DateTime.UtcNow,
            IsRead = false
        };
    }

    public void MarkAsRead() => IsRead = true;
}