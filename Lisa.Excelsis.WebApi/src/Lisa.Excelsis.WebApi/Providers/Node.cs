using System.Collections.Generic;
using System.Dynamic;

namespace Lisa.Excelsis.WebApi
{
    internal class Node
    {
        public Node()
        {
            Children = new List<Node>();
        }

        public object Identity { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public ICollection<Node> Children { get; set; }

        public Node Find(object identity)
        {
            foreach (var child in Children)
            {
                if (identity.Equals(child.Identity))
                {
                    return child;
                }
            }

            return null;
        }

        public ExpandoObject CreateObject()
        {
            IDictionary<string, object> obj = new ExpandoObject();
            foreach (var child in Children)
            {
                child.Map(obj);
            }

            return (ExpandoObject) obj;
        }

        protected virtual void Map(IDictionary<string, object> obj)
        {
        }
    }

    internal class ScalarNode : Node
    {
        protected override void Map(IDictionary<string, object> obj)
        {
            obj.Add(Name, Value);
        }
    }

    internal class SubObjectNode : Node
    {
        protected override void Map(IDictionary<string, object> obj)
        {
            var value = CreateObject();
            obj.Add(Name, value);
        }
    }

    internal class ListNode : Node
    {
        protected override void Map(IDictionary<string, object> obj)
        {
            var list = new List<ExpandoObject>();
            obj.Add(Name, list);

            foreach (var child in Children)
            {
                var listItem = child.CreateObject();
                list.Add(listItem);
            }
        }
    }

    internal class ArrayNode : Node
    {
        protected override void Map(IDictionary<string, object> obj)
        {
            var array = new List<object>();
            obj.Add(Name, array);

            foreach (var child in Children)
            {
                array.Add(child.Value);
            }
        }
    }
}