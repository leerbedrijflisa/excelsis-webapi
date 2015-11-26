using System;
using System.ComponentModel.DataAnnotations;

namespace Lisa.Excelsis.WebApi
{
    public class AssessmentPost
    {
        public string StudentName { get; set; }
        public string StudentNumber { get; set; }
        [Required]
        public DateTime? Assessed { get; set; }
        [Required]
        public string[] Assessors { get; set; }
    }
}