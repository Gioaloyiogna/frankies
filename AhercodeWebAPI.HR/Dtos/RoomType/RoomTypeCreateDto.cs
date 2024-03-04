using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AhercodeWebAPI.HR.Dtos.Room
{
    public class RoomTypeCreateDto
    {

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        //public bool IsActive { get; set; }

    }
}
