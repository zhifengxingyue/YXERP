using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalesDAL.Manage
{
    public class ClientOrderDAL:BaseDAL
    {
        public static ClientDAL BaseProvider = new ClientDAL();


        #region 添加

        public static bool AddClientOrder(string orderID, int userQuantity, int years, decimal amount, decimal realAmount, string agentID, string clientiD, string createUserID, SqlTransaction tran)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderID),
                                     new SqlParameter("@UserQuantity",userQuantity),
                                     new SqlParameter("@Years" , years),
                                     new SqlParameter("@Amount" , amount),
                                     new SqlParameter("@RealAmount" ,realAmount),
                                     new SqlParameter("@AgentID" , agentID),
                                     new SqlParameter("@ClientiD" , clientiD),
                                     new SqlParameter("@CreateUserID" , createUserID)
                                   };
            return ExecuteNonQuery(tran, "M_AddClientOrder", paras, CommandType.StoredProcedure) > 0;
        }

        public static bool AddClientOrderDetail(string orderID, string productID, decimal price, int qunatity, string createUserID,SqlTransaction tran)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderID),
                                     new SqlParameter("@ProductID",productID),
                                     new SqlParameter("@Price" , price),
                                     new SqlParameter("@Quantity" , qunatity),
                                     new SqlParameter("@CreateUserID" , createUserID)
                                   };


            return ExecuteNonQuery(tran, "M_AddClientOrderDetail", paras, CommandType.StoredProcedure) > 0;

        }

        #endregion
    }
}
