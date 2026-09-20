using MediatR;
using SchoolPortal.Attendances.Application.Features.Attendances.Command.Record;
using SchoolPortal.Attendances.Application.Features.Attendances.Command.Update;
using SchoolPortal.Attendances.Application.Features.Attendances.Query.GetAttendanceForOccurrence;
using SchoolPortal.Attendances.Application.Features.Attendances.Query.GetStudentAttendanceHistory;
using SchoolPortal.Attendances.Application.Features.SessionOccurrences.Command.Create;
using SchoolPortal.Attendances.Application.Features.Sessions.Command.Create;
using SchoolPortal.SharedKernel.Identity;
using SchoolPortal.Web.Endpoints.Attendances.Requests.Attendances;
using SchoolPortal.Web.Endpoints.Attendances.Requests.SessionOccurrences;

namespace SchoolPortal.Web.Endpoints.Attendances;

public static class AttendanceEndpoints
{
    public static void MapAttendanceEndpoints(this IEndpointRouteBuilder app)
    {
        MapSessionEndpoints(app);
        MapAttendanceOperations(app);
    }

    private static void MapSessionEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/sessions").WithTags("Sessions");

        group.MapPost("/", CreateSession).RequireAuthorization(Permissions.Attendance.Mark);

        group.MapPost("/{sessionId:int}/occurrences", CreateSessionOccurrence)
            .RequireAuthorization(Permissions.Attendance.Mark);
    }

    private static void MapAttendanceOperations(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/attendances").WithTags("Attendances");

        group.MapPost("/", RecordAttendance).RequireAuthorization(Permissions.Attendance.Mark);

        group.MapPut("/{id:int}", UpdateAttendanceStatus).RequireAuthorization(Permissions.Attendance.Update);

        group.MapGet("/by-occurrence/{sessionOccurrenceId:int}", GetAttendanceForOccurrence)
            .RequireAuthorization(Permissions.Attendance.View);

        group.MapGet("/by-student/{studentId:int}", GetStudentAttendanceHistory)
            .RequireAuthorization(Permissions.Attendance.View);
    }

    private static async Task<IResult> CreateSession(CreateSessionCommand command, IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        return result.IsSuccess
            ? Results.Created($"/api/sessions/{result.Value}", result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> CreateSessionOccurrence(int sessionId, CreateOccurrenceRequest body,
        IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateSessionOccurrenceCommand(sessionId, body.Date), ct);

        return result.IsSuccess
            ? Results.Created($"/api/session-occurrences/{result.Value}", result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> RecordAttendance(RecordAttendanceCommand command, IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        return result.IsSuccess
            ? Results.Created($"/api/attendances/{result.Value}", result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> UpdateAttendanceStatus(int id, UpdateAttendanceStatusRequest body,
        IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateAttendanceStatusCommand(id, body.Status, body.Note), ct);

        return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> GetAttendanceForOccurrence(int sessionOccurrenceId, IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(new GetAttendanceForOccurrenceQuery(sessionOccurrenceId), ct);

        return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> GetStudentAttendanceHistory(int studentId, IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(new GetStudentAttendanceHistoryQuery(studentId), ct);

        return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
    }
}