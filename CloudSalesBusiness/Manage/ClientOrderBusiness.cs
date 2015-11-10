using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using CloudSalesDAL.Manage;
using CloudSalesEntity.Manage;
namespace CloudSalesBusiness.Manage
{
    public class ClientOrderBusiness
    {
        #region 增
        /// <summary>
        /// 新增后台客户订单
        /// </summary>
        public static string AddClientOrder(ClientOrder model)
        {

            string orderID = Guid.NewGuid().ToString();
            SqlConnection conn = new SqlConnection(ClientOrderDAL.ConnectionString);
            conn.Open();
            SqlTransaction tran = conn.BeginTransaction();

            try
            {
                bool bl = ClientOrderDAL.AddClientOrder(orderID,model.UserQuantity,model.Years,model.Amount,model.RealAmount,model.AgentID,model.ClientID,model.CreateUserID, tran);
                if (bl)
                {
                    //单据明细
                    foreach (var detail in model.Details)
                    {
                        if ( !ClientOrderDAL.AddClientOrderDetail(orderID,detail.ProductID,detail.Price,detail.Qunatity,detail.CreateUserID,tran))
                        {
                            orderID = string.Empty;
                            tran.Rollback();
                            conn.Dispose();
                        }
                    }

                    tran.Commit();
                    conn.Dispose();
                }
                else
                {
                    orderID = string.Empty;
                    tran.Rollback();
                    conn.Dispose();
                }
            }
            catch
            {
                orderID = string.Empty;
                tran.Rollback();
                conn.Dispose();
            }

            return orderID;
        }

        #endregion
    }
}
