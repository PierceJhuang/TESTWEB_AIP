namespace AIPWeb.Models
{
    public class Rootobject
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public string PublicKey { get; set; }
        public Datum[] Data { get; set; }
    }

    public class Datum
    {
        public string Progress { get; set; }
        public string ClaimNo { get; set; }
        public Detail[] Detail { get; set; }
    }

    public class Detail
    {
        public string ClaimObject { get; set; }
        public float ClaimAmt { get; set; }
        public string PayDay { get; set; }
    }

}



