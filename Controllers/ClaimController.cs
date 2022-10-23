using AIPWeb.Data;
using AIPWeb.Helpers;
using AIPWeb.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AIPWeb.Controllers
{
    public class ClaimController : BaseController
    {
        #region Index

        public ActionResult Index()
        {

            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Login");
            }

            ViewData["Header"] = GetHeader();

            return View();
        }

        public JsonResult Add(ClaimModel claimModel)
        {
            try
            {

                if (!IsUserLoggedIn())
                {
                    throw new Exception("系統已逾時，請重新登入");
                }

                List<string> ErrorMessage = new List<string>();

                string Extensions = ConfigurationManager.AppSettings["ExtensionList"];
                string[] ExtensionList = Extensions.Split(';');

                if (claimModel.UploadFile != null && claimModel.UploadFile.ContentLength > 0)
                {
                    if (claimModel.UploadFile.ContentLength > 10 * 1024 * 1024)
                    {
                        ErrorMessage.Add("檔案超過10MB");
                    }
                    else if (!ExtensionList.Contains(System.IO.Path.GetExtension(claimModel.UploadFile.FileName).ToLower()))
                    {
                        ErrorMessage.Add("檔案格式不正確");
                    }
                }

                if (string.IsNullOrEmpty(claimModel.AccidentTime))
                {
                    ErrorMessage.Add("事故時間不可以為空");
                }
                if (string.IsNullOrEmpty(claimModel.AccidentDate))
                {
                    ErrorMessage.Add("事故日期不可以為空");
                }

                if (string.IsNullOrEmpty(claimModel.AccidentProcess))
                {
                    ErrorMessage.Add("事故經過不可以為空");
                }

                if (ErrorMessage.Count > 0)
                {

                    throw new Exception(String.Join("\n", ErrorMessage.ToArray()));
                }

                APIHelper aPIHelper = new APIHelper();
                string No = aPIHelper.GetNo();

                string newFilePath = "";
                string FileName = "";

                if (claimModel.UploadFile != null && claimModel.UploadFile.ContentLength > 0)
                {
                    FileName = claimModel.UploadFile.FileName;
                    string path = Server.MapPath("~/Upload/") + No;

                    //判断该目录是否存在,不存在及创建
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    newFilePath = Path.Combine(path, claimModel.UploadFile.FileName).Replace("\\", "/");

                    if (System.IO.File.Exists(newFilePath))
                    {
                        System.IO.File.Delete(newFilePath);
                    }

                    claimModel.UploadFile.SaveAs(newFilePath);

                }

                Insurance insurance = new Insurance
                {
                    No = No,
                    AccidentDate = claimModel.AccidentDate,
                    AccidentTime = claimModel.AccidentTime,
                    AccidentPlace = claimModel.AccidentPlace,
                    AccidentProcess = claimModel.AccidentProcess,
                    AccidentTerminal = claimModel.AccidentTerminal,
                    Remark = claimModel.Remark,
                    FileName = FileName,
                    FilePath = newFilePath,
                    CreatedTime = DateTime.Now,
                    CreatedUser = GetCurrentUserName()
                };

                aPIHelper.AddInsurance(insurance);

                List<string> ParamList = new List<string>();
                ParamList.Add(No);
                ParamList.Add(claimModel.AccidentDate);
                ParamList.Add(claimModel.AccidentTerminal);
                ParamList.Add(claimModel.AccidentPlace);
                ParamList.Add(claimModel.AccidentProcess);
                ParamList.Add(claimModel.Remark);

                MailHelper.SendMail("桃園國際機場意外理賠平台-新增案件:" + No, ParamList, newFilePath);

                return Json(new { Result = "Success", No = No }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message + (ex.InnerException == null ? "" : "<br/>" + ex.InnerException.Message) }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Update

        public ActionResult Update(string No)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Login");
            }

            ViewData["Header"] = GetHeader();

            APIHelper aPIHelper = new APIHelper();

            Insurance insurance = aPIHelper.GetInsurance(No);

            string IsUpdateRight = "N";

            if (insurance != null && insurance.CreatedTime.HasValue)
            {
                DateTime DueDate = insurance.CreatedTime.Value.AddDays(7);

                if (DateTime.Today < DueDate)
                {
                    IsUpdateRight = "Y";
                }
            }

            ClaimModel claimModel = new ClaimModel()
            {
                No = insurance?.No,
                AccidentDate = insurance?.AccidentDate,
                AccidentTime = insurance?.AccidentTime,
                AccidentPlace = insurance.AccidentPlace,
                AccidentProcess = insurance.AccidentProcess,
                AccidentTerminal = insurance.AccidentTerminal,
                Remark = insurance.Remark,
                IsUpdateRight = IsUpdateRight
            };

            ViewData["claimModel"] = claimModel;

            return View();
        }

        public JsonResult UpdateData(ClaimModel claimModel)
        {
            try
            {

                if (!IsUserLoggedIn())
                {
                    throw new Exception("系統已逾時，請重新登入");
                }

                string No = claimModel.No;

                APIHelper aPIHelper = new APIHelper();
                Insurance insurance = aPIHelper.GetInsurance(No);

                insurance.AccidentDate = claimModel.AccidentDate;
                insurance.AccidentTime = claimModel.AccidentTime;
                insurance.AccidentPlace = claimModel.AccidentPlace;
                insurance.AccidentProcess = claimModel.AccidentProcess;
                insurance.AccidentTerminal = claimModel.AccidentTerminal;
                insurance.Remark = claimModel.Remark;
                insurance.ModifiedTime = DateTime.Now;
                insurance.ModifiedUser = GetCurrentUserName();

                if (string.IsNullOrEmpty(claimModel.AccidentTime))
                {
                    throw new Exception("事故時間不可以為空");
                }

                if (string.IsNullOrEmpty(claimModel.AccidentDate))
                {
                    throw new Exception("事故日期不可以為空");
                }

                if (string.IsNullOrEmpty(claimModel.AccidentProcess))
                {
                    throw new Exception("事故經過不可以為空");
                }

                string newFilePath = insurance.FilePath;

                if (claimModel.UploadFile != null && claimModel.UploadFile.ContentLength > 0)
                {
                    string Extensions = ConfigurationManager.AppSettings["ExtensionList"];
                    string[] ExtensionList = Extensions.Split(';');

                    if (claimModel.UploadFile.ContentLength > 10 * 1024 * 1024)
                    {
                        throw new Exception("檔案超過10MB");
                    }
                    else if (!ExtensionList.Contains(System.IO.Path.GetExtension(claimModel.UploadFile.FileName).ToLower()))
                    {
                        throw new Exception("檔案格式不正確");
                    }

                    string path = Server.MapPath("~/Upload/") + No;

                    //判断该目录是否存在,不存在及创建
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    newFilePath = Path.Combine(path, claimModel.UploadFile.FileName).Replace("\\", "/");

                    if (System.IO.File.Exists(newFilePath))
                    {
                        System.IO.File.Delete(newFilePath);
                    }

                    claimModel.UploadFile.SaveAs(newFilePath);

                    insurance.FileName = claimModel.UploadFile.FileName;
                    insurance.FilePath = newFilePath;
                }


                aPIHelper.UpdateInsurance(insurance);

                List<string> ParamList = new List<string>();
                ParamList.Add(No);
                ParamList.Add(claimModel.AccidentDate);
                ParamList.Add(claimModel.AccidentTerminal);
                ParamList.Add(claimModel.AccidentPlace);
                ParamList.Add(claimModel.AccidentProcess);
                ParamList.Add(claimModel.Remark);

                MailHelper.SendMail("桃園國際機場意外理賠平台-更新案件:" + No, ParamList, newFilePath);


                return Json(new { Result = "Success", No = No }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message + (ex.InnerException == null ? "" : "<br/>" + ex.InnerException.Message) }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Result

        public ActionResult Result(string Mode, string No)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Login");
            }

            ViewData["Header"] = GetHeader();

            if (Mode.ToUpper() == "A")
            {
                ViewData["Message"] = "已新增完成.";
            }
            else
            {
                ViewData["Message"] = "已修改成功.";
            }

            ViewData["No"] = No;
            ViewData["Account"] = GetCurrentUserName();

            return View();
        }

        #endregion

    }
}