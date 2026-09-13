using SchoolPortal.Attendances.Domain.Attendances;

namespace SchoolPortal.Web.Endpoints.Attendances.Requests.Attendances;

public sealed record UpdateAttendanceStatusRequest(AttendanceStatus Status, string? Note);