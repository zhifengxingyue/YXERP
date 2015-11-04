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
        /// 公司规模
        /// </summary>
        /// <returns></returns>
        public static List<ExtentEntity> GetExtents()
        {
            List<ExtentEntity> list = new List<ExtentEntity>();
            list.Add(new ExtentEntity() { ExtentID = "1", ExtentName = "0-49人" });
            list.Add(new ExtentEntity() { ExtentID = "2", ExtentName = "50-99人" });
            list.Add(new ExtentEntity() { ExtentID = "3", ExtentName = "100-199人" });
            list.Add(new ExtentEntity() { ExtentID = "4", ExtentName = "200-499人" });
            list.Add(new ExtentEntity() { ExtentID = "5", ExtentName = "500-999人" });
            list.Add(new ExtentEntity() { ExtentID = "6", ExtentName = "1000人以上" });
            return list;
        }


        #endregion

        #region 添加

        public string CreateCustomer(string name, int type, string sourceid, string activityid, string industryid, int extent, string citycode, string address, 
                                     string contactname, string mobile, string officephone, string email, string jobs, string desc, string ownerid, string operateid, string agentid, string clientid)
        {
            string id = Guid.NewGuid().ToString();
            bool bl = CustomDAL.BaseProvider.CreateCustomer(id, name, type, sourceid, activityid, industryid, extent, citycode, address, contactname, mobile, officephone, email, jobs, desc, ownerid, operateid, agentid, clientid);
            if (!bl)
            {
                id = "";
            }
            return id;
        }

        #endregion

        #region 编辑/删除

        #endregion
    }
}
