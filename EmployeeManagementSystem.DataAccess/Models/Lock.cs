﻿using System;
using System.Collections.Generic;

namespace EmployeeManagementSystem.DataAccess.Models;

public partial class Lock
{
    public string Resource { get; set; } = null!;

    public int Updatecount { get; set; }

    public DateTime? Acquired { get; set; }
}
