using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CloudSalesEntity;
using System.Web.Script.Serialization;

public static class ExpandClass
{
    /// <summary>
    /// 顶层菜单编码
    /// </summary>
    public const string CLIENT_TOP_CODE = "100000000";
    /// <summary>
    /// 默认菜单编码
    /// </summary>
    public const string CLIENT_DEFAULT_CODE = "101000000";

    /// <summary>
    /// 获取下级菜单
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="menuCode"></param>
    /// <returns></returns>
    public static List<Menu> GetChildMenuByCode(HttpContext httpContext, string menuCode)
    {
        if (httpContext.Session["ClientManager"] != null)
        {
            return ((CloudSalesEntity.Users)httpContext.Session["ClientManager"]).Menus.Where(m => m.PCode == menuCode).ToList();
        }
        else
        {
            return new List<Menu>();
        }
    }
    /// <summary>
    /// 加载二级菜单
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public static List<Menu> GetControllers(HttpContext httpContext, string controller)
    {
        if (httpContext.Session["ClientManager"] != null)
        {
            CloudSalesEntity.Users model = (CloudSalesEntity.Users)httpContext.Session["ClientManager"];
            var menu = model.Menus.Where(m => m.Controller.ToUpper() == controller.ToUpper() && m.Layer == 2).FirstOrDefault();
            if (menu != null)
            {
                return model.Menus.Where(m => m.PCode == menu.PCode).ToList();
            }
        }
        return new List<Menu>();
    }

    /// <summary>
    /// 加载三级菜单
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public static List<Menu> GetActions(HttpContext httpContext, string controller)
    {
        if (!string.IsNullOrEmpty(controller))
        {
            return ((CloudSalesEntity.Users)httpContext.Session["ClientManager"]).Menus.Where(m => m.Controller.ToUpper() == controller.ToUpper() && m.Layer == 3).ToList();
        }
        else
        {
            return new List<Menu>();
        }
    }

    /// <summary>
    /// 将对象转换成JSON对象
    /// </summary>
    /// <param name="html"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string ToJSONString(this HtmlHelper html, object data)
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        if (data != null && !string.IsNullOrEmpty(data.ToString()))
        {
            return serializer.Serialize(data);
        }

        return string.Empty;
    }
}
