namespace AhercodeWebAPI.HR.Dtos.Company
{
    public class CompanyPutDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public string TinNumber { get; set; }
        public bool? NonTaxable { get; set; }
        public decimal? FixRate { get; set; }
    }
}
