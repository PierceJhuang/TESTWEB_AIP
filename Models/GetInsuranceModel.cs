using AIPWeb.Data;

namespace AIPWeb.Models
{
    public class GetInsuranceModel : APIResponseModel<GetInsuranceModelParam>
    {
    }


    public class GetInsuranceModelParam
    {
        public Insurance insurance;
    }

}
