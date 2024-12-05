using Uniqlo.Views.Account.Enums;

namespace Uniqlo.Extensions;

public static class RolesExtension
{
    public static string GetRole(this Roles role)
    {
        return role switch
        {
            Roles.User => nameof(Roles.User),
            Roles.Admin => nameof(Roles.Admin)
        };
    }
}
