using System.Collections.Generic;

namespace Stop.API.Importers
{
    public interface ICSVImporter<T>
    {
        IEnumerable<T> Import(char seperatorChar, string data);
    }
}
