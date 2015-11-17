using System;
using System.Collections.Generic;
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
        public IList<AssessorPost> Assessors { get; set; }
    }
    public class AssessorPost
    {
        [Required]
        public string UserName { get; set; }
    }
}