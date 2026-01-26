using PruebaAngular.Domain.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PruebaAngular.Infrastructure.Data.Specifications
{
    public abstract class BaseSpecification<T> : ISpecification<T>
    {
        public Expression<Func<T, bool>> Criteria { get; private set; }
        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();
        public List<string> IncludeStrings { get; } = new List<string>();
        public Expression<Func<T, object>> OrderBy { get; private set; }
        public Expression<Func<T, object>> OrderByDescending { get; private set; }
        public int Take { get; private set; }
        public int Skip { get; private set; }
        public bool IsPagingEnabled { get; private set; } = false;
        public bool FromCache { get; set; }
        public int CacheExpiration { get; set; }
        public string CacheKey { get; set; }

        protected BaseSpecification() { }

        protected BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        protected BaseSpecification(Expression<Func<T, bool>> criteria, object cacheKeyParam)
        {
            Criteria = criteria;
            CacheKey = $"{typeof(T).Name}_{cacheKeyParam}";
        }

        protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        protected virtual void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }

        protected virtual void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }

        protected virtual void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        protected virtual void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
        {
            OrderByDescending = orderByDescExpression;
        }
    }

    public abstract class BaseLinqSpecification<T> : BaseSpecification<T>
    {
        public Func<DbContext, IQueryable<T>> LinqQuery { get; set; }

        protected BaseLinqSpecification() : base() { }

        protected BaseLinqSpecification(Expression<Func<T, bool>> criteria) : base(criteria) { }

        protected BaseLinqSpecification(Expression<Func<T, bool>> criteria, object cacheKeyParam) : base(criteria, cacheKeyParam) { }
    }

    public abstract class BaseDapperSpecification<T> : BaseSpecification<T>
    {
        public string SqlQuery { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();

        protected BaseDapperSpecification() : base() { }

        protected BaseDapperSpecification(object cacheKeyParam)
        {
            CacheKey = $"{typeof(T).Name}_{cacheKeyParam}";
        }

        protected void AddParameter(string name, object value)
        {
            Parameters[name] = value;
        }
    }
}
