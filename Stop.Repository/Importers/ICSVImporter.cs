using System.Collections.Generic;

namespace Stop.Repository.Importers
{
    public interface ICSVImporter<T>
    {
        IEnumerable<T> Import(char seperatorChar, string data);
    }
}
