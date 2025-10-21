#nullable enable
using System.ComponentModel.DataAnnotations;


namespace DossierTimeline.Web.Models;


public class AuditLog
{
    [Key]
    public Guid Id { get; set; }


    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;


    [MaxLength(256)]
    public string? UserId { get; set; }


    [Required, MaxLength(64)]
    public string Action { get; set; } = string.Empty; // Create/Update/Delete/Download/View/Export/Approve


    [Required, MaxLength(64)]
    public string EntityType { get; set; } = string.Empty; // Document/User/Settings


    [Required, MaxLength(64)]
    public string EntityId { get; set; } = string.Empty; // Guid or key as string


    public string? Metadata { get; set; } // JSON payload (TEXT in SQLite)
}