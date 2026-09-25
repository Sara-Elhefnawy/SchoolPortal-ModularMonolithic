using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SchoolPortal.SharedKernel.Mediator;

/// <summary>
/// Default IMediator + IPublisher implementation.
/// How Send works:
/// 1. We know the CONCRETE type of the incoming request at runtime
///    (e.g. GetAllProductsQuery), even though we only see it as
///    IRequest&lt;TResponse&gt; at compile time.
/// 2. We build the matching handler's interface type:
///    IRequestHandler&lt;GetAllProductsQuery, ResultOfT&lt;...&gt;&gt;
/// 3. We ask the DI container for whatever's registered against that exact
///    interface. This works because AddMediatorPattern() registered every
///    handler against its specific closed generic interface at startup.
/// 4. We invoke Handle(...) on it via a cached delegate, so we only pay the
///    reflection cost ONCE per request TYPE, not once per call.
///
/// How Publish works: same idea, but for zero-to-many
/// INotificationHandler&lt;T&gt; listeners, found by the notification's runtime type.
/// </summary>
public sealed class Mediator(IServiceProvider provider) : IMediator, IPublisher
{
    private static readonly ConcurrentDictionary<Type, HandlerInvoker> Invokers = new();

    // One dispatcher per notification TYPE, created once and reused, for the same
    // reason Send caches invokers: reflection only on the first publish of a type.
    private static readonly ConcurrentDictionary<Type, NotificationDispatcher> Dispatchers = new();

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken ct = default)
    {
        var requestType = request.GetType();

        var invoker = Invokers.GetOrAdd(requestType, _ => BuildInvoker(requestType, typeof(TResponse)));

        return (Task<TResponse>)invoker(provider, request, ct);
    }

    public Task Publish(INotification notification, CancellationToken ct = default)
    {
        // GetType() gives the REAL type (StudentPhoneUpdated), even when the variable
        // is typed as IDomainEvent or INotification.
        var dispatcher = Dispatchers.GetOrAdd(notification.GetType(), static type =>
            // typeof(NotificationDispatcher<>) with the empty brackets is the OPEN generic
            // definition. MakeGenericType fills the slot with the runtime type. Same
            // reason as typeof(IRequestHandler<,>) in BuildInvoker below.
            (NotificationDispatcher)Activator.CreateInstance(
                typeof(NotificationDispatcher<>).MakeGenericType(type))!);

        return dispatcher.Dispatch(provider, notification, ct);
    }

    private static HandlerInvoker BuildInvoker(Type requestType, Type responseType)
    {
        var handlerInterfaceType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);

        var handleMethod = handlerInterfaceType.GetMethod(nameof(IRequestHandler<IRequest<object>, object>.Handle))
            ?? throw new InvalidOperationException($"Handle method not found on {handlerInterfaceType}");

        return (sp, request, ct) =>
        {
            var handler = sp.GetService(handlerInterfaceType)
                ?? throw new InvalidOperationException(
                    $"No handler registered for '{requestType.Name}'. " +
                    $"Did you forget to register it, or is it missing an IRequestHandler<{requestType.Name}, ...> implementation?");

            return (Task)handleMethod.Invoke(handler, [request, ct])!;
        };
    }

    private delegate Task HandlerInvoker(IServiceProvider provider, object request, CancellationToken ct);

    // Non-generic base so the dictionary can hold dispatchers for different notification types.
    private abstract class NotificationDispatcher
    {
        public abstract Task Dispatch(IServiceProvider sp, INotification notification, CancellationToken ct);
    }

    // Generic subclass: inside it TNotification is a real compile-time type, so
    // GetServices<INotificationHandler<TNotification>>() and handler.Handle(...) are
    // ordinary strongly-typed calls. No MethodInfo.Invoke, and if a handler throws you
    // get the original exception, not a TargetInvocationException wrapper.
    private sealed class NotificationDispatcher<TNotification> : NotificationDispatcher
        where TNotification : INotification
    {
        public override async Task Dispatch(IServiceProvider sp, INotification notification, CancellationToken ct)
        {
            // Zero handlers is valid: the loop simply doesn't run.
            // Sequential, not Task.WhenAll: scoped handlers usually share one DbContext,
            // which is not thread-safe.
            foreach (var handler in sp.GetServices<INotificationHandler<TNotification>>())
            {
                await handler.Handle((TNotification)notification, ct);
            }
        }
    }
}

public static class MediatorServiceCollectionExtensions
{
    /// <summary>
    /// Registers IMediator/IPublisher, then scans the given assembly for every
    /// class implementing IRequestHandler&lt;,&gt; OR INotificationHandler&lt;&gt;
    /// and registers each against its specific closed generic interface.
    /// </summary>
    public static IServiceCollection AddMediatorPattern(this IServiceCollection services, Assembly assembly)
    {
        // TryAdd: each module calls this with its own assembly, and the second call
        // must not pile up duplicate registrations. Mediator holds no per-instance
        // state (its caches are static), so two registrations of it are harmless.
        services.TryAddScoped<IMediator, Mediator>();
        services.TryAddScoped<IPublisher, Mediator>();

        var requestHandlerType = typeof(IRequestHandler<,>);
        var notificationHandlerType = typeof(INotificationHandler<>);

        var handlerRegistrations = from type in assembly.GetTypes()
            where !type.IsAbstract && !type.IsInterface
            from @interface in type.GetInterfaces()
            where @interface.IsGenericType && (@interface.GetGenericTypeDefinition() == requestHandlerType ||
                                               @interface.GetGenericTypeDefinition() == notificationHandlerType)
            select (Interface: @interface, Implementation: type);

        foreach (var (@interface, implementation) in handlerRegistrations)
        {
            // Plain AddScoped (NOT TryAdd) is deliberate: for notifications we WANT
            // several implementations registered against the same INotificationHandler<T>.
            services.AddScoped(@interface, implementation);
        }

        return services;
    }
}