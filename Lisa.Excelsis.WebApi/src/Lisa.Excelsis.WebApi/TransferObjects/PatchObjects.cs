using System.ComponentModel.DataAnnotations;

namespace Lisa.Excelsis.WebApi
{
    public class CategoryAdd
    {
        [Required]
        [Value(@"^\d+$", 1202)]
        public int? Order { get; set; }

        [Required]
        [Value(@".*?", 1208, type: "string")]
        public string Name { get; set; }

        public int ExamId { get; set; }
    }

    public class CriterionAdd
    {
        [Required]
        [Value(@"^\d+$", 1202)]
        public int? Order { get; set; }

        [Required]
        [Value(@".*?", 1208, type: "string")]
        public string Title { get; set; }

        [Required]
        [Value(@".*?", 1208, type: "string")]
        public string Description { get; set; }

        [Required]
        [Value(@"^(fail|pass|excellent)$", 1204, permitted1: "fail", permitted2: "pass", permitted3: "excellent")]
        public string Weight { get; set; }

        public int ExamId { get; set; }
        public int CategoryId { get; set; }
    }
}
