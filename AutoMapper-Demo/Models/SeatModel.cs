using AutoMapperDemo.Entities;

namespace AutoMapperDemo.Models
{
    public sealed class SeatModel
    {
        public int Number { get; set; }

        public Car? Car { get; set; }
    }
}