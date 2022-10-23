using AIPWeb.Data;
using System.Collections.Generic;

namespace AIPWeb.Models
{
    public class GetHomeLogListModel : APIResponseModel<GetHomeLogListModelParam>
    {
    }


    public class GetHomeLogListModelParam
    {
        public List<Log> logList;
    }

}
