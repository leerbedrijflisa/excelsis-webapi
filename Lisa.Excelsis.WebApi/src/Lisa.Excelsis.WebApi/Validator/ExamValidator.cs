using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lisa.Excelsis.WebApi
{
    public class ExamValidator : Validator
    {
        public void ValidatePatches(int id, IEnumerable<Patch> patches)
        {
            _errors = new List<Error>();
            _fatalError = string.Empty;

            foreach (Patch patch in patches)
            {     
                //Add Category
                Allow("add", id, patch, @"^categories$", ExamExists, ValueIsCategoryObject);
                //Add Criterion
                Allow("add", id, patch, @"^categories/\d+/criteria$", CategoryExists, ValueIsCriteriaObject);

                //Replace Category
                Allow("replace", id, patch, @"^categories/\d+/order$", CategoryExists, ValueIsInt);
                Allow("replace", id, patch, @"^categories/\d+/name$", CategoryExists, ValueIsString);
                //Replace Criterion
                Allow("replace", id, patch, @"^categories/\d+/criteria/\d+/order$", CriterionExists, ValueIsInt);
                Allow("replace", id, patch, @"^categories/\d+/criteria/\d+/title$", CriterionExists, ValueIsString);
                Allow("replace", id, patch, @"^categories/\d+/criteria/\d+/description$", CriterionExists, ValueIsString);
                Allow("replace", id, patch, @"^categories/\d+/criteria/\d+/weight$", CriterionExists, ValueIsWeight);
                //Replace Exam
                Allow("replace", id, patch, @"^subject$", ExamExists, ValueIsString);
                Allow("replace", id, patch, @"^name$", ExamExists, ValueIsString);
                Allow("replace", id, patch, @"^cohort$", ExamExists, ValueIsCohort);
                Allow("replace", id, patch, @"^crebo$", ExamExists, ValueIsCrebo);
                Allow("replace", id, patch, @"^status$", ExamExists, ValueIsStatus);

                //Remove Category
                Allow("remove", id, patch, @"^categories$", CategoryExists, ValueIsInt);
                Allow("remove", id, patch, @"^categories/\d+/criteria$", CriterionRemoveExists, ValueIsInt);

                //Move Criterion
                Allow("move", id, patch, @"^categories/\d+/criteria/\d+$", CriterionExists, CategoryTargetExists, ValueIsInt);
            }

            SetRemainingPatchErrors(patches);
        }

        public void ValidatePosts(ExamPost exam)
        {
            _errors = new List<Error>();
            ValidateDataAnnotations(exam);
        }

        //Check if resource exists
        private static void ExamExists(int id, Patch patch)
        {
            var query = @"SELECT count(*) as count FROM Exams WHERE Id = @Id";
            dynamic result = _db.Execute(query, new { Id = id });
            if(result.count == 0)
            {
                _errors.Add(new Error(1501, new ErrorProps { Field = "Exam", Value = id.ToString() }));
            }
        }

        private static void CriterionExists(int id, Patch patch)
        {
            var field = patch.Field.Split('/');
            var query = @"SELECT count(*) as count FROM Criteria 
                          WHERE Id = @Id AND CategoryId = @Cid AND ExamId = @Eid";
            dynamic result = _db.Execute(query, new { Id = field[3], Cid = field[1], Eid = id});
            if (result.count == 0)
            {
                _errors.Add(new Error(1502, new ErrorProps { Field = "Criterion", Value = field[3], Parent = "Category", ParentId = field[1] }));
            }
        }

        private static void CriterionRemoveExists(int id, Patch patch)
        {
            var field = patch.Field.Split('/');
            var query = @"SELECT count(*) as count FROM Criteria 
                          WHERE Id = @Id AND CategoryId = @Cid AND ExamId = @Eid";
            dynamic result = _db.Execute(query, new { Id = patch.Value.ToString(), Cid = field[1], Eid = id });
            if (result.count == 0)
            {
                _errors.Add(new Error(1502, new ErrorProps { Field = "Criterion", Value = patch.Value.ToString(), Parent = "Category", ParentId = field[1] }));
            }
        }

        private static void CategoryExists(int id, Patch patch)
        {
            var field = patch.Field.Split('/');
            var query = @"SELECT count(*) as count FROM Categories 
                          WHERE Id = @Id AND ExamId = @Eid";
            dynamic result = _db.Execute(query, new { Id = field[1], Eid = id });
            if (result.count == 0)
            {
                _errors.Add(new Error(1502, new ErrorProps { Field = "Category", Value = field[1], Parent = "Exam", ParentId = id.ToString() }));
            }
        }

        private static void CategoryRemoveExists(int id, Patch patch)
        {
            var field = patch.Field.Split('/');
            var query = @"SELECT count(*) as count FROM Categories 
                          WHERE Id = @Id AND ExamId = @Eid";
            dynamic result = _db.Execute(query, new { Id = patch.Value, Eid = id });
            if (result.count == 0)
            {
                _errors.Add(new Error(1502, new ErrorProps { Field = "Category", Value = patch.Value.ToString(), Parent = "Exam", ParentId = id.ToString() }));
            }
        }

        private static void CategoryTargetExists(int id, Patch patch)
        {
            var field = patch.Field.Split('/');
            var query = @"SELECT count(*) as count FROM Categories 
                          WHERE Id = @Id AND ExamId = @Eid";
            dynamic result = _db.Execute(query, new { Id = patch.Value, Eid = id });
            if (result.count == 0)
            {
                _errors.Add(new Error(1502, new ErrorProps { Field = "Category", Value = patch.Value.ToString(), Parent = "Exam", ParentId = id.ToString() }));
            }
        }

        //Check if value is valid
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

        private static void ValueIsInt(int id, Patch patch)
        {
            if (ValueIsNotEmpty(patch))
            {               
                if (Regex.IsMatch(patch.Value.ToString(), @"\d+"))
                {
                    _errors.Add(new Error(1202, new ErrorProps { Field = "value", Value = patch.Value.ToString()}));
                }
            }
        }

        private static void TargetIsInt(int id, Patch patch)
        {
            if (TargetIsNotEmpty(patch))
            {
                if (Regex.IsMatch(patch.Target.ToString(), @"\d+"))
                {
                    _errors.Add(new Error(1202, new ErrorProps { Field = "value", Value = patch.Value.ToString() }));
                }
            }
        }

        private static void ValueIsCategoryObject(int id, Patch patch)
        {            
            try
            {
                CategoryAdd category = patch.Value.ToObject<CategoryAdd>();
                ValidateDataAnnotations(category);
            }
            catch (Exception e)
            {
                _fatalError = e.Message;
            }
        }

        private static void ValueIsCriteriaObject(int id, Patch patch)
        {   
            try {
                CriterionAdd criterion = patch.Value.ToObject<CriterionAdd>();
                ValidateDataAnnotations(criterion);
            }
            catch(Exception e)
            {
                _fatalError = e.Message;
            }
        }

        private static void ValueIsWeight(int id, Patch patch)
        {
            if (ValueIsNotEmpty(patch))
            {
                if (Regex.IsMatch(patch.Value.ToString(), @"^(fail|pass|excellent)$"))
                {
                    _errors.Add(new Error(1208, new ErrorProps { Field = "value", Value = patch.Value.ToString(), Type = "string" }));
                }
            }
        }

        private static void ValueIsCohort(int id, Patch patch)
        {
            if (ValueIsNotEmpty(patch))
            {
                if (Regex.IsMatch(patch.Value.ToString(), @"^(19|20)\d{2}$"))
                {
                    _errors.Add(new Error(1207, new ErrorProps { Field = "value", Value = patch.Value.ToString(), Count = 4, Min = 1900, Max = 2099 }));
                }
            }
        }

        private static void ValueIsCrebo(int id, Patch patch)
        {
            if (ValueIsNotEmpty(patch))
            {
                if (Regex.IsMatch(patch.Value.ToString(), @"^\d{8}$"))
                {
                    _errors.Add(new Error(1203, new ErrorProps { Field = "value", Value = patch.Value.ToString(), Count = 8}));
                }
            }
        }

        private static void ValueIsStatus(int id, Patch patch)
        {
            if (ValueIsNotEmpty(patch))
            {
                if (Regex.IsMatch(patch.Value.ToString(), @"^(draft|published)$"))
                {
                    _errors.Add(new Error(1204, new ErrorProps { Field = "value", Value = patch.Value.ToString(), Permitted1 = "draft", Permitted2 = "published", Permitted3 = "" }));
                }
            }
        }

        private static bool ValueIsNotEmpty(Patch patch)
        {
            if (patch.Value == null)
            {
                _errors.Add(new Error(1101, new ErrorProps { Field = "value" }));
            }
            return (patch.Value == null);
        }

        private static bool TargetIsNotEmpty(Patch patch)
        {
            if (patch.Target == null)
            {
                _errors.Add(new Error(1101, new ErrorProps { Field = "target" }));
            }
            return (patch.Value != null);
        }

        private static readonly Database _db = new Database();
    }
}
