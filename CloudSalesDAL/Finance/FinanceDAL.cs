﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalesDAL
{
    public class FinanceDAL : BaseDAL
    {
        public static FinanceDAL BaseProvider = new FinanceDAL();

        #region 查询

        public DataSet GetPayableBills(int paystatus, int invoicestatus, string begintime, string endtime, string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@totalCount",SqlDbType.Int),
                                       new SqlParameter("@pageCount",SqlDbType.Int),
                                       new SqlParameter("@PayStatus",paystatus),
                                       new SqlParameter("@InvoiceStatus",invoicestatus),
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@Keywords",keyWords),
                                       new SqlParameter("@pageSize",pageSize),
                                       new SqlParameter("@pageIndex",pageIndex),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Value = totalCount;
            paras[1].Value = pageCount;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            DataSet ds = GetDataSet("P_GetPayableBills", paras, CommandType.StoredProcedure);
            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);
            return ds;
        }

        public DataSet GetPayableBillByID(string billingid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@BillingID",billingid),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };

            DataSet ds = GetDataSet("P_GetPayableBillByID", paras, CommandType.StoredProcedure, "Billing|Pays|Invoices");
            return ds;
        }

        public DataSet GetOrderBills(int paystatus, int invoicestatus, string begintime, string endtime, string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@totalCount",SqlDbType.Int),
                                       new SqlParameter("@pageCount",SqlDbType.Int),
                                       new SqlParameter("@PayStatus",paystatus),
                                       new SqlParameter("@InvoiceStatus",invoicestatus),
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@Keywords",keyWords),
                                       new SqlParameter("@pageSize",pageSize),
                                       new SqlParameter("@pageIndex",pageIndex),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Value = totalCount;
            paras[1].Value = pageCount;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            DataSet ds = GetDataSet("P_GetOrderBills", paras, CommandType.StoredProcedure);
            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);
            return ds;
        }

        public DataSet GetOrderBillByID(string billingid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@BillingID",billingid),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };

            DataSet ds = GetDataSet("P_GetOrderBillByID", paras, CommandType.StoredProcedure, "Billing|Pays|Invoices");
            return ds;
        }

        #endregion

        #region 添加

        public bool CreateStorageBillingPay(string billingid, int type, int paytype, decimal paymoney, DateTime paytime, string remark, string userid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@BillingID",billingid),
                                       new SqlParameter("@Type",type),
                                       new SqlParameter("@PayType",paytype),
                                       new SqlParameter("@PayMoney",paymoney),
                                       new SqlParameter("@PayTime",paytime),
                                       new SqlParameter("@Remark",remark),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };

            return ExecuteNonQuery("P_CreateStorageBillingPay", paras, CommandType.StoredProcedure) > 0;
        }

        public bool CreateStorageBillingInvoice(string id, string billingid, int type, decimal invoicemoney, string invoicecode, string remark, string userid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@InvoiceID",id),
                                       new SqlParameter("@BillingID",billingid),
                                       new SqlParameter("@Type",type),
                                       new SqlParameter("@InvoiceMoney",invoicemoney),
                                       new SqlParameter("@InvoiceCode",invoicecode),
                                       new SqlParameter("@Remark",remark),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };

            return ExecuteNonQuery("P_CreateStorageBillingInvoice", paras, CommandType.StoredProcedure) > 0;
        }

        public bool CreateBillingPay(string billingid, int type, int paytype, decimal paymoney, DateTime paytime, string remark, string userid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@BillingID",billingid),
                                       new SqlParameter("@Type",type),
                                       new SqlParameter("@PayType",paytype),
                                       new SqlParameter("@PayMoney",paymoney),
                                       new SqlParameter("@PayTime",paytime),
                                       new SqlParameter("@Remark",remark),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };

            return ExecuteNonQuery("P_CreateBillingPay", paras, CommandType.StoredProcedure) > 0;
        }

        #endregion

        #region 编辑/删除

        public bool DeleteStorageBillingInvoice(string invoiceid, string billingid, string userid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@InvoiceID",invoiceid),
                                       new SqlParameter("@BillingID",billingid),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };

            return ExecuteNonQuery("P_DeleteStorageBillingInvoice", paras, CommandType.StoredProcedure) > 0;
        }

        #endregion
    }
}
