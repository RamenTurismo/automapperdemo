using System.Collections.Generic;

namespace AutoMapperDemo.Entities
{
    public class Car
    {
        public Car()
        {
            Plate = string.Empty;
            Seats = new HashSet<Seat>();
        }

        public string Plate { get; set; }

        public ICollection<Seat> Seats { get; set; }
    }
}
