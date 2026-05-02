using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocTrackerService.DTO
{
    public class SearchReadingLogDto
    {
        public int LogId { get; set; }
        public string? DocTitle { get; set; } 
        public string? UserName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreatedTime { get; set; }
        public string ClientIP { get; set; } = null!;
    }
}
