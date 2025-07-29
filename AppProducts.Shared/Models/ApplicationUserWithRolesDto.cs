using System.ComponentModel.DataAnnotations;

namespace AppProducts.Shared.Models;

public class ApplicationUserWithRolesDto : ApplicationUserDto
{
    public List<string>? Roles { get; set; }
}
