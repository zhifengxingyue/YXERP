using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using CloudSalesEntity;
using YXERP.Models;
using CloudSalesBusiness;
using CloudSalesEnum;

namespace YXERP.Controllers
{
    public class OrdersController : BaseController
    {
        //
        // GET: /Orders/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult MyOrder()
        {
            ViewBag.Title = "我的订单";
            ViewBag.Type = (int)EnumSearchType.Myself;
            return View("Orders");
        }
        public ActionResult BranchOrder()
        {
            ViewBag.Title = "下属订单";
            ViewBag.Type = (int)EnumSearchType.Branch;
            return View("Orders");
        }
        public ActionResult Orders()
        {
            ViewBag.Title = "所有订单";
            ViewBag.Type = (int)EnumSearchType.All;
            return View("Orders");
        }

        public ActionResult ChooseProducts(string id)
        {
            ViewBag.Type = (int)EnumDocType.Order;
            ViewBag.GUID = id;
            ViewBag.Title = "新建订单-选择产品";
            return View("FilterProducts");
        }

        #region Ajax

        public JsonResult GetOrders(string filter)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FilterOrders model = serializer.Deserialize<FilterOrders>(filter);
            int totalCount = 0;
            int pageCount = 0;

            var list = OrdersBusiness.BaseBusiness.GetOrders(model.SearchType, model.TypeID, model.Status, model.UserID, model.TeamID, model.AgentID, model.BeginTime, model.EndTime, model.Keywords, model.PageSize, model.PageIndex, ref totalCount, ref pageCount, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult CreateOrder(string customerid)
        {
            string orderid = OrdersBusiness.BaseBusiness.CreateOrder(customerid, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("id", orderid);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

    }
}
