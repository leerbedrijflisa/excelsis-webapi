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
                Allow<CategoryAdd>(patch, "add", new Regex(@"^categories$"), validateField: new Action<dynamic>[] { ExamIsEditable },
                                                                             validateValue: new Action<CategoryAdd, object>[] { ValueIsCategoryObject });
                //Add Criterion
                Allow<CriterionAdd>(patch, "add", new Regex(@"^categories/(?<Id>\d+)/criteria$"), validateField: new Action<dynamic>[] { CategoryExists, ExamIsEditable },
                                                                                                  validateValue: new Action<CriterionAdd, object>[] { ValueIsCriteriaObject });
                //Replace Category
                Allow<int>(patch, "replace", new Regex(@"^categories/(?<Id>\d+)/order$"), validateField: new Action<dynamic>[] { CategoryExists, ExamIsEditable },
                                                                                          validateValue: new Action<int, object>[] { ValueIsInt });
                Allow<string>(patch, "replace", new Regex(@"^categories/(?<Id>\d+)/name$"), validateField: new Action<dynamic>[] { CategoryExists, ExamIsEditable },
                                                                                            validateValue: new Action<string, object>[] { ValueIsString });
                //Replace Criterion
                Allow<int>(patch, "replace", new Regex(@"^categories/(?<Cid>\d+)/criteria/(?<Id>\d+)/order$"), validateField: new Action<dynamic>[] { CriterionExists, ExamIsEditable },
                                                                                                               validateValue: new Action<int, object>[] { ValueIsInt });
                Allow<string>(patch, "replace", new Regex(@"^categories/(?<Cid>\d+)/criteria/(?<Id>\d+)/title$"), validateField: new Action<dynamic>[] { CriterionExists, ExamIsEditable },
                                                                                                                  validateValue: new Action<string, object>[] { ValueIsString });
                Allow<string>(patch, "replace", new Regex(@"^categories/(?<Cid>\d+)/criteria/(?<Id>\d+)/description$"), validateField: new Action<dynamic>[] { CriterionExists, ExamIsEditable },
                                                                                                                        validateValue: new Action<string, object>[] { ValueIsString });
                Allow<string>(patch, "replace", new Regex(@"^categories/(?<Cid>\d+)/criteria/(?<Id>\d+)/weight$"), validateField: new Action<dynamic>[] { CriterionExists, ExamIsEditable },
                                                                                                                   validateValue: new Action<string, object>[] { ValueIsWeight });
                //Replace Exam
                Allow<string>(patch, "replace", new Regex(@"^subject$"),validateField: new Action<dynamic>[] { ExamIsEditable }, validateValue: new Action<string, object>[] { ValueIsExamUrlParam });
                Allow<string>(patch, "replace", new Regex(@"^name$"),   validateField: new Action<dynamic>[] { ExamIsEditable }, validateValue: new Action<string, object>[] { ValueIsExamUrlParam });
                Allow<string>(patch, "replace", new Regex(@"^cohort$"), validateField: new Action<dynamic>[] { ExamIsEditable }, validateValue: new Action<string, object>[] { ValueIsCohort });
                Allow<string>(patch, "replace", new Regex(@"^crebo$"),  validateField: new Action<dynamic>[] { ExamIsEditable }, validateValue: new Action<string, object>[] { ValueIsCrebo });
                Allow<string>(patch, "replace", new Regex(@"^status$"), validateValue: new Action<string, object>[] { ValueIsStatus, ExamHasNoAssessments });

                //Remove Category
                Allow<int>(patch, "remove", new Regex(@"^(?<Parent>categories)$"), validateField: new Action<dynamic>[] { CategoryExists, CategoryHasChildren, ExamIsEditable },
                                                                                   validateValue: new Action<int, object>[] { ValueIsInt });
                Allow<int>(patch, "remove", new Regex(@"^categories/(?<Cid>\d+)/criteria$"), validateField: new Action<dynamic>[] { CriterionExists, ExamIsEditable },
                                                                                             validateValue: new Action<int, object>[] { ValueIsInt });

                //Move Criterion
                Allow<int>(patch, "move", new Regex(@"^categories/(?<Cid>\d+)/criteria/(?<Id>\d+)$"), validateField: new Action<dynamic>[] { CriterionExists, CategoryTargetExists, ExamIsEditable });
                
            }

            SetRemainingPatchError(patches);

            ResourceId = 0;
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
                 errors.Add(new Error(1305, new ErrorProps { Field = "Criterion", Value = parameters.Id, Parent = "Category", ParentId = parameters.Cid }));
            }
        }

        private void CategoryExists(dynamic parameters)
        {
            if (!_db.CategoryExists(ResourceId, parameters.Id))
            {
                 errors.Add(new Error(1305, new ErrorProps { Field = "Category", Value = parameters.Id, Parent = "Exam", ParentId = ResourceId.ToString() }));
            }
        }

        private void CategoryTargetExists(dynamic parameters)
        {
            Match match = Regex.Match(parameters.Target, @"^categories/(?<Cid>\d+)$");
            if (!match.Success)
            {
                 errors.Add(new Error(1209, new ErrorProps { Field = "Target"}));
            }
            
            if (!_db.CategoryExists(ResourceId, match.Groups["Cid"].Value))
            {
                 errors.Add(new Error(1305, new ErrorProps { Field = "Category", Value = match.Groups["Cid"].Value, Parent = "Exam", ParentId = ResourceId.ToString() }));
            }
        }

        private void CategoryHasChildren(dynamic parameters)
        {
            if(_db.HasChildren("Criteria", "CategoryId", parameters.Value))
            {
                errors.Add(new Error(1306, new ErrorProps { Field = "Category", Value = parameters.Value}));
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
                //TODO:  set error
                errors.Add(new Error(1501, new ErrorProps { Message = "After stripping the value there was nothing left to show." }));
            }
        }

        private void ValueIsWeight(string value, dynamic parameters)
        {
            if (!Regex.IsMatch(value, @"^(fail|pass|excellent)$"))
            {
                errors.Add(new Error(1204, new ErrorProps { Field = "value", Value = value, Permitted = new string[] { "fail", "pass", "excellent" } }));
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
            if (!Regex.IsMatch(value, @"^\d{5}$"))
            {
                 errors.Add(new Error(1203, new ErrorProps { Field = "value", Value = value, Count = 5}));
            }
        }

        private void ValueIsStatus(string value, dynamic parameters)
        {
            if (!Regex.IsMatch(value, @"^(draft|published|inactive)$"))
            {
                errors.Add(new Error(1204, new ErrorProps { Field = "value", Value = value, Permitted = new string[] { "draft", "published", "inactive" } }));
            }
        }

        private void ExamHasNoAssessments(string value, dynamic parameters)
        {
            dynamic result = _db.FetchAssessmentsByExam(ResourceId);
            if (result.Count > 0 && value == "draft")
            {
                errors.Add(new Error(1307, new ErrorProps { Value = parameters.Value, Id = ResourceId }));
            }
        }

        private void ExamIsEditable(dynamic value)
        {
            dynamic result = _db.FetchExam(ResourceId);
            if (result.Status != "draft")
            {
                errors.Add(new Error(1308, new ErrorProps { Id = ResourceId }));
            }
        }
            
        private static readonly Database _db = new Database();
    }
}
