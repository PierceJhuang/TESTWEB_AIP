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
    public class PieChartController : BaseController
    {
        public ActionResult Index()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Login");
            }

            ViewData["Header"] = GetHeader();

            return View();

        }

        public JsonResult Query(ExportReportModel model)
        {
            try
            {
                #region 檢查

                if (!IsUserLoggedIn())
                {
                    throw new Exception("系統已逾時，請重新登入");
                }

                List<string> ErrorMessage = new List<string>();

                if (string.IsNullOrEmpty(model.StartDate))
                {
                    ErrorMessage.Add("開始日期不可以為空");
                }
                else if (!DateTime.TryParse(model.StartDate, out DateTime dtSDate))
                {
                    ErrorMessage.Add("開始日期格式不正確");
                }


                if (string.IsNullOrEmpty(model.EndDate))
                {
                    ErrorMessage.Add("結束日期不可以為空");
                }
                else if (!DateTime.TryParse(model.EndDate, out DateTime dtEDate))
                {
                    ErrorMessage.Add("結束日期格式不正確");
                }

                if (ErrorMessage.Count > 0)
                {
                    throw new Exception(String.Join("\n", ErrorMessage.ToArray()));
                }

                #endregion

                #region 取值

                APIHelper aPIHelper = new APIHelper();

                GetPieChartModelParam modelParam = aPIHelper.GetPieChart(model);

                #endregion

                return Json(new { Result = "Success", TerminalList= modelParam.TerminalList , PlaceList = modelParam.PlaceList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message + (ex.InnerException == null ? "" : "<br/>" + ex.InnerException.Message) }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}