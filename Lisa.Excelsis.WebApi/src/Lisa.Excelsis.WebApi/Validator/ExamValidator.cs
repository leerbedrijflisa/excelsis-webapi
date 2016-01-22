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
            errors = new List<Error>();
            ResourceId = id;

            foreach (Patch patch in patches)
            {
                //Add Category
                Allow<CategoryAdd>(patch, "add", new Regex(@"^categories$"), validateValue: ValueIsCategoryObject);
                //Add Criterion
                Allow<CriterionAdd>(patch, "add", new Regex(@"^categories/(?<Id>\d+)/criteria$"), validateField: CategoryExists,
                                                                                                    validateValue: ValueIsCriteriaObject);
                //Replace Category
                Allow<int>(patch, "replace", new Regex(@"^categories/(?<Id>\d+)/order$"), validateField: CategoryExists,
                                                                                                 validateValue: ValueIsInt);
                Allow<string>(patch, "replace", new Regex(@"^categories/(?<Id>\d+)/name$"), validateField: CategoryExists,
                                                                                                validateValue: ValueIsString);
                //Replace Criterion
                Allow<int>(patch, "replace", new Regex(@"^categories/(?<Cid>\d+)/criteria/(?<Id>\d+)/order$"), validateField: CriterionExists,
                                                                                                                      validateValue: ValueIsInt);
                Allow<string>(patch, "replace", new Regex(@"^categories/(?<Cid>\d+)/criteria/(?<Id>\d+)/title$"), validateField: CriterionExists,
                                                                                                                      validateValue: ValueIsString);
                Allow<string>(patch, "replace", new Regex(@"^categories/(?<Cid>\d+)/criteria/(?<Id>\d+)/description$"), validateField: CriterionExists,
                                                                                                                                validateValue: ValueIsString);
                Allow<string>(patch, "replace", new Regex(@"^categories/(?<Cid>\d+)/criteria/(?<Id>\d+)/description$"), validateField: CriterionExists,
                                                                                                                            validateValue: ValueIsString);
                Allow<string>(patch, "replace", new Regex(@"^categories/(?<Cid>\d+)/criteria/(?<Id>\d+)/weight$"), validateField: CriterionExists,
                                                                                                                       validateValue: ValueIsWeight);
                //Replace Exam
                Allow<string>(patch, "replace", new Regex(@"^subject$"), validateValue: ValueIsExamUrlParam);
                Allow<string>(patch, "replace", new Regex(@"^name$"), validateValue: ValueIsExamUrlParam);
                Allow<string>(patch, "replace", new Regex(@"^cohort$"), validateValue: ValueIsCohort);
                Allow<string>(patch, "replace", new Regex(@"^crebo$"), validateValue: ValueIsCrebo);
                Allow<string>(patch, "replace", new Regex(@"^status$"), validateValue: ValueIsStatus);

                //Remove Category
                Allow<int>(patch, "remove", new Regex(@"^(?<Parent>categories)$"), ValueIsInt, CategoryExists, CategoryHasChildren);
                Allow<int>(patch, "remove", new Regex(@"^categories/(?<Cid>\d+)/criteria$"), validateField: CriterionExists,
                                                                                                        validateValue: ValueIsInt);

                //Move Criterion
                Allow<int>(patch, "move", new Regex(@"^categories/(?<Cid>\d+)/criteria/(?<Id>\d+)$"), null, CriterionExists, CategoryTargetExists);
                
            }

            SetRemainingPatchError(patches);

            ResourceId = null;
            return errors;
        }
    

        public override IEnumerable<Error> ValidatePost(ExamPost exam)
        {
            Allow<string>("name", exam.Name, validateValue: ValueIsExamUrlParam);
            Allow<string>("subject", exam.Subject, validateValue: ValueIsExamUrlParam);
            Allow<string>("cohort", exam.Cohort, validateValue: ValueIsCohort);
            Allow<string>("crebo", exam.Crebo, validateValue: ValueIsCrebo, optional: true);

            return errors;
        }

        private void ValueIsCategoryObject(CategoryAdd value, dynamic parameters)
        {
            Allow<string>("name", value.Name, validateValue: ValueIsString);
            Allow<int>("order", value.Order, validateValue: ValueIsInt);
        }

        private void ValueIsCriteriaObject(CriterionAdd value, dynamic parameters)
        {
            Allow<int>("order", value.Order, validateValue: ValueIsInt);
            Allow<string>("title", value.Title, validateValue: ValueIsString);
            Allow<string>("description", value.Description, validateValue: ValueIsString);
            Allow<string>("weight", value.Weight, validateValue: ValueIsWeight);
        }

        //Check if resource exists
        private void CriterionExists(dynamic parameters)
        {
            if (!_db.CriterionExists(ResourceId, parameters.Cid, parameters.Id))
            {
                 errors.Add(new Error(1502, new ErrorProps { Field = "Criterion", Value = parameters.Id, Parent = "Category", ParentId = parameters.Cid }));
            }
        }

        private void CategoryExists(dynamic parameters)
        {
            if (!_db.CategoryExists(ResourceId, parameters.Id))
            {
                 errors.Add(new Error(1502, new ErrorProps { Field = "Category", Value = parameters.Id, Parent = "Exam", ParentId = ResourceId.ToString() }));
            }
        }

        private void CategoryTargetExists(dynamic parameters)
        {
            Match match = Regex.Match(parameters.Target, @"^categories/(?<Cid>\d+)$");
            if (!match.Success)
            {
                //TODO: return error
                 errors.Add(new Error(0, new ErrorProps { }));
            }
            
            if (!_db.CategoryExists(ResourceId, match.Groups["Cid"].Value))
            {
                 errors.Add(new Error(1502, new ErrorProps { Field = "Category", Value = match.Groups["Cid"].Value, Parent = "Exam", ParentId = ResourceId.ToString() }));
            }
        }

        private void CategoryHasChildren(dynamic parameters)
        {
            if(_db.HasChildren("Criteria", "CategoryId", parameters.Value))
            {
                errors.Add(new Error(0, new ErrorProps { }));
            }
        }

        //Check if value is valid
        private void ValueIsString(string value, dynamic parameters)
        {
            if (string.IsNullOrEmpty(value))
            {
                 errors.Add(new Error(1208, new ErrorProps { Field = "value", Value = value, Type = "string" }));
            }
        }
    

        private void ValueIsInt(int value, dynamic parameters)
        {           
            if (!Regex.IsMatch(value.ToString(), @"^\d+$"))
            {
                 errors.Add(new Error(1202, new ErrorProps { Field = "value", Value = value.ToString()}));
            }
        }

        private void TargetIsInt(string value, dynamic parameters)
        {
            if (!Regex.IsMatch(value, @"^\d+$"))
            {
                 errors.Add(new Error(1202, new ErrorProps { Field = "value", Value = value }));
            }
        }       

        private void ValueIsExamUrlParam(string value, dynamic parameters)
        {
            var cleaned = Utils.CleanParam(value);
            if (string.IsNullOrWhiteSpace(cleaned))
            {
                errors.Add(new Error(0, new ErrorProps { }));
            }
        }

        private void ValueIsWeight(string value, dynamic parameters)
        {
            if (!Regex.IsMatch(value, @"^(fail|pass|excellent)$"))
            {
                errors.Add(new Error(1204, new ErrorProps { Field = "value", Value = value, Permitted1 = "fail", Permitted2 = "pass", Permitted3 = "excellent" }));
            }
        }

        private void ValueIsCohort(string value, dynamic parameters)
        {
            if (!Regex.IsMatch(value, @"^(19|20)\d{2}$"))
            {
                 errors.Add(new Error(1207, new ErrorProps { Field = "value", Value = value, Count = 4, Min = 1900, Max = 2099 }));
            }
        }

        private void ValueIsCrebo(string value, dynamic parameters)
        {
            if (!Regex.IsMatch(value, @"^\d{8}$"))
            {
                 errors.Add(new Error(1203, new ErrorProps { Field = "value", Value = value, Count = 8}));
            }
        }

        private void ValueIsStatus(string value, dynamic parameters)
        {
            if (!Regex.IsMatch(value, @"^(draft|published)$"))
            {
                errors.Add(new Error(1204, new ErrorProps { Field = "value", Value = value, Permitted1 = "draft", Permitted2 = "published", Permitted3 = "" }));
            }
        }
        private static readonly Database _db = new Database();
    }
}
