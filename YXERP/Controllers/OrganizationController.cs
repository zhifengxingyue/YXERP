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
            return View();
        }

        public ActionResult RolePermission(string id)
        {
            ViewBag.Model = OrganizationBusiness.GetRoleByID(id, CurrentUser.AgentID);
            ViewBag.Menus = CommonBusiness.ClientMenus.Where(m => m.PCode == ExpandClass.CLIENT_TOP_CODE).ToList();
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

            if (string.IsNullOrEmpty(model.DepartID))
            {
                model.DepartID = new OrganizationBusiness().CreateDepartment(model.Name, model.ParentID, model.Description, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            }
            else
            {
                bool bl = new OrganizationBusiness().UpdateDepartment(model.DepartID, model.Name, model.Description, CurrentUser.UserID, OperateIP, CurrentUser.AgentID);
                if (!bl)
                {
                    model.DepartID = "";
                }
            }
            JsonDictionary.Add("model", model);
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
            var status = new OrganizationBusiness().UpdateDepartmentStatus(departid, CloudSalesEnum.EnumStatus.Delete, CurrentUser.UserID, OperateIP, CurrentUser.AgentID);
            JsonDictionary.Add("status", (int)status);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRoles()
        {
            var list = OrganizationBusiness.GetRoles(CurrentUser.AgentID);
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 保存角色
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult SaveRole(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Role model = serializer.Deserialize<Role>(entity);

            if (string.IsNullOrEmpty(model.RoleID))
            {
                model.RoleID = new OrganizationBusiness().CreateRole(model.Name, model.ParentID, model.Description, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            }
            else
            {
                bool bl = new OrganizationBusiness().UpdateRole(model.RoleID, model.Name, model.Description, CurrentUser.UserID, OperateIP, CurrentUser.AgentID);
                if (!bl)
                {
                    model.RoleID = "";
                }
            }
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="roleid"></param>
        /// <returns></returns>
        public JsonResult DeleteRole(string roleid)
        {
            int result = 0;
            bool bl = new OrganizationBusiness().DeleteRole(roleid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, out result);
            JsonDictionary.Add("status", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 保存角色权限
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public JsonResult SaveRolePermission(string roleid, string permissions)
        {
            if (permissions.Length > 0)
            {
                permissions = permissions.Substring(0, permissions.Length - 1);
               
            }
            bool bl = new OrganizationBusiness().UpdateRolePermission(roleid, permissions, CurrentUser.UserID, OperateIP);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取明道用户列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetMDUsers()
        {
            var list = MD.SDK.UserBusiness.GetUserAll(CurrentUser.MDToken, "", 1, 1000);
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

    }
}
