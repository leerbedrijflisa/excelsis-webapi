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
                            Validate = val.ValidateCategory,
                            BuildQuery = _db.QueryBuilderAdd
                        },
                        new PatchValidationProps
                        {
                            FieldRegex = @"^(?<parent>categories)/(?<parentId>\d+)/(?<child>criteria)$",
                            Validate = val.ValidateCriterion,
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
                            Validate = val.ValidateRemove,
                            BuildQuery = _db.QueryBuilderRemove
                        },
                        new PatchValidationProps
                        {
                            FieldRegex = @"^(?<parent>categories)/(?<parentId>\d+)/(?<child>criteria)$",
                            Validate = val.ValidateRemove,
                            BuildQuery = _db.QueryBuilderRemove,
                            Parent = "CategoryId"
                        }
                    }
            });
            pVal.Add(new PatchValidation
            {
                Action = "replace",
                properties = new List<PatchValidationProps>
                    {
                        new PatchValidationProps
                        {
                            FieldRegex = @"^(?<child>categories)/(?<childId>\d+)/(?<property>order)$",
                            Validate = val.ValidateCategoryOrder,
                            BuildQuery = _db.QueryBuilderReplace
                        },
                        new PatchValidationProps
                        {
                            FieldRegex = @"^(?<child>categories)/(?<childId>\d+)/(?<property>name)$",
                            Validate = val.ValidateCategoryName,
                            BuildQuery = _db.QueryBuilderReplace
                        },
                        new PatchValidationProps
                        {
                            FieldRegex = @"^(?<parent>categories)/(?<parentId>\d+)/(?<child>criteria)/(?<childId>\d+)/(?<property>order)$",
                            Parent = "CategoryId",
                            Validate = val.ValidateCriterionOrder,
                            BuildQuery = _db.QueryBuilderReplace
                        },
                        new PatchValidationProps
                        {
                            FieldRegex = @"^(?<parent>categories)/(?<parentId>\d+)/(?<child>criteria)/(?<childId>\d+)/(?<property>title)$",
                            Parent = "CategoryId",
                            Validate = val.ValidateCriterionTitle,
                            BuildQuery = _db.QueryBuilderReplace
                        },
                        new PatchValidationProps
                        {
                            FieldRegex = @"^(?<parent>categories)/(?<parentId>\d+)/(?<child>criteria)/(?<childId>\d+)/(?<property>description)$",
                            Parent = "CategoryId",
                            Validate = val.ValidateCriterionDescription,
                            BuildQuery = _db.QueryBuilderReplace
                        },
                        new PatchValidationProps
                        {
                            FieldRegex = @"^(?<parent>categories)/(?<parentId>\d+)/(?<child>criteria)/(?<childId>\d+)/(?<property>weight)$",
                            Parent = "CategoryId",
                            Validate = val.ValidateCriterionWeight,
                            BuildQuery = _db.QueryBuilderReplace
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
