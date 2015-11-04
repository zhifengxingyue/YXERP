using CloudSalesEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CloudSalesBusiness;
using System.Web.Script.Serialization;
using CloudSalesEntity;

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
            return View();
        }
        public ActionResult BranchCustomer()
        {
            return View("MyCustomer");
        }
        public ActionResult Customers()
        {
            return View("MyCustomer");
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

        #endregion

    }
}
