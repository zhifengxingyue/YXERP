﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CloudSalesBusiness.Manage;
using System.Web.Script.Serialization;
using CloudSalesEntity.Manage;
namespace YXManage.Controllers
{
    [YXManage.Common.UserAuthorize]
    public class ExpressCompanyController:BaseController
    {
        #region view
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Detail(string id)
        {
            ViewBag.ExpressID =id;

            return View();
        }
        #endregion

        #region ajax
        public JsonResult GetExpressCompanys(int pageIndex, string keyWords)
        {
            int totalCount = 0, pageCount = 0;
            var list = ExpressCompanyBusiness.GetExpressCompanys(keyWords, PageSize, pageIndex, ref totalCount, ref pageCount);
            JsonDictionary.Add("Items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetExpressCompanyDetail(string id)
        {
            var item = ExpressCompanyBusiness.GetExpressCompanyDetail(id);
            JsonDictionary.Add("Item", item);
            JsonDictionary.Add("Result", 1);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveExpressCompany(string expressCompany)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ExpressCompany model = serializer.Deserialize<ExpressCompany>(expressCompany);

            bool flag = false;
            if (model.AutoID == 0)
            {
                model.CreateUserID = string.Empty;
                flag = ExpressCompanyBusiness.InsertExpressCompany(model);
            }
            else
            {
                model.CreateUserID = string.Empty;
                flag = ExpressCompanyBusiness.UpdateExpressCompany(model);
            }
            JsonDictionary.Add("Result", flag ? 1 : 0);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteExpressCompany(string id)
        {
            bool flag = ExpressCompanyBusiness.DeleteExpressCompany(id);
            JsonDictionary.Add("Result", flag ? 1 : 0);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion

    }
}
