//using AhercodeWebAPI.HR.Data;
using AhercodeWebAPI.HR.Dtos.Notes;
using AhercodeWebAPI.HR.Dtos.Room;
using AhercodeWebAPI.HR.UserModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AhercodeWebAPI.HR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}
        private readonly FrankiesHotelContext _context;
        private readonly IMapper mapper;
        public RoomsController(FrankiesHotelContext context, IMapper mapper)
        {
            _context = context;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetRooms() {
            var response = await _context.Rooms.ToListAsync();
            return Ok(response);
        }
        // POST: api/AddRoom
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Room>> AddRoom(RoomDto roomDto)
        {

            var room = mapper.Map<Room>(roomDto);
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();

            //return CreatedAtAction(nameof(Room), new { id = room.Id }, room);
            return Ok(room.Id);
        }
        // DELETE: api/DeleteRoom
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpDelete]
        public async Task<ActionResult<Room>> DeleteRoom(int id)
        {

            //var notes = await _context.Notes.FindAsync(id);

            //if (notes == null)
            //{
            //    return BadRequest("Could not find note");
            //}

            //_context.Notes.Remove(notes);
            //await _context.SaveChangesAsync();
            //return NoContent();

            var room = await _context.Rooms.FindAsync(id);
             _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            //return CreatedAtAction(nameof(Room), new { id = room.Id }, room);
            return Ok();
        }

    }
}
