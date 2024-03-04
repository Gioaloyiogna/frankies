using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AhercodeWebAPI.HR.Dtos.Room
{
    public class RoomDto
    {
        public int id { get; set; }
        public string Name { get; set; }
        public int TypeId { get; set; }
        public bool IsActive { get; set; }

    }
}
