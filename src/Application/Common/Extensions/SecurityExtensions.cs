using Chrono.Application.Entities.Common;

namespace Chrono.Application.Common.Extensions;

public static class SecurityExtensions
{
    public static bool IsPermitted(this BaseAuditableEntity context, string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return false;
        }

        // Only the owner is permitted to view/edit the object.
        return context.CreatedBy.UserId == userId;
    }
}
