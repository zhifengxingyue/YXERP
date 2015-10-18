﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
