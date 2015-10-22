using System;
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
        public static OrganizationDAL BaseProvider = new OrganizationDAL();

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

        public DataSet GetUserByMDUserID(string userid)
        {
            SqlParameter[] paras = { 
                                    new SqlParameter("@MDUserID",userid)
                                   };
            return GetDataSet("GetUserByMDUserID", paras, CommandType.StoredProcedure, "User|Department|Role|Permission");


        }

        public DataTable GetDepartments(string agentid)
        {
            string sql = "select * from Department where AgentID=@AgentID and Status<>9";

            SqlParameter[] paras = { 
                                    new SqlParameter("@AgentID",agentid)
                                   };

            return GetDataTable(sql, paras, CommandType.Text);
        }

        public DataTable GetRoles(string agentid)
        {
            string sql = "select * from Role where AgentID=@AgentID and Status<>9";

            SqlParameter[] paras = { 
                                    new SqlParameter("@AgentID",agentid)
                                   };

            return GetDataTable(sql, paras, CommandType.Text);
        }

        public DataSet GetRoleByID(string roleid, string agentid)
        {
            string sql = "select * from Role where RoleID=@RoleID and AgentID=@AgentID and Status<>9; select * from RolePermission where RoleID=@RoleID";

            SqlParameter[] paras = { 
                                       new SqlParameter("@RoleID",roleid),
                                       new SqlParameter("@AgentID",agentid)
                                   };

            return GetDataSet(sql, paras, CommandType.Text, "Role|Menus");
        }

        #endregion

        #region 添加

        public bool CreateDepartment(string departid, string name, string parentid, string description, string operateid, string agentid, string clientid)
        {
            string sql = "insert into Department(DepartID,Name,ParentID,Status,Description,CreateUserID,AgentID,ClientID) "+
                        " values(@DepartID,@Name,@ParentID,1,@Description,@CreateUserID,@AgentID,@ClientID)";

            SqlParameter[] paras = { 
                                       new SqlParameter("@DepartID",departid),
                                       new SqlParameter("@Name",name),
                                       new SqlParameter("@ParentID",parentid),
                                       new SqlParameter("@Description",description),
                                       new SqlParameter("@CreateUserID",operateid),
                                       new SqlParameter("@AgentID",agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };

            return ExecuteNonQuery(sql, paras, CommandType.Text) > 0;
        }

        public bool CreateRole(string roleid, string name, string parentid, string description, string operateid, string agentid, string clientid)
        {
            string sql = "insert into Role(RoleID,Name,ParentID,Status,IsDefault,Description,CreateUserID,AgentID,ClientID) " +
                        " values(@RoleID,@Name,@ParentID,1,0,@Description,@CreateUserID,@AgentID,@ClientID)";

            SqlParameter[] paras = { 
                                       new SqlParameter("@RoleID",roleid),
                                       new SqlParameter("@Name",name),
                                       new SqlParameter("@ParentID",parentid),
                                       new SqlParameter("@Description",description),
                                       new SqlParameter("@CreateUserID",operateid),
                                       new SqlParameter("@AgentID",agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };

            return ExecuteNonQuery(sql, paras, CommandType.Text) > 0;
        }

        public bool CreateUser(string userid, string loginname, string loginpwd, string name, string mobile, string email, string citycode, string address, string jobs,
                               string roleid, string departid, string parentid, string agentid, string clientid, string mduserid, string mdprojectid, int isAppAdmin, string operateid, out int result)
        {
            result = 0;
            SqlParameter[] paras = { 
                                       new SqlParameter("@Result",result),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@LoginName",loginname),
                                       new SqlParameter("@LoginPwd",loginpwd),
                                       new SqlParameter("@Name",name),
                                       new SqlParameter("@Mobile",mobile),
                                       new SqlParameter("@Email",email),
                                       new SqlParameter("@CityCode",citycode),
                                       new SqlParameter("@Address",address),
                                       new SqlParameter("@Jobs",jobs),
                                       new SqlParameter("@RoleID",roleid),
                                       new SqlParameter("@DepartID",departid),
                                       new SqlParameter("@ParentID",parentid),
                                       new SqlParameter("@AgentID",agentid),
                                       new SqlParameter("@MDUserID",mduserid),
                                       new SqlParameter("@MDProjectID",mdprojectid),
                                       new SqlParameter("@IsAppAdmin",isAppAdmin),
                                       new SqlParameter("@CreateUserID",operateid),
                                       new SqlParameter("@ClientID",clientid)
                                   };

            paras[0].Direction = ParameterDirection.Output;

            bool bl = ExecuteNonQuery("P_InsterUser", paras, CommandType.StoredProcedure) > 0;
            result = Convert.ToInt32(paras[0].Value);

            return bl;
        }

        #endregion

        #region 编辑/删除

        public bool UpdateDepartment(string departid, string name, string description, string agentid)
        {
            string sql = "update Department set Name=@Name,Description=@Description where DepartID=@DepartID and AgentID=@AgentID";

            SqlParameter[] paras = { 
                                       new SqlParameter("@DepartID",departid),
                                       new SqlParameter("@Name",name),
                                       new SqlParameter("@Description",description),
                                       new SqlParameter("@AgentID",agentid)
                                   };

            return ExecuteNonQuery(sql, paras, CommandType.Text) > 0;
        }

        public bool UpdateRole(string roleid, string name, string description, string agentid)
        {
            string sql = "update Role set Name=@Name,Description=@Description where RoleID=@RoleID and AgentID=@AgentID";

            SqlParameter[] paras = { 
                                       new SqlParameter("@RoleID",roleid),
                                       new SqlParameter("@Name",name),
                                       new SqlParameter("@Description",description),
                                       new SqlParameter("@AgentID",agentid)
                                   };
            return ExecuteNonQuery(sql, paras, CommandType.Text) > 0;
        }

        public bool DeleteRole(string roleid, string agentid, out int result)
        {
            result = 0;
            SqlParameter[] paras = { 
                                       new SqlParameter("@Result",result),
                                       new SqlParameter("@RoleID",roleid),
                                       new SqlParameter("@AgentID",agentid)
                                   };
            paras[0].Direction = ParameterDirection.Output;
            bool bl = ExecuteNonQuery("P_DeleteRole", paras, CommandType.StoredProcedure) > 0;
            result = Convert.ToInt32(paras[0].Value);
            return bl;
        }

        public bool UpdateRolePermission(string roleid, string permissions, string userid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@RoleID",roleid),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@Permissions",permissions)
                                   };
            return ExecuteNonQuery("P_UpdateRolePermission", paras, CommandType.StoredProcedure) > 0;
        }

        #endregion

    }
}
