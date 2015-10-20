using System;
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

        public ActionResult InfoPage() 
        {
            return View();
        }
        /// <summary>
        /// 明道登录
        /// </summary>
        /// <returns></returns>
        public ActionResult MDLogin()
        {
            return Redirect(OauthBusiness.GetAuthorizeUrl());
        }

        //明道登录回掉
        public ActionResult MDCallBack(string code)
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
                        Session["ClientManager"] = model;
                        return Redirect("/Home/Index");
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
                                Session["ClientManager"] = OrganizationBusiness.GetUserByMDUserID(user.user.id, user.user.project.id, operateip);
                                return Redirect("/Home/Index");
                            }

                        }
                        else
                        {
                            int result = 0;
                            string userid = OrganizationBusiness.CreateUser("", "", user.user.name, user.user.mobile_phone, user.user.email, "", "", "", "", "", "", "", user.user.id, user.user.project.id, 1, "", out result);
                            if (!string.IsNullOrEmpty(userid))
                            {
                                Session["ClientManager"] = OrganizationBusiness.GetUserByMDUserID(user.user.id, user.user.project.id, operateip);
                                return Redirect("/Home/Index");
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
