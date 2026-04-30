using DocTrackerEFModels.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IGenericRepository<Table> where Table : class
    {
        DocTrackerDbContext GetDbContext();

        IQueryable<Table> GetAll();

        Task<IEnumerable<Table>> GetAllAsync();

        ValueTask<Table?> GetTableByIDAsync<PrimaryKeyType>(PrimaryKeyType id);

        void Add(Table entity);

        void Delete<PrimaryKeyType>(PrimaryKeyType id);

        void Update(Table entity);

        Task<bool> SaveChangesAsync();

    }
}
