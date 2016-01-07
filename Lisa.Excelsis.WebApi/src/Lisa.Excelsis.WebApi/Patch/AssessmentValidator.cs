using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    public class AssessmentValidator : PatchValidator
    {
        public IEnumerable<Error> ValidatePatches(int id, IEnumerable<Patch> patches)
        {
            _errors = new List<Error>();

            foreach (Patch patch in patches)
            {
                //Add Mark
                Allow("add", id, patch, @"^observations/\d/marks$", ObservationExists, ValueIsMark);

                //Replace Observation
                Allow("replace", id, patch, @"^observations/\d/result$", ObservationExists, ValueIsResult);
                //Replace  Assessment
                Allow("replace", id, patch, @"^studentname$", AssessmentExists, ValueIsStudentName);
                Allow("replace", id, patch, @"^studentnumber$", AssessmentExists, ValueIsStudentNumber);
                Allow("replace", id, patch, @"^assessed$", AssessmentExists, ValueIsDateTime);

                //Remove Mark
                Allow("remove", id, patch, @"^observations/\d/marks", ObservationExists, ValueIsMark);
            }

            if (!IsValid(patches))
            {
                return Errors();
            }

            return new List<Error>();
        }

        //Check if resource exists
        private static void AssessmentExists(int id, Patch patch)
        {
        }

        private static void ObservationExists(int id, Patch patch)
        {
        }

        //Check if value is valid
        private static void ValueIsDateTime(int id, Patch patch)
        {
        }

        private static void ValueIsResult(int id, Patch patch)
        {
        }

        private static void ValueIsStudentNumber(int id, Patch patch)
        {
        }

        private static void ValueIsStudentName(int id, Patch patch)
        {
        }

        private static void ValueIsMark(int id, Patch patch)
        {
        }

        private static void ValueIsString(int id, Patch patch)
        {
        }
    }
}
