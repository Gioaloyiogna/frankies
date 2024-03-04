using System;

namespace AhercodeWebAPI.HR.Dtos.Tax
{
    public class TaxCreateDto
    {
        public string? name { get; set; }
        public decimal? rate { get; set; }
        public bool? isLevy { get; set; }
    }
}
