namespace LibraryApp.Domain.Common;

public abstract class BaseEntity : ISoftDeletable
{
    public int Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
}
