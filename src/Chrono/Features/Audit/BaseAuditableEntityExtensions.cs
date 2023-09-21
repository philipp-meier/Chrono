using Chrono.Entities.Common;

namespace Chrono.Features.Audit;

public static class BaseAuditableEntityExtensions
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
