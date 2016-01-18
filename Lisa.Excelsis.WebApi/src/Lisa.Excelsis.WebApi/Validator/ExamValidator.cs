using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lisa.Excelsis.WebApi
{
    class ExamValidator : Validator<ExamPost>
    {
        public override IEnumerable<Error> ValidatePatches(int id, IEnumerable<Patch> patches)
        {
            List<Error> errors = new List<Error>();
            ResourceId = id;

            foreach (Patch patch in patches)
            {
                var _errors = new Error[] {
                    //Add Category
                    Allow<CategoryAdd>(patch, "add", @"^categories$", validateValue: ValueIsCategoryObject),
                    //Add Criterion
                    Allow<CriterionAdd>(patch, "add", @"^categories/\d+/criteria$", validateField: CategoryExists,
                                                                                    validateValue: ValueIsCriteriaObject),
                    //Replace Category
                    Allow<int>(patch, "replace", @"^categories/\d+/order$", validateField: CategoryExists,
                                                                            validateValue: ValueIsInt),
                    Allow<string>(patch, "replace", @"^categories/\d+/name$", validateField: CategoryExists,
                                                                              validateValue: ValueIsString),
                    //Replace Criterion
                    Allow<int>(patch, "replace", @"^categories/\d+/criteria/\d+/order$", validateField: CriterionExists,
                                                                                         validateValue: ValueIsInt),
                    Allow<string>(patch, "replace", @"^categories/\d+/criteria/\d+/title$", validateField: CriterionExists,
                                                                                            validateValue: ValueIsString),
                    Allow<string>(patch, "replace", @"^categories/\d+/criteria/\d+/description$", validateField: CriterionExists,
                                                                                                  validateValue: ValueIsString),
                    Allow<string>(patch, "replace", @"^categories/\d+/criteria/\d+/description$", validateField: CriterionExists,
                                                                                                  validateValue: ValueIsString),
                    Allow<string>(patch, "replace", @"^categories/\d+/criteria/\d+/weight$", validateField: CriterionExists,
                                                                                             validateValue: ValueIsWeight),
                    //Replace Exam
                    Allow<string>(patch, "replace", @"^subject$", validateValue: ValueIsString),
                    Allow<string>(patch, "replace", @"^name$", validateValue: ValueIsString),
                    Allow<string>(patch, "replace", @"^cohort$", validateValue: ValueIsCohort),
                    Allow<string>(patch, "replace", @"^crebo$", validateValue: ValueIsCrebo),
                    Allow<string>(patch, "replace", @"^status$", validateValue: ValueIsStatus),

                    //Remove Category
                    Allow<int>(patch, "remove", @"^categories$", validateField: CategoryExists,
                                                                 validateValue: ValueIsInt),
                    Allow<int>(patch, "remove", @"^categories/\d+/criteria$", validateField: CriterionExists,
                                                                              validateValue: ValueIsInt),

                    //Move Criterion
                    //Allow("move", id, patch, @"^categories/\d+/criteria/\d+$", validateField: CriterionExists, CategoryTargetExists, validateValue: ValueIsInt)
                };
                errors.AddRange(_errors);
                errors.AddRange(SetRemainingPatchError(patches));
            }

            ResourceId = null;
            return errors;
        }
    

        public override IEnumerable<Error> ValidatePost(ExamPost exam)
        {
            return new Error[] {
               Allow<string>(exam.Name, validateValue: ValueIsString),
            };
        }

        //Check if resource exists
        private Error CriterionExists(dynamic parameters)
        {
            dynamic result = _db.CriterionExists(ResourceId, parameters.Parent, parameters.Child);
            if (result.count == 0)
            {
                return new Error(1502, new ErrorProps { Field = "Criterion", Value = parameters.Child, Parent = "Category", ParentId = parameters.Parent });
            }

            return null;
        }

        private Error CategoryExists(dynamic parameters)
        {
            dynamic result = _db.CategoryExists(ResourceId, parameters.Parent);
            if (result.count == 0)
            {
                return new Error(1502, new ErrorProps { Field = "Category", Value = parameters.Parent, Parent = "Exam", ParentId = ResourceId.ToString() });
            }

            return null;
        }

        private static Error CategoryTargetExists(string value)
        {
            //var field = patch.Field.Split('/');
            //var query = @"SELECT count(*) as count FROM Categories 
            //              WHERE Id = @Id AND ExamId = @Eid";
            //dynamic result = _db.Execute(query, new { Id = patch.Value, Eid = id });
            //if (result.count == 0)
            //{
            //    _errors.Add(new Error(1502, new ErrorProps { Field = "Category", Value = value, Parent = "Exam", ParentId = id.ToString() }));
            //}
            return null;
        }

        //Check if value is valid
        private Error ValueIsString(string value)
        {
            if (value == null)
            {
                return new Error(1208, new ErrorProps { Field = "value", Value = value, Type = "string" });
            }

            return null;
        }
    

        private Error ValueIsInt(int value)
        {           
            if (Regex.IsMatch(value.ToString(), @"\d+"))
            {
                return new Error(1202, new ErrorProps { Field = "value", Value = value.ToString()});
            }

            return null;
        }

        private Error TargetIsInt(string value)
        {
            if (Regex.IsMatch(value, @"\d+"))
            {
                return new Error(1202, new ErrorProps { Field = "value", Value = value });
            }

            return null;
        }

        private Error ValueIsCategoryObject(CategoryAdd value)
        {
            //    try
            //    {
            //        CategoryAdd category = patch.Value.ToObject<CategoryAdd>();
            //        ValidateDataAnnotations(category);
            //    }
            //    catch (Exception e)
            //    {
            //        _fatalError = e.Message;
            //    }
            return null;
        }

        private Error ValueIsCriteriaObject(CriterionAdd value)
        {
            //    try {
            //        CriterionAdd criterion = patch.Value.ToObject<CriterionAdd>();
            //        ValidateDataAnnotations(criterion);
            //    }
            //    catch(Exception e)
            //    {
            //        _fatalError = e.Message;
            //    }
            return null;
        }

        private Error ValueIsWeight(string value)
        {
            if (Regex.IsMatch(value, @"^(fail|pass|excellent)$"))
            {
                return new Error(1208, new ErrorProps { Field = "value", Value = value, Type = "string" });
            }

            return null;
        }

        private Error ValueIsCohort(string value)
        {
            if (Regex.IsMatch(value, @"^(19|20)\d{2}$"))
            {
                return new Error(1207, new ErrorProps { Field = "value", Value = value, Count = 4, Min = 1900, Max = 2099 });
            }

            return null;
        }

        private Error ValueIsCrebo(string value)
        {
            if (Regex.IsMatch(value, @"^\d{8}$"))
            {
                return new Error(1203, new ErrorProps { Field = "value", Value = value, Count = 8});
            }

            return null;
        }

        private static Error ValueIsStatus(string value)
        {
            if (Regex.IsMatch(value, @"^(draft|published)$"))
            {
                return new Error(1204, new ErrorProps { Field = "value", Value = value, Permitted1 = "draft", Permitted2 = "published", Permitted3 = "" });
            }

            return null;
        }

        //private static bool ValueIsNotEmpty(Patch patch)
        //{
        //    if (patch.Value == null)
        //    {
        //         return new Error(1101, new ErrorProps { Field = "value" }));
        //    }
        //    return (patch.Value == null);
        //}

        //private static bool TargetIsNotEmpty(Patch patch)
        //{
        //    if (patch.Target == null)
        //    {
        //         return new Error(1101, new ErrorProps { Field = "target" }));
        //    }
        //    return (patch.Value != null);
        //}

        private static readonly Database _db = new Database();
    }
}
