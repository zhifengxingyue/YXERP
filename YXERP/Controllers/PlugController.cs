﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CloudSalesBusiness;
using System.IO;

namespace YXERP.Controllers
{
    public class PlugController : BaseController
    {
        /// <summary>
        /// 根据cityCode获取下级地区列表
        /// </summary>
        /// <param name="pcode"></param>
        /// <returns></returns>
        public JsonResult GetCityByPCode(string cityCode)
        {
            var list = CommonBusiness.GetCitys().Where(c => c.PCode == cityCode);
            JsonDictionary.Add("Items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        public JsonResult UploadFile()
        {
            string oldPath = "",
                   folder = CloudSalesTool.AppSettings.Settings["UploadTempPath"], 
                   action = "";
            if (Request.Form.AllKeys.Contains("oldPath"))
            {
                oldPath = Request.Form["oldPath"];
            }
            if (Request.Form.AllKeys.Contains("folder") && !string.IsNullOrEmpty(Request.Form["folder"]))
            {
                folder = Request.Form["folder"];
            }
            string uploadPath = HttpContext.Server.MapPath(folder);

            if (Request.Form.AllKeys.Contains("action"))
            {
                action = Request.Form["action"];
            }
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            List<string> list = new List<string>();
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i];
                //判断图片类型
                string ContentType = file.ContentType;
                Dictionary<string, string> types = new Dictionary<string, string>();
                types.Add("image/x-png", "1");
                types.Add("image/png", "1");
                types.Add("image/gif", "1");
                types.Add("image/jpeg", "1");
                types.Add("image/tiff", "1");
                types.Add("application/x-MS-bmp", "1");
                types.Add("image/pjpeg", "1");
                if (!types.ContainsKey(ContentType))
                {
                    continue;
                }
                if (string.IsNullOrEmpty(oldPath) || oldPath == "/modules/images/default.png")
                {
                    string[] arr = file.FileName.Split('.');
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmssms") + new Random().Next(1000, 9999).ToString() + "." + arr[arr.Length - 1];
                    string filePath = uploadPath + fileName;
                    file.SaveAs(filePath);
                    list.Add(folder + fileName);
                }
                else
                {
                    file.SaveAs(HttpContext.Server.MapPath(oldPath));
                    list.Add(oldPath);
                }
            }

            JsonDictionary.Add("Items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        } 

    }
}
