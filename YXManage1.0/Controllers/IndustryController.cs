﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CloudSalesBusiness.Manage;
using System.Web.Script.Serialization;
using CloudSalesEntity;
using CloudSalesEntity.Manage;
namespace YXManage.Controllers
{
     [YXManage.Common.UserAuthorize]
    public class IndustryController:BaseController
    {
        #region view
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Detail(string id)
        {
            ViewBag.IndustryID = id;

            return View();
        }
        #endregion

        #region ajax
        public JsonResult GetIndustrys()
        {
            var list = IndustryBusiness.GetIndustrys();
            JsonDictionary.Add("Items", list);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetIndustryDetail(string id)
        {
            var item = IndustryBusiness.GetIndustryDetail(id);
            JsonDictionary.Add("Item", item);
            JsonDictionary.Add("Result", 1);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveIndustry(string industry)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Industry model = serializer.Deserialize<Industry>(industry);

            bool flag = false;
            if (string.IsNullOrEmpty( model.IndustryID) )
            {
                model.CreateUserID = string.Empty;
                flag = !string.IsNullOrEmpty(IndustryBusiness.InsertIndustry(model.Name, model.Description, CurrentUser.UserID, string.Empty)) ? true : false;
            }
            else
            {
                model.CreateUserID = string.Empty;
                flag = IndustryBusiness.UpdateIndustry(model.IndustryID, model.Name, model.Description);
            }
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
