using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Stop.API.Controllers;
using Stop.API.Mappers;
using Stop.API.Models;
using Stop.API.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Stop.API.UnitTests
{
    public class StopControllerTests
    {
        private class Arrangement
        {
            public IMapper Mapper { get; }
            public StopController SUT { get; }
            public IPlacesRepository PlacesRepository { get; }
            public ICSVStopRepository CSVStopRepository { get; }
            public Arrangement(ICSVStopRepository csvStopRepository, IPlacesRepository placesRepository, IMapper mapper)
            {
                Mapper = mapper;
                PlacesRepository = placesRepository;
                CSVStopRepository = csvStopRepository;

                SUT = new StopController(csvStopRepository, placesRepository, mapper);
            }
        }

        private class ArrangementBuilder
        {
            private readonly Mock<IPlacesRepository> placesRepository = new Mock<IPlacesRepository>();
            private readonly Mock<ICSVStopRepository> csvStopRepository = new Mock<ICSVStopRepository>();

            public IMapper Mapper()
            {
               var myProfile = new MappingProfile();
               var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
               return new Mapper(configuration);
            }

            public ArrangementBuilder WithPlacesRepositoryWithValues()
            {
                var places = new List<Place>
                    { new Place() { PlaceName = "test", Geometry = new Geometry() { Location = new Coordinates() { Lat= 1, Lng = 1 } } } };

                WithPlacesRepository(places);

                return this;
            }

            public ArrangementBuilder WithPlacesRepositoryWithoutValues()
            {
                WithPlacesRepository(new List<Place>());
                return this;
            }

            public ArrangementBuilder WithCSVStopRepositoryWithValues()
            {
                var value = new CSVStop() { StopId = "123", StopName = "Test", Latitude = 1, Longitude = 1 };

                csvStopRepository.Setup(m => m.Get(It.IsAny<string>())).Returns(value);
                csvStopRepository.Setup(m => m.GetAll()).Returns(new List<CSVStop>() { value });
                return this;
            }

            private void WithPlacesRepository(List<Place> places)
            {
                placesRepository.Setup(m => m.Find(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()))
                   .ReturnsAsync(places);
            }

            public Arrangement Build()
            {
                return new Arrangement(csvStopRepository.Object, placesRepository.Object, Mapper());
            }
        }

        [Fact]
        public void Ctor_WithNullCSVStopRepository_ShouldThrowNullException()
        {
            // arrange
            var arrangement = new ArrangementBuilder()
                .WithPlacesRepositoryWithValues()
                .Build();

            // act
            var error = Record.Exception(() => new StopController(null, arrangement.PlacesRepository, arrangement.Mapper));

            // assert
            error.Should().BeOfType<ArgumentNullException>();
            error.Message.Should().Be("Value cannot be null. (Parameter 'stopRepository')");
        }

        [Fact]
        public void Ctor_WithNullPlacesRepository_ShouldThrowNullException()
        {
            // arrange
            var arrangement = new ArrangementBuilder()
                .WithCSVStopRepositoryWithValues()
                .Build();

            // act
            var error = Record.Exception(() => new StopController(arrangement.CSVStopRepository, null, arrangement.Mapper));

            // assert
            error.Should().BeOfType<ArgumentNullException>();
            error.Message.Should().Be("Value cannot be null. (Parameter 'placesRepository')");
        }

        [Fact]
        public void Ctor_WithNullMapper_ShouldThrowArgumentNullException()
        {
            // arrange
            var arrangement = new ArrangementBuilder()
                .WithPlacesRepositoryWithValues()
                .WithCSVStopRepositoryWithValues()
                .Build();

            // act
            var error = Record.Exception(() => new StopController(arrangement.CSVStopRepository, arrangement.PlacesRepository, null));

            // assert
            error.Should().BeOfType<ArgumentNullException>();
            error.Message.Should().Be("Value cannot be null. (Parameter 'mapper')");
        }

       [Fact]
       public void GetStops_Within_LatLngParam_WithCorrectValues_ShouldReturn_Ok()
        {
            // arrangement
            var arrangement = new ArrangementBuilder()
              .WithCSVStopRepositoryWithValues()
              .Build();

            // act
            var result = arrangement.SUT.Get(1, 1, 1, 1);

            // assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetStops_Within_LatLngParam_WithIncorrectData_ShouldReturn_NotFound()
        {
            // arrangement
            var arrangement = new ArrangementBuilder()
              .WithCSVStopRepositoryWithValues()
              .Build();

            // act
            var result = arrangement.SUT.Get(10, 10, 20, 20);

            // assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void GetStop_WithCorrectStopId_ShouldReturn_Ok()
        {
            // arrangement
            var arrangement = new ArrangementBuilder()
              .WithCSVStopRepositoryWithValues()
              .Build();

            // act
            var result = arrangement.SUT.Get("123");

            // assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void Get_Stop_WithInCorrectStopId_ShouldReturn_NotFound()
        {
            // arrangement
            var arrangement = new ArrangementBuilder()
              .Build();

            // act
            var result = arrangement.SUT.Get("12345");

            // assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetPlace_WithCorrectStopId_Should_Return_Ok()
        {
            // arrangement
            var arrangement = new ArrangementBuilder()
              .WithPlacesRepositoryWithValues()
              .WithCSVStopRepositoryWithValues()
              .Build();

            // act
            var result = await arrangement.SUT.GetAsync("123", 10);

            // assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetPlace_WithCorrectStopId_ShouldReturn_NotFound()
        {
            // arrangement
            var arrangement = new ArrangementBuilder()
              .WithPlacesRepositoryWithValues()
              .Build();

            // act
            var result = await arrangement.SUT.GetAsync("123", 10);

            // assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetPlace_WithNoPlacesNearby_ShouldReturn_NotFound()
        {
            // arrangement
            var arrangement = new ArrangementBuilder()
              .WithCSVStopRepositoryWithValues()
              .WithPlacesRepositoryWithoutValues()
              .Build();

            // act
            var result = await arrangement.SUT.GetAsync("123", 10);

            // assert
            result.Should().BeOfType<NotFoundResult>();
        }

    }
}
