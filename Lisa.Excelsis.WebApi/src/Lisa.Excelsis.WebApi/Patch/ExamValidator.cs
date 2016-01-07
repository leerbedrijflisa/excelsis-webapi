using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    public class ExamValidator : PatchValidator
    {
        public IEnumerable<Error> ValidatePatches(int id, IEnumerable<Patch> patches)
        {
            _errors = new List<Error>();

            foreach (Patch patch in patches)
            {     
                //Add Category
                Allow("add", id, patch, @"^categories$", ExamExists, ValueIsCategoryObject);
                //Add Criterion
                Allow("add", id, patch, @"^categories/\d/criteria$", CategoryExists, ValueIsCriteriaObject);

                //Replace Category
                Allow("replace", id, patch, @"^categories/\d/order$", CategoryExists, ValueIsInt);
                Allow("replace", id, patch, @"^categories/\d/name$", CategoryExists, ValueIsString);
                //Replace Criterion
                Allow("replace", id, patch, @"^categories/\d/criteria/\d/order$", CriterionExists, ValueIsInt);
                Allow("replace", id, patch, @"^categories/\d/criteria/\d/title$", CriterionExists, ValueIsString);
                Allow("replace", id, patch, @"^categories/\d/criteria/\d/description$", CriterionExists, ValueIsString);
                Allow("replace", id, patch, @"^categories/\d/criteria/\d/weight$", CriterionExists, ValueIsWeight);
                //Replace Exam
                Allow("replace", id, patch, @"^subject$", ExamExists, ValueIsString);
                Allow("replace", id, patch, @"^name$", ExamExists, ValueIsString);
                Allow("replace", id, patch, @"^cohort$", ExamExists, ValueIsCohort);
                Allow("replace", id, patch, @"^crebo$", ExamExists, ValueIsCrebo);
                Allow("replace", id, patch, @"^status$", ExamExists, ValueIsStatus);

                //Remove Category
                Allow("remove", id, patch, @"^categories$", CategoryExists, ValueIsInt);
                Allow("remove", id, patch, @"^categories/\d/criteria/$", CriterionExists, ValueIsInt);

                //Move Criterion
                Allow("move", id, patch, @"^categories/\d/criteria/\d$", CriterionExists, TargetExists, ValueIsInt);
            }

            if (!IsValid(patches))
            {
                return Errors();
            }

            return new List<Error>();
        }

        //Check if resource exists
        private static void ExamExists(int id, Patch patch)
        {
            var query = @"SELECT count(*) FROM Exams WHERE Id = @Id";
            dynamic result = _db.Execute(query, new { Id = id });
            if(result.count > 0)
            {
                _errors.Add(new Error(1501, new ErrorProps { Field = "Exam", Value = id.ToString() }));
            }
        }

        private static void CriterionExists(int id, Patch patch)
        {
            var field = patch.Field.Split('/');
            var query = @"SELECT count(*) FROM Criteria 
                          WHERE Id = @Id AND CategoryId = @Cid AND ExamId = @Eid";
            dynamic result = _db.Execute(query, new { Id = field[3], Cid = field[1], Eid = id});
            if (result.count > 0)
            {
                _errors.Add(new Error(1502, new ErrorProps { Field = "Criterion", Value = field[3], Parent = "Category", ParentId = field[1] }));
            }
        }

        private static void CategoryExists(int id, Patch patch)
        {
            var field = patch.Field.Split('/');
            var query = @"SELECT count(*) FROM Categories 
                          WHERE Id = @Id AND ExamId = @Eid";
            dynamic result = _db.Execute(query, new { Id = field[1], Eid = id });
            if (result.count > 0)
            {
                _errors.Add(new Error(1502, new ErrorProps { Field = "Category", Value = field[1], Parent = "Exam", ParentId = id.ToString() }));
            }
        }

        private static void TargetExists(int id, Patch patch)
        {
        }

        //Check if value is valid
        private static void ValueIsString(int id, Patch patch)
        {
        }

        private static void ValueIsInt(int id, Patch patch)
        {
        }

        private static void ValueIsCategoryObject(int id, Patch patch)
        {
        }

        private static void ValueIsCriteriaObject(int id, Patch patch)
        {
        }

        private static void ValueIsWeight(int id, Patch patch)
        {
        }

        private static void ValueIsCohort(int id, Patch patch)
        {
        }

        private static void ValueIsCrebo(int id, Patch patch)
        {
        }

        private static void ValueIsStatus(int id, Patch patch)
        {
        }

        private static readonly Database _db = new Database();
    }
}
