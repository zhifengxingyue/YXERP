﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalesDAL
{
    public class OrganizationDAL :BaseDAL
    {
        #region 查询

        public DataSet GetUserByUserName(string loginname, string pwd)
        {

            SqlParameter[] paras = { 
                                    new SqlParameter("@LoginName",loginname),
                                    new SqlParameter("@LoginPwd",pwd)
                                   };
            return GetDataSet("P_GetUserToLogin", paras, CommandType.StoredProcedure, "User|Department|Role|Permission");
        }

        public DataTable GetUserByUserID(string userid)
        {
            string sql = "select * from Users where UserID=@UserID";

            SqlParameter[] paras = { 
                                    new SqlParameter("@UserID",userid)
                                   };

            return GetDataTable(sql, paras, CommandType.Text);
        }

        public DataTable GetDepartments(string agentid)
        {
            string sql = "select * from Department where AgentID=@AgentID and Status<>9";

            SqlParameter[] paras = { 
                                    new SqlParameter("@AgentID",agentid)
                                   };

            return GetDataTable(sql, paras, CommandType.Text);
        }

        #endregion

        #region 添加

        public string AddDepartment(string name, string parentid, string description, string operateid, string agentid, string clientid)
        {
            string id = Guid.NewGuid().ToString();
            string sql = "insert into Department(DepartID,Name,ParentID,Status,Description,CreateUserID,AgentID,ClientID) "+
                        " values(@DepartID,@Name,@ParentID,1,@Description,@CreateUserID,@AgentID,@ClientID)";

            SqlParameter[] paras = { 
                                       new SqlParameter("@DepartID",id),
                                       new SqlParameter("@Name",name),
                                       new SqlParameter("@ParentID",parentid),
                                       new SqlParameter("@Description",description),
                                       new SqlParameter("@CreateUserID",operateid),
                                       new SqlParameter("@AgentID",agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };

            if (ExecuteNonQuery(sql, paras, CommandType.Text) > 0)
            {
                return id;
            }
            return "";
        }

        #endregion

        #region 编辑/删除

        public bool UpdateDepartment(string departid, string name, string description)
        {
            string sql = "update Department set Name=@Name,Description=@Description where DepartID=@DepartID";

            SqlParameter[] paras = { 
                                       new SqlParameter("@DepartID",departid),
                                       new SqlParameter("@Name",name),
                                       new SqlParameter("@Description",description)
                                   };

            return ExecuteNonQuery(sql, paras, CommandType.Text) > 0;
        }

        #endregion

    }
}
