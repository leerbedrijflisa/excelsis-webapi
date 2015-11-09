using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    using Field = KeyValuePair<string, object>;

    public class RowInfo
    {
        public RowInfo(IEnumerable<Field> row)
        {
            Identity = new object();

            foreach (var field in row)
            {
                if (ContainsIdentity(field))
                {
                    Identity = field.Value;
                }
                else if (IsPartOfSubObject(field))
                {
                    var prefix = field.Key.Substring(0, field.Key.IndexOf("_"));
                    if (!_subObjects.ContainsKey(prefix))
                    {
                        _subObjects.Add(prefix, new List<Field>());
                    }

                    var fieldName = field.Key.Substring(field.Key.IndexOf("_") + 1);
                    var fields = (List<Field>) _subObjects[prefix];
                    fields.Add(new Field(fieldName, field.Value));
                }
                else if (IsPartOfListItem(field))
                {
                    var prefix = field.Key.Substring(1, field.Key.IndexOf("_") - 1);
                    if (!_lists.ContainsKey(prefix))
                    {
                        _lists.Add(prefix, new List<Field>());
                    }

                    var fieldName = field.Key.Substring(field.Key.IndexOf("_") + 1);
                    var fields = (List<Field>) _lists[prefix];
                    fields.Add(new Field(fieldName, field.Value));
                }
                else if (IsArrayItem(field))
                {
                    var name = field.Key.Substring(1);
                    if (!_arrays.ContainsKey(name))
                    {
                        _arrays.Add(name, new List<object>());
                    }

                    var items = (List<object>) _arrays[name];
                    items.Add(field.Value);
                }
                else
                {
                    _scalars.Add(field);
                }
            }
        }

        public object Identity { get; set; }

        public IEnumerable<Field> Scalars
        {
            get { return _scalars; }
        }

        public IDictionary<string, IEnumerable<Field>> SubObjects
        {
            get { return _subObjects; }
        }

        public IDictionary<string, IEnumerable<Field>> Lists
        {
            get { return _lists; }
        }

        public IDictionary<string, IEnumerable<object>> Arrays
        {
            get { return _arrays; }
        }

        private bool ContainsIdentity(Field field)
        {
            return field.Key.StartsWith("@");
        }

        private bool IsPartOfSubObject(Field field)
        {
            return field.Key.Contains("_") && !field.Key.StartsWith("@") && !field.Key.StartsWith("#");
        }

        private bool IsPartOfListItem(Field field)
        {
            return field.Key.StartsWith("#") && field.Key.Contains("_");
        }

        private bool IsArrayItem(Field field)
        {
            return field.Key.StartsWith("#") && !field.Key.Contains("_");
        }

        private List<Field> _scalars = new List<Field>();
        private Dictionary<string, IEnumerable<Field>> _subObjects = new Dictionary<string, IEnumerable<Field>>();
        private Dictionary<string, IEnumerable<Field>> _lists = new Dictionary<string, IEnumerable<Field>>();
        private Dictionary<string, IEnumerable<object>> _arrays = new Dictionary<string, IEnumerable<object>>();
    }
}