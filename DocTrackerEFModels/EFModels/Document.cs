using System;
using System.Collections.Generic;

namespace DocTrackerEFModels.EFModels;

public partial class Document
{
    public int DocId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public int? OrderNo { get; set; }

    public virtual ICollection<ReadingLog> ReadingLogs { get; set; } = new List<ReadingLog>();
}
