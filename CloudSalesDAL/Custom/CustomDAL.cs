﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalesDAL
{
    public class CustomDAL : BaseDAL
    {
        public static CustomDAL BaseProvider = new CustomDAL();

        #region 查询

        public DataSet GetCustomers(int searchtype, int type, string sourceid, string stageid, int status, int mark, string searchuserid, string searchteamid, string searchagentid, 
                                    string begintime, string endtime, string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@totalCount",SqlDbType.Int),
                                       new SqlParameter("@pageCount",SqlDbType.Int),
                                       new SqlParameter("@SearchType",searchtype),
                                       new SqlParameter("@Type",type),
                                       new SqlParameter("@SourceID",sourceid),
                                       new SqlParameter("@StageID",stageid),
                                       new SqlParameter("@Status",status),
                                       new SqlParameter("@Mark",mark),
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
            DataSet ds = GetDataSet("P_GetCustomers", paras, CommandType.StoredProcedure);
            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);
            return ds;
        }

        #endregion

        #region 添加

        public bool CreateCustomer(string customerid, string name, int type, string sourceid, string activityid, string industryid, int extent, string citycode, string address, string contactname, 
                                   string mobile, string officephone, string email, string jobs, string desc, string ownerid, string operateid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@CustomerID",customerid),
                                     new SqlParameter("@Name",name),
                                     new SqlParameter("@Type",type),
                                     new SqlParameter("@SourceID",sourceid),
                                     new SqlParameter("@ActivityID",activityid),
                                     new SqlParameter("@IndustryID" , industryid),
                                     new SqlParameter("@Extent" , extent),
                                     new SqlParameter("@CityCode" , citycode),
                                     new SqlParameter("@Address" , address),
                                     new SqlParameter("@ContactName" , contactname),
                                     new SqlParameter("@MobilePhone" , mobile),
                                     new SqlParameter("@OfficePhone" , officephone),
                                     new SqlParameter("@Email" , email),
                                     new SqlParameter("@Jobs" , jobs),
                                     new SqlParameter("@Description" , desc),
                                     new SqlParameter("@OwnerID" , ownerid),
                                     new SqlParameter("@CreateUserID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_CreateCustomer", paras, CommandType.StoredProcedure) > 0;
        }

        #endregion

        #region 编辑/删除

        public bool UpdateCustomerStage(string customerid, string stageid, string operateid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@CustomerID",customerid),
                                     new SqlParameter("@StageID",stageid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateCustomerStage", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateCustomerOwner(string customerid, string userid, string operateid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@CustomerID",customerid),
                                     new SqlParameter("@UserID",userid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateCustomerOwner", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateCustomerAgent(string customerid, string newagentid, string operateid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@CustomerID",customerid),
                                     new SqlParameter("@NewAgentID",newagentid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateCustomerAgent", paras, CommandType.StoredProcedure) > 0;
        }

        #endregion
    }
}
