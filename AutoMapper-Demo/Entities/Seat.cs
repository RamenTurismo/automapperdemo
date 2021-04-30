namespace AutoMapperDemo.Entities
{
    public class Seat
    {
        public Seat()
        {
            CarPlate = string.Empty;
        }

        public int Number { get; set; }

        public string CarPlate { get; set; }

        public Car? Car { get; set; }
    }
}
