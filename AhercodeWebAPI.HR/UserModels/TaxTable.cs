﻿using System;
using System.Collections.Generic;

namespace AhercodeWebAPI.HR.UserModels
{
    public partial class TaxTable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? Rate { get; set; }
        public bool? IsLevy { get; set; }
    }
}
