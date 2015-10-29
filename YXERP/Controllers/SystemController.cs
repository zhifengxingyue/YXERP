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

        public ActionResult Stages()
        {
            ViewBag.Items = new SystemBusiness().GetCustomStages(CurrentUser.AgentID, CurrentUser.ClientID);
            return View();
        }

        public ActionResult Teams()
        {
            return View();
        }

        public ActionResult OrderType()
        {
            return View();
        }

        public ActionResult Target()
        {
            return View();
        }

        public ActionResult Warehouse()
        {
            return View();
        }

        public ActionResult DepotSeat(string id = "")
        {
            if (string.IsNullOrEmpty(id)) 
            {
                return Redirect("/System/Warehouse");
            }
            ViewBag.Ware = new SystemBusiness().GetWareByID(id, CurrentUser.ClientID);
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

        #region 客户阶段配置

        public JsonResult SaveCustomStage(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            CustomStageEntity model = serializer.Deserialize<CustomStageEntity>(entity);

            int result = 0;

            if (string.IsNullOrEmpty(model.StageID))
            {
                model.StageID = new SystemBusiness().CreateCustomStage(model.StageName, model.Sort, "", CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID, out result);
            }
            else
            {
                bool bl = new SystemBusiness().UpdateCustomStage(model.StageID, model.StageName, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
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

        public JsonResult SaveStageItem(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            StageItemEntity model = serializer.Deserialize<StageItemEntity>(entity);

            if (string.IsNullOrEmpty(model.ItemID))
            {
                model.ItemID = new SystemBusiness().CreateStageItem(model.ItemName, model.StageID, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            }
            else
            {
                bool bl = new SystemBusiness().UpdateStageItem(model.ItemID, model.ItemName, model.StageID, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
                if (!bl)
                {
                    model.ItemID = "";
                }
            }
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteCustomStage(string id)
        {
            bool bl = new SystemBusiness().DeleteCustomStage(id, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteStageItem(string id, string stageid)
        {
            bool bl = new SystemBusiness().DeleteStageItem(id, stageid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        } 

        #endregion

        #region 订单类型

        public JsonResult GetOrderTypes()
        {

            var list = new SystemBusiness().GetOrderTypes(CurrentUser.AgentID, CurrentUser.ClientID).ToList();
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOrderTypeByID(string id)
        {

            var model = new SystemBusiness().GetOrderTypeByID(id, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveOrderType(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            OrderTypeEntity model = serializer.Deserialize<OrderTypeEntity>(entity);

            if (string.IsNullOrEmpty(model.TypeID))
            {
                model.TypeID = new SystemBusiness().CreateOrderType(model.TypeName, model.TypeCode, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            }
            else
            {
                bool bl = new SystemBusiness().UpdateOrderType(model.TypeID, model.TypeName, model.TypeCode, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
                if (!bl)
                {
                    model.TypeID = "";
                }
            }
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteOrderType(string id)
        {
            bool bl = new SystemBusiness().DeleteOrderType(id, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region 仓库货位

        /// <summary>
        /// 获取仓库列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetWareHouses(string keyWords, int pageSize, int pageIndex, int totalCount)
        {
            int pageCount = 0;
            List<WareHouse> list = new SystemBusiness().GetWareHouses(keyWords, pageSize, pageIndex, ref totalCount, ref pageCount, CurrentUser.ClientID);
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
        /// 根据ID获取仓库
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetWareHouseByID(string id)
        {
            WareHouse model = new SystemBusiness().GetWareByID(id, CurrentUser.ClientID);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 保存仓库
        /// </summary>
        /// <param name="ware"></param>
        /// <returns></returns>
        public JsonResult SaveWareHouse(string ware)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            WareHouse model = serializer.Deserialize<WareHouse>(ware);

            string id = string.Empty;
            if (string.IsNullOrEmpty(model.WareID))
            {
                id = new SystemBusiness().AddWareHouse(model.WareCode, model.Name, model.ShortName, model.CityCode, model.Status.Value, model.Description, CurrentUser.UserID, CurrentUser.ClientID);
            }
            else if (new SystemBusiness().UpdateWareHouse(model.WareID, model.WareCode, model.Name, model.ShortName, model.CityCode, model.Status.Value, model.Description, CurrentUser.UserID, CurrentUser.ClientID))
            {
                id = model.WareID;
            }


            JsonDictionary.Add("ID", id);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 编辑仓库状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public JsonResult UpdateWareHouseStatus(string id, int status)
        {
            bool bl = new SystemBusiness().UpdateWareHouseStatus(id, (CloudSalesEnum.EnumStatus)status, CurrentUser.UserID, CurrentUser.ClientID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        /// <summary>
        /// 删除仓库
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DeleteWareHouse(string id)
        {
            bool bl = new SystemBusiness().UpdateWareHouseStatus(id, CloudSalesEnum.EnumStatus.Delete, CurrentUser.UserID, CurrentUser.ClientID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        /// <summary>
        /// 保存货位
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public JsonResult SaveDepotSeat(string obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            DepotSeat model = serializer.Deserialize<DepotSeat>(obj);

            string id = string.Empty;
            if (string.IsNullOrEmpty(model.DepotID))
            {
                id = new SystemBusiness().AddDepotSeat(model.DepotCode, model.WareID, model.Name, model.Status.Value, model.Description, CurrentUser.UserID, CurrentUser.ClientID);
            }
            else if (new SystemBusiness().UpdateDepotSeat(model.DepotID, model.Name, model.Status.Value, model.Description, CurrentUser.UserID, CurrentUser.ClientID))
            {
                id = model.WareID;
            }


            JsonDictionary.Add("ID", id);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取货位列表
        /// </summary>
        /// <param name="keyWords"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public JsonResult GetDepotSeats(string wareid, string keyWords, int pageSize, int pageIndex, int totalCount)
        {
            int pageCount = 0;
            List<DepotSeat> list = new SystemBusiness().GetDepotSeats(keyWords, pageSize, pageIndex, ref totalCount, ref pageCount, CurrentUser.ClientID, wareid);
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
        /// 根据仓库获取货位
        /// </summary>
        /// <param name="wareid"></param>
        /// <returns></returns>
        public JsonResult GetDepotSeatsByWareID(string wareid)
        {
            List<DepotSeat> list = new SystemBusiness().GetDepotSeatsByWareID(wareid, CurrentUser.ClientID);
            JsonDictionary.Add("Items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取货位详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetDepotByID(string id = "")
        {
            var model = new SystemBusiness().GetDepotByID(id);
            JsonDictionary.Add("Item", model);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 删除货位
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DeleteDepotSeat(string id)
        {
            bool bl = new SystemBusiness().UpdateDepotSeatStatus(id, CloudSalesEnum.EnumStatus.Delete, CurrentUser.UserID, CurrentUser.ClientID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 编辑货位状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public JsonResult UpdateDepotSeatStatus(string id, int status)
        {
            bool bl = new SystemBusiness().UpdateDepotSeatStatus(id, (CloudSalesEnum.EnumStatus)status, CurrentUser.UserID, CurrentUser.ClientID);
            JsonDictionary.Add("Status", bl);
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
