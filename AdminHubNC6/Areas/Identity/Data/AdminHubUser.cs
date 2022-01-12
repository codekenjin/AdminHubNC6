using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminHubNC6.Areas.Identity.Data;

// Add profile data for application users by adding properties to the AdminHubUser class
public class AdminHubUser : IdentityUser
{
    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string? FirstName { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string? LastName { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string? UserPost { get; set; }
}