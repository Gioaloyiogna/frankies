using System;

namespace AhercodeWebAPI.HR.Dtos.Booking
{
    public class BookingPutDto
    {
        public int? id {  get; set; }
        public int? roomId { get; set; }
        public int? prv_roomId { get; set; }
        public int? customerId { get; set; }
        public DateTime? bookEnd { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}
