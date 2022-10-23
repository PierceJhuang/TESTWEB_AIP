using AIPWeb.Data;
using AIPWeb.Helpers;
using AIPWeb.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using OfficeOpenXml;


namespace AIPWeb.Controllers
{
    public class ExportReportController : BaseController
    {
        public ActionResult Index(string No)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Login");
            }

            ViewData["Header"] = GetHeader();

         
            return View();

        }

        public JsonResult Export(ExportReportModel model)
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
                else if(!DateTime.TryParse(model.StartDate,out DateTime dtSDate))
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

                #region 產Excel

                DateTime dExportDate = DateTime.Now;
                string strFileName = $"Report_{dExportDate.ToString("yyyyMMddHHmmss")}.xlsx";
                string sFolderPath = "download";
                string sActualFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, sFolderPath);
                if (!Directory.Exists(sActualFolderPath)) Directory.CreateDirectory(sActualFolderPath);
                //相對路徑
                string sFilePath = Path.Combine(sFolderPath, strFileName);
                //實體路徑
                string strActualPath = Path.Combine(sActualFolderPath, strFileName);

                FileInfo fileInfo = new FileInfo(strActualPath);
                ExcelPackage ep = new ExcelPackage(fileInfo);

                ExcelWorksheet ws = ep.Workbook.Worksheets.Add("Report");

                ws.Column(1).Width = 5;
                ws.Column(2).Width = 12;
                ws.Column(3).Width = 13;

                ws.Cells[1, 1].Value = "項次";
                ws.Cells[1, 2].Value = "通報序號";
                ws.Cells[1, 3].Value = "理賠案號";
                ws.Cells[1, 4].Value = "事故日期";
                ws.Cells[1, 5].Value = "事故經過";
                ws.Cells[1, 6].Value = "處理進度";
                ws.Cells[1, 7].Value = "理賠金額";
                ws.Cells[1, 8].Value = "給付日期";
                ws.Cells[1, 9].Value = "給付對象";

                int iRow = 2;
                foreach (var ClaimReport in ClaimReportList)
                {
                    ws.Cells[iRow, 1].Value = ClaimReport.Num;
                    ws.Cells[iRow, 2].Value = ClaimReport.No;
                    ws.Cells[iRow, 3].Value = ClaimReport.ClaimNo;
                    ws.Cells[iRow, 4].Value = ClaimReport.AccidentDate;
                    ws.Cells[iRow, 5].Value = ClaimReport.AccidentProcess;
                    ws.Cells[iRow, 6].Value = ClaimReport.Progress;
                    ws.Cells[iRow, 7].Value = ClaimReport.ClaimAmt;
                    ws.Cells[iRow, 8].Value = ClaimReport.PayDay;
                    ws.Cells[iRow, 9].Value = ClaimReport.ClaimObject;

                    iRow++;
                }

                ep.Save();



                #endregion

                return Json(new { Result = "Success", FilePath = sFilePath }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message + (ex.InnerException == null ? "" : "<br/>" + ex.InnerException.Message) }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}