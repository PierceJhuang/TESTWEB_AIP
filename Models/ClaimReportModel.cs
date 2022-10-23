using System.Web;

namespace AIPWeb.Models
{
    public class ClaimReportModel
    {
        public string Num { get; set; }
        public string No { get; set; }
        public string ClaimNo { get; set; }

        public string AccidentDate { get; set; }

        public string AccidentProcess { get; set; }
      
        /// <summary>
        ///  0 : 未進件 
        ///  25 : 受理中
        ///  50 : 處理中
        ///  75 : 部分給付
        ///  100 : 給付完成
        /// </summary>
        public int Percent { get; set; }

        public string Progress { get; set; }

        public string ClaimObject { get; set; }
        public string ClaimAmt { get; set; }
        public string PayDay { get; set; }

        public string ClassName { get; set; }

    }

}
