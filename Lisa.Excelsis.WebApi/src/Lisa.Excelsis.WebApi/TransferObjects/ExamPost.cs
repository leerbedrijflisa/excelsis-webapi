using System.ComponentModel.DataAnnotations;

namespace Lisa.Excelsis.WebApi
{
    public class ExamPost
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Cohort { get; set; }
        [Required]
        public string Crebo { get; set; }
    } 
}