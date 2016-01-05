using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    public class ExamValidator : PatchValidator
    {
        public void ValidatePatches(object resource, IEnumerable<Patch> patches)
        {
            foreach (Patch patch in patches)
            {
                patch.Errors = new List<Error>();

                //Add Category
                Allow("add", resource, patch, @"categories", ExamExists, ValueIsCategoryObject);
                //Add Criterion
                Allow("add", resource, patch, @"categories/{id}/criteria", CategoryExists, ValueIsCriteriaObject);

                //Replace Category
                Allow("replace", resource, patch, @"categories/{id}/order", CategoryExists, ValueIsInt);
                Allow("replace", resource, patch, @"categories/{id}/name", CategoryExists, ValueIsString);
                //Replace Criterion
                Allow("replace", resource, patch, @"categories/{id}/criteria/{id}/order", CriterionExists, ValueIsInt);
                Allow("replace", resource, patch, @"categories/{id}/criteria/{id}/title", CriterionExists, ValueIsString);
                Allow("replace", resource, patch, @"categories/{id}/criteria/{id}/description", CriterionExists, ValueIsString);
                Allow("replace", resource, patch, @"categories/{id}/criteria/{id}/weight", CriterionExists, ValueIsWeight);
                //Replace Exam
                Allow("replace", resource, patch, @"subject", ExamExists, ValueIsString);
                Allow("replace", resource, patch, @"name", ExamExists, ValueIsString);
                Allow("replace", resource, patch, @"cohort", ExamExists, ValueIsCohort);
                Allow("replace", resource, patch, @"crebo", ExamExists, ValueIsCrebo);
                Allow("replace", resource, patch, @"status", ExamExists, ValueIsStatus);

                //Remove Category
                Allow("remove", resource, patch, @"categories", CategoryExists, ValueIsInt);
                Allow("remove", resource, patch, @"categories/{id}/criteria/", CriterionExists, ValueIsInt);

                //Move Criterion
                Allow("move", resource, patch, @"categories/{id}/criteria/{id}", CriterionExists, TargetExists, ValueIsInt);
            }

            IsValid(patches);
        }

        //Check if resource exists
        private static void ExamExists(dynamic resource, Patch patch)
        {
            var query = @"SELECT count(*) FROM Exams WHERE Id = @Id";
            dynamic result = _db.Execute(query, new { Id = resource.Value });
            if(result.count > 0)
            {
                patch.Errors.Add(new Error(1501, new ErrorProps { Field = "Exam", Value = resource.Value}));
            }
        }

        private static void CriterionExists(dynamic resource, Patch patch)
        {
            var field = patch.Field.Split('/');
            var query = @"SELECT count(*) FROM Criteria 
                          WHERE Id = @Id AND CategoryId = @Cid AND ExamId = @Eid";
            dynamic result = _db.Execute(query, new { Id = field[3], Cid = field[1], Eid = resource.Value});
            if (result.count > 0)
            {
                patch.Errors.Add(new Error(1502, new ErrorProps { Field = "Criterion", Value = field[3], Parent = "Category", ParentId = field[1] }));
            }
        }

        private static void CategoryExists(dynamic resource, Patch patch)
        {
            var field = patch.Field.Split('/');
            var query = @"SELECT count(*) FROM Categories 
                          WHERE Id = @Id AND ExamId = @Eid";
            dynamic result = _db.Execute(query, new { Id = field[1], Eid = resource.Value });
            if (result.count > 0)
            {
                patch.Errors.Add(new Error(1502, new ErrorProps { Field = "Category", Value = field[1], Parent = "Exam", ParentId = resource.Value }));
            }
        }

        private static void TargetExists(dynamic resource, Patch patch)
        {
        }

        //Check if value is valid
        private static void ValueIsString(dynamic resource, Patch patch)
        {
        }

        private static void ValueIsInt(dynamic resource, Patch patch)
        {
        }

        private static void ValueIsCategoryObject(dynamic resource, Patch patch)
        {
        }

        private static void ValueIsCriteriaObject(dynamic resource, Patch patch)
        {
        }

        private static void ValueIsWeight(dynamic resource, Patch patch)
        {
        }

        private static void ValueIsCohort(dynamic resource, Patch patch)
        {
        }

        private static void ValueIsCrebo(dynamic resource, Patch patch)
        {
        }

        private static void ValueIsStatus(dynamic resource, Patch patch)
        {
        }

        private static readonly Database _db = new Database();
    }
}
