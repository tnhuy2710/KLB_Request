using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreApi.Data.Repositories.Base
{
    /// <summary>
    /// IRepository
    /// </summary>
    /// <typeparam name="T">Class Type</typeparam>
    /// <typeparam name="TC">Id Type</typeparam>
    public interface IRepository<T, in TC>
    {
        /// <summary>
        /// Get all items async
        /// </summary>
        /// <returns></returns>
        Task<IList<T>> GetAllAsync();

        /// <summary>
        /// Find an object by id.
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        Task<T> FindByIdAsync(TC entityId);

        /// <summary>
        /// Find an object by queries predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<T> FindOneAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Find objects by queries predicate.
        /// </summary>
        /// <param name="predicate">Query </param>
        /// <param name="pageSize">Total items in result</param>
        /// <param name="page">Current Page want to get items</param>
        /// <returns></returns>
        Task<IList<T>> FindAsync(Expression<Func<T, bool>> predicate, int page, int pageSize);

        /// <summary>
        /// Create Queryable without predicate.
        /// </summary>
        /// <returns></returns>
        IQueryable<T> Queryable();


        /// <summary>
        /// Create Queryable predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IQueryable<T> Queryable(Expression<Func<T, bool>> predicate);


        /// <summary>
        /// Create Queryable without predicate.
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        IQueryable<T> Queryable(int pageSize, int currentPage);


        /// <summary>
        /// Create Queryable predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageSize"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        IQueryable<T> Queryable(Expression<Func<T, bool>> predicate, int page, int pageSize);



        /// <summary>
        /// Check entity is exists or not. (Default check id)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> Exists(T entity);

        /// <summary>
        /// Insert object to database.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<T> InsertAsync(T entity);

        /// <summary>
        /// Insert list of object to database.
        /// </summary>
        /// <param name="entityList"></param>
        /// <returns></returns>
        Task<bool> InsertAsync(IList<T> entityList);

        /// <summary>
        /// Update object data to database
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="checkIsExists">Auto check entity exists or not.</param>
        /// <returns></returns>
        Task<T> UpdateAsync(T entity, bool checkIsExists = true);

        /// <summary>
        /// Delete object 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="checkIsExists"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(T entity, bool checkIsExists = true);
    }
}
