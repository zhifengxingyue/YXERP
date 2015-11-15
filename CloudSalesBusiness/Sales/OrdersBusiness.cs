﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CloudSalesEnum;
using CloudSalesDAL;
using CloudSalesEntity;
using System.Data;
using System.Data.SqlClient;

namespace CloudSalesBusiness
{
    public class OrdersBusiness
    {
        public static OrdersBusiness BaseBusiness = new OrdersBusiness();

        #region 查询

        public List<OrderEntity> GetOrders(EnumSearchType searchtype, string typeid,int status, string searchuserid, string searchteamid, string searchagentid,
                                                string begintime, string endtime, string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string agentid, string clientid)
        {
            List<OrderEntity> list = new List<OrderEntity>();
            DataSet ds = OrdersDAL.BaseProvider.GetOrders((int)searchtype, typeid, status, searchuserid, searchteamid, searchagentid, begintime, endtime, keyWords, pageSize, pageIndex, ref totalCount, ref pageCount, userid, agentid, clientid);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                OrderEntity model = new OrderEntity();
                model.FillData(dr);
                model.OrderType = SystemBusiness.BaseBusiness.GetOrderTypeByID(model.TypeID, model.AgentID, model.ClientID);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);

                model.StatusStr = model.Status == 0 ? "草案订单" 
                                : model.Status == 1 ? "未审核" 
                                : model.Status == 2 ? "已审核"
                                : model.Status == 9 ? "已删除" 
                                : "";

                list.Add(model);
            }
            return list;
        }

        public OrderEntity GetOrderByID(string orderid, string agentid, string clientid)
        {
            DataSet ds = OrdersDAL.BaseProvider.GetOrderByID(orderid, agentid, clientid);
            OrderEntity model = new OrderEntity();
            if (ds.Tables["Order"].Rows.Count > 0)
            {
                
                model.FillData(ds.Tables["Order"].Rows[0]);
                model.OrderType = SystemBusiness.BaseBusiness.GetOrderTypeByID(model.TypeID, model.AgentID, model.ClientID);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, model.AgentID);

                model.StatusStr = model.Status == 0 ? "草案订单" 
                                : model.Status == 1 ? "未审核" 
                                : model.Status == 2 ? "已审核"
                                : model.Status == 9 ? "已删除" 
                                : "";

                model.ExpressTypeStr = model.ExpressType == 0 ? "邮寄"
                                : model.ExpressType == 1 ? "海运"
                                : model.ExpressType == 2 ? "空运"
                                : model.ExpressType == 3 ? "自提"
                                : "";

                model.City = CommonBusiness.GetCityByCode(model.CityCode);

                if (ds.Tables["Customer"].Rows.Count > 0)
                {
                    model.Customer = new CustomerEntity();
                    model.Customer.FillData(ds.Tables["Customer"].Rows[0]);
                }
                if (ds.Tables["Details"].Rows.Count > 0)
                {
                    model.Details = new List<OrderDetail>();
                    foreach (DataRow dr in ds.Tables["Details"].Rows)
                    {
                        OrderDetail detail = new OrderDetail();
                        detail.FillData(dr);
                        model.Details.Add(detail);
                    }
                }
                
                
            }
            return model;
        }

        #endregion

        #region 添加


        public string CreateOrder(string customerid, string operateid, string agentid, string clientid)
        {
            string id = Guid.NewGuid().ToString();
            string code = DateTime.Now.ToString("yyyyMMddHHmmssfff");

            bool bl = OrdersDAL.BaseProvider.CreateOrder(id, code, customerid, operateid, agentid, clientid);
            if (!bl)
            {
                return "";
            }
            return id;
        }

        #endregion

        #region 编辑、删除

        public bool UpdateOrderPrice(string orderid, string autoid, string name, decimal price, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateOrderPrice(orderid, autoid, price, operateid, agentid, clientid);
            if (bl)
            {
                string msg = "修改产品" + name + "价格：" + price;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, autoid, agentid, clientid);
            }
            return bl;
        }

        public bool DeleteOrder(string orderid, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.DeleteOrder(orderid, operateid, agentid, clientid);
            if (bl)
            {
                string msg = "删除订单";
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, "", agentid, clientid);
            }
            return bl;
        }

        public bool UpdateOrderOwner(string orderid, string userid, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateOrderOwner(orderid, userid, operateid, agentid, clientid);
            if (bl)
            {
                var model = OrganizationBusiness.GetUserByUserID(userid, agentid);
                string msg = "订单拥有者更换为：" + model.Name;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, userid, agentid, clientid);
            }
            return bl;
        }

        public bool SubmitOrder(string orderid, string personName, string mobileTele, string cityCode, string address, string typeid, int expresstype, string remark, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.SubmitOrder(orderid, personName, mobileTele, cityCode, address, typeid, expresstype, remark, operateid, agentid, clientid);
            if (bl)
            {
                string msg = "确认提交订单";
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, operateid, agentid, clientid);
            }
            return bl;
        }

        public bool EffectiveOrder(string orderid, string operateid, string ip, string agentid, string clientid,out int result)
        {
            bool bl = OrdersDAL.BaseProvider.EffectiveOrder(orderid, operateid, agentid, clientid, out result);
            if (bl)
            {
                string msg = "审核订单";
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, "", agentid, clientid);
            }
            return bl;
        }

        #endregion
    }
}
