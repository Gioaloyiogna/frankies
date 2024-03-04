using System;

namespace AhercodeWebAPI.HR.Dtos.PaymentMethod
{
    public class PaymentMethodCreateDto
    {
        public short Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
