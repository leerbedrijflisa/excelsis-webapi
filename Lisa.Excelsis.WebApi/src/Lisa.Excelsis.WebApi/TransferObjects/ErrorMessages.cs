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
                case 1100:
                        message = string.Format("The body is empty.");
                    break;
                case 1101:
                        message = string.Format("Field '{0}' is required.", obj.field);
                    break;
                case 1102:
                        message = string.Format("Subfield '{0}' is required in the {1} named '{2}'.", obj.subField, obj.type, obj.field);
                    break;
                case 1200:
                        message = string.Format("The field '{0}' with value '{1}' can only contain letters.", obj.field, obj.value);
                    break;
                case 1201:
                        message = string.Format("The field '{0}' with value '{1}' can only contain letters and spaces.", obj.field, obj.value);
                    break;
                case 1202:
                        message = string.Format("The field '{0}' with value '{1}' can only contain numbers.", obj.field, obj.value);
                    break;
                case 1203:
                        message = string.Format("The field '{0}' with value '{1}' doesn't meet the requirements of {2} digits.", obj.field, obj.value, obj.count);
                    break;
                case 1204:
                        List<string> permitted = new List<string>(obj.permitted);
                        var values = permitted.Count() > 1 ? string.Join(", ", permitted.Take(permitted.Count() - 1)) + " or " + permitted.Last() : permitted.FirstOrDefault();
                        message = string.Format("The field '{0}' with value '{1}' can only contain {2}.", obj.field, obj.value, values);
                    break;
                case 1205:
                        message = string.Format("The field '{0}' is not patchable.", obj.field);
                    break;
                case 1206:
                        message = string.Format("The field '{0}' with value '{1}' must contain at least one contain letter or number.", obj.field, obj.value);
                    break;
                case 1207:
                        message = string.Format("The field '{0}' with value '{1}' doesn't meet the requirements of {2} digits between the range {3} and {4}.", obj.field, obj.value, obj.count, obj.min, obj.max);
                    break;
                case 1208:
                        message = string.Format("The field '{0}' with value '{1}' must be an {2}.", obj.field, obj.value, obj.type);
                    break;
                case 1300:
                        message = string.Format("The field '{0}' with id '{1}' doesn't exists.", obj.field, obj.value);
                    break;
                case 1301:
                        message = string.Format("The exam with subject '{0}', cohort '{1}', name '{2}' and crebo '{3}' already exists.", obj.subject, obj.cohort, obj.name, obj.crebo);
                    break;
                case 1302:
                        message = string.Format("The assessor with username '{0}' was not found.", obj.value);
                    break;
                case 1303:
                        message = string.Format("The action '{0}' doesn't exist.", obj.value);
                    break;
                case 1304:
                        message = string.Format("The field {0} is not found.", obj.field);
                    break;
            }
            return message;
        }
    }
}
