using Microsoft.EntityFrameworkCore;
using ShopSpire.Repository.Data;
using ShopSpireCore.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ShopSpire.Repository.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        //DbContext 
        private readonly ShopSpireDbContext _context;

        public GenericRepository(ShopSpireDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            _context.Set<T>().Remove(entity);
        }

        public async Task<List<T>> GetAllAsync()
        {
            var entities = await _context.Set<T>().ToListAsync();
            return entities;
        }

        public Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query.ToListAsync();
        }

        public  async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, params Func<IQueryable<T>, IQueryable<T>>[]? includes)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includes != null)
            {
                foreach(var include in includes)
                {
                    query = include(query);
                }
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            return await query.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            return entity;
                
        }

        public async Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T>query=_context.Set<T>();
            if(includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }

            }
            var entity =await query.FirstOrDefaultAsync(e=>EF.Property<int>(e,"Id")==id);
            return entity;
        }

        public async Task<T> GetByIdAsync(int id, params Func<IQueryable<T>, IQueryable<T>>[]? includess)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includess != null)
            {
                foreach (var include in includess)
                {
                    query = include(query);
                }
            }

            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public async Task UpdateAsync(int id, Action<T> updateAction)
        {
           var entity=await GetByIdAsync(id);
            if (entity != null)
            {
                updateAction(entity);
                _context.Set<T>().Update(entity);
            }
            else
            {
                throw new ArgumentException($"Entity with id {id} not found.");
            }
        }
    }
}
