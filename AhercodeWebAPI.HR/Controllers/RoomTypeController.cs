//using AhercodeWebAPI.HR.Data;
using AhercodeWebAPI.HR.Dtos.Room;
using AhercodeWebAPI.HR.UserModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AhercodeWebAPI.HR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsTypeController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}
        private readonly FrankiesHotelContext _context;
        private readonly IMapper mapper;
        public RoomsTypeController(FrankiesHotelContext context, IMapper mapper)
        {
            _context = context;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomType>>> GetRoomTypes()
        {
            var response = await _context.RoomTypes.ToListAsync();
            return Ok(response);
        }
        // POST: api/AddRoom
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RoomType>> AddRoomType(RoomTypeCreateDto roomTypeDto)
        {

            var roomType = mapper.Map<RoomType>(roomTypeDto);
            await _context.RoomTypes.AddAsync(roomType);
            await _context.SaveChangesAsync();

            //return CreatedAtAction(nameof(RoomType), new { id = roomType.Id }, roomType);
            return Ok(roomType.Id);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> RoomType(int id)
        {
            var rooms = await _context.RoomTypes.FindAsync(id);
            
            var relatedRooms = _context.Rooms.Where(te => te.TypeId == id);
           
            if (relatedRooms != null)
            {
                foreach (var relatedRoom in relatedRooms)
                {
                    var relatedBookings = _context.Bookings.Where(te => te.RoomId == relatedRoom.Id);
                    if (relatedBookings != null)
                    {
                        _context.Bookings.RemoveRange(relatedBookings);
                    }
                }
                _context.Rooms.RemoveRange(relatedRooms);
            }
            
            if (rooms == null)
            {
                return BadRequest("Could not delete room Type");
            }

            _context.RoomTypes.Remove(rooms);
            await _context.SaveChangesAsync();
            return NoContent();

        }

    }
}
