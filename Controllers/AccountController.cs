using AIPWeb.Data;
using AIPWeb.Helpers;
using AIPWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AIPWeb.Controllers
{
    public class AccountController : BaseController
    {
        public ActionResult Index()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Login");
            }

            if (GetCurrentUserRole().ToLower() !="admin")
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["Header"] = GetHeader();

            List<AccountModel> AccountList = new List<AccountModel>();

            AIPEntities Entity = new AIPEntities();

            List<User> UserList = Entity.User.ToList();

            foreach (User user in UserList)
            {
                AccountModel account = new AccountModel();

                account.Id = user.Id;
                account.Account = user.Account;
                account.Role = user.Role;
                account.RoleName = user.Role.ToUpper() == "ADMIN" ? "系統管理員" : "一般使用者";
                account.Name = user.Name;
                account.Password = RSAEncrypt.Decrypt(user.Password);

                AccountList.Add(account);

            }

            ViewData["AccountList"] = AccountList;

            return View();

        

        }

        public JsonResult Update(AccountModel accountModel)
        {
            try
            {
                if (!IsUserLoggedIn())
                {
                    throw new Exception("系統已逾時，請重新登入");
                }

                if (accountModel.Type.ToUpper()=="A" && string.IsNullOrWhiteSpace(accountModel.Account))
                {
                    throw new Exception("請輸入帳號");
                }

                if (string.IsNullOrWhiteSpace(accountModel.Password))
                {
                    throw new Exception("請輸入密碼");
                }

                if (string.IsNullOrWhiteSpace(accountModel.Name))
                {
                    throw new Exception("請輸入名稱");
                }

                AIPEntities Entity = new AIPEntities();

                if (accountModel.Type == "U")
                {

                    User _User = Entity.User.Where(x => x.Id == accountModel.Id).FirstOrDefault();

                    _User.Password = RSAEncrypt.Encrypt(accountModel.Password);

                    _User.ModifiedTime = DateTime.Now;
                    _User.ModifiedUser = GetCurrentUserAccount();
                }
                else
                {
                    User _User = Entity.User.Where(x => x.Account.ToLower() == accountModel.Account.ToLower()).FirstOrDefault();

                    if (_User != null)
                    {
                        throw new Exception("帳號已經存在");
                    }
                    else
                    {
                        _User = new User();
                    }

                    _User.Account = accountModel.Account;
                    _User.Name = accountModel.Name;
                    _User.Role = "User";
                    _User.Password = RSAEncrypt.Encrypt(accountModel.Password);

                    _User.CreatedTime = DateTime.Now;
                    _User.CreatedUser = GetCurrentUserAccount();
                    Entity.User.Add(_User);
                }

                Entity.SaveChanges();

                return Json(new { Result = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message + (ex.InnerException == null ? "" : "<br/>" + ex.InnerException.Message) }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}