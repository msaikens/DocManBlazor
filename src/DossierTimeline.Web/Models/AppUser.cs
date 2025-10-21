#nullable enable
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace DossierTimeline.Web.Models;


public class AppUser : IdentityUser
{
    [MaxLength(128)]
    public string? DisplayName { get; set; }

    public bool IsActive { get; set; } = true;
}