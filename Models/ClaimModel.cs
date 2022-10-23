using System.Web;

namespace AIPWeb.Models
{
    public class ClaimModel
    {
        public string No { get; set; }
        public string AccidentDate { get; set; }
        public string AccidentTime { get; set; }
        public string AccidentTerminal { get; set; }
        public string AccidentPlace { get; set; }
        public string AccidentProcess { get; set; }
        public string Remark { get; set; }
        public HttpPostedFileBase UploadFile { get; set; }

        public string IsUpdateRight { get; set; }

    }

}
