using AutoMapper;
using AutoMapperDemo.Entities;
using AutoMapperDemo.Models;

namespace AutoMapperDemo.Profiles
{
    public class CarProfile : Profile
    {
        public CarProfile()
        {
            CreateMap<Car, BasicCar>()
                .Include<Car, FrenchCarModel>();

            CreateMap<Car, FrenchCarModel>()
                .ForMember(x => x.Plate, x => x.MapFrom(e => new FrenchCarPlate(e.Plate)));

            CreateMap<Seat, SeatModel>();
        }
    }
}
