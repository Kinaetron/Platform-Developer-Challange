using Xunit;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using System.Linq;
using Stop.Model;

namespace Stop.API.IntegrationTests.Controllers
{
    public class StopControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient client;
        public StopControllerTests(WebApplicationFactory<Startup> factory)
        {
            client = factory.CreateClient() ?? throw new ArgumentNullException(nameof(factory));
        }

        [Fact]
        public async Task Endpoint_GetStop_WithInCorrectStopId_ShouldReturnNotFound()
        {
            // act
            var httpResponse = await client.GetAsync("/api/stops/20201");

            // assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Endpoint_GetStop_WithCorrectStopId_ShouldReturnStop()
        {
            // act
            var httpResponse = await client.GetAsync("/api/stops/390010582");

            // assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseObject = await JsonSerializer.DeserializeAsync<Model.StopViewModel>(await httpResponse.Content.ReadAsStreamAsync());
            
            responseObject.StopId.Should().Be("390010582");
            responseObject.StopName.Should().Be("Garage");
            responseObject.Location.Latitude.Should().Be(52.03564126);
            responseObject.Location.Longitude.Should().Be(1.088485654);
        }

        [Fact]
        public async Task Endpoint_GetStops_WithInCorrectMinMaxLatLng_ShouldReturnNotFound()
        {
            // act
            var httpResponse = await client.GetAsync("/api/stops?minLatitude=200&minLongitude=20" +
                                                     "&maxLatitude=400&maxLongitude=30");

            // assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Endpoint_GetStops_WithCorrectMinMaxLatLng_ShouldReturnStops()
        {
            // act
            var httpResponse = await client.GetAsync("/api/stops?minLatitude=51.95578072&minLongitude=1.057161711" +
                                                     "&maxLatitude=51.95953435&maxLongitude=1.060989199");

            // assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseObject = await JsonSerializer.DeserializeAsync<List<Model.StopViewModel>>(await httpResponse.Content.ReadAsStreamAsync());

            responseObject.Should().HaveCount(3);

            var firstObject = responseObject.FirstOrDefault(x => x.StopId == "390010061");
            firstObject.Should().NotBeNull();
            firstObject.StopName.Should().Be("Bus Shelter");
            firstObject.Location.Latitude.Should().Be(51.95762824);
            firstObject.Location.Longitude.Should().Be(1.057161711);


            var secondObject = responseObject.FirstOrDefault(x => x.StopId == "390010062");
            secondObject.Should().NotBeNull();
            secondObject.StopName.Should().Be("Bus Shelter");
            secondObject.Location.Latitude.Should().Be(51.95747221);
            secondObject.Location.Longitude.Should().Be(1.057282105);

            var thirdObject = responseObject.FirstOrDefault(x => x.StopId == "390010864");
            thirdObject.Should().NotBeNull();
            thirdObject.StopName.Should().Be("Temple Pattle");
            thirdObject.Location.Latitude.Should().Be(51.95953435);
            thirdObject.Location.Longitude.Should().Be(1.060989199);
        }

        [Fact]
        public async Task Endpoint_GetPlaceNearby_WithInCorrectStopId_ShouldReturnNotFound()
        {
            // act
            var httpResponse = await client.GetAsync("/api/1234/nearby?distance=20");

            // assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Endpoint_GetPlaceNearby_WithNoPlaceNearby_ShouldReturnNotFound()
        {
            // act
            var httpResponse = await client.GetAsync("/api/stops/390010710/nearby?distance=1");

            // assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Endpoint_GetPlaceNearby_WithPlacesNearby_ShouldReturnPlaces()
        {
            // act
            var httpResponse = await client.GetAsync("/api/stops/390010710/nearby?distance=5");

            // assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseObject = await JsonSerializer.DeserializeAsync<List<PlaceViewModel>>(await httpResponse.Content.ReadAsStreamAsync());

            responseObject.Should().HaveCount(1);
            responseObject[0].PlaceName.Should().Be("Four Sisters");
            responseObject[0].Location.Latitude.Should().Be(51.987072);
            responseObject[0].Location.Longitude.Should().Be(1.007034);
        }
    }
}
