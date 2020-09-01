using System;
using System.Linq;
using System.IO.Abstractions;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Stop.API.Models;
using Stop.API.Importers;

namespace Stop.API.Repositories
{
    public class CSVStopRepository : ICSVStopRepository
    {
        private readonly IEnumerable<CSVStop> stops;

        public CSVStopRepository(ICSVImporter<CSVStop> importer, IFileSystem fileSystem, IConfiguration configuration)
        {
            if (importer == null) {
                throw new ArgumentNullException(nameof(importer));
            }

            if(fileSystem == null) {
                throw new ArgumentNullException(nameof(fileSystem));
            }

            if (configuration == null) {
                throw new ArgumentNullException(nameof(configuration));
            }

            var data = fileSystem.File.ReadAllText(configuration["CSVFileLocation"]);
            stops = importer.Import(configuration["CharSeperator"][0], data);
        }

        public CSVStop Get(string id) =>
           stops.FirstOrDefault(x => x.StopId == id);

        public IEnumerable<CSVStop> GetAll() =>
            stops;
    }
}
