using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using YXERP.Models;

using CloudSalesBusiness;
using CloudSalesEntity;

namespace YXERP.Controllers
{
    public class FinanceController : BaseController
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

        public ActionResult PayableBillsDetail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect("PayableBills");
            }
            ViewBag.ID = id;
            return View();
        }

        #region Ajax

        public JsonResult GetPayableBills(string filter)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FilterBills model = serializer.Deserialize<FilterBills>(filter);
            int totalCount = 0;
            int pageCount = 0;

            var list = FinanceBusiness.BaseBusiness.GetPayableBills(model.PayStatus, model.InvoiceStatus, model.BeginTime, model.EndTime, model.Keywords, model.PageSize, model.PageIndex, ref totalCount, ref pageCount, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetPayableBillByID(string id)
        {
            var model = FinanceBusiness.BaseBusiness.GetPayableBillByID(id, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveStorageBillingPay(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            StorageBillingPay model = serializer.Deserialize<StorageBillingPay>(entity);

            bool bl = FinanceBusiness.BaseBusiness.CreateStorageBillingPay(model.BillingID, model.Type, model.PayType, model.PayMoney, model.PayTime, model.Remark, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            if (bl)
            {
                model.CreateUser = CurrentUser;
                JsonDictionary.Add("item", model);
            }
            
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveStorageBillingInvoice(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            StorageBillingInvoice model = serializer.Deserialize<StorageBillingInvoice>(entity);

            var id = FinanceBusiness.BaseBusiness.CreateStorageBillingInvoice(model.BillingID, model.Type, model.InvoiceMoney, model.InvoiceCode, model.Remark, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            model.InvoiceID = id;
            model.CreateUser = CurrentUser;
            JsonDictionary.Add("item", model);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteStorageBillingInvoice(string id, string billingid)
        {
            var bl = FinanceBusiness.BaseBusiness.DeleteStorageBillingInvoice(id, billingid, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion
    }
}
