using DocTrackerService.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocTrackerService.IService
{
    public interface IDocumentService
    {
        public Task<DocumentModel?> GetDocumentByID(int id);
    }
}
