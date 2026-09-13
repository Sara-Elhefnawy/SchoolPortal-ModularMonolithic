using MediatR;
using SchoolPortal.Attendances.Application.Features.Attendances.Command.Record;
using SchoolPortal.Attendances.Application.Features.Attendances.Command.Update;
using SchoolPortal.Attendances.Application.Features.Attendances.Query.GetAttendanceForOccurrence;
using SchoolPortal.Attendances.Application.Features.Attendances.Query.GetStudentAttendanceHistory;
using SchoolPortal.Attendances.Application.Features.SessionOccurrences.Command.Create;
using SchoolPortal.Attendances.Application.Features.Sessions.Command.Create;
using SchoolPortal.Web.Endpoints.Attendances.Requests.Attendances;
using SchoolPortal.Web.Endpoints.Attendances.Requests.SessionOccurrences;

namespace SchoolPortal.Web.Endpoints.Attendances;

public static class AttendanceEndpoints
{
    public static void MapAttendanceEndpoints(this IEndpointRouteBuilder app)
    {
        var sessions = app.MapGroup("/api/sessions").WithTags("Sessions");

        sessions.MapPost("/", async (CreateSessionCommand command, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(command, ct);
            return result.IsSuccess
                ? Results.Created($"/api/sessions/{result.Value}", result.Value)
                : Results.BadRequest(result.Error);
        });

        sessions.MapPost("/{sessionId:int}/occurrences", async (
            int sessionId, CreateOccurrenceRequest body, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new CreateSessionOccurrenceCommand(sessionId, body.Date), ct);
            return result.IsSuccess
                ? Results.Created($"/api/session-occurrences/{result.Value}", result.Value)
                : Results.BadRequest(result.Error);
        });

        var attendances = app.MapGroup("/api/attendances").WithTags("Attendances");

        attendances.MapPost("/", async (RecordAttendanceCommand command, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(command, ct);
            return result.IsSuccess
                ? Results.Created($"/api/attendances/{result.Value}", result.Value)
                : Results.BadRequest(result.Error);
        });

        attendances.MapPut("/{id:int}", async (
            int id, UpdateAttendanceStatusRequest body, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new UpdateAttendanceStatusCommand(id, body.Status, body.Note), ct);
            return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
        });

        attendances.MapGet("/by-occurrence/{sessionOccurrenceId:int}", async (
            int sessionOccurrenceId, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetAttendanceForOccurrenceQuery(sessionOccurrenceId), ct);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        });

        attendances.MapGet("/by-student/{studentId:int}", async (
            int studentId, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetStudentAttendanceHistoryQuery(studentId), ct);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        });
    }
}