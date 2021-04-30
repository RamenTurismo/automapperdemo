using AutoMapperDemo.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace AutoMapperDemo
{
    public class DemoDbContext : DbContext
    {
        private static readonly Random Random = new();
        
        public DbSet<Car> Cars => Set<Car>();

        public DbSet<Seat> Seats => Set<Seat>();

        public DemoDbContext(DbContextOptions<DemoDbContext> options) : base(options)
        {
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var plates = new[]
            {
                $"{Random.Next(10, 99)}-{Random.Next(100, 999)}-{Random.Next(10, 99)}",
                $"{Random.Next(10, 99)}-{Random.Next(100, 999)}-{Random.Next(10, 99)}",
                $"{Random.Next(10, 99)}-{Random.Next(100, 999)}-{Random.Next(10, 99)}"
            };

            modelBuilder.Entity<Car>(builder =>
            {
                builder.HasKey(e => e.Plate);

                builder.HasData(
                    new Car
                    {
                        Plate = plates[0]
                    },
                    new Car
                    {
                        Plate = plates[1]
                    },
                    new Car
                    {
                        Plate = plates[2]
                    });
            });

            modelBuilder.Entity<Seat>(builder =>
            {
                builder.HasKey(e => new { e.Number, e.CarPlate });

                builder.HasOne(e => e.Car)
                    .WithMany(e => e!.Seats)
                    .HasForeignKey(e => e.CarPlate);

                builder.HasData(
                    new Seat
                    {
                        CarPlate = plates[0],
                        Number = 1
                    },
                    new Seat
                    {
                        CarPlate = plates[0],
                        Number = 2
                    },
                    new Seat
                    {
                        CarPlate = plates[1],
                        Number = 1
                    },
                    new Seat
                    {
                        CarPlate = plates[1],
                        Number = 2
                    },
                    new Seat
                    {
                        CarPlate = plates[1],
                        Number = 3
                    },
                    new Seat
                    {
                        CarPlate = plates[2],
                        Number = 14
                    },
                    new Seat
                    {
                        CarPlate = plates[2],
                        Number = 27
                    },
                    new Seat
                    {
                        CarPlate = plates[2],
                        Number = 38
                    });
            });
        }
    }
}
