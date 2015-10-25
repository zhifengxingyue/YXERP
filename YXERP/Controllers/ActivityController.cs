using CloudSalesBusiness;
using CloudSalesEntity;
using CloudSalesEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace YXERP.Controllers
{

    public class ActivityController : BaseController
    {
        #region view
        public ActionResult Index()
        {
            return Redirect("/Activity/MyActivity");
        }

        public ActionResult MyActivity()
        {
            return View();
        }

        public ActionResult Detail(string id)
        {
            ViewBag.ActivityID = id ?? string.Empty;
            return View();
        }
        #endregion

        #region ajax
        /// <summary>
        /// 保存品牌
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        public JsonResult SavaActivity(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ActivityEntity model = serializer.Deserialize<ActivityEntity>(entity);

            string activityID = "";
            model.OwnerID = model.OwnerID.Trim('|');
            model.MemberID = model.MemberID.Trim('|');
            if (string.IsNullOrEmpty(model.ActivityID))
            {
                activityID = new ActivityBusiness().CreateActivity(model.Name, model.Poster, model.BeginTime.ToString(), model.EndTime.ToString(), model.Address, model.OwnerID,model.MemberID, model.Remark, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            }
            else
            {
                bool bl = new ActivityBusiness().UpdateActivity(model.ActivityID, model.Name,model.Poster, model.BeginTime.ToString(), model.EndTime.ToString(), model.Address, model.Remark,model.OwnerID,model.MemberID, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);

                if (bl)
                {
                    activityID = model.ActivityID;
                }
            }

            JsonDictionary.Add("ID", activityID);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取品牌列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetActivityList(string keyWords, int pageSize, int pageIndex)
        {
            int pageCount = 0;
            int totalCount = 0;
            List<ActivityEntity> list = new ActivityBusiness().GetActivitys(CurrentUser.UserID,EnumActivityStage.All,keyWords, pageSize, pageIndex, ref totalCount, ref pageCount,CurrentUser.AgentID,CurrentUser.ClientID);
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
        /// 编辑品牌状态
        /// </summary>
        /// <returns></returns>
        public JsonResult UpdateActivityStatus(string activityID, int status)
        {
            //bool bl = new ActivityBusiness().UpdateActivityStatus(activityID, (EnumStatus)status, OperateIP, CurrentUser.UserID);
            //JsonDictionary.Add("Status", bl);
            //return new JsonResult
            //{
            //    Data = JsonDictionary,
            //    JsonRequestBehavior = JsonRequestBehavior.AllowGet
            //};

          return  new JsonResult();
        }
        /// <summary>
        /// 删除品牌
        /// </summary>
        /// <returns></returns>
        public JsonResult DeleteActivity(string activityID)
        {
            //bool bl = new ActivityBusiness().(activityID, EnumStatus.Delete, OperateIP, CurrentUser.UserID);
            //JsonDictionary.Add("Status", bl);
            //return new JsonResult
            //{
            //    Data = JsonDictionary,
            //    JsonRequestBehavior = JsonRequestBehavior.AllowGet
            //};

            return new JsonResult();
        }

        /// <summary>
        /// 获取品牌详细信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetActivityDetail(string activityID)
        {
            ActivityEntity model = new ActivityBusiness().GetActivityByID(activityID);
            JsonDictionary.Add("Item", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

    }
}
