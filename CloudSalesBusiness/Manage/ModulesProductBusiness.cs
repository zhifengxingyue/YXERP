using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CloudSalesEntity.Manage;
using CloudSalesDAL.Manage;
using System.Data;
namespace CloudSalesBusiness.Manage
{
    public class ModulesProductBusiness
    {
        #region  增
        public static bool InsertModulesProduct(ModulesProduct model)
        {
            return ModulesProductDAL.BaseProvider.InsertModulesProduct(model.ModulesID, model.Period, model.PeriodQuantity, model.UserQuantity,
                model.Price, model.Description,model.Type,model.IsChild, model.CreateUserID);
        }
        #endregion

        #region  删
        public static bool DeleteModulesProduct(int id)
        {
            return ModulesProductDAL.BaseProvider.DeleteModulesProduct(id);
        }
        #endregion

        #region  查
        public static List<ModulesProduct> GetModulesProducts(string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            DataTable dt = CommonBusiness.GetPagerData("ModulesProduct as p,Modules as m ", " p.*,m.name ", " p.ModulesID=m.ModulesID and p.Status<>9 ", "p.AutoID", " p.UserQuantity asc,PeriodQuantity asc ", pageSize, pageIndex, out totalCount, out pageCount);
            List<ModulesProduct> list = new List<ModulesProduct>();
            ModulesProduct model;
            foreach (DataRow item in dt.Rows)
            { 
                model = new ModulesProduct();
                model.FillData(item);
                list.Add(model);
            }

            return list;
        }

        public static ModulesProduct GetModulesProductDetail(int id)
        {
            ModulesProduct model=new ModulesProduct();

            DataTable dt = ModulesProductDAL.BaseProvider.GetModulesProductDetail(id);
            model.FillData(dt.Rows[0]);

            return model;
        }
        #endregion

        #region  改
        public static bool UpdateModulesProduct(ModulesProduct model)
        {
            return ModulesProductDAL.BaseProvider.UpdateModulesProduct(model.AutoID,model.ModulesID, model.Period, model.PeriodQuantity, model.UserQuantity,
                model.Price, model.Description,model.Type,model.IsChild);
        }
        #endregion


    }
}
