using MediatR;
using SchoolPortal.Identities.Application.Directories;
using SchoolPortal.Identities.Domain.Roles;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Roles.Query.GetById;

public sealed class GetRoleByIdQueryHandler (
    IIdentityDirectory directory)
    : IRequestHandler<GetRoleByIdQuery, Result<ApplicationRole>> 
{
    public async Task<Result<ApplicationRole>> Handle(GetRoleByIdQuery request, CancellationToken ct)
    {
        var role = await directory.GetRoleByIdAsync(request.Id, ct);
        if (role is null)
            return Result<ApplicationRole>.Failure(RoleErrors.NotFound);

        return Result<ApplicationRole>.Success(role);
    }
}