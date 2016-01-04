using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lisa.Excelsis.WebApi
{
    public class PatchValidator
    {
        public static void Allow(string action, IEnumerable<Patch> patches, string field, params Action<Patch>[] funcs)
        {
            foreach(Patch patch in patches)
            {
                if(patch.Action.ToLower() == action)
                {
                    if(Regex.IsMatch(patch.Field.ToLower(), field))
                    {
                        foreach(Action<Patch> func in funcs)
                        {                            
                            func(patch);                           
                        }
                        patch.IsValidated = true;
                    }
                    else
                    {
                        patch.Errors.Remove()
                        if (!patch.Errors.Any(e => e.Code == 0))
                        {
                            patch.Errors = new Error(0, new ErrorProps { });
                        }
                    }
                }
                else
                {
                    if (!patch.Errors.Any(e => e.Code == 1303))
                    {
                        patch.Errors = new Error(1303, new ErrorProps{ Value = patch.Action });
                    }
                }               
            }
        }
    }
}
