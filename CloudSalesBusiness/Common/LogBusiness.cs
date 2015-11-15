using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CloudSalesDAL;
using System.Threading.Tasks;
using CloudSalesEnum;
using CloudSalesEntity;
using System.Data;

namespace CloudSalesBusiness
{
    public class LogBusiness
    {

        #region

        /// <summary>
        /// 获取日志
        /// </summary>
        /// <returns></returns>
        public static List<LogEntity> GetLogs(string guid, EnumLogObjectType type, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string agentid)
        {
            string tablename = "";
            switch (type)
            {
                case EnumLogObjectType.Customer:
                    tablename = "CustomerLog";
                    break;
                case EnumLogObjectType.Orders:
                    tablename = "OrdersLog";
                    break;
            }

            DataTable dt = CommonBusiness.GetPagerData(tablename, "*", "LogGUID='" + guid + "'", "AutoID", pageSize, pageIndex, out totalCount, out pageCount);

            List<LogEntity> list = new List<LogEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                LogEntity model = new LogEntity();
                model.FillData(dr);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, model.AgentID);

                list.Add(model);
            }
            return list;
        }

        #endregion

        #region 添加

        /// <summary>
        /// 记录登录日志
        /// </summary>
        /// <param name="loginname">用户名</param>
        /// <param name="status">登录结果</param>
        /// <param name="systemtype">系统类型</param>
        /// <param name="operateip">登录IP</param>
        public static async void AddLoginLog(string loginname, bool status, EnumSystemType systemtype, string operateip)
        {
            await LogDAL.AddLoginLog(loginname, status ? 1 : 0, (int)systemtype, operateip);
        }
        
        /// <summary>
        /// 记录操作日志
        /// </summary>
        public static async void AddOperateLog(string userid, string funcname, EnumLogType type, EnumLogModules modules, EnumLogEntity entity, string guid, string message, string operateip)
        {
            await LogDAL.AddOperateLog(userid, funcname, (int)type, (int)modules, (int)entity, guid, message, operateip);
        }
        
        /// <summary>
        /// 记录错误日志
        /// </summary>
        public static async void AddErrorLog(string userid, string message, EnumSystemType systemtype, string operateip)
        {
            await LogDAL.AddErrorLog(userid, message, (int)systemtype, operateip);
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        public static async void AddLog(string logguid, EnumLogObjectType type, string remark, string userid, string operateip, string guid, string agentid, string clientid)
        {
            string tablename = "";
            switch (type)
            {
                case EnumLogObjectType.Customer:
                    tablename = "CustomerLog";
                    break;
                case EnumLogObjectType.Orders:
                    tablename = "OrdersLog";
                    break;
            }
            await LogDAL.AddLog(tablename, logguid, remark, userid, operateip, guid, agentid, clientid);
        }

        #endregion
    }
}
