using System;
using System.Collections.Generic;
using System.Linq;
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
            }

            var patchErrors = SetRemainingPatchError(patches);
            if (patchErrors != null)
            {
                errors.AddRange(patchErrors);
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
            if (!_db.CriterionExists(ResourceId, parameters.Cid, parameters.Id))
            {
                return new Error(1502, new ErrorProps { Field = "Criterion", Value = parameters.Id, Parent = "Category", ParentId = parameters.Cid });
            }

            return null;
        }

        private Error CategoryExists(dynamic parameters)
        {
            if (!_db.CategoryExists(ResourceId, parameters.Parent))
            {
                return new Error(1502, new ErrorProps { Field = "Category", Value = parameters.Id, Parent = "Exam", ParentId = ResourceId.ToString() });
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
            
            if (!_db.CategoryExists(ResourceId, match.Groups["Cid"].Value))
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
            var errors = new Error[] {
                Allow<string>("name", value.Name, validateValue: ValueIsString),
                Allow<int>("order", value.Order, validateValue: ValueIsInt)
            };
            return errors.FirstOrDefault();
        }

        private Error ValueIsCriteriaObject(CriterionAdd value, dynamic parameters)
        {
            var errors = new Error[] {
                Allow<string>("order", value.Order, validateValue: ValueIsString),
                Allow<string>("title", value.Title, validateValue: ValueIsString),
                Allow<string>("description", value.Description, validateValue: ValueIsString),
                Allow<string>("weight", value.Weight, validateValue: ValueIsWeight)
            };
            return errors.FirstOrDefault();
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
