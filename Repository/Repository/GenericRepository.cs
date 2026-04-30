using DocTrackerEFModels.EFModels;
using Microsoft.EntityFrameworkCore;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class GenericRepository<Table> : IGenericRepository<Table> where Table : class
    {
        private readonly DocTrackerDbContext _dbContext;
        private readonly DbSet<Table> _entity;

        public GenericRepository(DocTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
            _entity = dbContext.Set<Table>();
        }

        public DocTrackerDbContext GetDbContext()
        {
            return _dbContext;
        }

        public IQueryable<Table> GetAll()
        {
            return _entity.AsNoTracking();
        }

        public async Task<IEnumerable<Table>> GetAllAsync()
        {
            return await _entity.AsNoTracking().ToListAsync();
        }

        public async ValueTask<Table?> GetTableByIDAsync<PrimaryKeyType>(PrimaryKeyType id)
        {
            return await _entity.FindAsync(id);
        }

        public void Add(Table entity)
        {
            _entity.Add(entity);
        }

        public void Delete<PrimaryKeyType>(PrimaryKeyType id)
        {
            _entity.Remove(_entity.Find(id));
        }
        public void Update(Table entity)
        {
            _entity.Update(entity);
        }
        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                var result = await _dbContext.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }



    }
}
