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
        public static Dictionary<string, List<Users>> Users
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
        /// 客户端账号是否存在
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
            pwd = CloudSalesTool.Encrypt.GetEncryptPwd(pwd, loginname);
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
                list.Add(model);
            }
            return list;
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
        public bool UpdateDepartment(string departid, string name, string description, string operateid, string operateip)
        {
            var dal = new OrganizationDAL();
            return dal.UpdateDepartment(departid, name, description);
        }

        /// <summary>
        /// 编辑部门状态
        /// </summary>
        /// <param name="departid">部门ID</param>
        /// <param name="status">状态</param>
        /// <param name="operateid">操作人</param>
        /// <param name="operateip">操作IP</param>
        /// <returns></returns>
        public EnumResultStatus UpdateDepartmentStatus(string departid, EnumStatus status, string operateid, string operateip)
        {
            if (status == EnumStatus.Delete)
            {
                object count = CommonBusiness.Select("UserDepart", "count(0)", "DepartID='" + departid + "' and Status=1");
                if (Convert.ToInt32(count) > 0)
                {
                    return EnumResultStatus.Exists;
                }
            }
            if (CommonBusiness.Update("Department", "Status", (int)status, "DepartID='" + departid + "'"))
            {
                return EnumResultStatus.Success;
            }
            else
            {
                return EnumResultStatus.Failed;
            }
        }

        #endregion
    }
}
