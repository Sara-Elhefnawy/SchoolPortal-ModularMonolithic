namespace SchoolPortal.SharedKernel.Mediator;

/// <summary>
/// Marker for "something happened" messages. Unlike IRequest<T>, there is NO
/// response type: the publisher fires the event and doesn't wait for a result
/// from anyone. That's what makes the publisher independent of its listeners.
/// </summary>
public interface INotification { }

/// <summary>
/// One listener for one notification type. Unlike IRequestHandler, you can
/// (and will) have MANY of these for the same TNotification: e.g. one that
/// emails the student and another that writes an audit row.
/// </summary>
public interface INotificationHandler<in TNotification> where TNotification : INotification
{
    Task Handle(TNotification notification, CancellationToken ct = default);
}

/// <summary>
/// Separate from IMediator on purpose: code that only announces events
/// (e.g. a command handler) shouldn't also get the ability to Send()
/// arbitrary requests, and vice versa.
/// </summary>
public interface IPublisher
{
    // Non-generic on purpose: ModuleDbContext holds events in a list typed as the
    // interface, so the compile-time type is not the event's real type. The
    // implementation must use notification.GetType() to find the right handlers.
    Task Publish(INotification notification, CancellationToken ct = default);
}