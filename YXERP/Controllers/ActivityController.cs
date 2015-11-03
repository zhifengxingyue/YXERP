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

        public ActionResult Operate(string id)
        {
            ViewBag.ActivityID = id ?? string.Empty;
            return View();
        }

        public ActionResult Detail(string id)
        {
            ViewBag.ActivityID = id ?? string.Empty;
            return View();
        }
        #endregion

        #region ajax

        #region 活动
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

            List<ActivityEntity> list =ActivityBusiness.GetActivitys(userID, (EnumActivityStage)stage,filterType, keyWords,beginTime,endTime, pageSize, pageIndex, ref totalCount, ref pageCount, CurrentUser.AgentID, CurrentUser.ClientID);
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
            ActivityEntity model =ActivityBusiness.GetActivityByID(activityID);
            JsonDictionary.Add("Item", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取用户详情
        /// </summary>
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
                activityID =ActivityBusiness.CreateActivity(model.Name, model.Poster, model.BeginTime.ToString(), model.EndTime.ToString(), model.Address, model.OwnerID,model.MemberID, model.Remark, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            }
            else
            {
                bool bl =ActivityBusiness.UpdateActivity(model.ActivityID, model.Name,model.Poster, model.BeginTime.ToString(), model.EndTime.ToString(), model.Address, model.Remark,model.OwnerID,model.MemberID, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);

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
        }
        #endregion

        #region 活动讨论
        /// <summary>
        /// 保存活动讨论
        /// </summary>
        public JsonResult SavaActivityReply(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ActivityReplyEntity model = serializer.Deserialize<ActivityReplyEntity>(entity);

            string replyID = "";
            replyID = ActivityBusiness.CreateActivityReply(model.Msg, model.FromReplyID, CurrentUser.UserID, CurrentUser.AgentID, model.FromReplyUserID, model.FromReplyAgentID);


            JsonDictionary.Add("ID", replyID);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取活动讨论列表
        /// </summary>
        public JsonResult GetActivityReplys(string activityID, int pageSize, int pageIndex)
        {
            int pageCount = 0;
            int totalCount = 0;

            List<ActivityReplyEntity> list = ActivityBusiness.GetActivityReplys(activityID, pageSize, pageIndex, ref totalCount, ref pageCount);
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
        /// 删除活动讨论
        /// </summary>
        /// <returns></returns>
        public JsonResult DeleteActivityReply(string activityID)
        {
            bool bl =ActivityBusiness.DeleteActivityReply(activityID);
            JsonDictionary.Add("Result", bl ? 1 : 0);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }

        #endregion

        #region 活动对明道打通
        /// <summary>
        /// 将活动分享到明道任务
        /// </summary>
        public JsonResult ShareTask(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ActivityEntity model = serializer.Deserialize<ActivityEntity>(entity);

            int error_code;
            model.OwnerID = model.OwnerID.Trim('|');
            string ownerID = OrganizationBusiness.GetUserByUserID(model.OwnerID, CurrentUser.AgentID).MDUserID;
            model.MemberID = model.MemberID.Trim('|');
            List<string> members=new List<string>();
            foreach(var m in model.MemberID.Split('|')){
            members.Add(OrganizationBusiness.GetUserByUserID(m,CurrentUser.AgentID).MDUserID);
            }

            string id = MD.SDK.TaskBusiness.AddTask(CurrentUser.MDToken, model.Name, ownerID, members, model.EndTime.Date.ToString(), string.Empty,model.Remark, out error_code);

            JsonDictionary.Add("Result", !string.IsNullOrEmpty(id) ? 1 : 0);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 将活动分享到明道日程
        /// </summary>
        public JsonResult ShareCalendar(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ActivityEntity model = serializer.Deserialize<ActivityEntity>(entity);

            string activityID = string.Empty;
            int error_code;
            model.OwnerID = model.OwnerID.Trim('|');
            string ownerID = OrganizationBusiness.GetUserByUserID(model.OwnerID, CurrentUser.AgentID).MDUserID;
            model.MemberID = model.MemberID.Trim('|');
            List<string> members = new List<string>();
            foreach (var m in model.MemberID.Split('|'))
            {
                members.Add(OrganizationBusiness.GetUserByUserID(m, CurrentUser.AgentID).MDUserID);
            }

            string id = MD.SDK.CalendarBusiness.AddCalendar(CurrentUser.MDToken, model.Name, members,model.Address,model.Remark,model.BeginTime.Date.ToString(), model.EndTime.Date.ToString(), out error_code);

            JsonDictionary.Add("Result", !string.IsNullOrEmpty(id) ? 1 : 0);
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
