using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lisa.Excelsis.WebApi
{
    public class AssessmentValidator : PatchValidator
    {
        public void ValidatePatches(int id, IEnumerable<Patch> patches)
        {
            _errors = new List<Error>();

            foreach (Patch patch in patches)
            {
                //Add Mark
                Allow("add", id, patch, @"^observations/\d+/marks$", ObservationExists, ValueIsMark);

                //Replace Observation
                Allow("replace", id, patch, @"^observations/\d+/result$", ObservationExists, ValueIsResult);
                //Replace  Assessment
                Allow("replace", id, patch, @"^studentname$", AssessmentExists, ValueIsStudentName);
                Allow("replace", id, patch, @"^studentnumber$", AssessmentExists, ValueIsStudentNumber);
                Allow("replace", id, patch, @"^assessed$", AssessmentExists, ValueIsDateTime);

                //Remove Mark
                Allow("remove", id, patch, @"^observations/\d+/marks", ObservationExists, ValueIsMark);
            }
        }

        //Check if resource exists
        private static void AssessmentExists(int id, Patch patch)
        {
            var query = @"SELECT count(*) as count FROM Assessments WHERE Id = @Id";
            dynamic result = _db.Execute(query, new { Id = id });
            if (result.count == 0)
            {
                _errors.Add(new Error(1501, new ErrorProps { Field = "Assessment", Value = id.ToString() }));
            }
        }

        private static void ObservationExists(int id, Patch patch)
        {
            var field = patch.Field.Split('/');
            var query = @"SELECT count(*) as count FROM Observations 
                          WHERE Id = @Id AND AssessmentId = @Aid";
            dynamic result = _db.Execute(query, new { Id = field[1], Aid = id });
            if (result.count == 0)
            {
                _errors.Add(new Error(1502, new ErrorProps { Field = "Observations", Value = field[1], Parent = "Assessment", ParentId = id.ToString() }));
            }
        }

        //Check if value is valid
        private static void ValueIsDateTime(int id, Patch patch)
        {
            DateTime dateTime;
            if (ValueIsNotEmpty(patch))
            {
                DateTime convertedDate = Convert.ToDateTime(patch.Value.ToString());
                if (!DateTime.TryParse(patch.Value.ToString(), out dateTime))
                {
                    _errors.Add(new Error(1208, new ErrorProps { Field = "value", Value = patch.Value.ToString(), Type = "datetime"}));
                }
            }
        }

        private static void ValueIsResult(int id, Patch patch)
        {
            if (ValueIsNotEmpty(patch))
            {
                if (!Regex.IsMatch(patch.Value.ToString(), @"^(seen|unseen|notrated)$"))
                {
                    _errors.Add(new Error(1204, new ErrorProps { Field = "value", Value = patch.Value.ToString(), Permitted1 = "seen", Permitted2 = "unseen", Permitted3 = "notrated"}));
                }
            }
        }

        private static void ValueIsStudentNumber(int id, Patch patch)
        {
            if (ValueIsNotEmpty(patch))
            {
                // REGEX : 8 digits (min and max)
                if (!Regex.IsMatch(patch.Value.ToString(), @"^\d{8}$"))
                {
                    _errors.Add(new Error(1203, new ErrorProps { Field = "value", Value = patch.Value.ToString(), Count = 8}));
                }
            }
        }

        private static void ValueIsStudentName(int id, Patch patch)
        {
            if (ValueIsNotEmpty(patch))
            {
                // REGEX : words with spaces
                if (!Regex.IsMatch(patch.Value.ToString(), @"^[a-zA-Z\s]*$"))
                {
                    _errors.Add(new Error(1201, new ErrorProps { Field = "value", Value = patch.Value.ToString()}));
                }
            }
        }

        private static void ValueIsMark(int id, Patch patch)
        {
            if(ValueIsNotEmpty(patch))
            {
                // REGEX : one word without spaces, lower dashes allowed
                if(!Regex.IsMatch(patch.Value.ToString(), @"^\b[a-zA-Z0-9_]+\b$"))
                {
                    _errors.Add(new Error(1200, new ErrorProps { Field = "value", Value = patch.Value.ToString()}));
                }
            }
        }

        private static void ValueIsString(int id, Patch patch)
        {
            if (ValueIsNotEmpty(patch))
            {
                var value = patch.Value.ToString() as string;
                if (value == null)
                {
                    _errors.Add(new Error(1208, new ErrorProps { Field = "value", Value = patch.Value.ToString(), Type = "string" }));
                }
            }
        }

        private static bool ValueIsNotEmpty(Patch patch)
        {
            if(patch.Value == null)
            {
                _errors.Add(new Error(1101, new ErrorProps{ Field = "value" }));                
            }
            return (patch.Value != null);
        }

        private static readonly Database _db = new Database();
    }
}
