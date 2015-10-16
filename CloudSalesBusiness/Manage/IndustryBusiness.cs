using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CloudSalesDAL;
using CloudSalesEntity;
using System.Data;

namespace CloudSalesBusiness
{
    public class IndustryBusiness
    {
        #region Cache

        private static List<Industry> _industrys;

        /// <summary>
        /// 行业
        /// </summary>
        public static List<Industry> Industrys
        {
            get
            {
                if (_industrys == null)
                {
                    _industrys = new List<Industry>();
                }
                return _industrys;
            }
            set
            {
                _industrys = value;
            }
        }

        #endregion

        #region 查询

        /// <summary>
        /// 获取行业列表
        /// </summary>
        /// <param name="clientid"></param>
        /// <returns></returns>
        public static List<Industry> GetIndustrys()
        {
            if (IndustryBusiness.Industrys.Count > 0)
            {
                return IndustryBusiness.Industrys;
            }
            else
            {
                List<Industry> list = new List<Industry>();
                DataTable dt = new IndustryDAL().GetIndustrys();
                foreach (DataRow item in dt.Rows)
                {
                    Industry model = new Industry();
                    model.FillData(item);
                    list.Add(model);
                }
                IndustryBusiness.Industrys = list;
                return list;
            }
        }

        #endregion

        #region 添加

        /// <summary>
        /// 添加行业
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="description">描述</param>
        /// <param name="userid">操作人</param>
        /// <param name="clientid">客户端ID</param>
        /// <returns>行业ID</returns>
        public string InsertIndustry(string name, string description, string userid, string clientid)
        {
            string id = new IndustryDAL().InsertIndustry(name, description, userid, clientid);
            //处理缓存
            if (!string.IsNullOrEmpty(id))
            {
                IndustryBusiness.Industrys.Add(new Industry()
                {
                    IndustryID = id,
                    Name = name,
                    CreateUserID = userid,
                    ClientID = clientid,
                    CreateTime = DateTime.Now,
                    Description = description
                });
            }
            return id;
        }

        #endregion
    }
}
