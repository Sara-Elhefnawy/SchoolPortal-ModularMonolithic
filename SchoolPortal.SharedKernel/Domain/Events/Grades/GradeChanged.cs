namespace SchoolPortal.SharedKernel.Domain.Events.Grades;

// OldScore/NewScore, not just NewScore — an audit-trail listener needs "from -> to" pair
//      if we only publish the new value, the listener can't know what changed without
//      going back to the DB, which defeats the point of listening to an event
public sealed record GradeChanged(int GradeId, int StudentId, decimal? OldScore, decimal? NewScore) : IDomainEvent;