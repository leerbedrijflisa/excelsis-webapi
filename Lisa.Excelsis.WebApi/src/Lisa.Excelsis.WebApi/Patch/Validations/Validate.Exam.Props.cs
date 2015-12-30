using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    partial class Validate
    {
        public static List<PatchValidation> GetExamPatchValidation()
        {
            /*
             *  FieldRegex is for validation of the json field 'field', this will be used to create a query.
             *  Parent defines the name of the database field to check if the parent exists.
             */
            List<PatchValidation> pVal = new List<PatchValidation>();
            Validate val = new Validate();

            pVal.Add(new PatchValidation
            {
                Action = "add",
                properties = new List<PatchValidationProps>
                    {
                        new PatchValidationProps
                        {
                            FieldRegex = @"^(?<child>categories)$",
                            Validate = val.ValidateCategoryObject,
                            BuildQuery = _db.QueryBuilderAdd
                        },
                        new PatchValidationProps
                        {
                            FieldRegex = @"^(?<parent>categories)/(?<parentId>\d+)/(?<child>criteria)$",
                            Validate = val.ValidateCriterionObject,
                            BuildQuery = _db.QueryBuilderAdd,
                            Parent = "CategoryId"                            
                        }
                    }
            });
            pVal.Add(new PatchValidation
            {
                Action = "remove",
                properties = new List<PatchValidationProps>
                    {
                        new PatchValidationProps
                        {
                            FieldRegex = @"^(?<child>categories)$",
                            ValueRegex = @"^\d+$",
                            Validate = val.ValidateRemoveOneResource,
                            BuildQuery = _db.QueryBuilderRemove,
                            ErrorInfo = new ErrorProps
                            {
                                Code = 1202,
                                Field = "value"
                            }
                        },
                        new PatchValidationProps
                        {
                            FieldRegex = @"^(?<parent>categories)/(?<parentId>\d+)/(?<child>criteria)$",
                            ValueRegex = @"^\d+$",
                            Validate = val.ValidateRemove,
                            BuildQuery = _db.QueryBuilderRemove,
                            Parent = "CategoryId",
                            ErrorInfo = new ErrorProps
                            {
                                Code = 1202,
                                Field = "value"
                            }
                        }
                    }
            });
            pVal.Add(new PatchValidation
            {
                Action = "replace",
                properties = new List<PatchValidationProps>
                    {
                        //Exam metadata
                        new PatchValidationProps
                        {
                            FieldRegex = @"^(?<child>subject)$",
                            ValueRegex = @"^[a-zA-Z\s,!?.:'""]*$",
                            Validate = val.ValidateValue,
                            BuildQuery = _db.QueryBuilderReplace,
                            Parent = "exams"
                        },
                        new PatchValidationProps
                        {
                            FieldRegex = @"^(?<child>name)$",
                            ValueRegex = @"^[a-zA-Z\s,!?.:'""]*$",
                            Validate = val.ValidateValue,
                            BuildQuery = _db.QueryBuilderReplace,
                            Parent = "exams"
                        },
                        new PatchValidationProps
                        {
                            FieldRegex = @"^(?<child>cohort)$",
                            ValueRegex = @"19|20{2}$",
                            Validate = val.ValidateValue,
                            BuildQuery = _db.QueryBuilderReplace,
                            Parent = "exams",
                            ErrorInfo = new ErrorProps
                            {
                                Code = 1207,
                                Field = "value",
                                Count = 4,
                                Min = 1900,
                                Max = 2099
                            }
                        },
                        new PatchValidationProps
                        {
                            FieldRegex = @"^(?<child>crebo)$",
                            ValueRegex = @"^\d{8}$",
                            Validate = val.ValidateValue,
                            BuildQuery = _db.QueryBuilderReplace,
                            Parent = "exams",
                            ErrorInfo = new ErrorProps
                            {
                                Code = 1203,
                                Field = "value",
                                Count = 8
                            }
                        },
                        new PatchValidationProps
                        {
                            FieldRegex = @"^(?<child>status)$",
                            ValueRegex = @"^(draft|published)$",
                            Validate = val.ValidateValue,
                            BuildQuery = _db.QueryBuilderReplace,
                            Parent = "exams",
                            ErrorInfo = new ErrorProps
                            {
                                Code = 1204,
                                Field = "value",
                                Permitted1 = "draft",
                                Permitted2 = "published"
                            }
                        },
                        // Category
                        new PatchValidationProps
                        {
                            FieldRegex = @"^(?<child>categories)/(?<childId>\d+)/(?<property>order)$",
                            ValueRegex = @"^\d+$",
                            Validate = val.ValidateReplaceCategory,
                            BuildQuery = _db.QueryBuilderReplace,
                            ErrorInfo = new ErrorProps
                            {
                                Code = 1202,
                                Field = "value"                                
                            }
                        },
                        new PatchValidationProps
                        {
                            FieldRegex = @"^(?<child>categories)/(?<childId>\d+)/(?<property>name)$",
                            ValueRegex = @"^[a-zA-Z\s,!?.:'""]*$",
                            Validate = val.ValidateReplaceCategory,
                            BuildQuery = _db.QueryBuilderReplace
                        },
                        //Criteria
                        new PatchValidationProps
                        {
                            FieldRegex = @"^(?<parent>categories)/(?<parentId>\d+)/(?<child>criteria)/(?<childId>\d+)/(?<property>order)$",
                            ValueRegex = @"^\d+$",
                            Parent = "CategoryId",
                            Validate = val.ValidateReplaceCriterion,
                            BuildQuery = _db.QueryBuilderReplace,
                            ErrorInfo = new ErrorProps
                            {
                                Code = 1202,
                                Field = "value"
                            }
                        },
                        new PatchValidationProps
                        {
                            FieldRegex = @"^(?<parent>categories)/(?<parentId>\d+)/(?<child>criteria)/(?<childId>\d+)/(?<property>title)$",
                            ValueRegex = @"^[a-zA-Z\s,!?.:'""]*$",
                            Parent = "CategoryId",
                            Validate = val.ValidateReplaceCriterion,
                            BuildQuery = _db.QueryBuilderReplace
                        },
                        new PatchValidationProps
                        {
                            FieldRegex = @"^(?<parent>categories)/(?<parentId>\d+)/(?<child>criteria)/(?<childId>\d+)/(?<property>description)$",
                            ValueRegex = @"^[a-zA-Z\s,!?.:'""]*$",
                            Parent = "CategoryId",
                            Validate = val.ValidateReplaceCriterion,
                            BuildQuery = _db.QueryBuilderReplace
                        },
                        new PatchValidationProps
                        {
                            FieldRegex = @"^(?<parent>categories)/(?<parentId>\d+)/(?<child>criteria)/(?<childId>\d+)/(?<property>weight)$",
                            ValueRegex = @"^(fail|pass|excellent)$",
                            Parent = "CategoryId",
                            Validate = val.ValidateReplaceCriterion,
                            BuildQuery = _db.QueryBuilderReplace,
                            ErrorInfo = new ErrorProps
                            {
                                Code = 1202,
                                Field = "value",
                                Permitted1 = "fail",
                                Permitted2 = "pass",
                                Permitted3 = "excellent"
                            }
                        }
                    }
            });
            pVal.Add(new PatchValidation
            {
                Action = "move",
                properties = new List<PatchValidationProps>
                    {
                        new PatchValidationProps
                        {
                            FieldRegex = @"^(?<parent>categories)/(?<parentId>\d+)/(?<child>criteria)/(?<childId>\d+)$"
                        }
                    }
            });
            return pVal;
        }
    }
}
