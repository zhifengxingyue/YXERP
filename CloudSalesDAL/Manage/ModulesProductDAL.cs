using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalesDAL.Manage
{
    public class ModulesProductDAL:BaseDAL
    {
        public static ModulesProductDAL BaseProvider = new ModulesProductDAL();
        #region 查询
        public DataTable GetModulesProductDetail(int id)
        {

            SqlParameter[] paras = { 
                                    new SqlParameter("@AutoID",id),
                                   };
            return GetDataTable("select * from ModulesProduct where AutoID=@AutoID and Status<>9", paras, CommandType.Text);
        }
        #endregion

        #region 添加

        public bool InsertModulesProduct(string modulesID, int period, int periodQuantity, int userQuantity, decimal price, 
                                   string description,  string userid)
        {
            SqlParameter[] parms = { 
                                       new SqlParameter("@ModulesID",modulesID),
                                       new SqlParameter("@Period",period),
                                       new SqlParameter("@PeriodQuantity",periodQuantity),
                                       new SqlParameter("@UserQuantity",userQuantity),
                                       new SqlParameter("@Price",price),
                                       new SqlParameter("@Description",description),
                                       new SqlParameter("@CreateUserID",userid)
                                   };

            string cmdTxt = "insert into ModulesProduct(ModulesID,Period,PeriodQuantity,UserQuantity,Price,Description,CreateUserID,CreateTime) values(@ModulesID,@Period,@PeriodQuantity,@UserQuantity,@Price,@Description,@CreateUserID,getdate())";

            return ExecuteNonQuery(cmdTxt, parms, CommandType.Text) > 0;
        }

        #endregion

        #region 编辑
       public bool UpdateModulesProduct(int autoID,string modulesID, int period, int periodQuantity, int userQuantity, decimal price, 
                                   string description)
        {
            SqlParameter[] parms = {
                                       new SqlParameter("@AutoID",autoID),
                                       new SqlParameter("@ModulesID",modulesID),
                                       new SqlParameter("@Period",period),
                                       new SqlParameter("@PeriodQuantity",periodQuantity),
                                       new SqlParameter("@UserQuantity",userQuantity),
                                       new SqlParameter("@Price",price),
                                       new SqlParameter("@Description",description),
                                   };

            string cmdTxt = "update ModulesProduct set ModulesID=@ModulesID,Period=@Period,PeriodQuantity=@PeriodQuantity,UserQuantity=@UserQuantity,Price=@Price,Description=@Description where AutoID=@AutoID";

            return ExecuteNonQuery(cmdTxt, parms, CommandType.Text) > 0;
        }

        #endregion

        #region 删
       public bool DeleteModulesProduct(int id)
       {
           SqlParameter[] parms = {
                                       new SqlParameter("@AutoID",id),
                                   };

           string cmdTxt = "update ModulesProduct set status=9 where AutoID=@AutoID";

           return ExecuteNonQuery(cmdTxt, parms, CommandType.Text) > 0;
       }
        #endregion
    }
}
