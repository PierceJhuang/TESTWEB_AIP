using AIPWeb.Data;
using AIPWeb.Helpers;
using AIPWeb.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace AIPWeb.Controllers
{
    public class ClaimReportController : BaseController
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

                List<ClaimReportModel> ClaimReportList = new List<ClaimReportModel>();

                APIHelper aPIHelper = new APIHelper();

                List<Insurance> EntityList = aPIHelper.GetExportReport(model);

                if (EntityList != null && EntityList.Count > 0)
                {
                    int iNum = 1;
                    foreach (var entity in EntityList)
                    {

                        //呼叫API

                        ServiceHelper serviceHelper = new ServiceHelper();

                        Rootobject rootobject = serviceHelper.GetServiceData(GetCurrentUserAccount(), entity.No);

                        if (rootobject?.Data?.Count() > 0)
                        {
                            foreach (var data in rootobject.Data)
                            {
                                if (data.Progress == "受理中")
                                {
                                    //無資料，狀態為受理中
                                    ClaimReportModel ClaimReport = new ClaimReportModel
                                    {
                                        Num = iNum.ToString().PadLeft(2, '0'),
                                        No = entity.No,
                                        AccidentDate =entity.AccidentDate,
                                        AccidentProcess = entity.AccidentProcess,
                                        Percent = 25,
                                        Progress = "受理中",
                                        ClaimAmt = "",
                                        ClaimNo = data.ClaimNo ?? "",
                                        ClaimObject = "",
                                        PayDay = "",
                                        ClassName = "bg-primary"
                                    };

                                    ClaimReportList.Add(ClaimReport);
                                    iNum++;

                                }
                                else
                                {
                                    foreach (var detail in data.Detail)
                                    {
                                        int Percent = 0;
                                        string ClassName = "bg-primary";

                                        switch (data.Progress)
                                        {
                                            case "處理中":
                                                Percent = 50;
                                                ClassName = "bg-secondary";
                                                break;

                                            case "部分給付":
                                                Percent = 75;
                                                ClassName = "bg-warning";
                                                break;
                                            case "給付完成":
                                                Percent = 100;
                                                ClassName = "bg-danger";
                                                break;

                                        }

                                        string PayDay = detail.PayDay;

                                        if (detail.PayDay?.Length == 8)
                                        {
                                            PayDay = detail.PayDay.Substring(0, 4) + "-" + detail.PayDay.Substring(4, 2) + "-" + detail.PayDay.Substring(6, 2);
                                        }

                                        ClaimReportModel ClaimReport = new ClaimReportModel
                                        {
                                            Num = iNum.ToString().PadLeft(2, '0'),
                                            No = entity.No,
                                            AccidentDate = entity.AccidentDate,
                                            AccidentProcess = entity.AccidentProcess,
                                            Percent = Percent,
                                            Progress = data.Progress,
                                            ClaimAmt = detail.ClaimAmt.ToString("N0"),
                                            ClaimNo = data.ClaimNo,
                                            ClaimObject = detail.ClaimObject,
                                            PayDay = PayDay,
                                            ClassName = ClassName
                                        };


                                        ClaimReportList.Add(ClaimReport);
                                        iNum++;
                                    }

                                }



                            }

                        }

                    }
                }

                ViewData["ClaimReportList"] = ClaimReportList;

                #endregion

                return Json(new { Result = "Success", ClaimReportList = ClaimReportList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message + (ex.InnerException == null ? "" : "<br/>" + ex.InnerException.Message) }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult List(ExportReportModel model)
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
                    string Msg = String.Join("\n", ErrorMessage.ToArray());
                    Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                    Response.StatusDescription = Msg.Replace('\r', ' ').Replace('\n', ' '); 

                    return Json(new { Message = Msg }, JsonRequestBehavior.AllowGet);
                }

                #endregion

                #region 取值

                List<ClaimReportModel> ClaimReportList = new List<ClaimReportModel>();

                APIHelper aPIHelper = new APIHelper();

                List<Insurance> EntityList = aPIHelper.GetExportReport(model);

                if (EntityList != null && EntityList.Count > 0)
                {
                    int iNum = 1;
                    foreach (var entity in EntityList)
                    {

                        //呼叫API

                        ServiceHelper serviceHelper = new ServiceHelper();

                        Rootobject rootobject = serviceHelper.GetServiceData(GetCurrentUserAccount(), entity.No);

                        if (rootobject?.Data?.Count() > 0)
                        {
                            foreach (var data in rootobject.Data)
                            {
                                if (data.Progress == "受理中")
                                {
                                    //無資料，狀態為受理中
                                    ClaimReportModel ClaimReport = new ClaimReportModel
                                    {
                                        Num = iNum.ToString().PadLeft(2, '0'),
                                        No = entity.No,
                                        AccidentDate = entity.AccidentDate,
                                        AccidentProcess = entity.AccidentProcess,
                                        Percent = 25,
                                        Progress = "受理中",
                                        ClaimAmt = "",
                                        ClaimNo = data.ClaimNo ?? "",
                                        ClaimObject = "",
                                        PayDay = "",
                                        ClassName = "bg-primary"
                                    };

                                    ClaimReportList.Add(ClaimReport);
                                    iNum++;

                                }
                                else
                                {
                                    foreach (var detail in data.Detail)
                                    {
                                        int Percent = 0;
                                        string ClassName = "bg-primary";

                                        switch (data.Progress)
                                        {
                                            case "處理中":
                                                Percent = 50;
                                                ClassName = "bg-secondary";
                                                break;

                                            case "部分給付":
                                                Percent = 75;
                                                ClassName = "bg-warning";
                                                break;
                                            case "給付完成":
                                                Percent = 100;
                                                ClassName = "bg-danger";
                                                break;

                                        }

                                        string PayDay = detail.PayDay;

                                        if (detail.PayDay?.Length == 8)
                                        {
                                            PayDay = detail.PayDay.Substring(0, 4) + "-" + detail.PayDay.Substring(4, 2) + "-" + detail.PayDay.Substring(6, 2);
                                        }

                                        ClaimReportModel ClaimReport = new ClaimReportModel
                                        {
                                            Num = iNum.ToString().PadLeft(2, '0'),
                                            No = entity.No,
                                            AccidentDate = entity.AccidentDate,
                                            AccidentProcess = entity.AccidentProcess,
                                            Percent = Percent,
                                            Progress = data.Progress,
                                            ClaimAmt = detail.ClaimAmt.ToString("N0"),
                                            ClaimNo = data.ClaimNo,
                                            ClaimObject = detail.ClaimObject,
                                            PayDay = PayDay,
                                            ClassName = ClassName
                                        };


                                        ClaimReportList.Add(ClaimReport);
                                        iNum++;
                                    }

                                }



                            }

                        }

                    }
                }

            #endregion

            return PartialView("List", ClaimReportList);

               
           
        }
    }

   
}