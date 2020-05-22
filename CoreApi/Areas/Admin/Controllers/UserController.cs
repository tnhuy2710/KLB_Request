using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApi.Areas.Admin.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using CoreApi.Areas.Admin.Models;
namespace CoreApi.Areas.Admin.Controllers
{
    public class UserController : BaseController
    {
        public IActionResult Index()
        {

            return View();
        }
        public ActionResult _LeftMenuPartial()
        {
            List<Function> list = new List<Function>();
            list.Add(new Function { Link="/aa/index1",LinkName="Home"});
            list.Add(new Function { Link = "/aa/index2", LinkName = "Home" });
            list.Add(new Function { Link = "/aa/index3", LinkName = "Home" });
            return PartialView("_LeftMenuPartial", list) ;
        }
    }
}
