using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lisa.Excelsis.WebApi
{
    class AssessmentValidator : Validator<AssessmentPost>
    {
        public override IEnumerable<Error> ValidatePatch(Patch patch)
        {
            return new Error[]
            {
                //Add Mark
                Allow<string>(patch, "add",     @"^observations/\d+/marks$",    validateField: ObservationExists,
                                                                                validateValue: ValueIsMark),
                //Replace Observation
                Allow<string>(patch, "replace", @"^observations/\d+/result$",   validateField: ObservationExists,
                                                                                validateValue: ValueIsResult),
                //Replace  Assessment
                Allow<string>(patch, "replace", @"^studentname$",               validateValue: ValueIsStudentName),
                Allow<string>(patch, "replace", @"^studentnumber$",             validateValue: ValueIsStudentNumber),
                Allow<DateTime>(patch, "replace", @"^assessed$",                validateValue: ValueIsDateTime),

                //Remove Mark
                Allow<string>(patch, "remove",  @"^observations/\d+/marks",     validateField: ObservationExists,
                                                                                validateValue: ValueIsMark),
                SetRemainingPatchError(patch)
            };   
        }

        public override IEnumerable<Error> ValidatePost(AssessmentPost assessment)
        {
            return new Error[]
            {
                Allow<string>(assessment.Student.Name, validateValue: ValueIsStudentName),
            };
        }

        //Check if resource exists
        private Error AssessmentExists(int id, string value, dynamic parameters)
        {
            if (_db.AssessmentExists(id))
            {
                return new Error(1501, new ErrorProps { Field = "Assessment", Value = id.ToString() });
            }

            return null;
        }

        private Error ObservationExists(int id, dynamic patch)
        {
            var field = patch.Field.Split('/');
            if (_db.ObservationExists(id, field[1]))
            {
                return new Error(1502, new ErrorProps { Field = "Observations", Value = field[1], Parent = "Assessment", ParentId = id.ToString() });
            }

            return null;
        }

        //Check if value is valid
        private Error ValueIsDateTime(DateTime value)
        {           
            if (value != null)
            {
                return new Error(1208, new ErrorProps { Field = "value", Value = "", Type = "datetime"});
            }
            
            return null;
        }

        private Error ValueIsResult(string value)
        {
            if (!Regex.IsMatch(value.ToLower(), @"^(seen|unseen|notrated)$"))
            {
                return new Error(1204, new ErrorProps { Field = "value", Value = value, Permitted1 = "seen", Permitted2 = "unseen", Permitted3 = "notrated"});
            }
            
            return null;
        }

        private Error ValueIsStudentNumber(string value)
        {
            // REGEX : 8 digits (min and max)
            if (!Regex.IsMatch(value.ToLower(), @"^\d{8}$"))
            {
                return new Error(1203, new ErrorProps { Field = "value", Value = value, Count = 8});
            }
            
            return null;
        }

        private Error ValueIsStudentName(string value)
        {
            // REGEX : words with spaces
            if (!Regex.IsMatch(value, @"^[a-zA-Z\s]*$"))
            {
                return new Error(1201, new ErrorProps { Field = "studentname", Value = value});
            }
            
            return null;
        }

        private Error ValueIsMark(string value)
        {
            // REGEX : one word without spaces, lower dashes allowed
            if(!Regex.IsMatch(value, @"^\b[a-zA-Z0-9_]+\b$"))
            {
                return new Error(1200, new ErrorProps { Field = "value", Value = value});
            }
            
            return null;
        }

        private Error ValueIsString(string value)
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                return new Error(1208, new ErrorProps { Field = "value", Value = value, Type = "string" });
            }
            
            return null;
        }

        private static readonly Database _db = new Database();
    }
}
