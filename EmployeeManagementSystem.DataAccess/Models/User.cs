using System;
using System.Collections.Generic;

namespace EmployeeManagementSystem.DataAccess.Models;

public partial class User
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Zipcode { get; set; } = null!;

    public string MobileNo { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int CountryId { get; set; }

    public int CityId { get; set; }

    public int StateId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int RoleId { get; set; }

    public virtual City City { get; set; } = null!;

    public virtual Country Country { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;

    public virtual State State { get; set; } = null!;
}
