using AIPWeb.Data;
using AIPWeb.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace AIPWeb.Controllers
{
    public class BaseController : Controller
    {
        protected string GetHost()
        {
            string strHost = Request.Url.Scheme + "://" + Request.Url.Authority;
            if (!string.IsNullOrEmpty(Request.ApplicationPath.Replace("/", "")))
            {
                strHost = strHost + Request.ApplicationPath;
            }
            return strHost;
        }

        public bool IsUserLoggedIn()
        {
            if (Session["_UserInfo"] == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public HeaderModel GetHeader()
        {
            HeaderModel header = new HeaderModel();

            if (IsUserLoggedIn())
            {
                SessionUserModel UserInfo = (SessionUserModel)Session["_UserInfo"];

                header.Account = UserInfo.Account;
                header.RoleName = UserInfo.RoleName;
                header.Role = UserInfo.Role;
                header.Name = UserInfo.Name;
            }

            return header;

        }

        public string GetCurrentUserAccount()
        {
            if (IsUserLoggedIn())
            {

                SessionUserModel UserInfo = (SessionUserModel)Session["_UserInfo"];

                return UserInfo.Account;
            }
            else
            {
                return "";
            }
        }

        public string GetCurrentUserName()
        {
            if (IsUserLoggedIn())
            {
                SessionUserModel UserInfo =(SessionUserModel) Session["_UserInfo"];
                  
                return UserInfo.Name;
            }
            else
            {
                return "";
            }
        }

        public string GetCurrentUserRole()
        {
            if (IsUserLoggedIn())
            {
                SessionUserModel UserInfo = (SessionUserModel)Session["_UserInfo"];

                return UserInfo.Role;
            }
            else
            {
                return "";
            }
        }

        void ValidateRequestHeader(HttpRequestMessage request)
        {
            string cookieToken = "";
            string formToken = "";

            IEnumerable<string> tokenHeaders;
            if (request.Headers.TryGetValues("RequestVerificationToken", out tokenHeaders))
            {
                string[] tokens = tokenHeaders.First().Split(':');
                if (tokens.Length == 2)
                {
                    cookieToken = tokens[0].Trim();
                    formToken = tokens[1].Trim();
                }
            }
            AntiForgery.Validate(cookieToken, formToken);
        }

    }
}