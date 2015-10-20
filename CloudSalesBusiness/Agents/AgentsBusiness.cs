using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalesBusiness
{
    public class AgentsBusiness
    {
        #region

        /// <summary>
        /// 是否明道网络已注册
        /// </summary>
        /// <param name="projectid"></param>
        /// <returns></returns>
        public static bool IsExistsMDProject(string projectid)
        {
            var count = CommonBusiness.Select("Agents", "count(0)", "MDProjectID='" + projectid + "'");
            return Convert.ToInt32(count) > 0;
        }

        #endregion
    }
}
