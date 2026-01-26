using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PruebaAngular.Domain.Core
{
    // Interfaces base del dominio
    public abstract class Entity
    {
        public virtual int Id { get; protected set; }
    }

    public interface IAggregateRoot
    {
    }

    // Interfaces de repositorio
    public interface IRepository<T> where T : class
    {
        IUnitOfWork UnitOfWork { get; }
        T Add(T entity);
        Task<T> AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
        T GetById(object id);
        Task<T> GetByIdAsync(object id);
        IEnumerable<T> GetAll();
        Task<IEnumerable<T>> GetAllAsync();
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    }

    public interface IUnitOfWork : IDisposable
    {
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }

    // Interfaz de especificación (patrón Specification)
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; }
        List<Expression<Func<T, object>>> Includes { get; }
        List<string> IncludeStrings { get; }
        Expression<Func<T, object>> OrderBy { get; }
        Expression<Func<T, object>> OrderByDescending { get; }
        int Take { get; }
        int Skip { get; }
        bool IsPagingEnabled { get; }
        bool FromCache { get; set; }
        int CacheExpiration { get; set; }
        string CacheKey { get; set; }
    }

    // Interfaz de repositorio de consulta
    public interface IQueryRepository
    {
        IQueryable<T> Where<T>(Expression<Func<T, bool>> predicate) where T : class;
        Task<List<T>> ToListAsync<T>() where T : class;
        Task<List<T>> FindAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
        IEnumerable<T> ApplySpecification<T>(ISpecification<T> spec) where T : class;
        Task<IEnumerable<T>> ApplySpecificationAsync<T>(ISpecification<T> spec) where T : class;
        IEnumerable<T> List<T>(ISpecification<T> spec) where T : class;
        Task<IEnumerable<T>> ListAsync<T>(ISpecification<T> spec) where T : class;
    }
}
