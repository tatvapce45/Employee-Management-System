﻿using System;
using System.Collections.Generic;

namespace EmployeeManagementSystem.DataAccess.Models;

public partial class Refreshtoken
{
    public int Id { get; set; }

    public string Token { get; set; } = null!;

    public DateTime Expires { get; set; }

    public bool IsRevoked { get; set; }

    public bool IsUsed { get; set; }

    public int EmployeeId { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
