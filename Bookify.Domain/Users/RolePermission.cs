namespace Bookify.Domain.Users;

public sealed class RolePermission
{
    public int RoleId { get; init; }

    public int PermissionId { get; init; }
}
