using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CloudSalesDAL.Manage;
using CloudSalesEntity.Manage;
using System.IO;
using System.Web;
namespace CloudSalesBusiness.Manage
{
    public class FeedBackBusiness
    {
        public static string FilePath = CloudSalesTool.AppSettings.Settings["UploadFilePath"];

        #region 增
        /// <summary>
        /// 新增建议反馈
        /// </summary>
        public static bool InsertFeedBack(FeedBack model)
        {
            string filePath = model.FilePath;
            if (!string.IsNullOrEmpty(filePath))
            {
                if (filePath.IndexOf("?") > 0)
                {
                    filePath = filePath.Substring(0, filePath.IndexOf("?"));
                }
                FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(filePath));
                filePath = FilePath + file.Name;
                if (file.Exists)
                {
                    file.MoveTo(HttpContext.Current.Server.MapPath(filePath));
                }
            }

            model.FilePath = filePath;
            return FeedBackDAL.BaseProvider.InsertFeedBack(model.Title, model.ContactName, model.MobilePhone, model.Type, model.FilePath, model.Remark, model.CreateUserID);
        }
        #endregion
    }
}
