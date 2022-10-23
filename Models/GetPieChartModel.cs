using AIPWeb.Data;
using System.Collections.Generic;

namespace AIPWeb.Models
{
    public class GetPieChartModel : APIResponseModel<GetPieChartModelParam>
    {
    }


    public class GetPieChartModelParam
    {     
        public List<PieChartModel> TerminalList { set; get; }

 
        public List<PieChartModel> PlaceList { set; get; }
    }

}
