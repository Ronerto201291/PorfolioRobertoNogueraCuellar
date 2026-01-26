using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PruebaAngular.Infrastructure.Data.Specifications;
using PruebaAngular.Domain.Core;

namespace PruebaAngular.Infrastructure.Data.Core
{
    public interface ICacheProvider
    {
        T Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan? expiration = null);
        void Remove(string key);
        bool Exists(string key);
    }

    public abstract class QueryRepositoryBase : IQueryRepository
    {
        protected readonly DbContext _context;
        protected readonly ICacheProvider _cacheProvider;

        protected QueryRepositoryBase(DbContext context, ICacheProvider cacheProvider = null)
        {
            _context = context;
            _cacheProvider = cacheProvider;
        }

        protected IQueryable<T> GetQueryable<T>() where T : class
        {
            return _context.Set<T>().AsNoTracking();
        }

        public virtual IQueryable<T> Where<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return _context.Set<T>().AsNoTracking().Where(predicate);
        }

        public virtual async Task<List<T>> ToListAsync<T>() where T : class
        {
            return await _context.Set<T>().AsNoTracking().ToListAsync();
        }

        public virtual async Task<List<T>> FindAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await _context.Set<T>().AsNoTracking().Where(predicate).ToListAsync();
        }

        public virtual IEnumerable<T> ApplySpecification<T>(ISpecification<T> spec) where T : class
        {
            return ApplySpecificationQuery(spec).ToList();
        }

        public virtual async Task<IEnumerable<T>> ApplySpecificationAsync<T>(ISpecification<T> spec) where T : class
        {
            return await ApplySpecificationQuery(spec).ToListAsync();
        }

        public virtual IEnumerable<T> List<T>(ISpecification<T> spec) where T : class
        {
            if (spec.FromCache && _cacheProvider != null)
            {
                var cacheKey = spec.CacheKey ?? $"{typeof(T).Name}_{spec.GetHashCode()}";
                if (_cacheProvider.Exists(cacheKey))
                {
                    return _cacheProvider.Get<IEnumerable<T>>(cacheKey);
                }

                var result = ApplySpecificationQuery(spec).ToList();
                _cacheProvider.Set(cacheKey, result, TimeSpan.FromMinutes(spec.CacheExpiration));
                return result;
            }

            return ApplySpecificationQuery(spec).ToList();
        }

        public virtual async Task<IEnumerable<T>> ListAsync<T>(ISpecification<T> spec) where T : class
        {
            if (spec.FromCache && _cacheProvider != null)
            {
                var cacheKey = spec.CacheKey ?? $"{typeof(T).Name}_{spec.GetHashCode()}";
                if (_cacheProvider.Exists(cacheKey))
                {
                    return _cacheProvider.Get<IEnumerable<T>>(cacheKey);
                }

                var result = await ApplySpecificationQuery(spec).ToListAsync();
                _cacheProvider.Set(cacheKey, result, TimeSpan.FromMinutes(spec.CacheExpiration));
                return result;
            }

            return await ApplySpecificationQuery(spec).ToListAsync();
        }

        private IQueryable<T> ApplySpecificationQuery<T>(ISpecification<T> spec) where T : class
        {
            // Check if it's a LINQ specification
            if (spec is BaseLinqSpecification<T> linqSpec && linqSpec.LinqQuery != null)
            {
                var query = linqSpec.LinqQuery(_context);
                
                // Apply includes
                foreach (var include in spec.Includes)
                {
                    query = query.Include(include);
                }

                foreach (var includeString in spec.IncludeStrings)
                {
                    query = query.Include(includeString);
                }

                // Apply ordering
                if (spec.OrderBy != null)
                {
                    query = query.OrderBy(spec.OrderBy);
                }
                else if (spec.OrderByDescending != null)
                {
                    query = query.OrderByDescending(spec.OrderByDescending);
                }

                // Apply paging
                if (spec.IsPagingEnabled)
                {
                    query = query.Skip(spec.Skip).Take(spec.Take);
                }

                return query;
            }

            // Standard specification
            var baseQuery = _context.Set<T>().AsNoTracking();

            if (spec.Criteria != null)
            {
                baseQuery = baseQuery.Where(spec.Criteria);
            }

            baseQuery = spec.Includes.Aggregate(baseQuery, (current, include) => current.Include(include));
            baseQuery = spec.IncludeStrings.Aggregate(baseQuery, (current, include) => current.Include(include));

            if (spec.OrderBy != null)
            {
                baseQuery = baseQuery.OrderBy(spec.OrderBy);
            }
            else if (spec.OrderByDescending != null)
            {
                baseQuery = baseQuery.OrderByDescending(spec.OrderByDescending);
            }

            if (spec.IsPagingEnabled)
            {
                baseQuery = baseQuery.Skip(spec.Skip).Take(spec.Take);
            }

            return baseQuery;
        }

        protected T GetFromCacheOrDatabase<T>(string cacheKey, Func<T> databaseQuery, TimeSpan? expiration = null)
        {
            if (_cacheProvider != null && _cacheProvider.Exists(cacheKey))
            {
                return _cacheProvider.Get<T>(cacheKey);
            }

            var result = databaseQuery();
            
            if (_cacheProvider != null && result != null)
            {
                _cacheProvider.Set(cacheKey, result, expiration);
            }

            return result;
        }

        protected async Task<T> GetFromCacheOrDatabaseAsync<T>(string cacheKey, Func<Task<T>> databaseQuery, TimeSpan? expiration = null)
        {
            if (_cacheProvider != null && _cacheProvider.Exists(cacheKey))
            {
                return _cacheProvider.Get<T>(cacheKey);
            }

            var result = await databaseQuery();
            
            if (_cacheProvider != null && result != null)
            {
                _cacheProvider.Set(cacheKey, result, expiration);
            }

            return result;
        }
    }
}
