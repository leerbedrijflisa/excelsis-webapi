using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lisa.Excelsis.WebApi
{
    public class AssessmentPost
    {
        public student Student { get; set; }
        [Required]
        public DateTime? Assessed { get; set; }
        [Required]
        public IList<AssessorPost> Assessors { get; set; }
    }

    public class student
    {
        public string Name { get; set; }
        public string Number { get; set; }
    }

    public class AssessorPost
    {
        [Required]
        public string UserName { get; set; }
    }
}