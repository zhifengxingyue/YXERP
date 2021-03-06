﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using MD.SDK.Business;
using CloudSalesBusiness;
using CloudSalesEntity;

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

        public ActionResult Login(string ReturnUrl)
        {
            if (Session["ClientManager"] != null)
            {
                return Redirect("/Home/Index");
            }
            HttpCookie cook = Request.Cookies["cloudsales"];
            if (cook != null)
            {
                if (cook["status"] == "1")
                {
                    string operateip = string.IsNullOrEmpty(Request.Headers.Get("X-Real-IP")) ? Request.UserHostAddress : Request.Headers["X-Real-IP"];
                    CloudSalesEntity.Users model = CloudSalesBusiness.OrganizationBusiness.GetUserByUserName(cook["username"], cook["pwd"], operateip);
                    if (model != null)
                    {
                        Session["ClientManager"] = model;
                        return Redirect("/Home/Index");
                    }
                }
                else
                {
                    ViewBag.UserName = cook["username"];
                }
            }

            ViewBag.ReturnUrl = ReturnUrl??string.Empty;
            return View();
        }

        public ActionResult Logout()
        {
            HttpCookie cook = Request.Cookies["cloudsales"];
            if (cook != null)
            {
                cook["status"] = "0";
                Response.Cookies.Add(cook);
            }
            

            Session["ClientManager"] = null;
            return Redirect("/Home/Login");
        }

        public ActionResult InfoPage() 
        {
            return View();
        }

        /// <summary>
        /// 明道登录
        /// </summary>
        /// <returns></returns>
        public ActionResult MDLogin(string ReturnUrl)
        {
            if(string.IsNullOrEmpty(ReturnUrl))
            return Redirect(OauthBusiness.GetAuthorizeUrl());
            else
                return Redirect(OauthBusiness.GetAuthorizeUrl() + "&state=" + ReturnUrl);
        }

        //明道登录回掉
        public ActionResult MDCallBack(string code, string state)
        {
            string operateip = string.IsNullOrEmpty(Request.Headers.Get("X-Real-IP")) ? Request.UserHostAddress : Request.Headers["X-Real-IP"];
            var user = OauthBusiness.GetMDUser(code);
            if (user.error_code <= 0)
            {
                var model = OrganizationBusiness.GetUserByMDUserID(user.user.id, user.user.project.id, operateip);
                //已注册云销账户
                if (model != null)
                {
                    //未注销
                    if (model.Status.Value != 9)
                    {
                        model.MDToken = user.user.token;
                        if (string.IsNullOrEmpty(model.Avatar)) model.Avatar = user.user.avatar;

                        Session["ClientManager"] = model;
                        if (string.IsNullOrEmpty(state))
                            return Redirect("/Home/Index");
                        else
                            return Redirect(state);
                    }
                }
                else
                {
                    int error = 0;
                    bool isAdmin = MD.SDK.Entity.App.AppBusiness.IsAppAdmin(user.user.token, user.user.id, out error);
                    if (isAdmin)
                    {
                        bool bl = AgentsBusiness.IsExistsMDProject(user.user.project.id);
                        //明道网络未注册
                        if (!bl)
                        {
                            int result = 0;
                            Clients clientModel = new Clients();
                            clientModel.CompanyName = user.user.project.name;
                            clientModel.ContactName = user.user.name;
                            clientModel.MobilePhone = user.user.mobile_phone;
                            var clientid = ClientBusiness.InsertClient(clientModel, "", "", "", out result, user.user.email, user.user.id, user.user.project.id);
                            if (!string.IsNullOrEmpty(clientid))
                            {
                                var current = OrganizationBusiness.GetUserByMDUserID(user.user.id, user.user.project.id, operateip);

                                current.MDToken = user.user.token;
                                if (string.IsNullOrEmpty(current.Avatar)) current.Avatar = user.user.avatar;
                                Session["ClientManager"] = current;

                                if(string.IsNullOrEmpty(state))
                                return Redirect("/Home/Index");
                                else
                                    return Redirect(state);
                            }

                        }
                        else
                        {
                            int result = 0;
                            var current = OrganizationBusiness.CreateUser("", "", user.user.name, user.user.mobile_phone, user.user.email, "", "", "", "", "", "", "", "", user.user.id, user.user.project.id, 1, "", out result);
                            if (current != null)
                            {
                                current.MDToken = user.user.token;
                                if (string.IsNullOrEmpty(current.Avatar)) current.Avatar = user.user.avatar;

                                Session["ClientManager"] = current;

                                if (string.IsNullOrEmpty(state))
                                    return Redirect("/Home/Index");
                                else
                                    return Redirect(state);
                            }
                        }
                    }
                    else
                    {
                        return Redirect("/Home/InfoPage");
                    }
                }
            }
            return Redirect("/Home/Login");
        }

        /// <summary>
        /// 员工登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public JsonResult UserLogin(string userName, string pwd, string remember)
        {
            bool bl = false;

            string operateip = string.IsNullOrEmpty(Request.Headers.Get("X-Real-IP")) ? Request.UserHostAddress : Request.Headers["X-Real-IP"];

            CloudSalesEntity.Users model = CloudSalesBusiness.OrganizationBusiness.GetUserByUserName(userName, pwd, operateip);
            if (model != null)
            {
                //保持登录状态
                HttpCookie cook = new HttpCookie("cloudsales");
                cook["username"] = userName;
                cook["pwd"] = pwd;
                cook["status"] = remember;
                cook.Expires = DateTime.Now.AddDays(7);
                Response.Cookies.Add(cook);

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
