﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Stop.Model;
using Xunit;

namespace Stop.Repository.UnitTests
{
    public class PlaceRepositoryTests
    {
        private class Arrangement
        {
            public PointsViewModel Points { get; }
            public PlacesRepository SUT { get; }

            public IConfiguration Configuration { get; }

            public IHttpClientFactory HttpClientFactory { get; }

            public Arrangement(PointsViewModel points, IConfiguration configuration, IHttpClientFactory httpClientFactory)
            {
                Points = points;
                Configuration = configuration;
                HttpClientFactory = httpClientFactory;
                SUT = new PlacesRepository(configuration, httpClientFactory);
            }

        }

        private class ArrangementBuilder
        {
            private readonly Mock<IConfiguration> configuration = new Mock<IConfiguration>();
            private readonly Mock<IHttpClientFactory> httpClientFactory = new Mock<IHttpClientFactory>();
            private readonly PointsViewModel payload = new PointsViewModel()
            {
                Results = new List<Point>()
                    {
                        new Point()
                        {
                            PlaceName = "test",
                            Geometry = new Geometry() { Location = new Coordinates() { Lat = 1, Lng = 2 } }
                        }
                    }
            };

            public ArrangementBuilder WithConfiguration()
            {
                configuration.Setup(m => m[It.Is<string>(s => s == "GooglePlaces:ConnectionString")]).Returns("{0}{1}{2}{3}");
                configuration.Setup(m => m[It.Is<string>(s => s == "GooglePlaces:Key")]).Returns(string.Empty);
                return this;
            }

            public ArrangementBuilder WithHttpClientFactory()
            {
                WithHttpClientFactory(HttpStatusCode.OK);
                return this;
            }

            public ArrangementBuilder WithHttpClientFactoryBadRequest()
            {
                WithHttpClientFactory(HttpStatusCode.BadRequest);
                return this;
            }

            private void WithHttpClientFactory(HttpStatusCode httpStatusCode)
            {
                var httpMessageHandler = new Mock<HttpMessageHandler>();
                var stringPayload = JsonSerializer.Serialize(payload);

                httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = httpStatusCode,
                    Content = new StringContent(stringPayload, Encoding.UTF8, "application/json"),
                });

                var client = new HttpClient(httpMessageHandler.Object)
                {
                    BaseAddress = new Uri("http://test.com/")
                };
                httpClientFactory.Setup(m => m.CreateClient(It.IsAny<string>())).Returns(client);
            }

            public Arrangement Build()
            {
                return new Arrangement(payload, configuration.Object, httpClientFactory.Object);
            }
        }

        [Fact]
        public void Ctor_WithNullConfiguration_ShouldThrowArgumentNullException()
        {
            // arrange
            var arrangement = new ArrangementBuilder()
                                 .WithHttpClientFactory()
                                 .Build();

            // act
            var error = Record.Exception(() => new PlacesRepository(null, arrangement.HttpClientFactory));

            //assert
            error.Should().BeOfType<ArgumentNullException>();
            error.Message.Should().Be("Value cannot be null. (Parameter 'configuration')");
        }

        [Fact]
        public void Ctor_WithNullHttpClientFactory_ShouldThrowNullException()
        {
            // arrange
            var arrangement = new ArrangementBuilder()
                                 .WithConfiguration()
                                 .Build();

            // act
            var error = Record.Exception(() => new PlacesRepository(arrangement.Configuration, null));

            //assert
            error.Should().BeOfType<ArgumentNullException>();
            error.Message.Should().Be("Value cannot be null. (Parameter 'httpClientFactory')");
        }

        [Fact]
        public void Find_Successful_Should_Return_JSON_ObjectsAsync()
        {
            // arrange
            var arrangement = new ArrangementBuilder()
                                .WithConfiguration()
                                .WithHttpClientFactory()
                                .Build();

            // act
            var results = arrangement.SUT.Find(20, 1, 2).Result.ToList();

            // assert
            results.Should().HaveCount(1);
        }

        [Fact]
        public void Find_Failed_Should_Return_Null()
        {
            // arrange
            var arrangement = new ArrangementBuilder()
                                .WithConfiguration()
                                .WithHttpClientFactoryBadRequest()
                                .Build();

            // act
            var results = arrangement.SUT.Find(20, 1, 2).Result.ToList();

            // assert 
            results.Should().HaveCount(0);
        }
    }
}
