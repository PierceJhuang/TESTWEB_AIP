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
    public class ClaimQueryController : BaseController
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

        public JsonResult Search(string No)
        {
            try
            {
                if (!IsUserLoggedIn())
                {
                    throw new Exception("系統已逾時，請重新登入");
                }

                if (string.IsNullOrWhiteSpace(No.Trim()))
                {
                    throw new Exception("搜尋文字不可以為空");
                }

                APIHelper aPIHelper = new APIHelper();
                Insurance _Insurance = aPIHelper.GetInsurance(No);

                //呼叫API
                ServiceHelper serviceHelper = new ServiceHelper();

                Rootobject rootobject = serviceHelper.GetServiceData(GetCurrentUserAccount(), _Insurance.No);

                string AccidentDate = "";

                if (!string.IsNullOrEmpty(_Insurance.AccidentDate))
                {
                    DateTime.TryParse(_Insurance.AccidentDate, out DateTime dt);

                    AccidentDate = dt.ToString("yyyy年MM月dd日");
                }


                int Progress = 0;
                string ClaimNo = "";
                List<Details> DetailList = new List<Details>();

                if (rootobject?.Data?.Count() > 0)
                {
                   
                    switch (rootobject.Data[0].Progress)
                    {
                        case "受理中":
                            Progress = 1;
                            ClaimNo = rootobject.Data[0].ClaimNo ?? "";
                            break;
                        case "處理中":
                            Progress = 2;
                            ClaimNo = rootobject.Data[0].ClaimNo;
                            break;

                        case "部分給付":
                            Progress = 3;
                            ClaimNo = rootobject.Data[0].ClaimNo;
                            break;
                        case "給付完成":
                            Progress = 4;
                            ClaimNo = rootobject.Data[0].ClaimNo;
                            break;

                    }

                    if (Progress == 1)
                    {
                        Details Detail = new Details()
                        {
                            ClaimAmt = "",
                            ClaimObject = "",
                            PayDay = ""
                        };

                        DetailList.Add(Detail);
                    }
                    else
                    {
                        foreach (var d in rootobject.Data[0].Detail)
                        {
                            string PayDay = d.PayDay;

                            if (d.PayDay?.Length == 8)
                            {
                                PayDay = d.PayDay.Substring(0, 4) + "-" + d.PayDay.Substring(4, 2) + "-" + d.PayDay.Substring(6, 2);
                            }

                            Details Detail = new Details()
                            {
                                ClaimAmt = d.ClaimAmt.ToString("N0"),
                                ClaimObject = d.ClaimObject,
                                PayDay = PayDay
                            };

                            DetailList.Add(Detail);

                        }
                    }


                }

                ClaimQueryModel claimQueryModel = new ClaimQueryModel
                {
                    No = _Insurance.No,
                    AccidentDate = AccidentDate,
                    AccidentPlace = _Insurance.AccidentPlace,
                    AccidentProcess = _Insurance.AccidentProcess,
                    AccidentTerminal = _Insurance.AccidentTerminal,
                    Progress = Progress,
                    ClaimNo = ClaimNo,
                    DetailList = DetailList


                };

                return Json(new { Result = "Success", Model = claimQueryModel }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message + (ex.InnerException == null ? "" : "<br/>" + ex.InnerException.Message) }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}