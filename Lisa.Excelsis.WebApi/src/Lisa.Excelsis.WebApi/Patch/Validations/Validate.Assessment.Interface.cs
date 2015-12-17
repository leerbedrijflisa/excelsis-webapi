using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    partial class Validate
    {
        public static List<PatchValidation> GetAssessmentPatchValidation()
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
                            FieldRegex = @"^(?<parent>observations)/(?<parentId>\d+)/(?<child>marks)$"
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
                            FieldRegex = @"^(?<parent>observations)/(?<parentId>\d+)/(?<child>marks)$"
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
                            FieldRegex = @"^(?<property>studentname)$",
                            Validate = val.ValidateStudentName,
                            BuildQuery = _db.QueryBuilderReplace
                        },
                        new PatchValidationProps
                        {
                            FieldRegex = @"^(?<property>studentnumber)$",
                            Validate = val.ValidateStudentNumber,
                            BuildQuery = _db.QueryBuilderReplace
                        },
                        new PatchValidationProps
                        {
                            FieldRegex = @"^(?<property>assessed)$",
                            Validate = val.ValidateAssessed,
                            BuildQuery = _db.QueryBuilderReplace
                        },
                        new PatchValidationProps
                        {
                            FieldRegex = @"^(?<parent>observations)/(?<parentId>\d+)/(?<property>result)$",
                            Parent = "CategoryId",
                            Validate = val.ValidateObservationResult,
                            BuildQuery = _db.QueryBuilderReplace
                        },
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
