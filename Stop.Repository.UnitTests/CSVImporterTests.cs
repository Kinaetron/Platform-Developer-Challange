using System;
using System.Text;
using System.Linq;
using Xunit;
using FluentAssertions;
using Stop.API.Mappers;
using Stop.Model;
using Stop.Repository.Importers;

namespace Stop.Repository.UnitTests
{
    public class CSVImporterTests
    {
        private class Arrangement<T>
        {
            public string StopData { get; }
            public CSVImporter<T> SUT { get; }
            public char CharSeperator { get; }

            public Arrangement(CSVImporter<T> importer, char charSeperator, string stopData)
            {
                SUT = importer;
                StopData = stopData;
                CharSeperator = charSeperator;
            }
        }

        private class ArrangementBuilder
        {
            private CSVStopMapping mapper;
            private string stopData = string.Empty;
            private readonly char seperatorChar = ',';

            public ArrangementBuilder WithStopMapper()
            {
                mapper = new CSVStopMapping();
                return this;
            }

            public ArrangementBuilder WithImportData()
            {
                stopData = new StringBuilder()
                .AppendLine("StopId,StopName,StopLatitude,StopLongitude")
                .AppendLine("390010001,Wades Lane,51.98371886,1.230877515")
                .AppendLine("90010024,The Boot,51.98072502,1.234434367").ToString();

                return this;
            }

            public Arrangement<CSVStopViewModel> Build()
            {
                var importer = new CSVImporter<CSVStopViewModel>(mapper);
                return new Arrangement<CSVStopViewModel>(importer, seperatorChar, stopData);
            }
        }


        [Fact]
        public void Ctor_WithNullCSVMapper_ShouldThrow_ArgumentNullException()
        {
            // act
            var error = Record.Exception(() => new CSVImporter<CSVStopViewModel>(null));

            //assert
            error.Should().BeOfType<ArgumentNullException>();
            error.Message.Should().Be("Value cannot be null. (Parameter 'csvMapper')");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Import_EmptyCSVFile_ShouldThrow_ArguementNullException(string stopData)
        {
            // arrange
            var arrangement = new ArrangementBuilder()
                                .WithStopMapper()
                                .Build();

            // act
            var error = Record.Exception(() => arrangement.SUT.Import(arrangement.CharSeperator, stopData));

            // assert
            error.Should().BeOfType<ArgumentNullException>();
            error.Message.Should().Be("Value cannot be null. (Parameter 'data')");
        }

        [Fact]
        public void ImportCSVFile_ShouldCreate_2StopObjects()
        {
            // arrange
            var arrangement = new ArrangementBuilder()
                                .WithStopMapper()
                                .WithImportData()
                                .Build();

            // act
            var csvImporter = arrangement.SUT.Import(arrangement.CharSeperator, arrangement.StopData).ToList();

            // assert
            csvImporter.Should().HaveCount(2);

            csvImporter[0].StopId.Should().Be("390010001");
            csvImporter[0].StopName.Should().Be("Wades Lane");
            csvImporter[0].Latitude.Should().Be(51.98371886);
            csvImporter[0].Longitude.Should().Be(1.230877515);

            csvImporter[1].StopId.Should().Be("90010024");
            csvImporter[1].StopName.Should().Be("The Boot");
            csvImporter[1].Latitude.Should().Be(51.98072502);
            csvImporter[1].Longitude.Should().Be(1.234434367);
        }
    }
}
