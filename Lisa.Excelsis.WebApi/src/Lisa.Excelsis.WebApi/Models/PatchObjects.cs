using System.ComponentModel.DataAnnotations;

namespace Lisa.Excelsis.WebApi
{
    public class CategoryAdd
    {
        [Required]
        public int? Order { get; set; }

        [Required]
        public string Name { get; set; }

        public int ExamId { get; set; }
    }

    public class CriterionAdd
    {
        [Required]
        public int? Order { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Weight { get; set; }

        public int ExamId { get; set; }
        public int CategoryId { get; set; }
    }
}