using MediatR;
using SchoolPortal.Identities.Application.Directories;
using SchoolPortal.Identities.Domain.Users;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Users.Query.GetById;

public sealed class GetUserByIdQueryHandler (
    IIdentityDirectory directory)
    : IRequestHandler<GetUserByIdQuery, Result<ApplicationUser>> 
{
    public async Task<Result<ApplicationUser>> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var user = await directory.GetUserByIdAsync(request.Id, ct);
        if (user is null)
            return Result<ApplicationUser>.Failure(UserErrors.NotFound);

        return Result<ApplicationUser>.Success(user);
    }
}