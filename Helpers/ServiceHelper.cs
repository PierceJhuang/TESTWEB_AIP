using AIPWeb.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

namespace AIPWeb.Helpers
{

    public class ServiceHelper
    {

        public string ServiceUrl = ConfigurationManager.AppSettings["ServiceUrl"];
        public string AccountServiceUrl = ConfigurationManager.AppSettings["AccountServiceUrl"];
        public string IsDev = ConfigurationManager.AppSettings["IsDev"];

        CLMService.Default service = new CLMService.Default();

        public ServiceHelper()
        {
            if (IsDev != "1")
            {
                service.Url = ServiceUrl;
            }
        }

        public Rootobject GetServiceData(string Account,string No)
        {

            if (IsDev == "1")
            {

                string Data = "{\"Status\":1,\"Message\":\"\",\"PublicKey\":\"\",\"Data\":[{\"Progress\":\"給付完成\",\"ClaimNo\":\"63BE09000011\",\"Detail\":[{\"ClaimObject\":\"陳淳榆 \",\"ClaimAmt\":5600.00,\"PayDay\":\"20200611\"},{\"ClaimObject\":\"桃苗汽車股份有限公司 \",\"ClaimAmt\":86529.00,\"PayDay\":\"20200617\"},{\"ClaimObject\":\"邱文正 \",\"ClaimAmt\":4160.00,\"PayDay\":\"20200617\"},{\"ClaimObject\":\"台昌有限公司 \",\"ClaimAmt\":27855.00,\"PayDay\":\"20200629\"},{\"ClaimObject\":\"桃鈴汽車股份有限公司 \",\"ClaimAmt\":28235.00,\"PayDay\":\"20200703\"},{\"ClaimObject\":\"王元昊 \",\"ClaimAmt\":24904.00,\"PayDay\":\"20200709\"},{\"ClaimObject\":\"鋐騏汽車材料商行劉念婷 \",\"ClaimAmt\":11865.00,\"PayDay\":\"20200717\"},{\"ClaimObject\":\"張地森 \",\"ClaimAmt\":2400.00,\"PayDay\":\"20200717\"}]}]}";
                                
                string NoData = "{\"Status\":1,\"Message\":\"\",\"PublicKey\":\"\",\"Data\":[{\"Progress\":\"受理中\",\"ClaimNo\":\"63BE11000006\",\"Detail\":[]}]}";

                Rootobject rootobject_data = new JavaScriptSerializer().Deserialize<Rootobject>(Data);

                Rootobject rootobject_NoData = new JavaScriptSerializer().Deserialize<Rootobject>(NoData);

                int i = new Random().Next();

                if (i % 2 == 0)
                {
                    return rootobject_data;
                }
                else
                {
                    return rootobject_NoData;
                }
                
            }
            else
            {
                CLMService.Default service = new CLMService.Default();
                service.Url = ServiceUrl;

                string Json = service.ClaimProgress(Account, No);

                Rootobject rootobject = new JavaScriptSerializer().Deserialize<Rootobject>(Json);

                return rootobject;

            }
        }

        public SessionUserModel CheckAccount(string Account, string Password)
        {
            SessionUserModel UserInfo = new SessionUserModel();

            if (IsDev == "1")
            {
                              // { "Status":1,"Message":"I00驗證成功","PublicKey":"","Data":[{ "UnitCode1":"53002013","UnitCode2":"Company","UnitCode3":"01","UnitCode4":"A123456789","Name":"王小明","Roles":"Y","Id":""}]}
                string Data = "{\"Status\":1,\"Message\":\"I00驗證成功\",\"PublicKey\":\"\",\"Data\":[{\"UnitCode1\":\"86517510\",\"UnitCode2\":\"01\",\"UnitCode3\":\"01\",\"UnitCode4\":\"1234567\",\"Name\":\"張ＯＯ\",\"Roles\":\"A\",\"Id\":\"\"}]}";
                //string NoData = "{\"Status\":0,\"Message\":\"E01驗證失敗，帳號或密碼錯誤\",\"PublicKey\":\"\",\"Data\":[]}";

                AccountVerificationModel _Model = new JavaScriptSerializer().Deserialize<AccountVerificationModel>(Data);

                if (_Model != null && _Model.Status == 1)
                {
                    if (_Model.Data.Length > 0)
                    {
                        UserInfo.Account = _Model.Data[0].Id;
                        UserInfo.Name = _Model.Data[0].Name;
                        UserInfo.Role = _Model.Data[0].Roles;
                        UserInfo.RoleName = GetRoleName(_Model.Data[0].Roles);
                    }
                }
                else
                {
                    throw new Exception(_Model?.Message);
                }
            }
            else
            {
                SignatureService.Signature service = new SignatureService.Signature();
                service.Url = AccountServiceUrl;

                string Json = service.AccountVerification(Account, Password);

                AccountVerificationModel _Model = new JavaScriptSerializer().Deserialize<AccountVerificationModel>(Json);

                if (_Model != null && _Model.Status == 1)
                {
                    UserInfo.Account = Account;

                    if (_Model.Data.Length > 0)
                    {
                        UserInfo.Name = _Model.Data[0].Name;
                        UserInfo.Role = _Model.Data[0].Roles;
                        UserInfo.RoleName = GetRoleName(_Model.Data[0].Roles);
                    }
                }
                else
                {
                    throw new Exception(_Model?.Message);
                }
            }

            return UserInfo;
        }

        private string GetRoleName(string Role)
        {
            string RoleName = Role;

            switch (Role.ToUpper())
            {
                case "A":
                    RoleName = "初審人員";
                    break;
                case "B":
                    RoleName = "複審人員";
                    break;
                case "C":
                    RoleName = "簽署人員";
                    break;
                case "D":
                    RoleName = "簽署平台維護員";
                    break;
                case "E":
                    RoleName = "消金上傳人員";
                    break;
                case "Y":
                    RoleName = "業務員";
                    break;
                case "Z":
                    RoleName = "系統管理者";
                    break;
            }
            return RoleName;
        
        }

    }
}
