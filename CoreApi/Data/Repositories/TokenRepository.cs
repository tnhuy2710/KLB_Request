using System.Threading.Tasks;
using CoreApi.Data.Repositories.Base;
using CoreApi.Models;
using Microsoft.AspNetCore.Http;

namespace CoreApi.Data.Repositories
{
    public interface ITokenRepository : IRepository<Token, int>
    {
        Task<Token> FindByValueAsync(string value);
    }

    public class TokenRepository : BaseRepository<Token, int>, ITokenRepository
    {
        public TokenRepository(IHttpContextAccessor httpContext, ApplicationDbContext db) : base(httpContext, db)
        {
        }


        public async Task<Token> FindByValueAsync(string value)
        {
            return await FindOneAsync(x => x.Value.Equals(value.Trim()));
        }
    }
}
