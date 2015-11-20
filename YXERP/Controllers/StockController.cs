using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CloudSalesBusiness;

namespace YXERP.Controllers
{
    public class StockController : BaseController
    {
        //
        // GET: /Stock/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Stocks()
        {
            return View();
        }


        #region Ajax

        public JsonResult GetProductStocks(string Keywords, int PageIndex, int PageSize)
        {
            int totalCount = 0;
            int pageCount = 0;

            var list = StockBusiness.BaseBusiness.GetProductStocks(Keywords, PageSize, PageIndex, ref totalCount, ref pageCount, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetProductDetailStocks(string productid)
        {
            var list = StockBusiness.BaseBusiness.GetProductDetailStocks(productid, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion
    }
}
