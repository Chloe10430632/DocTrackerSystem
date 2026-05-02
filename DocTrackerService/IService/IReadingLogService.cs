using DocTrackerService.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocTrackerService.IService
{
    public interface IReadingLogService
    {
        public Task<IEnumerable<SearchReadingLogDto>> GetAllLogsAsync(string keyword);
        public Task<bool> CheckAndCreateAsync(int userId, string ip,CreateReadingLogDto dto);
    }
}
