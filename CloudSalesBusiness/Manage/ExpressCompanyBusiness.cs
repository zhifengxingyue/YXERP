﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CloudSalesEntity.Manage;
using  CloudSalesDAL.Manage;
using System.Data;
namespace CloudSalesBusiness.Manage
{
    public class ExpressCompanyBusiness
    {
        #region  增
        public static bool InsertExpressCompany(ExpressCompany model)
        {
            return ExpressCompanyDAL.BaseProvider.InsertExpressCompany(model.Name, model.Website, model.CreateUserID);
        }
        #endregion

        #region  删
        public static bool DeleteExpressCompany(string id)
        {
            return ExpressCompanyDAL.BaseProvider.DeleteExpressCompany(id);
        }
        #endregion

        #region  查
        public static List<ExpressCompany> GetExpressCompanys()
        {
            DataTable dt = ExpressCompanyDAL.BaseProvider.GetExpressCompanys();
            List<ExpressCompany> list = new List<ExpressCompany>();
            ExpressCompany model;
            foreach (DataRow item in dt.Rows)
            {
                model = new ExpressCompany();
                model.FillData(item);
                list.Add(model);
            }

            return list;
        }

        public static List<ExpressCompany> GetExpressCompanys(string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            DataTable dt = CommonBusiness.GetPagerData("ExpressCompany", "*", " Status<>9 ", "AutoID", pageSize, pageIndex, out totalCount, out pageCount);
            List<ExpressCompany> list = new List<ExpressCompany>();
            ExpressCompany model;
            foreach (DataRow item in dt.Rows)
            {
                model = new ExpressCompany();
                model.FillData(item);
                list.Add(model);
            }

            return list;
        }

        public static ExpressCompany GetExpressCompanyDetail(string id)
        {
            ExpressCompany model = new ExpressCompany();

            DataTable dt = ExpressCompanyDAL.BaseProvider.GetExpressCompanyDetail(id);
            model.FillData(dt.Rows[0]);

            return model;
        }
        #endregion

        #region  改
        public static bool UpdateExpressCompany(ExpressCompany model)
        {
            return ExpressCompanyDAL.BaseProvider.UpdateExpressCompany(model.ExpressID, model.Name, model.Website);
        }
        #endregion
    }
}
