using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalesDAL
{
    public class CustomDAL : BaseDAL
    {
        public static CustomDAL BaseProvider = new CustomDAL();

        #region 查询

        #endregion

        #region 添加

        public bool CreateCustomer(string customerid, string name, int type, string sourceid, string activityid, string industryid, int extent, string citycode, string address, string contactname, 
                                   string mobile, string officephone, string email, string jobs, string desc, string ownerid, string operateid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@CustomerID",customerid),
                                     new SqlParameter("@Name",name),
                                     new SqlParameter("@Type",type),
                                     new SqlParameter("@SourceID",sourceid),
                                     new SqlParameter("@ActivityID",activityid),
                                     new SqlParameter("@IndustryID" , industryid),
                                     new SqlParameter("@Extent" , extent),
                                     new SqlParameter("@CityCode" , citycode),
                                     new SqlParameter("@Address" , address),
                                     new SqlParameter("@ContactName" , contactname),
                                     new SqlParameter("@MobilePhone" , mobile),
                                     new SqlParameter("@OfficePhone" , officephone),
                                     new SqlParameter("@Email" , email),
                                     new SqlParameter("@Jobs" , jobs),
                                     new SqlParameter("@Description" , desc),
                                     new SqlParameter("@OwnerID" , ownerid),
                                     new SqlParameter("@CreateUserID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_CreateCustomer", paras, CommandType.StoredProcedure) > 0;
        }

        #endregion

        #region 编辑/删除

        #endregion
    }
}
