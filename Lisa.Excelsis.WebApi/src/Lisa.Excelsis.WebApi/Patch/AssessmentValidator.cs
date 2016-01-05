using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    public class AssessmentValidator
    {
        public static void ValidateAssessmentPatches(object resource, IEnumerable<Patch> patches)
        {
            foreach (Patch patch in patches)
            {
                patch.Errors = new List<Error>();

                //Add Mark
                PatchValidator.Allow("add", resource, patch, @"observations/{id}/marks", ObservationExists, ValueIsMark);

                //Replace Observation
                PatchValidator.Allow("replace", resource, patch, @"observations/{id}/result", ObservationExists, ValueIsResult);
                //Replace  Assessment
                PatchValidator.Allow("replace", resource, patch, @"studentname", AssessmentExists, ValueIsStudentName);
                PatchValidator.Allow("replace", resource, patch, @"studentnumber", AssessmentExists, ValueIsStudentNumber);
                PatchValidator.Allow("replace", resource, patch, @"assessed", AssessmentExists, ValueIsDateTime);

                //Remove Mark
                PatchValidator.Allow("remove", resource, patch, @"observations/{id}/marks", ObservationExists, ValueIsMark);
            }
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
