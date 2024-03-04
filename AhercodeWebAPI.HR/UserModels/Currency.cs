using System;
using System.Collections.Generic;

namespace AhercodeWebAPI.HR.UserModels
{
    public partial class Currency
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? Rate { get; set; }
        public bool? IsBase { get; set; }
        public string Symbol { get; set; }
    }
}
