using System;
using System.Collections.Generic;

namespace DocTrackerEFModels.EFModels;

public partial class ReadingLog
{
    public int LogId { get; set; }

    public int UserId { get; set; }

    public int DocId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public DateTime CreatedTime { get; set; }

    public string ClientIp { get; set; } = null!;

    public virtual Document Doc { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
