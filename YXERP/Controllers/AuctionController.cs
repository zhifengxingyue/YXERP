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
        /// <summary>
        /// 获取产品列表
        /// </summary>
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

        /// <summary>
        /// 根据人数、年数获取最佳产品组合
        /// </summary>
        public JsonResult GetBestWay(int quantity,int periodQuantity) 
        {
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

        /// <summary>
        /// 根据人数、年数生成客户订单
        /// </summary>
        public JsonResult AddClientOrder(int quantity, int periodQuantity)
        {
            int pageCount = 0;
            int totalCount = 0;
            List<ModulesProduct> list = ModulesProductBusiness.GetModulesProducts(string.Empty, int.MaxValue, 1, ref totalCount, ref pageCount);

            //获取订单产品的最佳组合
            var way = ModulesProductBusiness.GetBestWay(quantity, list.OrderByDescending(m => m.UserQuantity).Where(m => m.PeriodQuantity == periodQuantity).ToList());

            //获取订单参数
            ClientOrder model=new ClientOrder();
            model.UserQuantity=way.TotalQuantity;
            model.Years=periodQuantity;
            model.Amount=way.TotalMoney;
            model.RealAmount = way.TotalMoney;
            model.AgentID=CurrentUser.AgentID;
            model.ClientID=CurrentUser.ClientID;
            model.CreateUserID=CurrentUser.UserID;

            model.Details=new List<ClientOrderDetail>();
            foreach (var p in way.Products) 
            {
                ClientOrderDetail detail = new ClientOrderDetail();
                detail.ProductID = p.Key;
                detail.Qunatity = p.Value;
                detail.CreateUserID = CurrentUser.CreateUserID;
                detail.Price = list.Find(m => m.ProductID == p.Key).Price;
                model.Details.Add(detail);
            }

            string orderID= ClientOrderBusiness.AddClientOrder(model);
            JsonDictionary.Add("ID", orderID);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion

    }
}
