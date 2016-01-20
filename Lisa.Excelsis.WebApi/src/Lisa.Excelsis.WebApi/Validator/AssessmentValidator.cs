using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lisa.Excelsis.WebApi
{
    class AssessmentValidator : Validator<AssessmentPost>
    {
        public override IEnumerable<Error> ValidatePatches(int id, IEnumerable<Patch> patches)
        {
            errors = new List<Error>();
            ResourceId = id;

            foreach (var patch in patches)
            {
                //Add Mark
                Allow<string>(patch, "add", new Regex(@"^observations/(?<Id>\d+)/marks$"), validateField: ObservationExists,
                                                                                            validateValue: ValueIsMark);
                    //Remove Mark
                Allow<string>(patch, "remove", new Regex(@"^observations/(?<Id>\d+)/marks"), validateField: ObservationExists,
                                                                                                validateValue: ValueIsMark);
                //Replace Observation
                Allow<string>(patch, "replace", new Regex(@"^observations/(?<Id>\d+)/result$"), validateField: ObservationExists,
                                                                                                validateValue: ValueIsResult);
                //Replace  Assessment
                Allow<string>(patch, "replace", new Regex(@"^studentname$"), validateValue: ValueIsStudentName);
                Allow<string>(patch, "replace", new Regex(@"^studentnumber$"), validateValue: ValueIsStudentNumber);
                Allow<DateTime>(patch, "replace", new Regex(@"^assessed$"), validateValue: ValueIsDateTime);                              
            }

            SetRemainingPatchError(patches);

            ResourceId = null;
            return errors;
        }

        public override IEnumerable<Error> ValidatePost(AssessmentPost assessment)
        {
            Allow<string>("studentName", assessment.Student?.Name, validateValue: ValueIsStudentName, optional: true);
            Allow<string>("studentNumber", assessment.Student?.Number, validateValue: ValueIsStudentNumber, optional: true);
            Allow<string[]>("assessors", assessment.Assessors, validateValue: ValueIsAssessors);
            Allow<DateTime>("assessed", assessment.Assessed, validateValue: ValueIsDateTime);

            return errors;
        }
        
        //Check if resource exists        
        private void ObservationExists(dynamic parameters)
        {
            if (!_db.ObservationExists(ResourceId, parameters.Id))
            {
                errors.Add(new Error(1502, new ErrorProps { Field = "Observations", Value = parameters.Id, Parent = "Assessment", ParentId = ResourceId.ToString() }));
            }
        }

        //Check if value is valid
        private void ValueIsDateTime(DateTime value, dynamic parameters)
        {           
            if (value == null)
            {
               errors.Add(new Error(1208, new ErrorProps { Field = "value", Value = "", Type = "datetime"}));
            }        
        }

        private void ValueIsResult(string value, dynamic parameters)
        {
            if (!Regex.IsMatch(value.ToLower(), @"^(seen|unseen|notrated)$"))
            {
                errors.Add(new Error(1204, new ErrorProps { Field = "value", Value = value, Permitted1 = "seen", Permitted2 = "unseen", Permitted3 = "notrated"}));
            }
        }

        private void ValueIsStudentNumber(string value, dynamic parameters)
        {
            // REGEX : 8 digits (min and max)
            if (!Regex.IsMatch(value.ToLower(), @"^\d{8}$"))
            {
                errors.Add(new Error(1203, new ErrorProps { Field = "value", Value = value, Count = 8}));
            }
        }

        private void ValueIsStudentName(string value, dynamic parameters)
        {
            // REGEX : words with spaces
            if (!Regex.IsMatch(value, @"^[a-zA-Z\s]*$"))
            {
                errors.Add(new Error(1201, new ErrorProps { Field = "studentname", Value = value}));
            }
        }

        private void ValueIsMark(string value, dynamic parameters)
        {
            // REGEX : one word without spaces, lower dashes allowed
            if(!Regex.IsMatch(value, @"^\b[a-zA-Z0-9_]+\b$"))
            {
                errors.Add(new Error(1200, new ErrorProps { Field = "value", Value = value}));
            }
        }

        private void ValueIsAssessors(string[] value, dynamic parameters)
        {
            dynamic result = _db.SelectAssessors(value);

            if (result.Count != value.Count())
            {
                foreach (var assessor in value)
                {
                    if (result.Count == 0 || (result.Count > 0 && !((IEnumerable<dynamic>)result).Any(a => a.UserName == assessor)))
                    {
                        errors.Add(new Error(1302, new ErrorProps { Value = assessor }));
                    }
                }
            }
        }
          

        private void ValueIsString(string value, dynamic parameters)
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                errors.Add(new Error(1208, new ErrorProps { Field = "value", Value = value, Type = "string" }));
            }
        }

        private static readonly Database _db = new Database();
    }
}
