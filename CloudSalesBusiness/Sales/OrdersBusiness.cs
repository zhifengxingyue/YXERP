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
                                : "";

                list.Add(model);
            }
            return list;
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


        #endregion
    }
}
