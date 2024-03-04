using System;
using System.Collections.Generic;

namespace AhercodeWebAPI.HR.UserModels
{
    public partial class BankDetail
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public long? AccountNumber { get; set; }
    }
}
