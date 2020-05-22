using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApi.Areas.Admin.Controllers.Base;
using CoreApi.Security.Attributes;
using CoreApi.Security.AuthorizationPolicies.RolePolicy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using CoreApi.Areas.Admin.Models;

namespace CoreApi.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {

      
        [HttpGet()]
        [Route("aa")]
        public ActionResult Index()
        {
           
            var tt= Guid.NewGuid().ToString();

            return View("Index");
        }
        
        
        [HttpGet()]
        [Route("bb")]
        public ActionResult UpdateIndex()
        {

            return View("Index");
        }
        public ActionResult _LeftMenuPartial()
        {
            List<Function> list = new List<Function>();
            list.Add(new Function { Link = "/aa/index1", LinkName = "Home" });
            list.Add(new Function { Link = "/aa/index2", LinkName = "Home" });
            list.Add(new Function { Link = "/aa/index3", LinkName = "Home" });
            return PartialView("_LeftMenuPartial", list);
        }
    }
}
