using CoreApi.Data.Repositories.Base;
using CoreApi.Models;
using Microsoft.AspNetCore.Http;

namespace CoreApi.Data.Repositories
{
    public interface IOptionRepository : IRepository<Option, int>
    {

    }

    public class OptionRepository : BaseRepository<Option, int>, IOptionRepository
    {
        public OptionRepository(IHttpContextAccessor httpContext, ApplicationDbContext db) : base(httpContext, db)
        {
        }


    }
}
