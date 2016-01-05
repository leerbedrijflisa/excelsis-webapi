using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    public class AssessmentValidator : PatchValidator
    {
        public IEnumerable<Error> ValidatePatches(object resource, IEnumerable<Patch> patches)
        {
            foreach (Patch patch in patches)
            {
                _errors = new List<Error>();

                //Add Mark
                Allow("add", resource, patch, @"observations/{id}/marks", ObservationExists, ValueIsMark);

                //Replace Observation
                Allow("replace", resource, patch, @"observations/{id}/result", ObservationExists, ValueIsResult);
                //Replace  Assessment
                Allow("replace", resource, patch, @"studentname", AssessmentExists, ValueIsStudentName);
                Allow("replace", resource, patch, @"studentnumber", AssessmentExists, ValueIsStudentNumber);
                Allow("replace", resource, patch, @"assessed", AssessmentExists, ValueIsDateTime);

                //Remove Mark
                Allow("remove", resource, patch, @"observations/{id}/marks", ObservationExists, ValueIsMark);
            }

            if (!IsValid(patches))
            {
                return Errors();
            }
            
            return new List<Error>();
        }

        //Check if resource exists
        private static void AssessmentExists(dynamic resource, Patch patch)
        {
        }

        private static void ObservationExists(dynamic resource, Patch patch)
        {
        }

        //Check if value is valid
        private static void ValueIsDateTime(dynamic resource, Patch patch)
        {
        }

        private static void ValueIsResult(dynamic resource, Patch patch)
        {
        }

        private static void ValueIsStudentNumber(dynamic resource, Patch patch)
        {
        }

        private static void ValueIsStudentName(dynamic resource, Patch patch)
        {
        }

        private static void ValueIsMark(dynamic resource, Patch patch)
        {
        }

        private static void ValueIsString(dynamic resource, Patch patch)
        {
        }
    }
}
