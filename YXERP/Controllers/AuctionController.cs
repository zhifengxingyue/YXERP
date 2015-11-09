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

        public JsonResult GetBestWay(int quantity,int periodQuantity) 
        {
            //List<ProductEntity> list = new List<ProductEntity>();
            //list.Add(new ProductEntity() { ID = "1", Name = "5人", Quantity = 5, Price = 500 });
            //list.Add(new ProductEntity() { ID = "2", Name = "10人", Quantity = 10, Price = 950 });
            //list.Add(new ProductEntity() { ID = "3", Name = "20人", Quantity = 20, Price = 1800 });
            //list.Add(new ProductEntity() { ID = "4", Name = "50人", Quantity = 50, Price = 4000 });
            //list.Add(new ProductEntity() { ID = "5", Name = "100人", Quantity = 100, Price = 7000 });

            int pageCount = 0;
            int totalCount = 0;
            List<ModulesProduct> list = ModulesProductBusiness.GetModulesProducts(string.Empty, int.MaxValue, 1, ref totalCount, ref pageCount);
            var way = ModulesProductBusiness.GetBestWay(quantity, list.OrderByDescending(m => m.UserQuantity).Where(m => m.PeriodQuantity ==periodQuantity).ToList());

            List<Dictionary<string, string>> products = new List<Dictionary<string, string>>();
            foreach (var p in way.Products) 
            {
                Dictionary<string, string> product = new Dictionary<string, string>();
                product.Add("id", p.Key);
                product.Add("count",p.Value.ToString());
                products.Add(product);
            }

            JsonDictionary.Add("Items", products);
            JsonDictionary.Add("TotalMoney", way.TotalMoney);
            JsonDictionary.Add("TotalQuantity", way.TotalQuantity);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion

    }
}
