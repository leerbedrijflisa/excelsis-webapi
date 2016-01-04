using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    public class AssessmentValidator
    {
        public void ValidateAssessmentPatches(dynamic resource, IEnumerable<Patch> patches)
        {
            //Add Mark
            PatchValidator.Allow("add", patches, @"observations/{id}/marks", ObservationExists, ValueIsMark);

            //Replace Observation
            PatchValidator.Allow("replace", patches, @"observations/{id}/result", ObservationExists, ValueIsResult);
            //Replace  Assessment
            PatchValidator.Allow("replace", patches, @"studentname", AssessmentExists, ValueIsStudentName);
            PatchValidator.Allow("replace", patches, @"studentnumber", AssessmentExists, ValueIsStudentNumber);
            PatchValidator.Allow("replace", patches, @"assessed", AssessmentExists, ValueIsDateTime);

            //Remove Mark
            PatchValidator.Allow("remove", patches, @"observations/{id}/marks", ObservationExists, ValueIsMark);
        }

        //Check if resource exists
        private void AssessmentExists(Patch patch)
        {
        }

        private void ObservationExists(Patch patch)
        {
        }

        //Check if value is valid
        private void ValueIsDateTime(Patch patch)
        {
        }

        private void ValueIsResult(Patch patch)
        {
        }

        private void ValueIsStudentNumber(Patch patch)
        {
        }

        private void ValueIsStudentName(Patch patch)
        {
        }

        private void ValueIsMark(Patch patch)
        {
        }

        private void ValueIsString(Patch patch)
        {
        }
    }
}
