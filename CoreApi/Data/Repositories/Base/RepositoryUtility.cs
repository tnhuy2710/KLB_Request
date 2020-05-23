using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CoreApi.Data.Repositories.Base
{
    public class RepositoryUtility
    {

        protected readonly IHttpContextAccessor HttpContext;

        public RepositoryUtility(IHttpContextAccessor httpContext)
        {
            this.HttpContext = httpContext;
        }


    }
}
