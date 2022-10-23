using AIPWeb.Data;
using AIPWeb.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

namespace AIPWeb.Helpers
{

    public class APIHelper
    {

        public string TIAClmServiceUrl = ConfigurationManager.AppSettings["TIAClmServiceUrl"];

        public APIHelper()
        {
        }

        public string GetNo()
        {
            string No = "";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(TIAClmServiceUrl);

                HttpRequestMessage request = new System.Net.Http.HttpRequestMessage(HttpMethod.Post, "API/GetNo");

                string Json = "";
                request.Content = new StringContent(Json, Encoding.UTF8, "application/json");

                var response = client.SendAsync(request).Result;

                //判斷是否連線成功
                if (response.IsSuccessStatusCode)
                {
                    //取回傳值
                    var APIResult = response.Content.ReadAsStringAsync();

                    GetNoModel model = new JavaScriptSerializer().Deserialize<GetNoModel>(APIResult.Result);

                    if (model?.response?.returnCode == 1)
                    {
                        No = model?.response.data?.No;
                    }
                    else
                    {
                        throw new Exception(model?.response?.message);
                    }

                }

            }

            return No;
        }

        public void AddInsurance(Insurance insurance)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(TIAClmServiceUrl);

                HttpRequestMessage request = new System.Net.Http.HttpRequestMessage(HttpMethod.Post, "API/AddInsurance");

                string Json = new JavaScriptSerializer().Serialize(insurance);
                request.Content = new StringContent(Json, Encoding.UTF8, "application/json");

                var response = client.SendAsync(request).Result;

                //判斷是否連線成功
                if (response.IsSuccessStatusCode)
                {
                    //取回傳值
                    var APIResult = response.Content.ReadAsStringAsync();

                    ResponseModel model = new JavaScriptSerializer().Deserialize<ResponseModel>(APIResult.Result);

                    if (model?.response?.returnCode != 1)
                    {
                        throw new Exception(model?.response?.message);
                    }

                }

            }

        }

        public void UpdateInsurance(Insurance insurance)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(TIAClmServiceUrl);

                HttpRequestMessage request = new System.Net.Http.HttpRequestMessage(HttpMethod.Post, "API/UpdateInsurance");

                string Json = new JavaScriptSerializer().Serialize(insurance);
                request.Content = new StringContent(Json, Encoding.UTF8, "application/json");

                var response = client.SendAsync(request).Result;

                //判斷是否連線成功
                if (response.IsSuccessStatusCode)
                {
                    //取回傳值
                    var APIResult = response.Content.ReadAsStringAsync();

                    ResponseModel model = new JavaScriptSerializer().Deserialize<ResponseModel>(APIResult.Result);

                    if (model?.response?.returnCode != 1)
                    {
                        throw new Exception(model?.response?.message);
                    }

                }

            }

        }

        public Insurance GetInsurance(string No)
        {
            Insurance insurance = new Insurance();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(TIAClmServiceUrl);

                string strNo = string.IsNullOrWhiteSpace(No) ? "" : No;
                HttpRequestMessage request = new System.Net.Http.HttpRequestMessage(HttpMethod.Post, "api/GetInsurance?No="+strNo);

                request.Content = new StringContent("", Encoding.UTF8, "application/json");

                var response = client.SendAsync(request).Result;

                //判斷是否連線成功
                if (response.IsSuccessStatusCode)
                {
                    //取回傳值
                    var APIResult = response.Content.ReadAsStringAsync();

                    GetInsuranceModel model = new JavaScriptSerializer().Deserialize<GetInsuranceModel>(APIResult.Result);

                    if (model?.response?.returnCode == 1)
                    {
                        insurance = model?.response.data?.insurance;
                    }
                    else
                    {
                        throw new Exception(model?.response?.message);
                    }

                }

            }

            return insurance;
        }

        public List<Insurance> GetInsuranceReport(string No)
        {
            List<Insurance> insuranceList = new List<Insurance>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(TIAClmServiceUrl);


                string strNo = string.IsNullOrWhiteSpace(No) ? "" : No;
                HttpRequestMessage request = new System.Net.Http.HttpRequestMessage(HttpMethod.Post, "api/GetInsuranceReport?No="+ strNo);

                request.Content = new StringContent("", Encoding.UTF8, "application/json");

                var response = client.SendAsync(request).Result;

                //判斷是否連線成功
                if (response.IsSuccessStatusCode)
                {
                    //取回傳值
                    var APIResult = response.Content.ReadAsStringAsync();

                    GetInsuranceReportModel model = new JavaScriptSerializer().Deserialize<GetInsuranceReportModel>(APIResult.Result);

                    if (model?.response?.returnCode == 1)
                    {
                        insuranceList = model?.response.data?.insuranceList;
                    }
                    else
                    {
                        throw new Exception(model?.response?.message);
                    }

                }

            }

            return insuranceList;
        }


        public List<Insurance> GetExportReport(ExportReportModel model)
        {
            List<Insurance> insuranceList = new List<Insurance>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(TIAClmServiceUrl);

                HttpRequestMessage request = new System.Net.Http.HttpRequestMessage(HttpMethod.Post, "api/GetExportReport");

                string Json = new JavaScriptSerializer().Serialize(model);
                request.Content = new StringContent(Json, Encoding.UTF8, "application/json");

                var response = client.SendAsync(request).Result;

                //判斷是否連線成功
                if (response.IsSuccessStatusCode)
                {
                    //取回傳值
                    var APIResult = response.Content.ReadAsStringAsync();

                    GetInsuranceReportModel output = new JavaScriptSerializer().Deserialize<GetInsuranceReportModel>(APIResult.Result);

                    if (output?.response?.returnCode == 1)
                    {
                        insuranceList = output?.response.data?.insuranceList;
                    }
                    else
                    {
                        throw new Exception(output?.response?.message);
                    }

                }

            }

            return insuranceList;
        }

        public List<Log> GetHomeLogList()
        {
            List<Log> LogList = new List<Log>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(TIAClmServiceUrl);

                HttpRequestMessage request = new System.Net.Http.HttpRequestMessage(HttpMethod.Post, "API/GetHomeLogList");

                string Json = "";
                request.Content = new StringContent(Json, Encoding.UTF8, "application/json");

                var response = client.SendAsync(request).Result;

                //判斷是否連線成功
                if (response.IsSuccessStatusCode)
                {
                    //取回傳值
                    var APIResult = response.Content.ReadAsStringAsync();

                    GetHomeLogListModel model = new JavaScriptSerializer().Deserialize<GetHomeLogListModel>(APIResult.Result);

                    if (model?.response?.returnCode == 1)
                    {
                        LogList = model?.response.data?.logList;
                    }
                    else
                    {
                        throw new Exception(model?.response?.message);
                    }

                }

            }

            return LogList;
        }

        public List<Log> GetLogList()
        {
            List<Log> LogList = new List<Log>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(TIAClmServiceUrl);

                HttpRequestMessage request = new System.Net.Http.HttpRequestMessage(HttpMethod.Post, "API/GetLogList");

                string Json = "";
                request.Content = new StringContent(Json, Encoding.UTF8, "application/json");

                var response = client.SendAsync(request).Result;

                //判斷是否連線成功
                if (response.IsSuccessStatusCode)
                {
                    //取回傳值
                    var APIResult = response.Content.ReadAsStringAsync();

                    GetHomeLogListModel model = new JavaScriptSerializer().Deserialize<GetHomeLogListModel>(APIResult.Result);

                    if (model?.response?.returnCode == 1)
                    {
                        LogList = model?.response.data?.logList;
                    }
                    else
                    {
                        throw new Exception(model?.response?.message);
                    }

                }

            }

            return LogList;
        }

        public GetPieChartModelParam GetPieChart(ExportReportModel condition)
        {
            GetPieChartModelParam modelParam = new GetPieChartModelParam();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(TIAClmServiceUrl);

                HttpRequestMessage request = new System.Net.Http.HttpRequestMessage(HttpMethod.Post, "API/GetPieChart");

                string Json = new JavaScriptSerializer().Serialize(condition);

                request.Content = new StringContent(Json, Encoding.UTF8, "application/json");

                var response = client.SendAsync(request).Result;

                //判斷是否連線成功
                if (response.IsSuccessStatusCode)
                {
                    //取回傳值
                    var APIResult = response.Content.ReadAsStringAsync();

                    GetPieChartModel model = new JavaScriptSerializer().Deserialize<GetPieChartModel>(APIResult.Result);

                    if (model?.response?.returnCode == 1)
                    {
                        modelParam = model?.response.data;
                    }
                    else
                    {
                        throw new Exception(model?.response?.message);
                    }

                }

            }

            return modelParam;
        }


        public class NoModel
        {
            public string No { get; set; }
        }

    }
}
