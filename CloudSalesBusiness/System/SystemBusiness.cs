using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CloudSalesEntity;
using System.Data;
using CloudSalesDAL;

namespace CloudSalesBusiness
{
    public class SystemBusiness
    {
        #region Cache

        private static Dictionary<string, List<CustomSourceEntity>> _source;

        /// <summary>
        /// 客户来源
        /// </summary>
        private static Dictionary<string, List<CustomSourceEntity>> CustomSources
        {
            get
            {
                if (_source == null)
                {
                    _source = new Dictionary<string, List<CustomSourceEntity>>();
                }
                return _source;
            }
            set 
            {
                _source = value;
            }
        }

        #endregion

        #region 查询

        /// <summary>
        /// 获取客户来源列表
        /// </summary>
        /// <param name="clientid">客户端ID</param>
        /// <returns></returns>
        public List<CustomSourceEntity> GetCustomSources(string agentid,string clientid)
        {
            if (CustomSources.ContainsKey(clientid)) 
            {
                return CustomSources[clientid];
            }

            List<CustomSourceEntity> list = new List<CustomSourceEntity>();
            DataTable dt = SystemDAL.BaseProvider.GetCustomSources(clientid);
            foreach (DataRow dr in dt.Rows)
            {
                CustomSourceEntity model = new CustomSourceEntity();
                model.FillData(dr);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, agentid);
                list.Add(model);
            }
            CustomSources.Add(clientid, list);

            return list;

        }
        /// <summary>
        /// 根据ID获取客户来源
        /// </summary>
        /// <param name="sourceid"></param>
        /// <returns></returns>
        public CustomSourceEntity GetCustomSourcesByID(string sourceid, string agentid, string clientid)
        {
            var list = GetCustomSources(agentid, clientid);
            if (list.Where(m => m.SourceID == sourceid).Count() > 0)
            {
                return list.Where(m => m.SourceID == sourceid).FirstOrDefault();
            }

            CustomSourceEntity model = new CustomSourceEntity();
            DataTable dt = SystemDAL.BaseProvider.GetCustomSourceByID(sourceid);
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, agentid);
            }
            CustomSources[clientid].Add(model);
            return model;
        }
        #endregion

        #region 添加

        /// <summary>
        /// 创建客户来源
        /// </summary>
        /// <param name="sourcecode">来源编码</param>
        /// <param name="sourcename">名称</param>
        /// <param name="ischoose">是否允许选择</param>
        /// <param name="userid">创建人</param>
        /// <param name="clientid">客户端ID</param>
        /// <returns></returns>
        public string CreateCustomSource(string sourcecode, string sourcename, int ischoose, string userid, string agentid, string clientid,out int result)
        {
            string sourceid = Guid.NewGuid().ToString();

            bool bl = SystemDAL.BaseProvider.CreateCustomSource(sourceid, sourcecode, sourcename, ischoose , userid, clientid, out result);
            if (bl)
            {
                if (!CustomSources.ContainsKey(clientid)) 
                {
                    GetCustomSources(agentid, clientid);
                }

                CustomSources[clientid].Add(new CustomSourceEntity()
                {
                    SourceID = sourceid.ToLower(),
                    SourceName = sourcename,
                    SourceCode = sourcecode,
                    IsChoose = ischoose,
                    IsSystem = 0,
                    Status = 1,
                    CreateTime = DateTime.Now,
                    CreateUserID = userid,
                    CreateUser = OrganizationBusiness.GetUserByUserID(userid, agentid),
                    ClientID = clientid
                });

                return sourceid;
            }
            return "";
        }

        #endregion

        #region 编辑/删除

        /// <summary>
        /// 编辑客户来源
        /// </summary>
        /// <param name="sourceid"></param>
        /// <param name="sourcename"></param>
        /// <param name="ischoose"></param>
        /// <param name="userid"></param>
        /// <param name="agentid"></param>
        /// <param name="clientid"></param>
        /// <returns></returns>
        public bool UpdateCustomSource(string sourceid, string sourcename, int ischoose, string userid,string ip, string agentid, string clientid)
        {
            var model = GetCustomSourcesByID(sourceid, agentid, clientid);
            if (string.IsNullOrEmpty(sourcename))
            {
                sourcename = model.SourceName;
            }
            bool bl = SystemDAL.BaseProvider.UpdateCustomSource(sourceid, sourcename, ischoose, clientid);
            if (bl)
            {
                model.SourceName = sourcename;
                model.IsChoose = ischoose;
            }
            return bl;
        }

        /// <summary>
        /// 删除客户来源
        /// </summary>
        /// <param name="sourceid"></param>
        /// <param name="userid"></param>
        /// <param name="ip"></param>
        /// <param name="agentid"></param>
        /// <param name="clientid"></param>
        /// <returns></returns>
        public bool DeleteCustomSource(string sourceid, string userid, string ip, string agentid, string clientid)
        {

            bool bl = SystemDAL.BaseProvider.DeleteCustomSource(sourceid, clientid);
            if (bl)
            {
                var model = GetCustomSourcesByID(sourceid, agentid, clientid);
                model.Status = 9;
            }
            return bl;
        }

        #endregion


    }
}
