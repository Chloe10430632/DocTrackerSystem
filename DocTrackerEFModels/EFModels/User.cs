using System;
using System.Collections.Generic;

namespace DocTrackerEFModels.EFModels;

public partial class User
{
    public int UserId { get; set; }

    public string? UserName { get; set; }

    public string? Account { get; set; }

    public string? PasswordHash { get; set; }

    public int? RoleId { get; set; }

    public string? PictureUrl { get; set; }

    public virtual ICollection<ReadingLog> ReadingLogs { get; set; } = new List<ReadingLog>();

    public virtual Role? Role { get; set; }
}
