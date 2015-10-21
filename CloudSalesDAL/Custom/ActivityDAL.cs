﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalesDAL
{
    public class ActivityDAL : BaseDAL
    {
        public static ActivityDAL BaseProvider = new ActivityDAL();

        #region 查询

        public DataTable GetActivitys(string userid, int stage, string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@totalCount",SqlDbType.Int),
                                       new SqlParameter("@pageCount",SqlDbType.Int),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@Stage",stage),
                                       new SqlParameter("@keyWords",keyWords),
                                       new SqlParameter("@pageSize",pageSize),
                                       new SqlParameter("@pageIndex",pageIndex),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Value = totalCount;
            paras[1].Value = pageCount;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            DataTable dt = GetDataTable("P_GetActivitys", paras, CommandType.StoredProcedure);
            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);
            return dt;

        }

        public DataTable GetActivityByID(string activityid)
        {
            string sqlText = "select * from Activity where ActivityID=@ActivityID";
            SqlParameter[] paras = { 
                                     new SqlParameter("@ActivityID",activityid)
                                   };

            return GetDataTable(sqlText, paras, CommandType.Text);
        }

        public DataTable GetActivityByCode(string activitycode)
        {
            string sqlText = "select * from Activity where ActivityCode=@ActivityCode";
            SqlParameter[] paras = { 
                                     new SqlParameter("@ActivityCode",activitycode)
                                   };

            return GetDataTable(sqlText, paras, CommandType.Text);
        }

        #endregion

        #region 添加

        public string GetActivityCode()
        {
            string code = GetCode(8);
            if (CommonDAL.Select("ActivityID", "Count(0)", "ActivityCode='" + code + "'").ToString() != "0")
            {
                return GetActivityCode();
            }
            return code;
        }

        public bool CreateActivity(string activityid, string name, string poster, string begintime, string endtime, string address, string ownerid, string remark, string userid, string agentid, string clientid)
        {
            string sqlText = @"insert into Activity(ActivityID,Name,ActivityCode,Poster,BeginTime,EndTime,Address,Status,OwnerID,Remark,CreateUserID,AgentID,ClientID)
                                values(@ActivityID,@Name,@ActivityCode,@Poster,@BeginTime,@EndTime,@Address,1,@OwnerID,@Remark,@CreateUserID,@AgentID,@ClientID)";
            SqlParameter[] paras = { 
                                     new SqlParameter("@ActivityID",activityid),
                                     new SqlParameter("@Name",name),
                                     new SqlParameter("@ActivityCode",GetActivityCode()),
                                     new SqlParameter("@Poster" , poster),
                                     new SqlParameter("@BeginTime" , begintime),
                                     new SqlParameter("@EndTime" , endtime),
                                     new SqlParameter("@Address" , address),
                                     new SqlParameter("@OwnerID" , ownerid),
                                     new SqlParameter("@Remark" , remark),
                                     new SqlParameter("@CreateUserID" , userid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0;
        }

        #endregion

        #region 编辑/删除

        public bool UpdateActivity(string activityid, string name, string poster, string begintime, string endtime, string address, string remark)
        {
            string sqlText = @"update Activity set Name=@Name,Poster=@Poster,BeginTime=@BeginTime,EndTime=@EndTime,Address=@Address,Remark=@Remark)
                               where ActivityID=@ActivityID ";
            SqlParameter[] paras = { 
                                     new SqlParameter("@ActivityID",activityid),
                                     new SqlParameter("@Name",name),
                                     new SqlParameter("@Poster" , poster),
                                     new SqlParameter("@BeginTime" , begintime),
                                     new SqlParameter("@EndTime" , endtime),
                                     new SqlParameter("@Address" , address),
                                     new SqlParameter("@Remark" , remark)
                                   };

            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0;
        }

        #endregion
    }
}