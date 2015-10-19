using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using MD.SDK.Business;
using CloudSalesBusiness;

namespace YXERP.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            if (Session["ClientManager"] == null)
            {
                return Redirect("/Home/Login");
            }
            return View();
        }

        public ActionResult Login()
        {
            if (Session["ClientManager"] != null)
            {
                return Redirect("/Home/Index");
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session["ClientManager"] = null;
            return Redirect("/Home/Login");
        }

        /// <summary>
        /// 明道登录
        /// </summary>
        /// <returns></returns>
        public ActionResult MDLogin()
        {
            return Redirect(OauthBusiness.GetAuthorizeUrl());
        }

        //处理明道登录回掉
        public ActionResult MDCallBack(string code)
        {
            var user = OauthBusiness.GetMDUser(code);
            //是否已添加到云销
            var model = OrganizationBusiness.GetUserByMDUserID(user.user.id, user.user.project.id);
            if (!string.IsNullOrEmpty(model.UserID))
            {
                model.MDToken = user.user.token;
                Session["ClientManager"] = model;
                return Redirect("/Home/Index");
            }
            else
            {
                int error = 0;
                bool isAdmin = MD.SDK.Entity.App.AppBusiness.IsAppAdmin(user.user.token, user.user.id, out error);
                if (isAdmin)
                {

                }
                else
                {
 
                }
            }
            return View();
        }


        /// <summary>
        /// 员工登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public JsonResult UserLogin(string userName, string pwd)
        {
            bool bl = false;

            string operateip = string.IsNullOrEmpty(Request.Headers.Get("X-Real-IP")) ? Request.UserHostAddress : Request.Headers["X-Real-IP"];

            CloudSalesEntity.Users model = CloudSalesBusiness.OrganizationBusiness.GetUserByUserName(userName, pwd, operateip);
            if (model != null)
            {
                Session["ClientManager"] = model;
                bl = true;
            }
            return new JsonResult
            {
                Data = bl,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

    }
}
