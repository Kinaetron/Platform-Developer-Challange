using System;
using System.Linq;
using System.IO.Abstractions;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Stop.Repository.Importers;
using Stop.Model;

namespace Stop.Repository
{
    public class CSVStopRepository : ICSVStopRepository
    {
        private readonly IEnumerable<CSVStopViewModel> stops;

        public CSVStopRepository(ICSVImporter<CSVStopViewModel> importer, IFileSystem fileSystem, IConfiguration configuration)
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

        public CSVStopViewModel Get(string id) =>
           stops.FirstOrDefault(x => x.StopId == id);

        public IEnumerable<CSVStopViewModel> GetAll() =>
            stops;
    }
}
