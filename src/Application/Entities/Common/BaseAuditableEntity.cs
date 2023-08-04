namespace Chrono.Application.Entities.Common;

public abstract class BaseAuditableEntity : BaseEntity
{
    public DateTime Created { get; set; }
    public User CreatedBy { get; set; }
    public DateTime LastModified { get; set; }
    public User LastModifiedBy { get; set; }
}
