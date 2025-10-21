#nullable enable
using System.ComponentModel.DataAnnotations;


namespace DossierTimeline.Web.Models;


public class Setting
{
    [Key]
    public int Id { get; set; }


    [Required, MaxLength(128)]
    public string Key { get; set; } = string.Empty;


    [MaxLength(4000)]
    public string? Value { get; set; }
}