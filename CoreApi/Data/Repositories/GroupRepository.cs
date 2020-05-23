using CoreApi.Data.Repositories.Base;
using CoreApi.Models;
using Microsoft.AspNetCore.Http;

namespace CoreApi.Data.Repositories
{
    public interface IGroupRepository : IRepository<Group, string>
    {

    }

    public class GroupRepository : BaseRepository<Group, string>, IGroupRepository
    {
        public GroupRepository(IHttpContextAccessor httpContext, ApplicationDbContext db) : base(httpContext, db)
        {
        }


    }
}
