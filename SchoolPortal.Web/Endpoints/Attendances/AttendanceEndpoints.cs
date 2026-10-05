using System.Security.Claims;
using SchoolPortal.Attendances.Application.Features.Attendances.Command.Record;
using SchoolPortal.Attendances.Application.Features.Attendances.Command.Update;
using SchoolPortal.Attendances.Application.Features.Attendances.Query.GetAttendanceForOccurrence;
using SchoolPortal.Attendances.Application.Features.Attendances.Query.GetStudentAttendanceHistory;
using SchoolPortal.Attendances.Application.Features.SessionOccurrences.Cancel;
using SchoolPortal.Attendances.Application.Features.SessionOccurrences.Command.Create;
using SchoolPortal.Attendances.Application.Features.SessionOccurrences.Query.GetBySession;
using SchoolPortal.Attendances.Application.Features.Sessions.Command.Create;
using SchoolPortal.Attendances.Application.Features.Sessions.Query.GetByCourseSection;
using SchoolPortal.Attendances.Application.Features.Sessions.Query.GetToday;
using SchoolPortal.SharedKernel.Identity;
using SchoolPortal.SharedKernel.Mediator;
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
        
        group.MapGet("/today", GetTodaysSessions).RequireAuthorization(Permissions.Attendance.View);
        
        group.MapGet("/", GetSessions).RequireAuthorization(Permissions.Attendance.View);

        group.MapGet("/{sessionId:int}/occurrences", GetSessionOccurrences)
            .RequireAuthorization(Permissions.Attendance.View);

        group.MapPost("/", CreateSession).RequireAuthorization(Permissions.Attendance.Mark);

        group.MapPost("/{sessionId:int}/occurrences", CreateSessionOccurrence)
            .RequireAuthorization(Permissions.Attendance.Mark);
        
        group.MapPost("/{id:int}/cancel", CancelSessionOccurrence)
            .RequireAuthorization(policy => policy.RequireRole(Roles.Admin, Roles.Teacher));
    }

    private static void MapAttendanceOperations(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/attendances").WithTags("Attendances");

        group.MapPost("/", RecordAttendance).RequireAuthorization(policy => policy.RequireRole(Roles.Admin, Roles.Teacher));

        group.MapPut("/{id:int}", UpdateAttendanceStatus).RequireAuthorization(policy => policy.RequireRole(Roles.Admin, Roles.Teacher));

        group.MapGet("/by-occurrence/{sessionOccurrenceId:int}", GetAttendanceForOccurrence)
            .RequireAuthorization(policy => policy.RequireRole(Roles.Admin, Roles.Teacher));

        group.MapGet("/by-student/{studentId:int}", GetStudentAttendanceHistory)
            .RequireAuthorization(policy => policy.RequireRole(Roles.Admin, Roles.Teacher));

        group.MapGet("/me", GetMyAttendanceHistory).RequireAuthorization(policy => policy.RequireRole(Roles.Student));
    }

    private static async Task<IResult> CancelSessionOccurrence(int id, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(new CancelSessionOccurrenceCommand(id), ct);

        return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> GetTodaysSessions(IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(new GetTodaysSessionsQuery(), ct);

        return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
    }
    
    private static async Task<IResult> GetSessions(int courseSectionId, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(new GetSessionsByCourseSectionQuery(courseSectionId), ct);

        return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(new { error = result.Error });
    }

    private static async Task<IResult> GetSessionOccurrences(int sessionId, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(new GetOccurrencesBySessionQuery(sessionId), ct);

        return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(new { error = result.Error });
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

    private static async Task<IResult> GetMyAttendanceHistory(ClaimsPrincipal user, IMediator mediator,
        CancellationToken ct)
    {
        // TEMP: dump claims so you can see what's on the principal
        var claims = user.Claims.Select(c => $"{c.Type} = {c.Value}").ToArray();
        // If you're running with a JWT, also check user.Identity?.IsAuthenticated

        var studentIdClaim = user.FindFirst("student_id")?.Value;
        if (!int.TryParse(studentIdClaim, out var studentId))
        {
            return Results.Problem(
                title: "Not linked to a student record",
                detail: "Claims: " + string.Join(" | ", claims),
                statusCode: StatusCodes.Status404NotFound);
        }

        var result = await mediator.Send(new GetStudentAttendanceHistoryQuery(studentId), ct);

        return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
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