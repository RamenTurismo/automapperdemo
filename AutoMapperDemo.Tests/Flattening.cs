using AutoMapper;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AutoMapperDemo.Tests
{
    public class Flattening
    {
        private readonly User _user;
        private readonly Ticket _ticket;
        private readonly Article _article;
        private readonly IMapper _mapper;

        public Flattening()
        {
            _user = new User
            {
                Name = "Anakin"
            };

            _ticket = new Ticket
            {
                User = _user
            };

            _article = new Article
            {
                Name = "Tabac",
                Price = 99.99m
            };

            _ticket.AddTicketLineItem(_article, 8);

            _mapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Ticket, TicketDto>()
                        .ForMember(x => x.CustomerName, x =>
                        {
                            x.Condition(e => e.User is not null);
                            x.MapFrom(e => e.User!.Name);
                        })
                        .ReverseMap();
                })
                .CreateMapper();
        }

        [Fact]
        public void Should_unflattened()
        {
            TicketDto dto = _mapper.Map<Ticket, TicketDto>(_ticket);
            Ticket model = _mapper.Map<TicketDto, Ticket>(dto);

            model.User!.Name.Should().Be("Anakin");

            // Limitation
            model.GetTotal().Should().NotBe(799.92m);
        }

        [Fact]
        public void Should_flattened()
        {
            TicketDto dto = _mapper.Map<Ticket, TicketDto>(_ticket);

            dto.CustomerName.Should().Be("Anakin");
            dto.Total.Should().Be(799.92m);
        }

        public sealed record ComplexTicketDto
        {
            public string? UserName { get; init; }
            
            public decimal Total { get; init; }
        }

        public sealed record TicketDto
        {
            public string? UserName { get; init; }

            public string? CustomerName { get; init; }

            public decimal Total { get; init; }
        }

        public sealed record Ticket
        {
            private readonly IList<TicketLine> _ticketLineItems = new List<TicketLine>();

            public User? User { get; init; }

            public TicketLine[] GetTicketLineItems()
            {
                return _ticketLineItems.ToArray();
            }

            public void AddTicketLineItem(Article article, int quantity)
            {
                _ticketLineItems.Add(new TicketLine(article, quantity));
            }

            public decimal GetTotal()
            {
                return _ticketLineItems.Sum(li => li.GetTotal());
            }
        }

        public sealed record Article
        {
            public decimal Price { get; init; }

            public string? Name { get; init; }
        }

        public sealed record TicketLine
        {
            public TicketLine(Article article, int quantity)
            {
                Article = article;
                Quantity = quantity;
            }

            public Article Article { get; init; }

            public int Quantity { get; init; }

            public decimal GetTotal()
            {
                return Quantity * Article.Price;
            }
        }

        public sealed record User
        {
            public string? Name { get; init; }
        }
    }
}