using AIPWeb.Data;
using AIPWeb.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using System.IO;
using System.Net.Mail;
using System.Net;

namespace AIPWeb.Helpers
{

    public class MailHelper
    {

        public static void SendMail(string strSubject, List<string> strParam , string AttachmentPath)
        {
            string IsSend = ConfigurationManager.AppSettings["IsSend"];

            if (IsSend != "0")
            {
                //設定smtp主機
                string smtpAddress = ConfigurationManager.AppSettings["SmtpHost"];
                //設定Port
                int portNumber = 25;
                bool enableSSL = true;
                //填入寄送方帳號和密碼
                string emailFrom = ConfigurationManager.AppSettings["SmtpAccount"];
                string password = ConfigurationManager.AppSettings["SmtpPassword"];
                //收信方email
                string emailTo = ConfigurationManager.AppSettings["ToMan"];

                //內容
                string body = GetMailHtml(strParam[0], strParam[1], strParam[2], strParam[3], strParam[4],strParam[5]);

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("automail@tfmi.com.tw");

                    string[] ToManList = emailTo.Split(';');

                    foreach (string ToMan in ToManList)
                    {
                        mail.To.Add(ToMan);
                    }

                    mail.Subject = strSubject;
                    mail.Body = body;
                    // 若你的內容是HTML格式，則為True
                    mail.IsBodyHtml = true;

                    //夾帶檔案
                    if (!string.IsNullOrEmpty(AttachmentPath))
                    {
                        mail.Attachments.Add(new Attachment(AttachmentPath));
                    }

                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                    {
                        smtp.Credentials = new NetworkCredential(emailFrom, password);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mail);
                    }
                }
            }
        }


       private static string GetMailHtml(string No, string AccidentDate, string AccidentTerminal, string AccidentPlace, string AccidentProcess,string Remark)
        {
            string Result = "";

            string WebUrl = ConfigurationManager.AppSettings["WebUrl"];

            string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Views", "Claim", "Email.html");

            using (StreamReader streamReader = new StreamReader(FilePath, Encoding.GetEncoding("UTF-8")))
            {
                Result = streamReader.ReadToEnd();
            }

            Result = Result.Replace("@WebUrl", WebUrl)
                           .Replace("@No", No)
                           .Replace("@AccidentDate", AccidentDate)
                           .Replace("@AccidentTerminal", AccidentTerminal)
                           .Replace("@AccidentPlace", AccidentPlace)
                           .Replace("@AccidentProcess", AccidentProcess)
                           .Replace("@Remark", Remark);


            return Result;
        }





    }
}
