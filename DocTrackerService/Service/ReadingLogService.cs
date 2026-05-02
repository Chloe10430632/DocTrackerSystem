using DocTrackerEFModels.EFModels;
using DocTrackerService.DTO;
using DocTrackerService.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocTrackerService.Service
{
    public class ReadingLogService : IReadingLogService
    {
        private readonly IGenericRepository<ReadingLog> _dbLogs;

        public ReadingLogService(IGenericRepository<ReadingLog> dbLogs)
        {
            _dbLogs = dbLogs;
        }

        public Task<bool> CheckAndCreateAsync(int userId, string ip,CreateReadingLogDto dto)
        {
            var newLog = new ReadingLog
            {
                UserId = userId,
                DocId = dto.DocId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                CreatedTime = DateTime.Now,
                ClientIp = ip
            };

            _dbLogs.Add(newLog);
           return _dbLogs.SaveChangesAsync();
        }

        public async Task<IEnumerable<SearchReadingLogDto>> GetAllLogsAsync(string keyword)
        {
            var logs = _dbLogs.GetAll();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                logs = logs.Where(l => l.User.UserName.Contains(keyword) || l.Doc.Title.Contains(keyword));
            }

            var selectLogs = logs
                .OrderByDescending(l=>l.CreatedTime)
                .Select(l => new SearchReadingLogDto
            {
                LogId = l.LogId,
                DocTitle = l.Doc.Title,
                UserName = l.User.UserName,
                StartTime = l.StartTime,
                EndTime = l.EndTime,
                CreatedTime = l.CreatedTime,
                ClientIP = l.ClientIp
            });

            return await selectLogs.ToListAsync();
        }
    }
}
