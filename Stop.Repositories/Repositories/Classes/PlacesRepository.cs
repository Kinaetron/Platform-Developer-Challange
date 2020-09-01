using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Stop.Model;
using Microsoft.Extensions.Configuration;

namespace Stop.Repository
{
    public class PlacesRepository : IPlacesRepository
    {
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory httpClientFactory;

        public PlacesRepository(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<IEnumerable<Point>> Find(double distance, double latitude, double longtitude)
        {
            var results = new List<Point>();

            var connectionString = string.Format(configuration["GooglePlaces:ConnectionString"], 
                latitude, longtitude, distance, configuration["GooglePlaces:Key"]);


            using (var client = httpClientFactory.CreateClient())
            {
                var response = await client.GetAsync(connectionString);
                if (response.IsSuccessStatusCode)
                {
                    var places = await JsonSerializer.DeserializeAsync<PointsViewModel>(await response.Content.ReadAsStreamAsync());
                    results = places.Results;
                }
            }

            return results;
        }
    }
}
