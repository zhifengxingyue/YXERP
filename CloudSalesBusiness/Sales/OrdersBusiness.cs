using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CloudSalesEnum;
using CloudSalesDAL;
using CloudSalesEntity;
using System.Data;
using System.Data.SqlClient;

namespace CloudSalesBusiness
{
    public class OrdersBusiness
    {
        public static OrdersBusiness BaseBusiness = new OrdersBusiness();
        #region 查询

        /// <summary>
        /// 获取单据列表
        /// </summary>
        /// <param name="userid">创建人（拥有者）</param>
        /// <param name="type">类型</param>
        /// <param name="status">状态</param>
        /// <param name="keywords">关键词</param>
        /// <param name="pageSize">页Size</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总数</param>
        /// <param name="pageCount"><总页/param>
        /// <param name="clientID">客户端ID</param>
        /// <returns></returns>
        public static List<StorageDoc> GetStorageDocList(string userid, EnumDocType type, EnumDocStatus status, string keywords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientID)
        {
            DataSet ds = OrdersDAL.GetStorageDocList(userid, (int)type, (int)status, keywords, pageSize, pageIndex, ref totalCount, ref pageCount, clientID);

            List<StorageDoc> list = new List<StorageDoc>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                StorageDoc model = new StorageDoc();
                model.FillData(dr);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, clientID);
                model.StatusStr = GetDocStatusStr(model.DocType, model.Status);

                list.Add(model);
            }
            return list;
        }
        
        /// <summary>
        /// 获取单据详情
        /// </summary>
        /// <param name="docid">单据ID</param>
        /// <param name="clientid">客户端ID</param>
        /// <returns></returns>
        public static StorageDoc GetStorageDetail(string docid, string clientid)
        {
            DataSet ds = OrdersDAL.GetStorageDetail(docid, clientid);
            StorageDoc model = new StorageDoc();
            if (ds.Tables.Contains("Doc") && ds.Tables["Doc"].Rows.Count > 0)
            {
                model.FillData(ds.Tables["Doc"].Rows[0]);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, clientid);
                model.StatusStr = GetDocStatusStr(model.DocType, model.Status);

                model.Details = new List<StorageDetail>();
                foreach (DataRow item in ds.Tables["Details"].Rows)
                {
                    StorageDetail details = new StorageDetail();
                    details.FillData(item);
                    model.Details.Add(details);
                }
            }

            return model;
        }

        /// <summary>
        /// 单据状态
        /// </summary>
        /// <param name="doctype">类型</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        private static string GetDocStatusStr(int doctype, int status)
        {
            string str = "";
            switch (status)
            {
                case 0:
                    str = "待审核";
                    break;
                case 1:
                    str = doctype == 1 ? "部分上架" 
                        : doctype == 2 ? "部分出库" 
                        : "部分审核";
                    break;
                case 2:
                    str = doctype == 1 ? "已上架"
                        : doctype == 2 ? "已出库"
                        : "已审核";
                    break;
                case 4:
                    str = "已作废";
                    break;
                case 9:
                    str = "已删除";
                    break;
            }
            return str;
        }

        /// <summary>
        /// 获取单据操作记录
        /// </summary>
        /// <param name="docid"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="totalCount"></param>
        /// <param name="pageCount"></param>
        /// <param name="clientID"></param>
        /// <returns></returns>
        public static List<StorageDocAction> GetStorageDocAction(string docid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string agentid)
        {
            DataTable dt = CommonBusiness.GetPagerData("StorageDocAction", "*", "DocID='" + docid + "'", "AutoID", pageSize, pageIndex, out totalCount, out pageCount);

            List<StorageDocAction> list = new List<StorageDocAction>();
            foreach (DataRow dr in dt.Rows)
            {
                StorageDocAction model = new StorageDocAction();
                model.FillData(dr);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, agentid);

                list.Add(model);
            }
            return list;
        }
        

        #endregion

        #region 添加


        public string CreateOrder(string customerid, string operateid, string agentid, string clientid)
        {
            string id = Guid.NewGuid().ToString();
            string code = DateTime.Now.ToString("yyyyMMddHHmmssfff");

            bool bl = OrdersDAL.BaseProvider.CreateOrder(id, code, customerid, operateid, agentid, clientid);
            if (!bl)
            {
                return "";
            }
            return id;
        }

        /// <summary>
        /// 创建单据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string CreateStorageDoc(StorageDoc model, string userid, string operateip, string clientid)
        {
            string docid = Guid.NewGuid().ToString();
            SqlConnection conn = new SqlConnection(OrdersDAL.ConnectionString);
            conn.Open();
            SqlTransaction tran = conn.BeginTransaction();

            try
            {
                bool bl = OrdersDAL.AddStorageDoc(docid, model.DocType, model.TotalMoney, model.CityCode, model.Address, model.Remark, userid, operateip, clientid, tran);
                if (bl)
                {
                    //单据明细
                    foreach (var detail in model.Details)
                    {
                        if (!OrdersDAL.AddStorageDocDetail(docid, detail.AutoID, detail.ProductDetailID, detail.Quantity, detail.Price, detail.TotalMoney, detail.BatchCode, clientid, tran))
                        {
                            tran.Rollback();
                            conn.Dispose();
                            return "";
                        }
                    }
                    tran.Commit();
                    conn.Dispose();
                    return docid;
                }
                else
                {
                    tran.Rollback();
                    conn.Dispose();
                    return "";
                }
            }
            catch
            {
                tran.Rollback();
                conn.Dispose();
                return "";
            }            
        }

        #endregion

        #region 编辑、删除

        
        /// <summary>
        /// 删除单据
        /// </summary>
        /// <param name="docid">单据ID</param>
        /// <param name="userid">操作人</param>
        /// <param name="operateip">操作IP</param>
        /// <param name="clientid">客户端ID</param>
        /// <returns></returns>
        public bool DeleteDoc(string docid, string userid, string operateip, string clientid)
        {
            return new OrdersDAL().UpdateStorageStatus(docid, (int)EnumDocStatus.Delete, "删除单据", userid, operateip, clientid);
        }
        /// <summary>
        /// 作废单据
        /// </summary>
        /// <param name="docid"></param>
        /// <param name="userid"></param>
        /// <param name="operateip"></param>
        /// <param name="clientid"></param>
        /// <returns></returns>
        public bool InvalidDoc(string docid, string userid, string operateip, string clientid)
        {
            return new OrdersDAL().UpdateStorageStatus(docid, (int)EnumDocStatus.Invalid, "作废单据", userid, operateip, clientid);
        }
        /// <summary>
        /// 更换入库仓库
        /// </summary>
        /// <param name="autoid">单据详情ID</param>
        /// <param name="wareid">仓库ID</param>
        /// <param name="depotid">货位ID</param>
        /// <param name="clientid"></param>
        /// <returns></returns>
        public bool UpdateStorageDetailWare(string autoid, string wareid, string depotid, string userid, string operateip, string clientid)
        {
            return new OrdersDAL().UpdateStorageDetailWare(autoid, wareid, depotid);
        }
        /// <summary>
        /// 审核上架
        /// </summary>
        /// <param name="ids">明细ID</param>
        /// <param name="userid">审核人</param>
        /// <param name="clientid">客户端ID</param>
        /// <returns></returns>
        public bool AuditStorageIn(string ids, string userid, string operateip, string clientid)
        {
            bool bl = false;

            foreach (string autoid in ids.Split(','))
            {
                if (new OrdersDAL().AuditStorageIn(autoid, userid, operateip, clientid))
                {
                    bl = true;
                }
            }

            return bl;
        }

        #endregion
    }
}
