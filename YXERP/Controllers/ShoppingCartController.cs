﻿using CloudSalesBusiness;
using CloudSalesEntity;
using CloudSalesEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using YXERP.Models;

namespace YXERP.Controllers
{
    public class ShoppingCartController : BaseController
    {
        //
        // GET: /ShoppingCart/

        public ActionResult Index()
        {
            return View();
        }

        #region Ajax 订单和购物车相关

        /// <summary>
        /// 过滤产品
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public JsonResult GetProductListForShopping(string filter)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FilterProduct model = serializer.Deserialize<FilterProduct>(filter);
            int totalCount = 0;
            int pageCount = 0;

            List<Products> list = new ProductsBusiness().GetFilterProducts(model.CategoryID, model.Attrs, model.DocType, model.BeginPrice, model.EndPrice, model.Keywords, model.OrderBy, model.IsAsc, 20, model.PageIndex, ref totalCount, ref pageCount, CurrentUser.ClientID);
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
        /// 加入到购物车
        /// </summary>
        /// <param name="productid"></param>
        /// <param name="detailsid"></param>
        /// <param name="quantity"></param>
        /// <param name="ordertype"></param>
        /// <returns></returns>
        public JsonResult AddShoppingCart(string productid, string detailsid, int quantity, string unitid, int isBigUnit, EnumDocType ordertype, string remark = "", string guid = "")
        {
            var bl = ShoppingCartBusiness.AddShoppingCart(productid, detailsid, quantity, unitid, isBigUnit, ordertype, remark, guid, CurrentUser.UserID, OperateIP);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        /// <summary>
        /// 获取购物车产品数
        /// </summary>
        /// <param name="ordertype"></param>
        /// <returns></returns>
        public JsonResult GetShoppingCartCount(EnumDocType ordertype, string guid = "")
        {
            if (string.IsNullOrEmpty(guid))
            {
                guid = CurrentUser.UserID;
            }
            var count = ShoppingCartBusiness.GetShoppingCartCount(ordertype, guid);
            JsonDictionary.Add("Quantity", count);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        /// <summary>
        /// 获取购物车产品
        /// </summary>
        /// <param name="ordertype"></param>
        /// <returns></returns>
        public JsonResult GetShoppingCart(EnumDocType ordertype, string guid = "")
        {
            if (string.IsNullOrEmpty(guid))
            {
                guid = CurrentUser.UserID;
            }

            var list = ShoppingCartBusiness.GetShoppingCart(ordertype, guid);
            JsonDictionary.Add("Items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 编辑购物车产品数量
        /// </summary>
        /// <param name="autoid"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public JsonResult UpdateCartQuantity(string autoid, int quantity)
        {
            var bl = ShoppingCartBusiness.UpdateCartQuantity(autoid, quantity, CurrentUser.UserID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        /// <summary>
        /// 删除购物车产品
        /// </summary>
        /// <param name="autoid"></param>
        /// <returns></returns>
        public JsonResult DeleteCart(string autoid)
        {
            var bl = ShoppingCartBusiness.DeleteCart(autoid, CurrentUser.UserID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

    }
}
