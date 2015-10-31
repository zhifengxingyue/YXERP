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

        public ActionResult Activitys()
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
        /// 获取活动列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetActivityList(string keyWords, int pageSize, int pageIndex, int isAll, string beginTime, string endTime, int stage, int filterType)
        {
            int pageCount = 0;
            int totalCount = 0;
            string userID=CurrentUser.UserID;
            if (isAll == 1)
                userID = string.Empty;

            List<ActivityEntity> list = new ActivityBusiness().GetActivitys(userID, (EnumActivityStage)stage,filterType, keyWords,beginTime,endTime, pageSize, pageIndex, ref totalCount, ref pageCount, CurrentUser.AgentID, CurrentUser.ClientID);
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
        /// 获取活动详细信息
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

        public JsonResult GetUserDetail(string id)
        {
            Users model = OrganizationBusiness.GetUserByUserID(id, CurrentUser.AgentID);
            JsonDictionary.Add("Item", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 保存活动
        /// </summary>
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
        /// 删除活动
        /// </summary>
        /// <returns></returns>
        public JsonResult DeleteActivity(string activityID)
        {
            bool bl = new ActivityBusiness().DeleteActivity(activityID);
            JsonDictionary.Add("Result", bl?1:0);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            return new JsonResult();
        }

        #endregion

    }
}
