using ShopSpire.Repository.Data;
using ShopSpireCore.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSpire.Repository.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        //DbContext 
        private readonly ShopSpireDbContext _context;
        private readonly Dictionary<Type, object> _repository;

        public UnitOfWork(ShopSpireDbContext context)
        {
            _context = context;
            _repository = new Dictionary<Type, object>();
        }

        public Task<int> CompleteAsync()
        {
          return _context.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            var type= typeof(TEntity);
            if (!_repository.ContainsKey(type))
            {
                var repositoryInstance = new GenericRepository<TEntity>(_context);
                _repository[type] = repositoryInstance;
            }
            return (GenericRepository<TEntity>)_repository[type];
        }
    }
}
