using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CoreApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CoreApi.Data.Repositories.Base
{
    /// <summary>
    /// Base Repository
    /// </summary>
    /// <typeparam name="TModel">Model Class</typeparam>
    /// <typeparam name="TModelType">Id Type</typeparam>
    public abstract class BaseRepository<TModel, TModelType> : RepositoryUtility, IRepository<TModel, TModelType> where TModel : Entity<TModelType>, new ()
    {
        protected readonly ApplicationDbContext Db;

        protected BaseRepository(IHttpContextAccessor httpContext, ApplicationDbContext db) : base(httpContext)
        {
            Db = db;
        }


        public virtual async Task<IList<TModel>> GetAllAsync()
        {
            return await Db.Set<TModel>().ToListAsync();
        }

        public virtual async Task<TModel> FindByIdAsync(TModelType id)
        {
            return await FindOneAsync(x => x.Id.Equals(id));
        }

        public virtual async Task<TModel> FindOneAsync(Expression<Func<TModel, bool>> predicate)
        {
            return await Db.Set<TModel>().FirstOrDefaultAsync(predicate);
        }

        public async Task<IList<TModel>> FindAsync(Expression<Func<TModel, bool>> predicate, int pageSize, int currentPage)
        {
            return await Queryable(predicate, pageSize, currentPage).ToArrayAsync();
        }

        public IQueryable<TModel> Queryable()
        {
            return Db.Set<TModel>();
        }

        public IQueryable<TModel> Queryable(Expression<Func<TModel, bool>> predicate)
        {
            return Db.Set<TModel>().Where(predicate);
        }

        public IQueryable<TModel> Queryable(int page, int pageSize)
        {
            // Handle Page
            if (page <= 0)
                page = 1;       // Page always = 1

            if (pageSize == 0)
                return Db.Set<TModel>();

            return Db.Set<TModel>()
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        public IQueryable<TModel> Queryable(Expression<Func<TModel, bool>> predicate, int page, int pageSize)
        {
            if (page <= 0)
                page = 1;

            if (pageSize == 0)
                return Queryable()
                    .Where(predicate);

            return Queryable(predicate).Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        public virtual async Task<bool> Exists(TModel entity)
        {
            var itemCheck = await FindByIdAsync(entity.Id);
            if (itemCheck != null) return true;
            return false;
        }

        public virtual async Task<TModel> InsertAsync(TModel entity)
        {
            Db.Set<TModel>().Add(entity);
            return await Db.SaveChangesAsync() > 0 ? entity : null;
        }

        public virtual async Task<bool> InsertAsync(IList<TModel> entityList)
        {
            await Db.Set<TModel>().AddRangeAsync(entityList);
            return await Db.SaveChangesAsync() > 0;
        }

        public virtual async Task<TModel> UpdateAsync(TModel entity, bool checkIsExists = true)
        {
            if (checkIsExists)
                if (!await Exists(entity)) throw new Exception("This entity not found, so you cant update.");

            // Passed
            Db.Set<TModel>().Update(entity);

            return await Db.SaveChangesAsync() > 0 ? entity : null;
        }

        public virtual async Task<bool> DeleteAsync(TModel entity, bool checkIsExists = true)
        {
            if (checkIsExists)
                if (!await Exists(entity)) throw new Exception("This entity not found, so you cant deleted.");

            // Passed
            Db.Set<TModel>().Remove(entity);

            return await Db.SaveChangesAsync() > 0;
        }

    }
}
