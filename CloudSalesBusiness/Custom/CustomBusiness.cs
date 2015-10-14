using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using CloudSalesDAL;
using CloudSalesEntity;
using System.Data;
using CloudSalesEnum;

namespace CloudSalesBusiness
{
    public class CustomBusiness
    {
        #region 查询

        /// <summary>
        /// 获取客户来源列表
        /// </summary>
        /// <param name="clientid">客户端ID</param>
        /// <returns></returns>
        public List<CustomSourceEntity> GetCustomSources(string clientid)
        {
            List<CustomSourceEntity> list = new List<CustomSourceEntity>();
            DataTable dt = CustomDAL.BaseProvider.GetCustomSources(clientid);
            foreach (DataRow dr in dt.Rows)
            {
                CustomSourceEntity model = new CustomSourceEntity();
                model.FillData(dt.Rows[0]);
                list.Add(model);
            }
            return list;

        }
        /// <summary>
        /// 根据ID获取客户来源
        /// </summary>
        /// <param name="sourceid"></param>
        /// <returns></returns>
        public CustomSourceEntity GetActivityByID(string sourceid)
        {
            CustomSourceEntity model = new CustomSourceEntity();
            DataTable dt = CustomDAL.BaseProvider.GetCustomSourceByID(sourceid);
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
            }
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
        public string CreateCustomSource(string sourcecode, string sourcename, bool ischoose, string userid, string clientid)
        {
            string sourceid = Guid.NewGuid().ToString();

            bool bl = CustomDAL.BaseProvider.CreateCustomSource(sourceid, sourcecode, sourcename, ischoose ? 1 : 0, userid, clientid);
            if (!bl)
            {
                return "";
            }
            return sourceid;
        }

        #endregion

        #region 编辑/删除

        #endregion
    }
}
