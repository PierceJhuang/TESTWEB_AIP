namespace AIPWeb.Models
{
    public class AccountVerificationModel
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public string PublicKey { get; set; }
        public AccountDatum[] Data { get; set; }
    }

    public class AccountDatum
    {
        public string UnitCode1 { get; set; }
        public string UnitCode2 { get; set; }
        public string UnitCode3 { get; set; }
        public string UnitCode4 { get; set; }
        public string Name { get; set; }
        public string Roles { get; set; }
        public string Id { get; set; }
    }


}
