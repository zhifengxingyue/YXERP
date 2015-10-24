using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YXERP.Controllers
{
    public class ActivityController : Controller
    {
        //
        // GET: /Activity/

        public ActionResult Index()
        {
            return Redirect("/Activity/MyActivity");
        }

        public ActionResult MyActivity()
        {
            return View();
        }

    }
}
