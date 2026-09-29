using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Identity;
using SchoolPortal.SharedKernel.Identity.Contracts;
using SchoolPortal.Students.Application.Features.Enrollments.Commands.Complete;
using SchoolPortal.Students.Application.Features.Enrollments.Commands.Enroll;
using SchoolPortal.Students.Application.Features.Enrollments.Commands.Withdraw;
using SchoolPortal.Students.Application.Features.Students.Commands.Create;
using SchoolPortal.Students.Application.Features.Students.Commands.Deactivate;
using SchoolPortal.Students.Application.Features.Students.Commands.Graduate;
using SchoolPortal.Students.Application.Features.Students.Commands.Suspend;
using SchoolPortal.Students.Application.Features.Students.Commands.UpdateAddress;
using SchoolPortal.Students.Application.Features.Students.Commands.UpdateEmail;
using SchoolPortal.Students.Application.Features.Students.Commands.UpdatePhone;
using SchoolPortal.Students.Application.Features.Students.Query.GetAll;
using SchoolPortal.Students.Application.Features.Students.Query.GetById;
using SchoolPortal.Web.Endpoints.Students.Requests.Students;

namespace SchoolPortal.Web.Endpoints.Students;

public static class StudentEndpoints
{
    public static void MapStudentEndpoints(this IEndpointRouteBuilder app)
    {
        MapStudentOperations(app);
        MapEnrollmentOperations(app);
    }

    private static void MapStudentOperations(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/students").WithTags("Students");
        
        group.MapGet("/me", Me).RequireAuthorization();
        
        group.MapGet("/", GetStudents).RequireAuthorization(Permissions.Students.View);
        
        group.MapGet("/{id:int}", GetStudentById).RequireAuthorization(Permissions.Students.View);

        group.MapPost("/", CreateStudent).RequireAuthorization(Permissions.Students.Create);

        group.MapPut("/{id:int}/address", UpdateStudentAddress).RequireAuthorization(Permissions.Students.Update);

        group.MapPut("/{id:int}/phone", UpdateStudentPhone).RequireAuthorization(Permissions.Students.Update);

        group.MapPut("/{id:int}/email", UpdateStudentEmail).RequireAuthorization(Permissions.Students.Update);

        group.MapPost("/{id:int}/suspend", SuspendStudent).RequireAuthorization(Permissions.Students.Update);

        group.MapPost("/{id:int}/graduate", GraduateStudent).RequireAuthorization(Permissions.Students.Update);

        group.MapPost("/{id:int}/deactivate", DeactivateStudent).RequireAuthorization(Permissions.Students.Update);
    }

    private static void MapEnrollmentOperations(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/enrollments").WithTags("Enrollments");

        /*
         * Enrollment changes student academic state,
         * so we protect it with the Students.Update permission.
         */
        group.MapPost("/", EnrollStudent).RequireAuthorization(Permissions.Students.Update);

        group.MapPost("/{id:int}/withdraw", WithdrawEnrollment).RequireAuthorization(Permissions.Students.Update);

        group.MapPost("/{id:int}/complete", CompleteEnrollment).RequireAuthorization(Permissions.Students.Update);
    }
    
    private static IResult Me(ICurrentUser user)
    {
        return Results.Ok(new
        {
            id = user.UserId!.Value,
            name = user.Name,
            roles = user.Roles,
            permissions = user.Permissions
        });
    }
    
    private static async Task<IResult> GetStudents(string? search, string? status, IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllStudentsQuery(search, status), ct);

        return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(new { error = result.Error });
    }

    private static async Task<IResult> GetStudentById(int id, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(new GetStudentByIdQuery(id), ct);

        // NotFound, not BadRequest: a missing student is a 404 for the Details page.
        return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(new { error = result.Error });
    }

    private static async Task<IResult> CreateStudent(CreateStudentCommand command, IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        return result.IsSuccess
            ? Results.Created($"/api/students/{result.Value}", result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> UpdateStudentAddress(int id, UpdateStudentAddressRequest body,
        IMediator mediator, CancellationToken ct)
    {
        var command = new UpdateStudentAddressCommand(id, body.City, body.State, body.ZipCode, body.Country,
            body.BuildingNumber);

        var result = await mediator.Send(command, ct);

        return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> UpdateStudentPhone(int id, UpdateStudentPhoneRequest body, IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateStudentPhoneCommand(id, body.PhoneNumber), ct);

        return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> UpdateStudentEmail(int id, UpdateStudentEmailRequest body, IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateStudentEmailCommand(id, body.Email), ct);

        return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> SuspendStudent(int id, SuspendStudentRequest body, IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(new SuspendStudentCommand(id, body.Reason), ct);

        return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> GraduateStudent(int id, GraduateStudentRequest body, IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(new GraduateStudentCommand(id, body.GraduationDate), ct);

        return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> DeactivateStudent(int id, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(new DeactivateStudentCommand(id), ct);

        return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> EnrollStudent(EnrollStudentCommand command, IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        return result.IsSuccess
            ? Results.Created($"/api/enrollments/{result.Value}", result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> WithdrawEnrollment(int id, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(new WithdrawEnrollmentCommand(id), ct);

        return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> CompleteEnrollment(int id, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(new CompleteEnrollmentCommand(id), ct);

        return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
    }
}