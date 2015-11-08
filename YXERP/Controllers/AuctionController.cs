using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CloudSalesEntity.Manage;
using CloudSalesBusiness.Manage;
namespace YXERP.Controllers
{
    public class AuctionController :BaseController
    {
        #region
        public ActionResult BuyNow()
        {
            return View();
        }

        #endregion

        #region ajax
        public JsonResult GetProductList() 
        {
            int pageCount = 0;
            int totalCount = 0;

            List<ModulesProduct> list = ModulesProductBusiness.GetModulesProducts(string.Empty, int.MaxValue, 1, ref totalCount, ref pageCount);

            JsonDictionary.Add("Items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);
            return new JsonResult 
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion

    }
}
