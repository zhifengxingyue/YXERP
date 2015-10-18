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
    public class OrganizationController : BaseController
    {
        //
        // GET: /Organization/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Department()
        {
            return View();
        }

        public ActionResult Roles()
        {
            ViewBag.Items = OrganizationBusiness.GetRoles(CurrentUser.AgentID);
            return View();
        }

        public ActionResult Users()
        {
            return View();
        }

        #region 部门

        /// <summary>
        /// 获取部门列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetDepartments()
        {
            var list = OrganizationBusiness.GetDepartments(CurrentUser.AgentID);
            JsonDictionary.Add("items", list);
            return new JsonResult() 
            {
                Data = JsonDictionary,
                JsonRequestBehavior=JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 保存部门
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult SaveDepartment(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Department model = serializer.Deserialize<Department>(entity);

            string ID = "";
            if (string.IsNullOrEmpty(model.DepartID))
            {
                ID = new OrganizationBusiness().CreateDepartment(model.Name, model.ParentID, model.Description, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            }
            else
            {
                bool bl = new OrganizationBusiness().UpdateDepartment(model.DepartID, model.Name, model.Description, CurrentUser.UserID, OperateIP);
                if (bl)
                {
                    ID = model.DepartID;
                }
            }
            JsonDictionary.Add("ID", ID);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="departid"></param>
        /// <returns></returns>
        public JsonResult DeleteDepartment(string departid)
        {
            var status = new OrganizationBusiness().UpdateDepartmentStatus(departid, CloudSalesEnum.EnumStatus.Delete, CurrentUser.UserID, OperateIP);
            JsonDictionary.Add("Status", (int)status);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

    }
}
