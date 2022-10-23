namespace AIPWeb.Models
{
    public class APIResponseModel<DataType>
    {

        public ResponseModel<DataType> response { set; get; }

        public class ResponseModel<DataType>
        {
           
            public int returnCode { set; get; }

           
            public string message { set; get; }

          
            public DataType data { set; get; }

        }

    }


}
