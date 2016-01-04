using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    public class ExamValidator
    {
        public void ValidateExamPatches(dynamic resource, IEnumerable<Patch> patches)
        {
            //Add Category
            PatchValidator.Allow("add", patches, @"categories", ExamExists, ValueIsCategoryObject);
            //Add Criterion
            PatchValidator.Allow("add", patches, @"categories/{id}/criteria", CategoryExists, ValueIsCriteriaObject);

            //Replace Category
            PatchValidator.Allow("replace", patches, @"categories/{id}/order", CategoryExists, ValueIsInt);
            PatchValidator.Allow("replace", patches, @"categories/{id}/name", CategoryExists, ValueIsString);
            //Replace Criterion
            PatchValidator.Allow("replace", patches, @"categories/{id}/criteria/{id}/order", CriterionExists, ValueIsInt);
            PatchValidator.Allow("replace", patches, @"categories/{id}/criteria/{id}/title", CriterionExists, ValueIsString);
            PatchValidator.Allow("replace", patches, @"categories/{id}/criteria/{id}/description", CriterionExists, ValueIsString);
            PatchValidator.Allow("replace", patches, @"categories/{id}/criteria/{id}/weight", CriterionExists, ValueIsWeight);
            //Replace Exam
            PatchValidator.Allow("replace", patches, @"subject", ExamExists, ValueIsString);
            PatchValidator.Allow("replace", patches, @"name", ExamExists, ValueIsString);
            PatchValidator.Allow("replace", patches, @"cohort", ExamExists, ValueIsCohort);
            PatchValidator.Allow("replace", patches, @"crebo", ExamExists, ValueIsCrebo);
            PatchValidator.Allow("replace", patches, @"status", ExamExists, ValueIsStatus);

            //Remove Category
            PatchValidator.Allow("remove", patches, @"categories",CategoryExists, ValueIsInt);
            PatchValidator.Allow("remove", patches, @"categories/{id}/criteria/",CriterionExists, ValueIsInt);

            //Move Criterion
            PatchValidator.Allow("move", patches, @"categories/{id}/criteria/{id}",CriterionExists, TargetExists, ValueIsInt);
        }

        //Check if resource exists
        private void CriterionExists(Patch patch)
        {
        }

        private void CategoryExists(Patch patch)
        {
        }

        private void ExamExists(Patch patch)
        {
        }

        private void TargetExists(Patch patch)
        {
        }

        //Check if value is valid
        private void ValueIsString(Patch patch)
        {
        }

        private void ValueIsInt(Patch patch)
        {
        }

        private void ValueIsCategoryObject(Patch patch)
        {
        }

        private void ValueIsCriteriaObject(Patch patch)
        {
        }

        private void ValueIsWeight(Patch patch)
        {
        }

        private void ValueIsCohort(Patch patch)
        {
        }

        private void ValueIsCrebo(Patch patch)
        {
        }

        private void ValueIsStatus(Patch patch)
        {
        }
    }
}
