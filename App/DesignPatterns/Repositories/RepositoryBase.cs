using Microsoft.EntityFrameworkCore;
using Project.App.Databases;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Repository
{
    public interface IRepositoryBase<T>
    {
        IQueryable<T> FindAll();
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
        T FirstOrDefault(Expression<Func<T, bool>> expression);
        void Add(T entity);
        void AddRange(T entity);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(T entity);
    }

    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        private readonly MariaDBContext DbContext;
        public RepositoryBase(MariaDBContext dbContext)
        {
            DbContext = dbContext;
        }

        public IQueryable<T> FindAll()
        {
            return DbContext.Set<T>().AsNoTracking();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return DbContext.Set<T>().Where(expression).AsNoTracking();
        }

        public T FirstOrDefault(Expression<Func<T, bool>> expression)
        {
            return DbContext.Set<T>().FirstOrDefault(expression);
        }

        public void Add(T entity)
        {
            DbContext.Set<T>().Add(entity);
        }

        public void AddRange(T entity)
        {
            DbContext.Set<T>().AddRange(entity);
        }

        public void Update(T entity)
        {
            DbContext.Set<T>().Update(entity);
        }

        public void Remove(T entity)
        {
            DbContext.Set<T>().Remove(entity);
        }
        public void RemoveRange(T entity)
        {
            DbContext.Set<T>().RemoveRange(entity);
        }
    }
}
