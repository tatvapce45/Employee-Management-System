using System;
using System.Collections.Generic;

namespace EmployeeManagementSystem.DataAccess.Models;

public partial class User
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? UserName { get; set; }

    public string Email { get; set; } = null!;

    public string? Address { get; set; }

    public string? Zipcode { get; set; }

    public string? MobileNo { get; set; }

    public string? Password { get; set; }

    public int? CountryId { get; set; }

    public int? CityId { get; set; }

    public int? StateId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int RoleId { get; set; }

    public string? GoogleUserId { get; set; }

    public virtual City? City { get; set; }

    public virtual Country? Country { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual State? State { get; set; }
}
