using System;
using System.ComponentModel.DataAnnotations;

namespace Lisa.Excelsis.WebApi
{
    public class AssessmentPost
    {
        public Student Student { get; set; }
        [Required]
        public DateTime? Assessed { get; set; }
        [Required]
        public string[] Assessors { get; set; }
    }

    public class Student
    {
        public string Name { get; set; }
        public string Number { get; set; }
    }

    public class ExamPost
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Cohort { get; set; }
        [Required]
        public string Status { get; set; }
        public string Crebo { get; set; }
    }
}