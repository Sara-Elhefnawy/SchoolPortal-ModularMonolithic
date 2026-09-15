using MediatR;
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
using SchoolPortal.Web.Endpoints.Students.Requests.Students;

namespace SchoolPortal.Web.Endpoints.Students;

public static class StudentEndpoints
{
    public static void MapStudentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/students").WithTags("Students");

        group.MapPost("/", async (CreateStudentCommand command, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(command, ct);
            return result.IsSuccess
                ? Results.Created($"/api/students/{result.Value}", result.Value)
                : Results.BadRequest(result.Error);
        });

        group.MapPut("/{id:int}/address", async (
            int id, UpdateStudentAddressRequest body, IMediator mediator, CancellationToken ct) =>
        {
            var command = new UpdateStudentAddressCommand(
                id, body.City, body.State, body.ZipCode, body.Country, body.BuildingNumber);
            var result = await mediator.Send(command, ct);
            return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
        });

        group.MapPut("/{id:int}/phone", async (
            int id, UpdateStudentPhoneRequest body, IMediator mediator, CancellationToken ct) =>
        {
            var command = new UpdateStudentPhoneCommand(
                id, body.PhoneNumber);
            var result = await mediator.Send(command, ct);
            return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
        });

        group.MapPut("/{id:int}/email", async (
            int id, UpdateStudentEmailRequest body, IMediator mediator, CancellationToken ct) =>
        {
            var command = new UpdateStudentEmailCommand(
                id, body.Email);
            var result = await mediator.Send(command, ct);
            return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
        });
        
        group.MapPost("/{id:int}/suspend", async (
            int id, SuspendStudentRequest body, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new SuspendStudentCommand(id, body.Reason), ct);
            return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
        });

        group.MapPost("/{id:int}/graduate", async (
            int id, GraduateStudentRequest body, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GraduateStudentCommand(id, body.GraduationDate), ct);
            return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
        });

        group.MapPost("/{id:int}/deactivate", async (
            int id, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new DeactivateStudentCommand(id), ct);
            return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
        });

        var enrollments = app.MapGroup("/api/enrollments").WithTags("Enrollments");

        enrollments.MapPost("/", async (EnrollStudentCommand command, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(command, ct);
            return result.IsSuccess
                ? Results.Created($"/api/enrollments/{result.Value}", result.Value)
                : Results.BadRequest(result.Error);
        });

        enrollments.MapPost("/{id:int}/withdraw", async (int id, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new WithdrawEnrollmentCommand(id), ct);
            return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
        });

        enrollments.MapPost("/{id:int}/complete", async (int id, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new CompleteEnrollmentCommand(id), ct);
            return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
        });
    }
}