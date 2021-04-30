namespace AutoMapperDemo.Models
{
    public sealed class FrenchCarModel : BasicCar
    {
        public FrenchCarPlate Plate { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Plate} with {SeatsCount} seats";
        }
    }
}