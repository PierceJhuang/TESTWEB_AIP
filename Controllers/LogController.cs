using AIPWeb.Data;
using AIPWeb.Helpers;
using AIPWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace AIPWeb.Controllers
{
    public class LogController : BaseController
    {
        public ActionResult Index()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Login");
            }

            ViewData["Header"] = GetHeader();


            APIHelper aPIHelper = new APIHelper();

            List<Log> EntityList = aPIHelper.GetLogList();

            List<LogModel> LogList = new List<LogModel>();

            if (EntityList != null && EntityList.Count > 0)
            {
                foreach (Log entity in EntityList)
                {
                    LogModel log = new LogModel
                    {

                        No = entity.InsuranceNo,
                        Action = entity.Action,
                        Date = entity.CreatedTime.Value.ToString("yyyy/MM/dd hh:mm:ss"),
                        Name = entity.CreatedUser

                    };
                    LogList.Add(log);
                }

            }
            ViewData["LogList"] = LogList;


            return View();

        }

    }

}