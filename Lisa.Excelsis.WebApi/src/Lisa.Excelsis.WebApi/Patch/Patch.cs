using System;
using System.Text.RegularExpressions;

namespace Lisa.Excelsis.WebApi
{
    public class PatchValidator
    {
        public static void Allow(string action, object resource, Patch patch, string regex, params Action<object, Patch>[] funcs)
        {
            if(patch.Action.ToLower() == action)
            {
                if(Regex.IsMatch(patch.Field.ToLower(), regex))
                {
                    foreach(Action<object, Patch> func in funcs)
                    {                            
                        func(resource, patch);                           
                    }
                    patch.IsValidated = true;
                }
            }
        }
    }
}
