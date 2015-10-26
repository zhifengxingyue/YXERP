using CloudSalesBusiness;
using CloudSalesEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace YXERP.Controllers
{
    public class SystemController : BaseController
    {
        //
        // GET: /System/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Sources()
        {
            return View();
        }

        #region Ajax

        #region 客户来源

        /// <summary>
        /// 获取客户来源列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCustomSources()
        {

            var list = new SystemBusiness().GetCustomSources(CurrentUser.AgentID, CurrentUser.ClientID).Where(m => m.Status == 1).ToList();
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取客户来源实体
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCustomSourceByID(string id)
        {

            var model = new SystemBusiness().GetCustomSourcesByID(id, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 保存客户来源
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult SaveCustomSource(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            CustomSourceEntity model = serializer.Deserialize<CustomSourceEntity>(entity);

            int result = 0;

            if (string.IsNullOrEmpty(model.SourceID))
            {
                model.SourceID = new SystemBusiness().CreateCustomSource(model.SourceCode, model.SourceName, model.IsChoose, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID, out result);
            }
            else
            {
                bool bl = new SystemBusiness().UpdateCustomSource(model.SourceID, model.SourceName, model.IsChoose, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
                if (bl)
                {
                    result = 1;
                }
            }
            JsonDictionary.Add("status", result);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 删除客户来源
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DeleteCustomSource(string id)
        {
            bool bl = new SystemBusiness().DeleteCustomSource(id, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        } 

        #endregion
        #endregion

    }
}
