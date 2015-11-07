using CloudSalesEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CloudSalesBusiness;
using System.Web.Script.Serialization;
using CloudSalesEntity;
using YXERP.Models;

namespace YXERP.Controllers
{
    public class CustomerController : BaseController
    {
        //
        // GET: /Customer/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MyCustomer()
        {
            ViewBag.Title = "我的客户";
            ViewBag.Type = (int)EnumSearchType.Myself;
            ViewBag.Stages = SystemBusiness.BaseBusiness.GetCustomStages(CurrentUser.AgentID, CurrentUser.ClientID);
            return View("Customers");
        }
        public ActionResult BranchCustomer()
        {
            ViewBag.Title = "下属客户";
            ViewBag.Type = (int)EnumSearchType.Branch;
            ViewBag.Stages = SystemBusiness.BaseBusiness.GetCustomStages(CurrentUser.AgentID, CurrentUser.ClientID);
            return View("Customers");
        }
        public ActionResult Customers()
        {
            ViewBag.Title = "所有客户";
            ViewBag.Type = (int)EnumSearchType.All;
            ViewBag.Stages = SystemBusiness.BaseBusiness.GetCustomStages(CurrentUser.AgentID, CurrentUser.ClientID);
            return View("Customers");
        }

        public ActionResult CreateCustomer(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                ViewBag.Sources = new SystemBusiness().GetCustomSources(CurrentUser.AgentID, CurrentUser.ClientID).Where(m => m.IsChoose == 1).ToList();
            }
            else
            {
                ViewBag.Sources = new SystemBusiness().GetCustomSources(CurrentUser.AgentID, CurrentUser.ClientID);
            }
            ViewBag.ActivityID = id;
            ViewBag.Industrys = CloudSalesBusiness.Manage.IndustryBusiness.GetIndustrys();
            ViewBag.Extents = CustomBusiness.GetExtents();
            return View();
        }

        #region Ajax


        public JsonResult GetCustomerSources()
        {
            var list = new SystemBusiness().GetCustomSources(CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveCustomer(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            CustomerEntity model = serializer.Deserialize<CustomerEntity>(entity);

            if (string.IsNullOrEmpty(model.CustomerID))
            {
                model.CustomerID = new CustomBusiness().CreateCustomer(model.Name, model.Type, model.SourceID, model.ActivityID, model.IndustryID, model.Extent, model.CityCode,
                                                                       model.Address, model.ContactName, model.MobilePhone, model.OfficePhone, model.Email, model.Jobs, model.Description, CurrentUser.UserID, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            }
            else
            {
                bool bl = false; //new OrganizationBusiness().UpdateDepartment(model.DepartID, model.Name, model.Description, CurrentUser.UserID, OperateIP, CurrentUser.AgentID);
                if (!bl)
                {
                    model.CustomerID = "";
                }
            }
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetCustomers(string filter)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FilterCustomer model = serializer.Deserialize<FilterCustomer>(filter);
            int totalCount = 0;
            int pageCount = 0;

            List<CustomerEntity> list = CustomBusiness.BaseBusiness.GetCustomers(model.SearchType, model.Type, model.SourceID, model.StageID, model.Status, model.Mark, model.UserID, model.TeamID, model.AgentID, model.BeginTime, model.EndTime, model.Keywords, model.PageSize, model.PageIndex, ref totalCount, ref pageCount, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateCustomMark(string ids, int mark)
        {
            bool bl = false;
            string[] list = ids.Split(',');
            foreach (var id in list)
            {
                if (!string.IsNullOrEmpty(id) && CustomBusiness.BaseBusiness.UpdateCustomerMark(id, mark, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID))
                {
                    bl = true;
                }
            }

            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateCustomOwner(string ids, string userid)
        {
            bool bl = false;
            string[] list = ids.Split(',');
            foreach (var id in list)
            {
                if (!string.IsNullOrEmpty(id) && CustomBusiness.BaseBusiness.UpdateCustomerOwner(id, userid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID))
                {
                    bl = true;
                }
            }


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
