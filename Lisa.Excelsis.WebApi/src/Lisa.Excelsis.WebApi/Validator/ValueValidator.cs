using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Lisa.Excelsis.WebApi
{
    public class ValueAttribute : ValidationAttribute
    {
        public string Pattern { get; set; }
        public int ErrorCode { get; set; }
        public ErrorProps ErrorProps { get; set; }

        public ValueAttribute(string pattern, int errorCode, 
            string permitted1 = null, string permitted2 = null, string permitted3 = null, string type = null, int min = 0, int max = 0, int count = 0)
        {
            Pattern = pattern;
            ErrorCode = errorCode;
            ErrorProps = new ErrorProps(){
                Permitted1 = permitted1,
                Permitted2 = permitted2,
                Permitted3 = permitted3,
                Type = type,
                Min = min,
                Max = max,
                Count = count
            };
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                if ((value is string || value is int) && Regex.IsMatch(value.ToString(), Pattern))
                {
                        return ValidationResult.Success;                   
                }
                else
                {
                    ErrorProps.Field = ErrorProps.Field ?? validationContext.DisplayName;
                    ErrorProps.Value = ErrorProps.Value ?? value.ToString();
                    Validator.DataAnnotationErrors.Add(new Error(ErrorCode, ErrorProps));
                }
            }
            else
            {
                Validator.DataAnnotationErrors.Add(new Error(1101,new ErrorProps { Field = validationContext.DisplayName }));
            }

            return new ValidationResult("");
        }
    }
}
