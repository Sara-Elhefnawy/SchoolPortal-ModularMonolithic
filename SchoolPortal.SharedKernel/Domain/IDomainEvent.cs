using MediatR;

namespace SchoolPortal.SharedKernel.Domain;

// This is just a "tag" interface — it adds no methods of its own.
// It exists so that in code, anyone reading `StudentPhoneNumberUpdated : IDomainEvent`
// immediately knows "this is something an entity raised," rather than some other
// unrelated MediatR message.
//
// It inherits INotification because that's the MediatR message type that supports
// "publish to zero, one, or many handlers" — as opposed to IRequest<T>, which MediatR
// sends to exactly one handler and expects a return value from. Events have no single
// "answer," so INotification is the correct fit.
public interface IDomainEvent : INotification;