using System.Collections.Generic;
using System.Linq;

namespace Lisa.Excelsis.WebApi
{
    public class ErrorMessages
    {
        public static string Get(int code, dynamic obj)
        {
            string message = string.Empty;
            switch (code)
            {
                case 0: message = string.Format("The Field {0} doesn't match the regex {1}", obj.Field, obj.Regex);
                    break;
                // 11XX CODES
                case 1100:
                        message = string.Format("The body is empty.");
                    break;
                case 1101:
                        message = string.Format("Field '{0}' is required.", obj.Field);
                    break;
                case 1102:
                        message = string.Format("Subfield '{0}' is required in the {1} named '{2}'.", obj.subField, obj. Type, obj.Field);
                    break;
                case 1103:
                        message = string.Format("Field '{0}' is not allowed in the request body.", obj.Field);
                    break;

               // 12XX CODES
                case 1200:
                        message = string.Format("The field '{0}' with value '{1}' can only contain letters.", obj.Field, obj.Value);
                    break;
                case 1201:
                        message = string.Format("The field '{0}' with value '{1}' can only contain letters and spaces.", obj.Field, obj.Value);
                    break;
                case 1202:
                        message = string.Format("The field '{0}' with value '{1}' can only contain numbers.", obj.Field, obj.Value);
                    break;
                case 1203:
                        message = string.Format("The field '{0}' with value '{1}' doesn't meet the requirements of {2} digits.", obj.Field, obj.Value, obj.Count);
                    break;
                case 1204:
                        
                        message = string.Format("The field '{0}' with value '{1}' can only contain {2}, {3} or {4}.", obj.Field, obj.Value, obj.Permitted1, obj.Permitted2, obj.Permitted3);
                    break;
                case 1205:
                        message = string.Format("The field '{0}' with value '{1}' is not patchable.", obj.Field, obj.Value);
                    break;
                case 1206:
                        message = string.Format("The field '{0}' with value '{1}' must contain at least one letter or number.", obj.Field, obj.Value);
                    break;
                case 1207:
                        message = string.Format("The field '{0}' with value '{1}' doesn't meet the requirements of {2} digits between the range {3} and {4}.", obj.Field, obj.Value, obj.Count, obj.Min, obj.Max);
                    break;
                case 1208:
                        message = string.Format("The field '{0}' with value '{1}' must be an {2}.", obj.Field, obj.Value, obj. Type);
                    break;

                // 13XX CODES
                case 1300:
                        message = string.Format("The field '{0}' with id '{1}' doesn't exists.", obj.Field, obj.Value);
                    break;
                case 1301:
                        message = string.Format("The exam with subject '{0}', cohort '{1}', name '{2}' and crebo '{3}' already exists.", obj.subject, obj.cohort, obj.name, obj.crebo);
                    break;
                case 1302:
                        message = string.Format("The assessor with username '{0}' was not found.", obj.Value);
                    break;
                case 1303:
                        message = string.Format("The action '{0}' doesn't exist.", obj.Value);
                    break;
                case 1304:
                        message = string.Format("The field {0} is not found.", obj.Field);
                    break;

                // 15XX CODES
                case 1500:
                    message = string.Format("The field '{0}' is not correct therefore it cannot be patched.", obj.Field);
                    break;
                case 1501:
                    message = string.Format("The resource '{0}' with value '{1}' was not found.", obj.Field, obj.Value);
                    break;
                case 1502:
                    message = string.Format("The resource '{0}' with value '{1}' was not found inside the parent '{2} with value '{3}'.", obj.Field, obj.Value, obj.Parent, obj.ParentId);
                    break;
            }
            return message;
        }
    }
}
