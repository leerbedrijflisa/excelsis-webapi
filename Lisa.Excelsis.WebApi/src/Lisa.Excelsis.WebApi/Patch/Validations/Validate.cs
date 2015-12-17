namespace Lisa.Excelsis.WebApi
{
    partial class Validate
    {
        public bool CheckResource(dynamic resource, string child, object childId)
        {
            var query = @"SELECT COUNT(*) as count FROM " + child + @"
                          WHERE " + resource.Name + @" = " + resource.Value + @" AND Id = @Id";
            var parameters = new { id = childId };

            dynamic result = _db.Execute(query, parameters);
            return (result.count > 0);
        }

        public bool CheckResourceInResource(dynamic resource, string parent, string parentId, string child, object childId)
        {
            var query = @"SELECT COUNT(*) as count FROM " + child + @"
                          WHERE " + resource.Name + @" = " + resource.Value + @" AND Id = @Id AND " + parent + @" = @ParentId";
            var parameters = new { Id = childId, ParentId = parentId };

            dynamic result = _db.Execute(query, parameters);
            return (result.count > 0);
        }

        private static readonly Database _db = new Database();
    }
}
