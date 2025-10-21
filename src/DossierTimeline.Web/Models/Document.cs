#nullable enable
using System.ComponentModel.DataAnnotations;


namespace DossierTimeline.Web.Models;


public class Document
{
    [Key]
    public Guid Id { get; set; }


    [Required, MaxLength(260)]
    public string FileName { get; set; } = string.Empty; // Stored file name (server-side)


    [Required, MaxLength(260)]
    public string OriginalFileName { get; set; } = string.Empty; // Original client file name


    [MaxLength(127)]
    public string? ContentType { get; set; }


    [Range(0, long.MaxValue)]
    public long SizeBytes { get; set; }


    [Required, MaxLength(512)]
    public string StoredPath { get; set; } = string.Empty; // Path or blob key


    [MaxLength(64)]
    public string? HashSha256 { get; set; } // For dedupe/chain-of-custody


    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedUtc { get; set; }


    [MaxLength(256)]
    public string? CreatedByUserId { get; set; }


    public DateTime? EventDateUtc { get; set; } // Timeline date


    [MaxLength(128)]
    public string? Category { get; set; }


    [MaxLength(2048)]
    public string? Tags { get; set; } // e.g., semicolon-delimited


    [MaxLength(4000)]
    public string? Notes { get; set; }
}