namespace Lisa.Excelsis.WebApi
{
    public class CategoryAdd
    {
        public int Order { get; set; }
        public string Name { get; set; }
        public int ExamId { get; set; }
    }

    public class CriterionAdd
    {
        public int Order { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Weight { get; set; }
        public int ExamId { get; set; }
        public int CategoryId { get; set; }
    }
}
