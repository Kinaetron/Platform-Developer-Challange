using Moq;
using Xunit;
using Stop.API.Repositories;
using Stop.API.Importers;
using Stop.API.Models;
using System;
using System.IO.Abstractions;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using FluentAssertions;

namespace Stop.API.UnitTests
{
    public class CSVStopRepositoryTests
    {
        private class Arrangement
        {
            public CSVStopRepository SUT { get; }
            public IFileSystem FileSystem { get; }
            public IConfiguration Configuration { get; }
            public ICSVImporter<CSVStop> Importer { get; }

            public Arrangement(ICSVImporter<CSVStop> importer, IFileSystem fileSystem, IConfiguration configuration)
            {
                Importer = importer;
                FileSystem = fileSystem;
                Configuration = configuration;
                SUT = new CSVStopRepository(importer, fileSystem, configuration);
            }
        }

        private class ArrangementBuilder
        {
            private readonly Mock<IFileSystem> fileSystem = new Mock<IFileSystem>();
            private readonly Mock<IConfiguration> configuration = new Mock<IConfiguration>();
            private readonly Mock<ICSVImporter<CSVStop>> csvImporter = new Mock<ICSVImporter<CSVStop>>();
            private readonly IEnumerable<CSVStop> stops = new List<CSVStop>()
            {
                new CSVStop() { StopId = "100", StopName = "TestStop", Latitude = 1, Longitude = 1}
            };

            public ArrangementBuilder WithFileSystem()
            {
                fileSystem.Setup(m => m.File.ReadAllText(It.IsAny<string>())).Returns(string.Empty);
                return this;
            }

            public ArrangementBuilder WithConfiguration()
            {
                configuration.Setup(m => m[It.Is<string>(s => s == "CharSeperator")]).Returns("#");
                configuration.Setup(m => m[It.Is<string>(s => s == "CSVFileLocation")]).Returns(string.Empty);
                return this;
            }

            public ArrangementBuilder WithImporter()
            {
                csvImporter.Setup(m => m.Import(It.IsAny<char>(), It.IsAny<string>())).Returns(stops);
                return this;
            }

            public Arrangement Build()
            {
                return new Arrangement(csvImporter.Object, fileSystem.Object, configuration.Object);
            }
        }

        [Fact]
        public void Ctor_WithNullCSVImporter_ShouldThrowArgumentNullException()
        {
            // arrange
          var arrangement = new ArrangementBuilder()
                               .WithFileSystem()
                               .WithConfiguration()
                               .Build();

            // act 
            var error = Record.Exception(() => 
            new CSVStopRepository(null, arrangement.FileSystem, arrangement.Configuration));

            // assert
            error.Should().BeOfType<ArgumentNullException>();
            error.Message.Should().Be("Value cannot be null. (Parameter 'importer')");
        }

        [Fact]
        public void Ctor_WithNullFileSystem_ShouldThrowArgumentNullException()
        {
            // arrange
            var arrangement = new ArrangementBuilder()
                                 .WithFileSystem()
                                 .WithConfiguration()
                                 .Build();

            // act 
            var error = Record.Exception(() =>
            new CSVStopRepository(arrangement.Importer, null, arrangement.Configuration));

            // assert
            error.Should().BeOfType<ArgumentNullException>();
            error.Message.Should().Be("Value cannot be null. (Parameter 'fileSystem')");
        }

        [Fact]
        public void Ctor_WithNullConfiguration_ShouldThrowArgumentNullException()
        {
            // arrange
            var arrangement = new ArrangementBuilder()
                                 .WithFileSystem()
                                 .WithConfiguration()
                                 .Build();

            // act 
            var error = Record.Exception(() =>
            new CSVStopRepository(arrangement.Importer, arrangement.FileSystem, null));

            // assert
            error.Should().BeOfType<ArgumentNullException>();
            error.Message.Should().Be("Value cannot be null. (Parameter 'configuration')");
        }
    }
}
