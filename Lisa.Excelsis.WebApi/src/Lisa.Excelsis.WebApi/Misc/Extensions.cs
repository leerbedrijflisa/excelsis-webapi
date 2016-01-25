using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    public static class Extensions
    {
        public static void AddDynamic<T>(this ICollection<T> destination, T source)
        {
            List<T> list = destination as List<T>;

            if (list != null)
            {
                list.Add(source);
            }
            else
            {               
                destination.Add(source);
            }
        }

        public static void AddDynamic<T>(this ICollection<T> destination, IEnumerable<T> source)
        {
            List<T> list = destination as List<T>;

            if (list != null)
            {
                list.AddRange(source);
            }
            else
            {
                foreach (var item in source)
                {
                    destination.Add(item);
                }
            }
        }
    }
}
