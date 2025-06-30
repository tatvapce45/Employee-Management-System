using System;
using System.Collections.Generic;

namespace EmployeeManagementSystem.DataAccess.Models;

public partial class Employee
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime HiringDate { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? DepartmentId { get; set; }

    public string Email { get; set; } = null!;

    public string MobileNo { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public int Age { get; set; }

    public decimal Salary { get; set; }

    public string Address { get; set; } = null!;

    public string Zipcode { get; set; } = null!;

    public int CountryId { get; set; }

    public int StateId { get; set; }

    public int CityId { get; set; }

    public int RoleId { get; set; }

    public string Password { get; set; } = null!;

    public int? ReportsTo { get; set; }

    public string? Position { get; set; }

    public string? UserName { get; set; }

    public byte[]? Image { get; set; }

    public string? ImageMimeType { get; set; }

    public string? ImageUrl { get; set; }

    public string? CloudinaryPublicId { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual City City { get; set; } = null!;

    public virtual Country Country { get; set; } = null!;

    public virtual Department? Department { get; set; }

    public virtual ICollection<Employee> InverseReportsToNavigation { get; set; } = new List<Employee>();

    public virtual ICollection<Refreshtoken> Refreshtokens { get; set; } = new List<Refreshtoken>();

    public virtual Employee? ReportsToNavigation { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual State1 State { get; set; } = null!;
}
