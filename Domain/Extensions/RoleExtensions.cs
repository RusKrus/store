using Store.Domain.Enums;

namespace Domain.Extensions;

public static class RoleExtensions
{
    public static bool HasGreaterRole(this UserRole currentUserRole, UserRole targetUserRole)
    {
        return currentUserRole > targetUserRole;
    }

    public static bool IsAdminOrAbove(this UserRole currentUserRole)
    {
        return currentUserRole > UserRole.Customer;
    }
}