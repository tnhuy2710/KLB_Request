using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApi.Areas.Admin.Controllers.Base;
using CoreApi.Security.Attributes;
using CoreApi.Security.AuthorizationPolicies.RolePolicy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreApi.Areas.Admin.Controllers
{
    public class DanhGiaController :BaseController
    {
        [HttpGet]
        [Route("a")]
        public IActionResult Index()
        {
            return View("index");
        }
        [HttpGet]
        [Route("b")]
        public IActionResult DanhGia()
        {
            return View("index");
        }
    }
}