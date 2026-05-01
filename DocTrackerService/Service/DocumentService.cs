using DocTrackerEFModels.EFModels;
using DocTrackerService.DTO;
using DocTrackerService.IService;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocTrackerService.Service
{
    public class DocumentService : IDocumentService
    {
        private readonly IGenericRepository<Document> _dbDocs;


        public DocumentService(IGenericRepository<Document> dbDocs)
        {
            _dbDocs = dbDocs;
        }

        public async Task<DocumentModel?> GetDocumentByID(int id)
        {
            var data = await _dbDocs.GetTableByIDAsync(id);

            DocumentModel doc = new DocumentModel()
            {
                Id = data.DocId,
                Title = data.Title,
                Content = data.Content,
                OrderNo = data.OrderNo
            };

            return doc;
        }

        
    }
}
