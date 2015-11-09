using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    using Field = KeyValuePair<string, object>;

    internal class MappingTree
    {
        public MappingTree()
        {
            Root = new Node();
        }

        public Node Root { get; set; }

        public void Add(IRowProvider rowData)
        {
            AddSubObject(Root, new KeyValuePair<string, IEnumerable<Field>>(null, rowData.Fields));
        }

        private void AddScalar(Node parent, Field scalar)
        {
            var node = new ScalarNode();
            node.Name = scalar.Key;
            node.Value = scalar.Value;
            parent.Children.Add(node);
        }

        private void AddList(Node parent, KeyValuePair<string, IEnumerable<Field>> list)
        {
            var node = parent.Find(list.Key);
            if (node == null)
            {
                node = new ListNode();
                node.Identity = list.Key;
                node.Name = list.Key;
                parent.Children.Add(node);
            }

            AddSubObject(node, new KeyValuePair<string, IEnumerable<Field>>(null, list.Value));
        }

        private void AddArray(Node parent, KeyValuePair<string, IEnumerable<object>> array)
        {
            var node = parent.Find(array.Key);
            if (node == null)
            {
                node = new ArrayNode();
                node.Identity = array.Key;
                node.Name = array.Key;
                parent.Children.Add(node);
            }

            foreach (var value in array.Value)
            {
                var item = new Node();
                item.Value = value;
                node.Children.Add(item);
            }
        }

        private void AddSubObject(Node parent, KeyValuePair<string, IEnumerable<Field>> subObject)
        {
            var row = new RowInfo(subObject.Value);
            var node = parent.Find(row.Identity);
            if (node == null)
            {
                node = new SubObjectNode();
                node.Identity = row.Identity;
                node.Name = subObject.Key;
                parent.Children.Add(node);

                foreach (var scalar in row.Scalars)
                {
                    AddScalar(node, scalar);
                }
            }
            
            foreach (var subObj in row.SubObjects)
            {
                AddSubObject(node, subObj);
            }

            foreach (var list in row.Lists)
            {
                AddList(node, list);
            }

            foreach (var array in row.Arrays)
            {
                AddArray(node, array);
            }
        }
    }
}