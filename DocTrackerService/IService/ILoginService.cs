using DocTrackerService.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocTrackerService.IService
{
    public interface ILoginService
    {
        public Task<bool> CheckAndLoginAsync(LoginModel data);
    }
}
