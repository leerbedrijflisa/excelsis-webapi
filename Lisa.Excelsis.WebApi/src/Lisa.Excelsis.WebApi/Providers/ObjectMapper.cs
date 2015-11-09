using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Lisa.Excelsis.WebApi
{
    public class ObjectMapper
    {
        public ExpandoObject Single(IRowProvider row)
        {
            var tree = new MappingTree();
            tree.Add(row);

            return tree.Root.Children.First().CreateObject();
        }

        public IEnumerable<ExpandoObject> Many(IDataProvider table)
        {
            var tree = new MappingTree();
            foreach (var row in table.Rows)
            {
                tree.Add(row);
            }

            // Collect the results instead of yielding them, because that makes the life time
            // of the data provider predictable. Specifically, if we just yield the results, they
            // will be retrieved after the connection to the database has already closed.
            var results = new List<ExpandoObject>();
            foreach (var node in tree.Root.Children)
            {
                results.Add(node.CreateObject());
            }
            return results;
        }
    }
}