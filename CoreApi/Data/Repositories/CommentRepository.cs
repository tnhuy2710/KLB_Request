using CoreApi.Data.Repositories.Base;
using CoreApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreApi.Data.Repositories
{
    public interface ICommentRepository : IRepository<Comment, string>
    {        
        Task<bool> InsertInto(Comment cOMMENT);
        Task<bool> Update(Comment cOMMENT);
        Task<bool> Delete(Comment cOMMENT);
        Task<IList<Comment>> GetCommentByFormID(string formID);
        Task<Comment> GetCommentByID(int ID);
        Task<Comment> GetCommentByDateComment(DateTimeOffset dateTime);
        Task<Comment> FindCommentByReplyID(string replyID);
    }

    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContext;
        public CommentRepository(IHttpContextAccessor httpContext, ApplicationDbContext db)
        {
            _db = db;
            _httpContext = httpContext;
        }
        public async Task<Comment> GetCommentByID(int ID)
        {
            return await _db.Comments.Where(x => x.Id.Equals(ID))
                                    .FirstOrDefaultAsync();
        }

        public async Task<Comment> GetCommentByDateComment(DateTimeOffset dateTime)
        {
            return await _db.Comments.Where(x => x.DateComment.Equals(dateTime))
                                    .FirstOrDefaultAsync();
        }

        public async Task<IList<Comment>> GetCommentByFormID(string formID)
        {
            return await _db.Comments.Where(x => x.FormID.Equals(formID))
                                    .Include(u => u.User)                                    
                                    .ToListAsync();
        }

        public Task<Comment> FindCommentByReplyID(string replyID)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> InsertInto(Comment cOMMENT)
        {
            if (cOMMENT != null)
            {
                try
                {
                    _db.Comments.Add(cOMMENT);
                    await _db.SaveChangesAsync();
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> Update(Comment cOMMENT)
        {            
            if(cOMMENT != null)
            {                
                try
                {
                    _db.Entry(cOMMENT).State = EntityState.Modified;
                    await _db.SaveChangesAsync();                    
                }
                catch
                {
                    return false;
                }
                return true;
            }            
            return false;
        }

        public async Task<bool> Delete(Comment cOMMENT)
        {                      
            if (cOMMENT != null)
            {                
                try
                {
                    _db.Comments.Remove(cOMMENT);
                    await _db.SaveChangesAsync();
                }
                catch
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        Task<IList<Comment>> IRepository<Comment, string>.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        Task<Comment> IRepository<Comment, string>.FindByIdAsync(string entityId)
        {
            throw new NotImplementedException();
        }

        Task<Comment> IRepository<Comment, string>.FindOneAsync(Expression<Func<Comment, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        Task<IList<Comment>> IRepository<Comment, string>.FindAsync(Expression<Func<Comment, bool>> predicate, int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        IQueryable<Comment> IRepository<Comment, string>.Queryable()
        {
            throw new NotImplementedException();
        }

        IQueryable<Comment> IRepository<Comment, string>.Queryable(Expression<Func<Comment, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        IQueryable<Comment> IRepository<Comment, string>.Queryable(int pageSize, int currentPage)
        {
            throw new NotImplementedException();
        }

        IQueryable<Comment> IRepository<Comment, string>.Queryable(Expression<Func<Comment, bool>> predicate, int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        Task<bool> IRepository<Comment, string>.Exists(Comment entity)
        {
            throw new NotImplementedException();
        }

        Task<Comment> IRepository<Comment, string>.InsertAsync(Comment entity)
        {
            throw new NotImplementedException();
        }

        Task<bool> IRepository<Comment, string>.InsertAsync(IList<Comment> entityList)
        {
            throw new NotImplementedException();
        }

        Task<Comment> IRepository<Comment, string>.UpdateAsync(Comment entity, bool checkIsExists)
        {
            throw new NotImplementedException();
        }

        Task<bool> IRepository<Comment, string>.DeleteAsync(Comment entity, bool checkIsExists)
        {
            throw new NotImplementedException();
        }        
    }
}
