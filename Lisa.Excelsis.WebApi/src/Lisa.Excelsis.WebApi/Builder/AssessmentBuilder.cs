using System;
using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    public class AssessmentBuilder : PatchBuilder
    {
        public void BuildPatches(int id, IEnumerable<Patch> patches)
        {
            transactions = new List<QueryData>();

            foreach (Patch patch in patches)
            {
                //Add Mark
                Build("add", id, patch, @"^observations/\d+/marks$", AddMark);

                //Replace Observation
                Build("replace", id, patch, @"^observations/\d+/result$", ReplaceResult);
                //Replace  Assessment
                Build("replace", id, patch, @"^studentname$", ReplaceStudentName);
                Build("replace", id, patch, @"^studentnumber$", ReplaceStudentNumber);
                Build("replace", id, patch, @"^assessed$", ReplaceAssessed);

                //Remove Mark
                Build("remove", id, patch, @"^observations/\d+/marks", RemoveMark);
            }

            ExecuteTransactions();
        }

        //Add
        private static QueryData AddMark(int id, Patch patch)
        {
            QueryData data = new QueryData();
            var field = patch.Field.Split('/');
            data.Query = @" INSERT INTO [Marks] ([Name],[ObservationId],[AssessmentId])
                            VALUES (@Name, @ObservationId, @AssessmentId)";
            data.Parameters = new { Name = patch.Value.ToString(), ObservationId = field[1], AssessmentId = id };
            return data;
        }

        //Replace
        private static QueryData ReplaceResult(int id, Patch patch)
        {
            QueryData data = new QueryData();
            var field = patch.Field.Split('/');
            data.Query = @"UPDATE [Observations] SET [Result] = @Result WHERE [Id] = @Oid";
            data.Parameters = new { Result = patch.Value.ToString(), Oid = field[1] };
            return data;
        }

        private static QueryData ReplaceStudentName(int id, Patch patch)
        {
            QueryData data = new QueryData();
            data.Query = @"UPDATE [Assessments] SET [StudentName] = @StudentName WHERE [Id] = @Aid";
            data.Parameters = new { StudentName = patch.Value.ToString(), Aid = id };
            return data;
        }

        private static QueryData ReplaceStudentNumber(int id, Patch patch)
        {
            QueryData data = new QueryData();
            data.Query = @"UPDATE [Assessments] SET [StudentNumber] = @StudentNumber WHERE [Id] = @Aid";
            data.Parameters = new { StudentNumber = patch.Value.ToString(), Aid = id };
            return data;
        }

        private static QueryData ReplaceAssessed(int id, Patch patch)
        {
            QueryData data = new QueryData();
            data.Query = @"UPDATE [Assessments] SET [Assessed] = @Assessed WHERE [Id] = @Aid";
            data.Parameters = new { Assessed = ((DateTime)patch.Value).ToString("s") + "Z", Aid = id };
            return data;
        }

        // Remove
        private static QueryData RemoveMark(int id, Patch patch)
        {
            QueryData data = new QueryData();
            data.Query = @" DELETE FROM [Marks] WHERE [Name] = @Name";
            data.Parameters = new { Name = patch.Value.ToString() };
            return data;
        }
    }
}