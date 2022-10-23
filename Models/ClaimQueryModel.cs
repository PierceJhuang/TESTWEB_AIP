using System.Collections.Generic;
using System.Web;

namespace AIPWeb.Models
{
    public class ClaimQueryModel
    {
        public string No { get; set; }
        public string AccidentDate { get; set; }
        public string AccidentTerminal { get; set; }
        public string AccidentPlace { get; set; }
        public string AccidentProcess { get; set; }

        public string ClaimNo { get; set; }

        /// <summary>
        ///  0 : 未進件 
        ///  1 : 受理中
        ///  2 : 處理中
        ///  3 : 部分給付
        ///  4 : 給付完成
        /// </summary>
        public int Progress { get; set; }
        public List<Details> DetailList { get; set; }

    }

    public class Details
    {
        public string ClaimObject { get; set; }
        public string ClaimAmt { get; set; }
        public string PayDay { get; set; }
    }

}
