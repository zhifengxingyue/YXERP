using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CloudSalesDAL;
using CloudSalesEntity;
using System.Data;

namespace CloudSalesBusiness
{
    public class FinanceBusiness
    {
        public static FinanceBusiness BaseBusiness = new FinanceBusiness();
        #region 查询

        public List<StorageBilling> GetPayableBills(int paystatus, int invoicestatus, string begintime, string endtime, string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string agentid, string clientid)
        {
            List<StorageBilling> list = new List<StorageBilling>();
            DataSet ds = FinanceDAL.BaseProvider.GetPayableBills(paystatus, invoicestatus, begintime, endtime, keyWords, pageSize, pageIndex, ref totalCount, ref pageCount, userid, agentid, clientid);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                StorageBilling model = new StorageBilling();
                model.FillData(dr);

                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, model.AgentID);

                model.PayStatusStr = model.PayStatus == 0 ? "未付款"
                                : model.PayStatus == 1 ? "部分付款"
                                : model.PayStatus == 2 ? "已付款"
                                : model.PayStatus == 9 ? "已删除"
                                : "";

                model.InvoiceStatusStr = model.InvoiceStatus == 0 ? "未开票"
                                : model.InvoiceStatus == 1 ? "部分开票"
                                : model.InvoiceStatus == 2 ? "已开票"
                                : model.InvoiceStatus == 9 ? "已删除"
                                : "";

                list.Add(model);
            }
            return list;
        }

        public StorageBilling GetPayableBillByID(string billingid, string agentid, string clientid)
        {
            StorageBilling model = new StorageBilling();
            DataSet ds = FinanceDAL.BaseProvider.GetPayableBillByID(billingid, agentid, clientid);
            if (ds.Tables["Billing"].Rows.Count > 0)
            {
                model.FillData(ds.Tables["Billing"].Rows[0]);

                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, model.AgentID);

                model.PayStatusStr = model.PayStatus == 0 ? "未付款"
                                : model.PayStatus == 1 ? "部分付款"
                                : model.PayStatus == 2 ? "已付款"
                                : model.PayStatus == 9 ? "已删除"
                                : "";

                model.InvoiceStatusStr = model.InvoiceStatus == 0 ? "未开票"
                                : model.InvoiceStatus == 1 ? "部分开票"
                                : model.InvoiceStatus == 2 ? "已开票"
                                : model.InvoiceStatus == 9 ? "已删除"
                                 : "";

                model.StorageBillingPays = new List<StorageBillingPay>();
                foreach (DataRow dr in ds.Tables["Pays"].Rows)
                {
                    StorageBillingPay pay = new StorageBillingPay();
                    pay.FillData(dr);
                    pay.CreateUser = OrganizationBusiness.GetUserByUserID(pay.CreateUserID, pay.AgentID);
                    model.StorageBillingPays.Add(pay);
                }

                model.StorageBillingInvoices = new List<StorageBillingInvoice>();
                foreach (DataRow dr in ds.Tables["Invoices"].Rows)
                {
                    StorageBillingInvoice invoice = new StorageBillingInvoice();
                    invoice.FillData(dr);
                    invoice.CreateUser = OrganizationBusiness.GetUserByUserID(invoice.CreateUserID, invoice.AgentID);
                    model.StorageBillingInvoices.Add(invoice);
                }
            }
            return model;
        }

        #endregion

        #region 添加

        public bool CreateStorageBillingPay(string billingid, int type, int paytype, decimal paymoney, DateTime paytime, string remark, string userid, string agentid, string clientid)
        {
            bool bl = FinanceDAL.BaseProvider.CreateStorageBillingPay(billingid, type, paytype, paymoney, paytime, remark, userid, agentid, clientid);
            return bl;
        }

        public string CreateStorageBillingInvoice(string billingid, int type, decimal invoicemoney, string invoicecode, string remark, string userid, string agentid, string clientid)
        {
            string id = Guid.NewGuid().ToString().ToLower();
            bool bl = FinanceDAL.BaseProvider.CreateStorageBillingInvoice(id, billingid, type, invoicemoney, invoicecode, remark, userid, agentid, clientid);
            if (bl)
            {
                return id;
            }
            return "";
        }

        #endregion

        #region 编辑/删除

        public bool DeleteStorageBillingInvoice(string invoiceid, string billingid, string userid, string agentid, string clientid)
        {
            return FinanceDAL.BaseProvider.DeleteStorageBillingInvoice(invoiceid, billingid, userid, agentid, clientid);
        }

        #endregion
    }
}
