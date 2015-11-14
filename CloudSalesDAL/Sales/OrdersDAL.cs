using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalesDAL
{
    public class OrdersDAL : BaseDAL
    {
        public static OrdersDAL BaseProvider = new OrdersDAL();
        #region 查询

        public DataSet GetOrders(int searchtype, string typeid, int status, string searchuserid, string searchteamid, string searchagentid, string begintime, string endtime, 
                                string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@totalCount",SqlDbType.Int),
                                       new SqlParameter("@pageCount",SqlDbType.Int),
                                       new SqlParameter("@SearchType",searchtype),
                                       new SqlParameter("@TypeID",typeid),
                                       new SqlParameter("@Status",status),
                                       new SqlParameter("@SearchUserID",searchuserid),
                                       new SqlParameter("@SearchTeamID",searchteamid),
                                       new SqlParameter("@SearchAgentID",searchagentid),
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@Keywords",keyWords),
                                       new SqlParameter("@pageSize",pageSize),
                                       new SqlParameter("@pageIndex",pageIndex),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Value = totalCount;
            paras[1].Value = pageCount;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            DataSet ds = GetDataSet("P_GetOrders", paras, CommandType.StoredProcedure);
            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);
            return ds;
        }

        public DataSet GetOrderByID(string orderid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@OrderID",orderid),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };

            DataSet ds = GetDataSet("P_GetOrderByID", paras, CommandType.StoredProcedure, "Order|Customer|Details");
            return ds;
        }

        #endregion

        #region 添加

        public bool CreateOrder(string orderid, string ordercode, string customerid, string operateid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@OrderCode",ordercode),
                                     new SqlParameter("@CustomerID" , customerid),
                                     new SqlParameter("@UserID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            return ExecuteNonQuery("P_CreateOrder", paras, CommandType.StoredProcedure) > 0;
        }

        #endregion

        #region 编辑、删除

        public bool UpdateOrderOwner(string orderid, string userid, string operateid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@UserID",userid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateOrderOwner", paras, CommandType.StoredProcedure) > 0;
        }

        public bool SubmitOrder(string orderid, string personName, string mobileTele, string cityCode, string address, string typeid, int expresstype, string remark, string operateid, string agentid, string clientid)
        {
            int result = 0;
            SqlParameter[] paras = { 
                                     new SqlParameter("@Result",result),
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@PersonName",personName),
                                     new SqlParameter("@MobileTele" , mobileTele),
                                     new SqlParameter("@CityCode" , cityCode),
                                     new SqlParameter("@Address" , address),
                                     new SqlParameter("@TypeID" , typeid),
                                     new SqlParameter("@ExpressType" , expresstype),
                                     new SqlParameter("@Remark" , remark),
                                     new SqlParameter("@UserID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            paras[0].Direction = ParameterDirection.Output;
            ExecuteNonQuery("P_SubmitOrder", paras, CommandType.StoredProcedure);
            result = Convert.ToInt32(paras[0].Value);
            return result == 1;
        }

        #endregion

    }
}
