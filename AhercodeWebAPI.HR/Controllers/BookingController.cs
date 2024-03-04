using AhercodeWebAPI.HR.Dtos.Booking;
using AhercodeWebAPI.HR.Dtos.Currency;
using AhercodeWebAPI.HR.UserModels;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AhercodeWebAPI.HR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : Controller
    {
        private readonly FrankiesHotelContext _context;
        private readonly IMapper mapper;

        public static IWebHostEnvironment _environment;
        public BookingController(IWebHostEnvironment environment, FrankiesHotelContext context, IMapper mapper)
        {
            _context = context;
            this.mapper = mapper;
            _environment = environment;
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}

        [HttpPost]
        public async Task<ActionResult<Booking>> AddBooking(BookingCreateDto bookingDto)
        {
            // Check if the booking period overlaps with existing bookings
            var overlappingBooking = await _context.Bookings
                .Where(b =>
                    b.RoomId == bookingDto.RoomId &&
                        (b.IsCancelled != true) &&
                    //b.Id != bookingDto.Id && // Exclude the current booking if editing
                    (
                        (bookingDto.BookStart >= b.BookStart && bookingDto.BookStart < b.BookEnd) ||
                        (bookingDto.BookEnd > b.BookStart && bookingDto.BookEnd <= b.BookEnd) ||
                        (bookingDto.BookStart <= b.BookStart && bookingDto.BookEnd >= b.BookEnd)
                    )
                )
                .FirstOrDefaultAsync();

            if (overlappingBooking != null)
            {
                return BadRequest("Booking period overlaps with an existing booking.");
            }
            //bookingDto.CheckInTime = System.DateTime.Now;
            var guestData = await _context.Guests.Where(x => x.Id == bookingDto.GuestId).FirstOrDefaultAsync();
            bookingDto.CompanyId = guestData.CompanyId;
            bookingDto.CustomerReceiptNumber = $"{guestData.Firstname.Substring(0, 3)}{DateTime.Now.ToString("MMddHHmmss")}";
            var book = mapper.Map<Booking>(bookingDto);
            await _context.Bookings.AddAsync(book);
            await _context.SaveChangesAsync();

            var reservedRooms = _context.Rooms.Where(x => x.Id == bookingDto.RoomId).ToList();
            foreach (var room in reservedRooms)
            {
                // Check if billing data already exists  for this room
                bool billingExists = _context.Billings.Any(x => x.RoomId == room.Id && x.Currency == "USD" && x.CustomerId == bookingDto.GuestId && x.IsAccomodation == true);
                if (billingExists)
                {
                    // Billing data already exists for this room, skip it
                    continue;
                }
                //Console.WriteLine("This does not exists",booking.Id);

                //var roomType = _context.RoomTypes.Where(x => x.Id == room.TypeId).ToList();

                var daysInterval = (int)((TimeSpan)(bookingDto.BookEnd - bookingDto.BookStart)).TotalDays;
                // Accommodation
                Billing billing = new Billing();
                billing.Debit = bookingDto.Price * daysInterval;
                //billing.Debit = roomType[0].Price;
                billing.CustomerId = bookingDto.GuestId;
                billing.CompanyId = bookingDto.CompanyId;
                billing.RoomId = room.Id;
                billing.Description = "Accommodation for " + room.Name;
                billing.Currency = "USD";
                billing.Timestamp = DateTime.Now;
                billing.IsAccomodation = true;
                billing.RoomRate = bookingDto.Price;
                billing.CustomerBookingId = book.Id;

                BillingsHistory billingsHistory = new BillingsHistory();
                billingsHistory.Debit = bookingDto.Price * daysInterval;
                //billingsHistory.Debit = roomType[0].Price;
                billingsHistory.CustomerId = bookingDto.GuestId;
                billingsHistory.CompanyId = bookingDto.CompanyId;
                billingsHistory.RoomId = room.Id;
                billingsHistory.Description = "Accommodation for " + room.Name;
                billingsHistory.Currency = "USD";
                billingsHistory.Timestamp = DateTime.Now;
                billingsHistory.IsAccomodation = true;
                billingsHistory.RoomRate = bookingDto.Price;
                billingsHistory.CustomerBookingId = book.Id;

                _context.Billings.Add(billing);
                _context.BillingsHistories.Add(billingsHistory);

                //booking.IsNightAudited = true;
                //_context.Bookings.UpdateRange(booking);
                //_context.SaveChanges();


            }

            //booking.IsNightAudited = true;
            //_context.Bookings.UpdateRange(booking);
            //_context.SaveChanges();
            
            await _context.SaveChangesAsync();

            return Ok(book.Id);
        }

        //[HttpPost]
        //public async Task<ActionResult<Booking>> AddBooking(BookingCreateDto bookingDto)
        //{

        //    var book = mapper.Map<Booking>(bookingDto);
        //    await _context.Bookings.AddAsync(book);
        //    await _context.SaveChangesAsync();

        //    return Ok(book.Id);
        //}

        [HttpPut]
        [Route("CheckIn")]
        public async Task<ActionResult<Booking>> CheckIn(CheckInOutDto checkInDto)
        {
            var bookings = await _context.Bookings.Where(x=>x.Id==checkInDto.Id).ToListAsync();
            foreach(var data in bookings)
            {
                data.CheckInTime = checkInDto.CheckInOutTime;
            }
             _context.Bookings.UpdateRange(bookings);
            await _context.SaveChangesAsync();

            //return CreatedAtAction(nameof(Room), new { id = room.Id }, room);
            return Ok(checkInDto.Id);
        }

        //[HttpPut]
        //[Route("cancelBookingBeforeToday")]
        //public async Task<ActionResult<Booking>> cancelBookingBeforeToday(DateTime today)
        //{
        //    var bookings = await _context.Bookings.Where(x => x.Id == checkInDto.Id).ToListAsync();
        //    foreach (var data in bookings)
        //    {
        //        data.CheckInTime = checkInDto.CheckInOutTime;
        //    }
        //    _context.Bookings.UpdateRange(bookings);
        //    await _context.SaveChangesAsync();

        //    //return CreatedAtAction(nameof(Room), new { id = room.Id }, room);
        //    return Ok(checkInDto.Id);
        //}

        [HttpPut]
        [Route("CheckOut")]
        public async Task<ActionResult<Booking>> CheckOut(CheckInOutDto checkOutDto)
        {
            List<Billing> billingData;
            List<Booking> bookings;

            if(checkOutDto.IsCorporate)
            {
                billingData = await _context.Billings.Where(x => x.CompanyId == checkOutDto.Id).ToListAsync();
                _context.Billings.RemoveRange(billingData);

                bookings = await _context.Bookings.Where(x => x.CompanyId == checkOutDto.Id && (x.IsCancelled == false || x.IsCancelled == null) && x.CheckInTime != null && x.CheckOutTime == null).ToListAsync();

            }
            else
            {
                billingData = await _context.Billings.Where(x => x.CustomerId == checkOutDto.Id).ToListAsync();
                _context.Billings.RemoveRange(billingData);

                bookings = await _context.Bookings.Where(x => x.GuestId == checkOutDto.Id && (x.IsCancelled == false || x.IsCancelled == null) && x.CheckInTime != null && x.CheckOutTime == null).ToListAsync();

            }

            foreach (var data in bookings)
            {
                data.CheckOutTime = DateTime.Now;
            }
            _context.Bookings.UpdateRange(bookings);
            await _context.SaveChangesAsync();

            //return CreatedAtAction(nameof(Room), new { id = room.Id }, room);
            return Ok(checkOutDto.Id);
        }

        //[HttpGet('')]
        //public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        //{
        //    var response = await _context.Bookings.Where(x=>x.IsCancelled==null||x.IsCancelled==false).ToListAsync();
        //    return Ok(response);
        //}

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            // Get the current date
            DateTime currentDate = DateTime.Now.Date;

            // Find bookings with null CheckInTime and BookEnd date before today
            var bookingsToCancel = await _context.Bookings
                .Where(b => b.CheckInTime == null && b.BookStart < currentDate)
                .ToListAsync();

            if (bookingsToCancel.Count == 0)
            {
                Console.WriteLine("bookingsToCancel.Count: "+ bookingsToCancel.Count);
                return NotFound("No eligible bookings found to cancel.");
            }

            // Set IsCancelled to true for the found bookings
            foreach (var booking in bookingsToCancel)
            {
                booking.IsCancelled = true;
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            var response = await _context.Bookings.Where(x=>(x.IsCancelled==null||x.IsCancelled==false)&&(x.CheckOutTime==null)).ToListAsync();
            return Ok(response);
        }
        [HttpGet("CheckedOut")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetCheckedOutBookings()
        {
            var response = await _context.Bookings.Where(x => (x.IsCancelled == null || x.IsCancelled == false) && (x.CheckOutTime != null))
                .OrderByDescending(x => x.CheckOutTime)
                .ToListAsync();
            return Ok(response);
        }

        [HttpGet("GetFilteredBooking")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetFilteredBooking()
        {
            // Get the current date
            DateTime currentDate = DateTime.Now.Date;

            // Find bookings with null CheckInTime and BookEnd date before today
            var bookingsToCancel = await _context.Bookings
                .Where(b => b.CheckInTime == null && b.BookStart < currentDate)
                .ToListAsync();

            if (bookingsToCancel.Count == 0)
            {
                Console.WriteLine("bookingsToCancel.Count: "+ bookingsToCancel.Count);
                return NotFound("No eligible bookings found to cancel.");
            }

            // Set IsCancelled to true for the found bookings
            foreach (var booking in bookingsToCancel)
            {
                booking.IsCancelled = true;
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            var response = await _context.Bookings.Where(x=>(x.IsCancelled==null||x.IsCancelled==false)&&(x.CheckOutTime==null)).ToListAsync();
            return Ok(response);
        }



        [HttpGet("CheckOccupancy")]
        public async Task<ActionResult<Booking>> GetOccupancyInfo()
        {
            // Get the current date
            DateTime currentDate = DateTime.Now.Date;

            // Count the number of occupied rooms
            int occupiedRooms = await _context.Bookings
            .Where(b => (b.CheckInTime != null && (b.IsCancelled == null || b.IsCancelled == false)) && (b.BookStart <= currentDate && b.BookEnd >= currentDate || b.BookStart <= currentDate && b.BookEnd >= currentDate && b.CheckOutTime == null))
            .Select(b => b.RoomId)
            .Distinct()
            .CountAsync();

            // Count the number of vacant rooms
            int totalRooms = await _context.Rooms.CountAsync();
            int vacantRooms = totalRooms - occupiedRooms;

            var occupancyInfo = new OccupancyInfoDto
            {
                OccupiedRooms = occupiedRooms,
                VacantRooms = vacantRooms
            };

            return Ok(occupancyInfo);
        }


        [HttpGet("CheckRoomAvailability")]
        public async Task<ActionResult<IEnumerable<Booking>>> CheckRoomAvailability(int roomId,DateTime bookEnd)
        {
            try
            {
                DateTime bookStart = DateTime.Today;
                var overlappingBooking = await _context.Bookings
                .Where(b =>
                    b.RoomId == roomId && (b.IsCancelled==null||b.IsCancelled==false)&&
                    //b.Id != bookingDto.Id && // Exclude the current booking if editing
                    (
                        (bookStart >= b.BookStart && bookStart < b.BookEnd) ||
                        (bookEnd > b.BookStart && bookEnd <= b.BookEnd) ||
                        (bookStart <= b.BookStart && bookEnd >= b.BookEnd)
                    )
                )
                .FirstOrDefaultAsync();

                if (overlappingBooking != null)
                {
                    return BadRequest("Booking period overlaps with an existing booking.");
                }
                else
                {
                    return Ok(overlappingBooking);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPut("RoomTransfer")]
        public async Task<ActionResult<IEnumerable<Booking>>> RoomTransfer(BookingPutDto bookingPut)
        {
            var oldRoom = _context.Bookings.Where(x => x.Id == bookingPut.id).ToList();
            foreach (var data in oldRoom)
            {
                data.CheckOutTime = DateTime.Now;
                //var room = mapper.Map<Booking>(data);
                _context.Bookings.Update(data);

            }

            //bookingPut.checkInTime = DateTime.Now;
            Booking book = new Booking();
            book.BookStart = DateTime.Now;
            book.BookEnd = bookingPut.bookEnd;
            book.RoomId = bookingPut.roomId;
            book.CheckInTime = DateTime.Now;
            book.GuestId = bookingPut.customerId;
            book.Timestamp = DateTime.Now;
            //var tax = _context.TaxTables.Where(x => x.Id == taxPut.Id).ToList();

            //var mapTaxes = mapper.Map
            _context.Bookings.Add(book);
            await _context.SaveChangesAsync();

            return Ok(book.Id);
        }
        //[HttpGet("RoomAvailabity")]
        //public async Task<ActionResult<IEnumerable<Booking>>> GetRoomAvailability(int roomId)
        //{
        //    // Get the current date
        //    DateTime currentDate = DateTime.Now.Date;

        //    // Find bookings with null CheckInTime and BookEnd date before today
        //    var bookings = await _context.Bookings
        //        .Where(b => b.CheckInTime == null && b.BookStart < currentDate)
        //        .ToListAsync();
        //    return Ok(bookings);
        //}

        [HttpDelete("{id}")]
        public async Task<IActionResult> Booking(int id)
        {
            var bookings = await _context.Bookings.FindAsync(id);

            if (Booking == null)
            {
                return BadRequest("Could not delete booking");
            }
            bookings.IsCancelled = true;
            _context.Bookings.Update(bookings);
            await _context.SaveChangesAsync();
            return Ok();

        }


        [HttpPost]
        [Route("NewGuestBooking")]
        public async Task<ActionResult<Booking>> NewGuestBooking(NewGuestBookingCreateDto bookingDto)
        {
            // Check if the booking period overlaps with existing bookings
            var overlappingBooking = await _context.Bookings
                .Where(b =>
                    b.RoomId == bookingDto.RoomId &&
                    //b.Id != bookingDto.Id && // Exclude the current booking if editing
                    (
                        (bookingDto.BookStart >= b.BookStart && bookingDto.BookStart < b.BookEnd) ||
                        (bookingDto.BookEnd > b.BookStart && bookingDto.BookEnd <= b.BookEnd) ||
                        (bookingDto.BookStart <= b.BookStart && bookingDto.BookEnd >= b.BookEnd)
                    )
                )
                .FirstOrDefaultAsync();

            if (overlappingBooking != null)
            {
                return BadRequest("Booking period overlaps with an existing booking.");
            }

            // Create a new guest object and save it to the database
            var guest = mapper.Map<Guest>(bookingDto);
            await _context.Guests.AddAsync(guest);
            await _context.SaveChangesAsync();


            //bookingDto.CheckInTime = System.DateTime.Now;

            // Now that the guest has been saved, you can obtain the guest's ID
            // and use it to create the booking record
            var book = mapper.Map<Booking>(bookingDto);
            book.GuestId = guest.Id; // Set the GuestId property in the booking
            await _context.Bookings.AddAsync(book);
            await _context.SaveChangesAsync();

            //bookingDto.CheckInTime = System.DateTime.Now;
            //var guest = mapper.Map<Guest>(bookingDto);
            //var book = mapper.Map<Booking>(bookingDto);
            //await _context.Guests.AddAsync(guest);
            //await _context.Bookings.AddAsync(book);
            //await _context.SaveChangesAsync();

            return Ok(book.Id);
        }
    }

}
