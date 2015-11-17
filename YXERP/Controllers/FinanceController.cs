using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YXERP.Controllers
{
    public class FinanceController : Controller
    {
        //
        // GET: /Finance/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PayableBills()
        {
            return View();
        }

    }
}
