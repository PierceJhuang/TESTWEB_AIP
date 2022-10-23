using AIPWeb.Data;
using System.Collections.Generic;

namespace AIPWeb.Models
{
    public class GetInsuranceReportModel : APIResponseModel<GetInsuranceReportModelParam>
    {
    }


    public class GetInsuranceReportModelParam
    {
        public List<Insurance> insuranceList;
    }

}
