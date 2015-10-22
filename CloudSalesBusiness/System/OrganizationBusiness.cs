using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using CloudSalesEntity;
using CloudSalesDAL;
using CloudSalesEnum;


namespace CloudSalesBusiness
{
    public class OrganizationBusiness
    {

        #region Cache

        public static Dictionary<string, List<Users>> _cacheUsers;
        /// <summary>
        /// 缓存用户信息
        /// </summary>
        private static Dictionary<string, List<Users>> Users
        {
            get 
            {
                if (_cacheUsers == null)
                {
                    _cacheUsers = new Dictionary<string, List<Users>>();
                }
                return _cacheUsers;
            }
            set
            {
                _cacheUsers = value;
            }
        }

        #endregion

        #region 查询

        /// <summary>
        /// 账号是否存在
        /// </summary>
        /// <param name="loginName">账号</param>
        /// <returns></returns>
        public static bool IsExistLoginName(string loginName)
        {
            object count = CommonBusiness.Select("Users", "count(0)", "LoginName='" + loginName + "'");
            return Convert.ToInt32(count) > 0;
        }

        /// <summary>
        /// 根据用户名密码获取会员信息（登录）
        /// </summary>
        /// <param name="loginname">用户名</param>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        public static Users GetUserByUserName(string loginname, string pwd, string operateip)
        {
            pwd = CloudSalesTool.Encrypt.GetEncryptPwd(pwd);
            DataSet ds = new OrganizationDAL().GetUserByUserName(loginname, pwd);
            Users model = null;
            if (ds.Tables.Contains("User") && ds.Tables["User"].Rows.Count > 0)
            {
                model = new Users();
                model.FillData(ds.Tables["User"].Rows[0]);

                model.Menus = CommonBusiness.ClientMenus;
            }

            //记录登录日志
            LogBusiness.AddLoginLog(loginname, model != null, CloudSalesEnum.EnumSystemType.Client, operateip);

            return model;
        }

        /// <summary>
        /// 根据明道用户ID和网络ID获取云销用户信息（登录）
        /// </summary>
        /// <param name="mduserid"></param>
        /// <param name="mdprojectid"></param>
        /// <returns></returns>
        public static Users GetUserByMDUserID(string mduserid, string mdprojectid, string operateip)
        {
            DataSet ds = new OrganizationDAL().GetUserByMDUserID(mduserid);
            Users model = null;
            if (ds.Tables.Contains("User") && ds.Tables["User"].Rows.Count > 0)
            {
                model = new Users();
                model.FillData(ds.Tables["User"].Rows[0]);

                model.Menus = CommonBusiness.ClientMenus;
            }
            //记录登录日志
            LogBusiness.AddLoginLog(mduserid, model != null, CloudSalesEnum.EnumSystemType.Client, operateip);
            return model;
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="keyWords"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="totalCount"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public static List<Users> GetUsers(string keyWords, string departID, string roleID, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            string whereSql = "u.Status<>9";
            whereSql += " and u.DepartID=d.DepartID and u.RoleID=r.RoleID";

            if (!string.IsNullOrEmpty(keyWords))
                whereSql += " and ( u.name like '%" + keyWords + "%'  or  d.name like '%" + keyWords + "%' or  r.name like '%" + keyWords + "%')";
            if (!string.IsNullOrEmpty(departID))
                whereSql += " and u.departID='" + departID + "'";

            if (!string.IsNullOrEmpty(roleID))
                whereSql += " and u.roleID='" + roleID + "'";

            DataTable dt = CommonBusiness.GetPagerData("Users as u ,Department as d,Role as r", "u.*,d.Name as DepartName,r.Name as RoleName", whereSql, "u.userID", pageSize, pageIndex, out totalCount, out pageCount);
            List<Users> list = new List<Users>();
            Users model;
            foreach (DataRow item in dt.Rows)
            {
                model = new Users();
                model.FillData(item);
                list.Add(model);
            }

            return list;
        }

        /// <summary>
        /// 获取部门列表
        /// </summary>
        /// <param name="agentid">代理商ID</param>
        /// <returns></returns>
        public static List<Department> GetDepartments(string agentid)
        {
            DataTable dt = new OrganizationDAL().GetDepartments(agentid);
            List<Department> list = new List<Department>();
            foreach (DataRow dr in dt.Rows)
            {
                Department model = new Department();
                model.FillData(dr);
                if (!string.IsNullOrEmpty(model.CreateUserID))
                {
                    model.CreateUser = GetUserByUserID(model.CreateUserID, model.AgentID);
                }
                list.Add(model);
            }
            return list;
        }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="agentid">代理商ID</param>
        /// <returns></returns>
        public static List<Role> GetRoles(string agentid)
        {
            DataTable dt = new OrganizationDAL().GetRoles(agentid);
            List<Role> list = new List<Role>();
            foreach (DataRow dr in dt.Rows)
            {
                Role model = new Role();
                model.FillData(dr);
                if (!string.IsNullOrEmpty(model.CreateUserID))
                {
                    model.CreateUser = GetUserByUserID(model.CreateUserID, model.AgentID);
                }
                list.Add(model);
            }
            return list;
        }

        /// <summary>
        /// 获取角色详情（权限明细）
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="agentid"></param>
        /// <returns></returns>
        public static Role GetRoleByID(string roleid, string agentid)
        {
            Role model = null;
            DataSet ds = OrganizationDAL.BaseProvider.GetRoleByID(roleid, agentid);
            if (ds.Tables.Contains("Role") && ds.Tables["Role"].Rows.Count > 0)
            {
                model = new Role();
                model.FillData(ds.Tables["Role"].Rows[0]);
                model.Menus = new List<Menu>();
                foreach (DataRow dr in ds.Tables["Menus"].Rows)
                {
                    Menu menu = new Menu();
                    menu.FillData(dr);
                    model.Menus.Add(menu);
                }
            }
            return model;
        }

        /// <summary>
        /// 获取用户信息(缓存)
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="agentid"></param>
        /// <returns></returns>
        public static Users GetUserByUserID(string userid, string agentid)
        {
            if (!Users.ContainsKey(agentid))
            {
                Users.Add(agentid, new List<Users>());
            }

            if (Users[agentid].Where(u => u.UserID == userid).Count() > 0)
            {
                return Users[agentid].Where(u => u.UserID == userid).FirstOrDefault();
            }
            else
            {
                DataTable dt = new OrganizationDAL().GetUserByUserID(userid);
                Users model = new Users();
                if (dt.Rows.Count > 0)
                {
                    model.FillData(dt.Rows[0]);
                    Users[agentid].Add(model);
                }
                return model;
            }
        }

        #endregion

        #region 添加

        /// <summary>
        /// 添加部门
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="parentid">上级ID</param>
        /// <param name="description">描述</param>
        /// <param name="operateid">操作人</param>
        /// <param name="clientid">客户端ID</param>
        /// <returns></returns>
        public string CreateDepartment(string name, string parentid, string description, string operateid, string agentid, string clientid)
        {
            string departid = Guid.NewGuid().ToString();
            bool bl = OrganizationDAL.BaseProvider.CreateDepartment(departid, name, parentid, description, operateid, agentid, clientid);
            if (bl)
            {
                return departid;
            }
            return "";
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="parentid">上级ID</param>
        /// <param name="description">描述</param>
        /// <param name="operateid">操作人</param>
        /// <param name="clientid">客户端ID</param>
        /// <returns></returns>
        public string CreateRole(string name, string parentid, string description, string operateid, string agentid, string clientid)
        {
            string roleid = Guid.NewGuid().ToString();
            bool bl = OrganizationDAL.BaseProvider.CreateRole(roleid, name, parentid, description, operateid, agentid, clientid);
            if (bl)
            {
                return roleid;
            }
            return "";
        }

        /// <summary>
        /// 添加员工
        /// </summary>
        /// <param name="loginname">登录名</param>
        /// <param name="loginpwd">密码</param>
        /// <param name="name">姓名</param>
        /// <param name="mobile">手机</param>
        /// <param name="email">邮箱</param>
        /// <param name="citycode">城市</param>
        /// <param name="address">地址</param>
        /// <param name="jobs">职位</param>
        /// <param name="roleid">角色ID</param>
        /// <param name="departid">部门ID</param>
        /// <param name="parentid">上级ID</param>
        /// <param name="agentid">代理商ID></param>
        /// <param name="clientid">客户端ID</param>
        /// <param name="mduserid">明道用户ID</param>
        /// <param name="mdprojectid">明道网络ID</param>
        /// <param name="isAppAdmin">是否应用管理员</param>
        /// <param name="operateid">操作人</param>
        /// <param name="result">返回结果 0 失败 1成功 2账号已存在</param>
        /// <returns></returns>
        public static string CreateUser(string loginname, string loginpwd, string name, string mobile, string email, string citycode, string address, string jobs,
                               string roleid, string departid, string parentid, string agentid, string clientid, string mduserid, string mdprojectid, int isAppAdmin, string operateid, out int result)
        {
            string userid = Guid.NewGuid().ToString();

            loginpwd = CloudSalesTool.Encrypt.GetEncryptPwd(loginpwd);

            bool bl = OrganizationDAL.BaseProvider.CreateUser(userid, loginname, loginpwd, name, mobile, email, citycode, address, jobs, roleid, departid, parentid, agentid, clientid, mduserid, mdprojectid, isAppAdmin, operateid, out result);
            if (bl)
            {
                return userid;
            }
            return "";
        }

        #endregion

        #region 编辑/删除

        /// <summary>
        /// 编辑部门
        /// </summary>
        /// <param name="departid">部门ID</param>
        /// <param name="name">名称</param>
        /// <param name="description">描述</param>
        /// <param name="operateid">操作人</param>
        /// <param name="operateip">操作IP</param>
        /// <returns></returns>
        public bool UpdateDepartment(string departid, string name, string description, string operateid, string operateip, string agentid)
        {
            var dal = new OrganizationDAL();
            return dal.UpdateDepartment(departid, name, description, agentid);
        }

        /// <summary>
        /// 编辑部门状态
        /// </summary>
        /// <param name="departid">部门ID</param>
        /// <param name="status">状态</param>
        /// <param name="operateid">操作人</param>
        /// <param name="operateip">操作IP</param>
        /// <returns></returns>
        public EnumResultStatus UpdateDepartmentStatus(string departid, EnumStatus status, string operateid, string operateip, string agentid)
        {
            if (status == EnumStatus.Delete)
            {
                object count = CommonBusiness.Select("UserDepart", "count(0)", "DepartID='" + departid + "' and Status=1");
                if (Convert.ToInt32(count) > 0)
                {
                    return EnumResultStatus.Exists;
                }
            }
            if (CommonBusiness.Update("Department", "Status", (int)status, "DepartID='" + departid + "' and AgentID='" + agentid + "'"))
            {
                return EnumResultStatus.Success;
            }
            else
            {
                return EnumResultStatus.Failed;
            }
        }

        /// <summary>
        /// 编辑角色
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="name">名称</param>
        /// <param name="description">描述</param>
        /// <param name="operateid">操作人</param>
        /// <param name="ip">IP</param>
        /// <param name="agentid">代理商ID</param>
        /// <returns></returns>
        public bool UpdateRole(string roleid, string name, string description, string operateid, string ip, string agentid)
        {
            return OrganizationDAL.BaseProvider.UpdateRole(roleid, name, description, agentid);
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="operateid"></param>
        /// <param name="ip"></param>
        /// <param name="agentid"></param>
        /// <param name="result">0 失败 1成功 10002 存在员工</param>
        /// <returns></returns>
        public bool DeleteRole(string roleid, string operateid, string ip, string agentid, out int result)
        {
            return OrganizationDAL.BaseProvider.DeleteRole(roleid, agentid, out result);
        }
        /// <summary>
        /// 编辑角色权限
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="permissions"></param>
        /// <param name="operateid"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public bool UpdateRolePermission(string roleid, string permissions, string operateid, string ip)
        {
            return OrganizationDAL.BaseProvider.UpdateRolePermission(roleid, permissions, operateid);
        }

        #endregion
    }
}
