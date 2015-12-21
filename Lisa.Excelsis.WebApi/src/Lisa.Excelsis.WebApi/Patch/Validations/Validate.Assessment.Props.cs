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
                            FieldRegex = @"^(?<parent>observations)/(?<parentId>\d+)/(?<child>marks)$",
                            ValueRegex = @"^[\w+\s]*$",
                            Validate = val.ValidateMark,
                            BuildQuery = _db.QueryBuilderAddMark,
                            Parent = "ObservationId"

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
                            FieldRegex = @"^(?<parent>observations)/(?<parentId>\d+)/(?<child>marks)$",
                            ValueRegex = @"^[\w+\s]*$",
                            Validate = val.ValidateMark,
                            BuildQuery = _db.QueryBuilderRemoveMark,
                            Parent = "ObservationId"                            
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
                            ValueRegex = @"^[a-zA-Z\s]*$",
                            Validate = val.ValidateValue,
                            BuildQuery = _db.QueryBuilderReplace,
                            Parent = "assessments"
                        },
                        new PatchValidationProps
                        {
                            FieldRegex = @"^(?<property>studentnumber)$",
                            ValueRegex = @"^\d{8}$",
                            Validate = val.ValidateValue,
                            BuildQuery = _db.QueryBuilderReplace,
                            Parent = "assessments"
                        },
                        new PatchValidationProps
                        {
                            FieldRegex = @"^(?<property>assessed)$",
                            Validate = val.ValidateAssessed,
                            BuildQuery = _db.QueryBuilderReplace,
                            Parent = "assessments"
                        },
                        new PatchValidationProps
                        {
                            FieldRegex = @"^(?<child>observations)/(?<childId>\d+)/(?<property>result)$",
                            ValueRegex = @"^(seen|unseen|notrated)$",
                            Validate = val.ValidateResult,
                            BuildQuery = _db.QueryBuilderReplace
                        },
                    }
            });
            
            return pVal;
        }
    }
}
