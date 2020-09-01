using System;
using System.Linq;
using System.Collections.Generic;
using TinyCsvParser;
using TinyCsvParser.Mapping;

namespace Stop.Repository.Importers
{
    public class CSVImporter<T> : ICSVImporter<T>
    {
        private readonly ICsvMapping<T> csvMapper;

        public CSVImporter(ICsvMapping<T> csvMapper)
        {
            this.csvMapper = csvMapper ?? throw new ArgumentNullException(nameof(csvMapper));
        }

        public IEnumerable<T> Import(char seperatorChar, string data)
        {
            if(string.IsNullOrEmpty(data)) {
                throw new ArgumentNullException(nameof(data));
            }

            var csvParserOptions = new CsvParserOptions(true, seperatorChar);
            var csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            var csvParser = new CsvParser<T>(csvParserOptions, csvMapper);

            var result = csvParser.ReadFromString(csvReaderOptions, data)
                .Where(x => x.IsValid)
                .Select(x => x.Result)
                .AsEnumerable();

            return result;
        }
    }
}
