using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.Identities.Application.Directories;
using SchoolPortal.Identities.Domain.Roles;
using SchoolPortal.Identities.Domain.Users;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Roles.Command.Create;

public sealed class CreateRoleCommandHandler(IIdentityDirectory directory)
    : IRequestHandler<CreateRoleCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateRoleCommand request, CancellationToken ct)
    {
        var name = request.Name.Trim();
        if (name.Length == 0)
            return Result<int>.Failure(UserErrors.NameRequired);

        if (await directory.GetRoleByNameAsync(name, ct) is not null)
            return Result<int>.Failure(RoleErrors.AlreadyExists);

        // ADJUST: use ApplicationRole's factory/constructor if it has one.
        var role = new ApplicationRole { Name = name };
        directory.AddRole(role);
        await directory.SaveChangesAsync(ct);

        return Result<int>.Success(role.Id);
    }
}