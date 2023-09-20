namespace Chrono.Entities.Common;

public abstract class BaseAuditableEntity : BaseEntity
{
    public DateTime Created { get; set; }
    public User CreatedBy { get; set; }
    public DateTime LastModified { get; set; }
    public User LastModifiedBy { get; set; }
}

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
