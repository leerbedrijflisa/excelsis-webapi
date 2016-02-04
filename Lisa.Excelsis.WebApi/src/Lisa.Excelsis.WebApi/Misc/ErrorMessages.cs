using System.Collections.Generic;
using System.Linq;

namespace Lisa.Excelsis.WebApi
{
    public class ErrorMessages
    {
        public static string Get(int code, ErrorProps obj)
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
                        message = string.Format("Subfield '{0}' is required in the {1} named '{2}'.", obj.SubField, obj. Type, obj.Field);
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
                        message = string.Format("The field '{0}' with value '{1}' doesn't meet the requirements of {2} digit number.", obj.Field, obj.Value, obj.Count);
                    break;
                case 1204:
                        List<string> permitted = new List<string>(obj.Permitted);
                        var values = permitted.Count() > 1 ? string.Join(", ", permitted.Take(permitted.Count() - 1)) + " or " + permitted.Last() : permitted.FirstOrDefault();
                        message = string.Format("The field '{0}' with value '{1}' can only contain {2}.", obj.Field, obj.Value, values);
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
                case 1209:
                        message = string.Format("The field '{0}' is not correct therefore it cannot be patched.", obj.Field);
                    break;
                case 1210:
                        permitted = new List<string>(obj.Permitted);
                        values = permitted.Count() > 1 ? string.Join(", ", permitted.Take(permitted.Count() - 1)) + " or " + permitted.Last() : permitted.FirstOrDefault();
                        message = string.Format("The field '{0}' with value '{1}' can only contain {2}.", obj.Field, obj.Value, values);
                    break;
                case 1211:
                        message = string.Format("The field '{0}' must be a valid datetime format.", obj.Field, obj.Value, obj.Example);
                    break;
                case 1212:
                        message = string.Format("The field '{0}' with value '{1}' can only contain letters, numbers and underscores.", obj.Field, obj.Value);
                    break;
                case 1213:
                        message = string.Format("You are not allowed to start an assessment from an exam with status {0}, the exam must be published first.", obj.Value);
                    break;

                // 13XX CODES
                case 1300:
                        message = string.Format("The field '{0}' with id '{1}' doesn't exists.", obj.Field, obj.Value);
                    break;
                case 1301:
                        message = string.Format("The exam with subject '{0}', cohort '{1}', name '{2}' and crebo '{3}' already exists.", obj.Subject, obj.Cohort, obj.Name, obj.Crebo);
                    break;
                case 1302:
                        message = string.Format("The assessor with username '{0}' was not found.", obj.Value);
                    break;
                case 1303:
                        message = string.Format("The action '{0}' is not correct.", obj.Action);
                    break;
                case 1304:
                        message = string.Format("The field {0} is not found.", obj.Field);
                    break;
                case 1305:
                        message = string.Format("The resource '{0}' with value '{1}' was not found inside the parent '{2} with value '{3}'.", obj.Field, obj.Value, obj.Parent, obj.ParentId);
                    break;
                case 1306:
                        message = string.Format("The resource '{0}' with value '{1}' can only be removed if it has no children.", obj.Field, obj.Value);
                    break;
                case 1307:
                        message = string.Format("You cannot change the status to '{0}' since the exam with id '{1} has assessments.", obj.Value, obj.Id);
                    break;
                case 1308:
                        message = string.Format("The exam with id '{0}' doesn't have the status 'draft'", obj.Id);
                    break;
                case 1309:
                        message = string.Format("The value '{0}' does not exist and cannot be removed", obj.Value);
                    break;

                // 15XX CODES
                case 1500:
                        message = string.Format("Exception: {0}", obj.Exception);
                    break;
                case 1501:
                        message = string.Format("Message: {0}", obj.Message);
                    break;
            }
            return message;
        }
    }
}
