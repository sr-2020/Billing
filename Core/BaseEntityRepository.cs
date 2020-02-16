using Core.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public interface IBaseRepository
    {
        int? GetId<T>(Expression<Func<T, bool>> predicate, string[] includes = null) where T : BaseEntity;
        T Get<T>(Expression<Func<T, bool>> predicate, string[] includes = null) where T : class;
        Task<T> GetAsync<T>(Expression<Func<T, bool>> predicate, string[] includes = null) where T : class;
        List<T> GetAll<T>(string[] includes = null) where T : class;
        List<T> GetList<T>(Expression<Func<T, bool>> predicate, string[] includes = null) where T : class;
        Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> predicate, string[] includes = null) where T : class;
        bool Exists<T>(Expression<Func<T, bool>> predicate) where T : class;
        void RemoveRange<T>(IEnumerable<T> entities) where T : class;
        void Remove<T>(T entity) where T : class;
        void AddRange<T>(IEnumerable<T> entities) where T : class;
        void Add<T>(T entity) where T : class;
    }


    public class BaseEntityRepository: IBaseRepository
    {
        protected readonly BillingContext Context;

        public BaseEntityRepository()
        {
            Context = new BillingContext();
        }

        public virtual void Add<T>(T entity) where T : class
        {
            this.Context.Set<T>().Add(entity);
        }

        public virtual void AddRange<T>(IEnumerable<T> entities) where T : class
        {
            this.Context.Set<T>().AddRange(entities);
        }

        public virtual void Remove<T>(T entity) where T : class
        {
            this.Context.Set<T>().Remove(entity);
        }

        public virtual void RemoveRange<T>(IEnumerable<T> entities) where T : class
        {
            this.Context.Set<T>().RemoveRange(entities);
        }

        public virtual List<T> GetList<T>(Expression<Func<T, bool>> predicate, string[] includes = null) where T : class
        {
            return this.FindInternal(this.Query<T>(), predicate, includes);
        }

        public Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> predicate, string[] includes = null) where T : class
        {
            return FindInternalAsync(this.Query<T>(), predicate, includes);
        }

        public virtual int? GetId<T>(Expression<Func<T, bool>> predicate, string[] includes = null) where T : BaseEntity
        {
            return this.AddIncludes(this.Query<T>(), includes).Where(predicate).Select(x => (int?)x.Id).FirstOrDefault();
        }

        public virtual T Get<T>(Expression<Func<T, bool>> predicate, string[] includes = null) where T : class
        {
            //Apply eager loading
            var res = AddIncludes(Query<T>(), includes);
            if(res != null)
                return res.FirstOrDefault(predicate);
            return null;
        }

        public Task<T> GetAsync<T>(Expression<Func<T, bool>> predicate, string[] includes = null) where T : class
        {
            return GetInternalAsync(Query<T>(), predicate, includes);
        }

        public virtual bool Exists<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return this.ExistsInternal(this.Query<T>(), predicate);
        }

        public virtual List<T> GetAll<T>(string[] includes = null) where T : class
        {
            return this.GetAllInternal(this.Query<T>(), includes);
        }

        #region protected

        protected List<T> FindInternal<T>(IQueryable<T> query, Expression<Func<T, bool>> predicate, string[] includes = null) where T : class
        {
            //Apply eager loading
            var res = AddIncludes(query, includes)
                .Where(predicate)
                .ToList();

            return res;
        }

        protected Task<List<T>> FindInternalAsync<T>(IQueryable<T> query, Expression<Func<T, bool>> predicate, string[] includes = null) where T : class
        {
            //Apply eager loading
            var res = AddIncludes(query, includes)
                .Where(predicate)
                .ToListAsync();
            return res;
        }

        protected Task<T> GetInternalAsync<T>(IQueryable<T> query, Expression<Func<T, bool>> predicate, string[] includes = null) where T : class
        {
            //Apply eager loading
            var res = AddIncludes(query, includes)
                .FirstOrDefaultAsync(predicate);
            return res;
        }

        protected bool ExistsInternal<T>(IQueryable<T> query, Expression<Func<T, bool>> predicate) where T : class
        {
            bool res = query.Any(predicate);
            return res;
        }

        protected List<T> GetAllInternal<T>(IQueryable<T> query, string[] includes = null) where T : class
        {
            //Apply eager loading
            var res = this.AddIncludes(query, includes)
                .ToList();
            return res;
        }

        protected virtual IQueryable<T> Query<T>() where T : class
        {
            return Context.Set<T>().AsNoTracking();
        }

        /// <summary>
        /// Add includes to query
        /// </summary>
        /// <param name="query">Query</param>
        /// <param name="includes">Navigation properties</param>
        /// <returns></returns>
        protected virtual IQueryable<T> AddIncludes<T>(IQueryable<T> query, Expression<Func<T, object>>[] includes) where T : class
        {
            if (includes != null)
            {
                foreach (var navigationProperty in includes)
                {
                    query = query.Include(navigationProperty);
                }
            }
            return query;
        }

        /// <summary>
        /// Add includes to query
        /// </summary>
        /// <param name="query">Query</param>
        /// <param name="includes">Navigation properties</param>
        /// <returns></returns>
        protected virtual IQueryable<T> AddIncludes<T>(IQueryable<T> query, string[] includes = null) where T : class
        {
            if (includes != null && includes.Length > 0)
            {
                foreach (string path in includes)
                {
                    query = query.Include(path);
                }
            }
            return query;
        }

        #endregion
    }
}
