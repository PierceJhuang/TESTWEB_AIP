using AIPWeb.Data;
using AIPWeb.Helpers;
using AIPWeb.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace AIPWeb.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Logout()
        {
            Session["_UserInfo"] = null;

            return RedirectToAction("Index", "Login");
        }

        public JsonResult CheckAccount(string strAccount , string strPassword)
        {
            try
            {
                string IPs = ConfigurationManager.AppSettings["IPList"];

                if (!string.IsNullOrWhiteSpace(IPs))
                {
                    string[] IPList = IPs.Split(';');

                    string ClientIP = GetIPAddress();

                    if (!IPList.Contains(ClientIP))
                    {
                        throw new Exception("IP位置(" + ClientIP + ")不在限定範圍內");
                    }
                }

                if (string.IsNullOrWhiteSpace(strAccount))
                {
                    throw new Exception("請輸入帳號");
                }

                if (string.IsNullOrWhiteSpace(strPassword))
                {
                    throw new Exception("請輸入密碼");
                }

                #region Session

                ServiceHelper serviceHelper = new ServiceHelper();

                SessionUserModel UserInfo = serviceHelper.CheckAccount(strAccount, strPassword);

                Session["_UserInfo"] = UserInfo;

                #endregion


                return Json(new { Result = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message + (ex.InnerException == null ? "" : "<br/>" + ex.InnerException.Message) }, JsonRequestBehavior.AllowGet);
            }
        }

        protected string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }
            return context.Request.ServerVariables["REMOTE_ADDR"];
        }
    }
}