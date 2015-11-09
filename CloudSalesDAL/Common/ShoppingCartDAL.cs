﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalesDAL
{
    public class ShoppingCartDAL : BaseDAL
    {
        public static DataTable GetShoppingCart(int ordertype, string userid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderType",ordertype),
                                     new SqlParameter("@UserID" , userid),
                                   };
            return GetDataTable("P_GetShoppingCart", paras, CommandType.StoredProcedure);
        }

        public static bool AddShoppingCart(string productid, string detailsid, int quantity, string unitid, int isBigUnit, int ordertype, string remark, string userid, string operateip)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderType",ordertype),
                                     new SqlParameter("@ProductDetailID",detailsid),
                                     new SqlParameter("@ProductID" , productid),
                                     new SqlParameter("@Quantity" , quantity),
                                     new SqlParameter("@UnitID" , unitid),
                                     new SqlParameter("@IsBigUnit" , isBigUnit),
                                     new SqlParameter("@Remark" , remark),
                                     new SqlParameter("@UserID" , userid),
                                     new SqlParameter("@OperateIP" , operateip)
                                   };
            return ExecuteNonQuery("P_AddShoppingCart", paras, CommandType.StoredProcedure) > 0;
        }

    }
}
