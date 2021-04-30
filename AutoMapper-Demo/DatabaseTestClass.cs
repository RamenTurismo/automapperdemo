using AutoMapper;
using AutoMapper.QueryableExtensions;
using AutoMapperDemo.Entities;
using AutoMapperDemo.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoMapperDemo
{
    public class DatabaseTestClass
    {
        private readonly IMapper _mapper;
        private readonly DemoDbContext _dbContext;

        public DatabaseTestClass(IMapper mapper, DemoDbContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task WithMapperMap()
        {
            List<FrenchCarModel> models = await _dbContext.Cars
                .Where(e => e.Plate.StartsWith("0"))
                .Select(e => _mapper.Map<FrenchCarModel>(e))
                .ToListAsync();

            List<Car> queryResults = await _dbContext.Cars
                .Where(e => e.Plate.StartsWith("0"))
                .AsNoTracking()
                .ToListAsync();

            List<FrenchCarModel> resultsModels = _mapper.Map<List<FrenchCarModel>>(queryResults);
        }

        public async Task WithProjectTo()
        {
            List<FrenchCarModel>? models = await _dbContext.Cars
                .Where(e => e.Plate.StartsWith("0"))
                .ProjectTo<FrenchCarModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task WithClassicSelect()
        {
            List<FrenchCarModel>? models = await _dbContext.Cars
                .Where(e => e.Plate.StartsWith("0"))
                .Select(e => new FrenchCarModel
                {
                    Plate = new FrenchCarPlate(e.Plate),
                    Seats = e.Seats.Select(s => new SeatModel
                    {
                        Number = s.Number
                    }).ToList()
                })
                .ToListAsync();
        }
    }
}