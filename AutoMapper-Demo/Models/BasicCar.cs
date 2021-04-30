using System.Collections.Generic;

namespace AutoMapperDemo.Models
{
    public class BasicCar
    {
        public BasicCar()
        {
            Seats = new HashSet<SeatModel>();
        }

        public ICollection<SeatModel> Seats { get; set; }

        public int SeatsCount => Seats.Count;
    }
}
