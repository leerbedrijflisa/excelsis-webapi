using System;
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
                    Allow<CategoryAdd>(patch, "add",    new Regex(@"^categories$"), validateValue: ValueIsCategoryObject),
                    //Add Criterion
                    Allow<CriterionAdd>(patch, "add",   new Regex(@"^categories/(?<Id>\d+)/criteria$"), validateField: CategoryExists,
                                                                                                        validateValue: ValueIsCriteriaObject),
                    //Replace Category
                    Allow<int>(patch, "replace",        new Regex(@"^categories/(?<Id>\d+)/order$"), validateField: CategoryExists,
                                                                                                     validateValue: ValueIsInt),
                    Allow<string>(patch, "replace",     new Regex(@"^categories/(?<Id>\d+)/name$"), validateField: CategoryExists,
                                                                                                    validateValue: ValueIsString),
                    //Replace Criterion
                    Allow<int>(patch, "replace",        new Regex(@"^categories/(?<Cid>\d+)/criteria/(?<Id>\d+)/order$"), validateField: CriterionExists,
                                                                                                                          validateValue: ValueIsInt),
                    Allow<string>(patch, "replace",     new Regex(@"^categories/(?<Cid>\d+)/criteria/(?<Id>\d+)/title$"), validateField: CriterionExists,
                                                                                                                          validateValue: ValueIsString),
                    Allow<string>(patch, "replace",     new Regex(@"^categories/(?<Cid>\d+)/criteria/(?<Id>\d+)/description$"), validateField: CriterionExists,
                                                                                                                                validateValue: ValueIsString),
                    Allow<string>(patch, "replace",     new Regex(@"^categories/(?<Cid>\d+)/criteria/(?<Id>\d+)/description$"), validateField: CriterionExists,
                                                                                                                                validateValue: ValueIsString),
                    Allow<string>(patch, "replace",     new Regex(@"^categories/(?<Cid>\d+)/criteria/(?<Id>\d+)/weight$"), validateField: CriterionExists,
                                                                                                                           validateValue: ValueIsWeight),
                    //Replace Exam
                    Allow<string>(patch, "replace",     new Regex(@"^subject$"), validateValue: ValueIsString),
                    Allow<string>(patch, "replace",     new Regex(@"^name$"), validateValue: ValueIsString),
                    Allow<string>(patch, "replace",     new Regex(@"^cohort$"), validateValue: ValueIsCohort),
                    Allow<string>(patch, "replace",     new Regex(@"^crebo$"), validateValue: ValueIsCrebo),
                    Allow<string>(patch, "replace",     new Regex(@"^status$"), validateValue: ValueIsStatus),

                    //Remove Category
                    Allow<int>(patch, "remove",         new Regex(@"^categories$"), validateField: CategoryExists,
                                                                                    validateValue: ValueIsInt),
                    Allow<int>(patch, "remove",         new Regex(@"^categories/(?<Cid>\d+)/criteria$"), validateField: CriterionExists,
                                                                                                         validateValue: ValueIsInt),

                    //Move Criterion
                    Allow<int>(patch, "move",           new Regex(@"^categories/(?<Cid>\d+)/criteria/(?<Id>\d+)$"), null, CriterionExists, CategoryTargetExists)
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
               Allow<string>("name", exam.Name, validateValue: ValueIsString),
               Allow<string>("subject", exam.Subject, validateValue: ValueIsString),
               Allow<string>("cohort", exam.Cohort, validateValue: ValueIsCohort),
               Allow<string>("crebo", exam.Crebo, validateValue: ValueIsCrebo, optional: true),
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

        private Error CategoryTargetExists(dynamic parameters)
        {
            Match match = Regex.Match(parameters.Target, @"^categories/(?<Cid>\d+)$");
            if (!match.Success)
            {
                //TODO: return error
                return new Error(0, new ErrorProps { });
            }

            dynamic result = _db.CategoryExists(ResourceId, match.Groups["Cid"].Value);
            if (result.count == 0)
            {
                return new Error(1502, new ErrorProps { Field = "Category", Value = match.Groups["Cid"].Value, Parent = "Exam", ParentId = ResourceId.ToString() });
            }

            return null;
        }

        //Check if value is valid
        private Error ValueIsString(string value, dynamic parameters)
        {
            if (value == null)
            {
                return new Error(1208, new ErrorProps { Field = "value", Value = value, Type = "string" });
            }

            return null;
        }
    

        private Error ValueIsInt(int value, dynamic parameters)
        {           
            if (Regex.IsMatch(value.ToString(), @"\d+"))
            {
                return new Error(1202, new ErrorProps { Field = "value", Value = value.ToString()});
            }

            return null;
        }

        private Error TargetIsInt(string value, dynamic parameters)
        {
            if (Regex.IsMatch(value, @"\d+"))
            {
                return new Error(1202, new ErrorProps { Field = "value", Value = value });
            }

            return null;
        }

        private Error ValueIsCategoryObject(CategoryAdd value, dynamic parameters)
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

        private Error ValueIsCriteriaObject(CriterionAdd value, dynamic parameters)
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

        private Error ValueIsWeight(string value, dynamic parameters)
        {
            if (Regex.IsMatch(value, @"^(fail|pass|excellent)$"))
            {
                return new Error(1208, new ErrorProps { Field = "value", Value = value, Type = "string" });
            }

            return null;
        }

        private Error ValueIsCohort(string value, dynamic parameters)
        {
            if (Regex.IsMatch(value, @"^(19|20)\d{2}$"))
            {
                return new Error(1207, new ErrorProps { Field = "value", Value = value, Count = 4, Min = 1900, Max = 2099 });
            }

            return null;
        }

        private Error ValueIsCrebo(string value, dynamic parameters)
        {
            if (Regex.IsMatch(value, @"^\d{8}$"))
            {
                return new Error(1203, new ErrorProps { Field = "value", Value = value, Count = 8});
            }

            return null;
        }

        private static Error ValueIsStatus(string value, dynamic parameters)
        {
            if (Regex.IsMatch(value, @"^(draft|published)$"))
            {
                return new Error(1204, new ErrorProps { Field = "value", Value = value, Permitted1 = "draft", Permitted2 = "published", Permitted3 = "" });
            }

            return null;
        }
        private static readonly Database _db = new Database();
    }
}
