using System.ComponentModel.DataAnnotations;

namespace Lisa.Excelsis.WebApi
{
    public class CriterionPost
    {
        [Required]
        public int Order { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Value { get; set; }
    }
}