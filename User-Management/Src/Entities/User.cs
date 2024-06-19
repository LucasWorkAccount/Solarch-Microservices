using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace User_Management.Model;

public partial class User
{
    public int Id { get; set; }

    public Guid? Uuid { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? Role { get; set; }
}
