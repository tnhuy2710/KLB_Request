using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApi.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CoreApi.Areas.Admin.Controllers.Base
{
    [Area("Admin")]
    [Route("[area]/[controller]")]

    [Authorize(AuthenticationSchemes = SchemeConstants.Cookie)]
    public class BaseController : Controller
    {

    }
}
