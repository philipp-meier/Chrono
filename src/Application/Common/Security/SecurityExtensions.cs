using Chrono.Domain.Common;

namespace Chrono.Application.Common.Security;

public static class SecurityExtensions
{
    public static bool IsPermitted(this BaseAuditableEntity context, string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return false;

        // Only the owner is permitted to view/edit the object.
        return context.CreatedBy.UserId == userId;
    }
}
